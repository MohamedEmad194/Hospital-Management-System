# X-Ray AI (FastAPI + Qwen2.5-VL)

## تشغيل Swagger

```powershell
cd e:\HMS\Model
.\run.ps1
```

يفتح المتصفح على: **http://127.0.0.1:8000/docs**

## نقاط النهاية

| Method | Path | الوصف |
|--------|------|--------|
| GET | `/` | حالة الخدمة |
| GET | `/health` | فحص سريع |
| POST | `/analyze` | رفع صورة (`image`) + `prompt` اختياري |

## ربط موقع المستشفى

1. شغّل FastAPI (هذا المجلد) على المنفذ **8000**
2. شغّل API الـ .NET (`Hospital Mangement System`) — يوجّه الطلبات عبر `XRayAi:BaseUrl`
3. شغّل React (`npm start`) — الصفحة: **/xray-ai** (مدير أو طبيب)

أول تحليل يحمّل نموذج **Qwen2.5-VL-3B** (حجم كبير — يحتاج ذاكرة ووقت).

## لماذا التحليل بطيء؟

- النموذج **3B** يُحمّل من Hugging Face في **أول طلب** (دقائق + عدة GB RAM).
- التشغيل على **CPU** أبطأ بكثير من GPU.
- كل صورة تمرّ بمرحلة **generate** (حتى 200 كلمة مخرجات).

لتسريع التجربة: استخدم GPU إن وُجد، أو نموذج أصغر، أو شغّل الخدمة مسبقاً وانتظر حتى `modelLoaded: true` في `/health`.

## تثبيت التبعيات يدوياً

```powershell
py -3 -m pip install -r requirements.txt
py -3 -m uvicorn project:app --host 127.0.0.1 --port 8000 --reload
```
