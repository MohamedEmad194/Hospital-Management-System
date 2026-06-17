"""
Chest X-ray analysis API (Qwen2.5-VL) — Swagger UI at /docs when running.

Run:  .\\run.ps1   or   py -3 -m uvicorn project:app --host 127.0.0.1 --port 8000 --reload
"""
from __future__ import annotations

import os
import shutil
import tempfile
from contextlib import asynccontextmanager
from pathlib import Path

from fastapi import FastAPI, File, Form, HTTPException, UploadFile
from fastapi.middleware.cors import CORSMiddleware

MODEL_ID = "Qwen/Qwen2.5-VL-3B-Instruct"
ALLOWED_SUFFIXES = {".png", ".jpg", ".jpeg", ".webp", ".bmp"}
DEFAULT_PROMPT = (
    "Analyze this chest X-ray. Provide Findings and Impression in medical style. "
    "This is assistive only, not a definitive diagnosis."
)

model = None
processor = None
model_load_error: str | None = None


def initialize_model() -> None:
    global model, processor, model_load_error
    if model is not None and processor is not None:
        return

    try:
        import torch
        from transformers import AutoProcessor, Qwen2_5_VLForConditionalGeneration

        model = Qwen2_5_VLForConditionalGeneration.from_pretrained(
            MODEL_ID,
            torch_dtype=torch.float32,
            device_map="auto",
        )
        processor = AutoProcessor.from_pretrained(MODEL_ID)
        model_load_error = None
    except Exception as exc:
        model_load_error = str(exc)
        raise


@asynccontextmanager
async def lifespan(app: FastAPI):
    # Load on first /analyze unless XRAY_PRELOAD=true
    if os.environ.get("XRAY_PRELOAD", "").lower() in ("1", "true", "yes"):
        try:
            initialize_model()
        except Exception:
            pass
    yield


app = FastAPI(
    title="Hospital X-Ray AI API",
    description="Upload a chest radiograph for Qwen2.5-VL analysis. Open **/docs** for Swagger UI.",
    version="1.0.0",
    lifespan=lifespan,
    docs_url="/docs",
    redoc_url="/redoc",
    openapi_url="/openapi.json",
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=[
        "http://localhost:3000",
        "https://localhost:3000",
        "http://127.0.0.1:3000",
        "http://localhost:5230",
        "http://127.0.0.1:5230",
    ],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


@app.get("/", tags=["Health"])
def root():
    loaded = model is not None and processor is not None
    return {
        "message": "X-Ray AI API is running",
        "modelId": MODEL_ID,
        "modelLoaded": loaded,
        "modelLoadError": model_load_error,
        "swagger": "/docs",
    }


@app.get("/health", tags=["Health"])
def health():
    return {"status": "ok", "modelLoaded": model is not None and processor is not None}


@app.post("/analyze", tags=["Analysis"])
async def analyze(
    image: UploadFile = File(..., description="Chest X-ray image (PNG, JPEG, WebP, BMP)"),
    prompt: str | None = Form(None, description="Optional analysis instructions"),
):
    if model_load_error is not None:
        raise HTTPException(
            status_code=503,
            detail={"error": "Model initialization failed", "details": model_load_error},
        )

    if model is None or processor is None:
        try:
            initialize_model()
        except Exception as exc:
            raise HTTPException(status_code=503, detail=str(exc)) from exc

    suffix = Path(image.filename or "upload.jpg").suffix.lower()
    if suffix not in ALLOWED_SUFFIXES:
        raise HTTPException(
            status_code=400,
            detail=f"Unsupported file type. Allowed: {', '.join(sorted(ALLOWED_SUFFIXES))}",
        )

    user_prompt = (prompt or "").strip() or DEFAULT_PROMPT

    with tempfile.NamedTemporaryFile(delete=False, suffix=suffix) as tmp:
        file_path = tmp.name
        with open(file_path, "wb") as buffer:
            shutil.copyfileobj(image.file, buffer)

    try:
        from qwen_vl_utils import process_vision_info

        messages = [
            {
                "role": "user",
                "content": [
                    {"type": "image", "image": file_path},
                    {"type": "text", "text": user_prompt},
                ],
            }
        ]

        text = processor.apply_chat_template(
            messages, tokenize=False, add_generation_prompt=True
        )
        image_inputs, _ = process_vision_info(messages)
        inputs = processor(
            text=[text], images=image_inputs, return_tensors="pt"
        ).to(model.device)

        output = model.generate(**inputs, max_new_tokens=200)
        result = processor.batch_decode(output, skip_special_tokens=True)[0]

        return {
            "success": True,
            "modelId": MODEL_ID,
            "report": result,
            "prompt": user_prompt,
        }
    finally:
        try:
            os.remove(file_path)
        except OSError:
            pass

    return {"report": result}