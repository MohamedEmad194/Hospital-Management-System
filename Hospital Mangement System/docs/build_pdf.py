# -*- coding: utf-8 -*-
"""
Build the Hospital Management System Backend Documentation PDF.
Target length: 25-30 pages with diagrams, flowcharts and mock screens.
"""

import os
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib import colors
from reportlab.lib.units import cm, mm
from reportlab.lib.enums import TA_JUSTIFY, TA_LEFT, TA_CENTER, TA_RIGHT
from reportlab.platypus import (
    SimpleDocTemplate, Paragraph, Spacer, Image, Table, TableStyle,
    PageBreak, KeepTogether, ListFlowable, ListItem
)
from reportlab.platypus.flowables import HRFlowable
from reportlab.pdfgen import canvas

BASE = os.path.dirname(os.path.abspath(__file__))
ASSETS = os.path.join(BASE, "pdf_assets")
OUT_PDF = os.path.join(BASE, "HMS_Backend_Documentation.pdf")

NAVY = colors.HexColor("#0E3B5F")
TEAL = colors.HexColor("#0E8A8A")
BLUE = colors.HexColor("#1E6FB8")
LIGHT_BLUE = colors.HexColor("#E3F2FD")
ACCENT = colors.HexColor("#E74C3C")
GREEN = colors.HexColor("#2E8B57")
ORANGE = colors.HexColor("#F39C12")
DARK = colors.HexColor("#2C3E50")
LIGHT_GRAY = colors.HexColor("#ECF0F1")

styles = getSampleStyleSheet()

styles.add(ParagraphStyle(name="HMSTitle", fontName="Helvetica-Bold",
                          fontSize=28, alignment=TA_CENTER,
                          textColor=NAVY, leading=34, spaceAfter=8))
styles.add(ParagraphStyle(name="HMSSubTitle", fontName="Helvetica",
                          fontSize=15, alignment=TA_CENTER,
                          textColor=TEAL, leading=20, spaceAfter=14))
styles.add(ParagraphStyle(name="HMSH1", fontName="Helvetica-Bold",
                          fontSize=17, alignment=TA_LEFT,
                          textColor=NAVY, leading=22, spaceBefore=6,
                          spaceAfter=8))
styles.add(ParagraphStyle(name="HMSH2", fontName="Helvetica-Bold",
                          fontSize=12.5, alignment=TA_LEFT,
                          textColor=TEAL, leading=17, spaceBefore=10,
                          spaceAfter=5))
styles.add(ParagraphStyle(name="HMSH3", fontName="Helvetica-Bold",
                          fontSize=10.5, alignment=TA_LEFT,
                          textColor=BLUE, leading=14, spaceBefore=6,
                          spaceAfter=3))
styles.add(ParagraphStyle(name="HMSBody", fontName="Helvetica",
                          fontSize=10, alignment=TA_JUSTIFY,
                          textColor=DARK, leading=13.4, spaceAfter=5,
                          firstLineIndent=12))
styles.add(ParagraphStyle(name="HMSBodyNoIndent", fontName="Helvetica",
                          fontSize=10, alignment=TA_JUSTIFY,
                          textColor=DARK, leading=13.4, spaceAfter=4))
styles.add(ParagraphStyle(name="HMSBullet", fontName="Helvetica",
                          fontSize=10, alignment=TA_LEFT,
                          textColor=DARK, leading=13.2, leftIndent=16,
                          bulletIndent=4, spaceAfter=2))
styles.add(ParagraphStyle(name="HMSCaption", fontName="Helvetica-Oblique",
                          fontSize=9, alignment=TA_CENTER,
                          textColor=colors.grey, leading=11,
                          spaceBefore=3, spaceAfter=8))
styles.add(ParagraphStyle(name="HMSCode", fontName="Courier",
                          fontSize=8.5, textColor=DARK,
                          leading=11,
                          backColor=LIGHT_GRAY,
                          leftIndent=10, rightIndent=10,
                          borderColor=colors.lightgrey,
                          borderPadding=5, borderWidth=0.5,
                          spaceBefore=3, spaceAfter=7))
styles.add(ParagraphStyle(name="HMSFooter", fontName="Helvetica",
                          fontSize=8.5, alignment=TA_CENTER,
                          textColor=colors.grey))
styles.add(ParagraphStyle(name="HMSChapterLead", fontName="Helvetica-Oblique",
                          fontSize=11, alignment=TA_JUSTIFY,
                          textColor=DARK, leading=15, spaceAfter=8,
                          leftIndent=8, rightIndent=8))
styles.add(ParagraphStyle(name="HMSTOCRow", fontName="Helvetica",
                          fontSize=11, textColor=DARK, leading=16))


def hr():
    return HRFlowable(width="100%", color=NAVY, thickness=1.2,
                      spaceBefore=2, spaceAfter=12)


def img(name, width=None, height=None, caption=None):
    path = os.path.join(ASSETS, name)
    if width is None:
        width = 14 * cm
    image = Image(path, width=width, height=height) if height else \
            Image(path, width=width, height=width * 0.60)
    image.hAlign = "CENTER"
    flowables = [image]
    if caption:
        flowables.append(Spacer(1, 1 * mm))
        flowables.append(Paragraph(caption, styles["HMSCaption"]))
    return flowables


def img_kt(name, width=None, height=None, caption=None):
    return KeepTogether(img(name, width=width, height=height, caption=caption))


def p(text, style="HMSBody"):
    return Paragraph(text, styles[style])


def bullets(items):
    flows = []
    for it in items:
        flows.append(Paragraph("• " + it, styles["HMSBullet"]))
    return flows


def code(text):
    """Render preformatted code inside a styled box."""
    safe = text.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;")
    safe = safe.replace("\n", "<br/>").replace(" ", "&nbsp;")
    return Paragraph(safe, styles["HMSCode"])


def table_kv(rows, col_widths=(5.0 * cm, 11.0 * cm), header=None):
    data = []
    if header:
        data.append(header)
    data.extend(rows)
    t = Table(data, colWidths=col_widths)
    style = [
        ("FONTNAME", (0, 0), (-1, -1), "Helvetica"),
        ("FONTSIZE", (0, 0), (-1, -1), 9.5),
        ("TEXTCOLOR", (0, 0), (-1, -1), DARK),
        ("VALIGN", (0, 0), (-1, -1), "TOP"),
        ("LEFTPADDING", (0, 0), (-1, -1), 6),
        ("RIGHTPADDING", (0, 0), (-1, -1), 6),
        ("TOPPADDING", (0, 0), (-1, -1), 5),
        ("BOTTOMPADDING", (0, 0), (-1, -1), 5),
        ("GRID", (0, 0), (-1, -1), 0.4, colors.lightgrey),
        ("BACKGROUND", (0, 0), (0, -1), LIGHT_BLUE),
        ("FONTNAME", (0, 0), (0, -1), "Helvetica-Bold"),
        ("TEXTCOLOR", (0, 0), (0, -1), NAVY),
    ]
    if header:
        style += [
            ("BACKGROUND", (0, 0), (-1, 0), NAVY),
            ("TEXTCOLOR", (0, 0), (-1, 0), colors.whitesmoke),
            ("FONTNAME", (0, 0), (-1, 0), "Helvetica-Bold"),
            ("ALIGN", (0, 0), (-1, 0), "CENTER"),
        ]
    t.setStyle(TableStyle(style))
    return t


def table_grid(data, col_widths, header=True, font_size=9.5,
               header_color=NAVY, body_color=DARK):
    t = Table(data, colWidths=col_widths)
    style = [
        ("FONTNAME", (0, 0), (-1, -1), "Helvetica"),
        ("FONTSIZE", (0, 0), (-1, -1), font_size),
        ("TEXTCOLOR", (0, 1 if header else 0), (-1, -1), body_color),
        ("VALIGN", (0, 0), (-1, -1), "TOP"),
        ("LEFTPADDING", (0, 0), (-1, -1), 6),
        ("RIGHTPADDING", (0, 0), (-1, -1), 6),
        ("TOPPADDING", (0, 0), (-1, -1), 5),
        ("BOTTOMPADDING", (0, 0), (-1, -1), 5),
        ("GRID", (0, 0), (-1, -1), 0.4, colors.lightgrey),
    ]
    if header:
        style += [
            ("BACKGROUND", (0, 0), (-1, 0), header_color),
            ("TEXTCOLOR", (0, 0), (-1, 0), colors.whitesmoke),
            ("FONTNAME", (0, 0), (-1, 0), "Helvetica-Bold"),
            ("ALIGN", (0, 0), (-1, 0), "CENTER"),
        ]
    t.setStyle(TableStyle(style))
    return t


# ====================================================================
# PAGE FOOTER / HEADER
# ====================================================================
def on_page(canv: canvas.Canvas, doc):
    page = doc.page
    canv.saveState()
    # Header bar (skip on cover)
    if page > 1:
        canv.setStrokeColor(NAVY)
        canv.setFillColor(NAVY)
        canv.rect(0, A4[1] - 1.2 * cm, A4[0], 0.05 * cm, stroke=0, fill=1)
        canv.setFont("Helvetica-Bold", 9)
        canv.setFillColor(NAVY)
        canv.drawString(2 * cm, A4[1] - 1.0 * cm,
                        "Hospital Management System — Backend Documentation")
        canv.drawRightString(A4[0] - 2 * cm, A4[1] - 1.0 * cm,
                             "Al-Hayat Hospital")
    # Footer
    canv.setStrokeColor(colors.lightgrey)
    canv.line(2 * cm, 1.6 * cm, A4[0] - 2 * cm, 1.6 * cm)
    canv.setFont("Helvetica", 8.5)
    canv.setFillColor(colors.grey)
    canv.drawString(2 * cm, 1.1 * cm,
                    "© 2026 Al-Hayat Hospital — Internal Document")
    canv.drawRightString(A4[0] - 2 * cm, 1.1 * cm,
                         f"Page {page}")
    canv.restoreState()


# ====================================================================
# BUILD STORY
# ====================================================================
story = []


# ---------------- COVER PAGE ----------------
story.append(Spacer(1, 3.5 * cm))
story.append(Paragraph("Hospital Management System", styles["HMSTitle"]))
story.append(Paragraph("Backend Technical Documentation", styles["HMSSubTitle"]))
story.append(Spacer(1, 0.3 * cm))
story.append(hr())
story.append(Spacer(1, 0.5 * cm))
story.append(img_kt("fig_architecture.png", width=11.5 * cm))
story.append(Spacer(1, 1.0 * cm))
cover_table = Table([
    ["Project",       "Al-Hayat Hospital — HMS"],
    ["Module",        "Server-Side / RESTful API"],
    ["Platform",      "ASP.NET Core 8.0  |  Entity Framework Core  |  SQL Server 2022"],
    ["Document Type", "Technical Backend Documentation"],
    ["Version",       "1.0"],
    ["Date",          "June 2026"],
    ["Audience",      "Developers, Architects, Reviewers, IT Operations"],
], colWidths=[4.0 * cm, 12.0 * cm])
cover_table.setStyle(TableStyle([
    ("FONTNAME", (0, 0), (0, -1), "Helvetica-Bold"),
    ("FONTNAME", (1, 0), (1, -1), "Helvetica"),
    ("FONTSIZE", (0, 0), (-1, -1), 10.5),
    ("TEXTCOLOR", (0, 0), (0, -1), NAVY),
    ("TEXTCOLOR", (1, 0), (1, -1), DARK),
    ("BOTTOMPADDING", (0, 0), (-1, -1), 6),
    ("TOPPADDING", (0, 0), (-1, -1), 6),
    ("LINEBELOW", (0, 0), (-1, -2), 0.3, colors.lightgrey),
]))
story.append(cover_table)
story.append(PageBreak())


# ---------------- TABLE OF CONTENTS ----------------
story.append(Paragraph("Table of Contents", styles["HMSH1"]))
story.append(hr())

toc_rows = [
    ("1.  Abstract", "3"),
    ("2.  Introduction", "4"),
    ("3.  Project Objectives & Scope", "5"),
    ("4.  Technology Stack", "6"),
    ("5.  System Architecture", "7"),
    ("6.  Request Lifecycle & Middleware Pipeline", "9"),
    ("7.  Project Structure & Code Organization", "10"),
    ("8.  Data Model & Database Design", "11"),
    ("9.  Entity-Relationship Diagram", "13"),
    ("10. Domain Entities Reference", "14"),
    ("11. Component / Module Diagram", "16"),
    ("12. Class Diagram (Domain Model)", "17"),
    ("13. Use-Case Diagram", "18"),
    ("14. Authentication & Authorization", "19"),
    ("15. Authentication Sequence", "20"),
    ("16. Appointment Booking Workflow", "21"),
    ("17. Billing & Payment Flowchart", "22"),
    ("18. Appointment State Diagram", "23"),
    ("19. REST API Reference", "24"),
    ("20. Mock Screens (UI consuming the API)", "26"),
    ("21. Deployment & Hosting", "29"),
    ("22. Security, Logging & Operations", "30"),
    ("23. Conclusion & Future Work", "31"),
]
toc_data = [[name, page] for name, page in toc_rows]
toc_table = Table(toc_data, colWidths=[13.0 * cm, 2.5 * cm])
toc_table.setStyle(TableStyle([
    ("FONTNAME", (0, 0), (-1, -1), "Helvetica"),
    ("FONTSIZE", (0, 0), (-1, -1), 11),
    ("TEXTCOLOR", (0, 0), (-1, -1), DARK),
    ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
    ("BOTTOMPADDING", (0, 0), (-1, -1), 8),
    ("TOPPADDING", (0, 0), (-1, -1), 4),
    ("ALIGN", (1, 0), (1, -1), "RIGHT"),
    ("LINEBELOW", (0, 0), (-1, -2), 0.25, colors.lightgrey),
    ("TEXTCOLOR", (1, 0), (1, -1), TEAL),
    ("FONTNAME", (1, 0), (1, -1), "Helvetica-Bold"),
]))
story.append(toc_table)
story.append(PageBreak())


# ---------------- 1. ABSTRACT ----------------
story.append(Paragraph("1. Abstract", styles["HMSH1"]))
story.append(hr())
story.append(p(
    "The Hospital Management System (HMS) is a comprehensive, server-side platform "
    "designed to digitise and automate the day-to-day administrative and clinical "
    "operations of <b>Al-Hayat Hospital</b>. It is built on top of <b>ASP.NET Core 8.0</b> "
    "and exposes a fully documented <b>RESTful API</b> consumed by a React web interface "
    "and a companion Flutter mobile application. The backend is responsible for the "
    "secure handling of all hospital data — including patients, doctors, appointments, "
    "electronic medical records, prescriptions, billing and pharmacy inventory."
))
story.append(p(
    "From an engineering perspective, the system is structured around three logical tiers: "
    "a thin <b>presentation</b> tier (the front-end clients), an <b>application</b> tier that "
    "contains the controllers, services and authentication infrastructure, and a "
    "<b>data</b> tier composed of Entity Framework Core mapped onto a SQL Server 2022 instance. "
    "Cross-cutting concerns — including JWT-based authentication, role-based "
    "authorization, request validation, structured logging through Serilog and request "
    "rate limiting — are all enforced at the API gateway level before any business code "
    "is invoked."
))
story.append(p(
    "This document is intended as the single reference for the backend portion of "
    "the platform. It explains the high-level architecture of the API, walks through the "
    "request lifecycle, describes the domain model and the database schema, presents UML "
    "and flowchart artefacts (use-case, class, sequence, state, activity, component and "
    "deployment), enumerates every public REST endpoint and finally provides "
    "representative screen mock-ups for the consuming clients. The document concludes "
    "with deployment guidelines and a roadmap for future improvements."
))
story.append(Spacer(1, 0.3 * cm))
story.append(p(
    "<b>Keywords:</b> Hospital Management, ASP.NET Core 8.0, Entity Framework Core, "
    "REST API, JWT, SQL Server, Layered Architecture, Microservices-ready, Swagger / OpenAPI.",
    style="HMSBodyNoIndent"
))
story.append(PageBreak())


# ---------------- 2. INTRODUCTION ----------------
story.append(Paragraph("2. Introduction", styles["HMSH1"]))
story.append(hr())
story.append(Paragraph("2.1 Background", styles["HMSH2"]))
story.append(p(
    "Healthcare institutions operate under continuously increasing volumes of patient "
    "data and complex coordination requirements between departments. Paper-based "
    "records and disconnected spreadsheets create duplication of effort, slow patient "
    "throughput and introduce clinically significant errors. Modern hospitals therefore "
    "require an integrated information platform that consolidates patient demographics, "
    "appointment scheduling, electronic medical records, pharmacy inventory and "
    "billing into a single authoritative data store. Al-Hayat Hospital commissioned the "
    "development of such a platform — the Hospital Management System (HMS) — to "
    "answer this need."
))

story.append(Paragraph("2.2 Problem Statement", styles["HMSH2"]))
story.append(p(
    "Before the introduction of HMS, daily operations relied on a mixture of manual "
    "registers, spreadsheets and isolated departmental applications. Patient histories "
    "had to be physically retrieved from archives, appointments were tracked on paper "
    "calendars and invoices were generated outside any inventory or service catalogue. "
    "The administrative overhead grew proportionally with patient volume and reporting "
    "for hospital management was largely reactive."
))
story.append(p(
    "The HMS backend is the technical answer to these issues. It centralises all data "
    "in a relational store, exposes business operations as well-defined REST endpoints, "
    "applies strict validation and authentication on every request, and serves both web "
    "and mobile front-ends from the same code base."
))

story.append(Paragraph("2.3 Document Purpose", styles["HMSH2"]))
story.append(p(
    "This document focuses exclusively on the <b>backend</b> portion of the project. "
    "It describes the structural design of the API, the persistence model, the security "
    "perimeter and the operational characteristics of the server. The reader is assumed "
    "to be familiar with general software engineering concepts and the .NET ecosystem. "
    "Front-end implementation details are referenced only where they are needed to "
    "understand the contract exposed by the backend."
))

story.append(Paragraph("2.4 Target Audience", styles["HMSH2"]))
story.extend(bullets([
    "<b>Backend developers</b> joining the project who need a high-level orientation "
    "before reading the source code.",
    "<b>Software architects</b> evaluating the structural quality of the platform.",
    "<b>QA engineers</b> writing integration and end-to-end test cases against the API.",
    "<b>DevOps / IT operations</b> teams responsible for deploying, monitoring and "
    "scaling the system.",
    "<b>Academic reviewers</b> who require a complete artefact describing both the "
    "design and the implementation choices of the project.",
]))
story.append(PageBreak())


# ---------------- 3. OBJECTIVES & SCOPE ----------------
story.append(Paragraph("3. Project Objectives & Scope", styles["HMSH1"]))
story.append(hr())

story.append(Paragraph("3.1 Objectives", styles["HMSH2"]))
story.append(p(
    "The HMS backend has been engineered around six core objectives. Each objective "
    "translated directly into one or more architectural decisions that are reflected in "
    "the source code:"
))
obj_data = [
    ["#", "Objective", "Architectural Response"],
    ["O1", "Provide a single source of truth for all hospital data.",
     "One relational database, accessed exclusively through HospitalDbContext (EF Core)."],
    ["O2", "Enforce strong authentication and role-based authorization.",
     "ASP.NET Identity + JWT bearer tokens, with [Authorize(Roles=…)] annotations."],
    ["O3", "Decouple business rules from data access and presentation.",
     "Service layer (I*Service / *Service) sits between controllers and DbContext."],
    ["O4", "Expose every business operation through a stable HTTP contract.",
     "RESTful controllers with versioned routes and Swagger/OpenAPI metadata."],
    ["O5", "Be observable and auditable in production.",
     "Serilog structured logs + daily rolling files + request lifecycle middleware."],
    ["O6", "Be extensible: new features must not require rewriting existing ones.",
     "Dependency-injection container, interface-driven services, AutoMapper profiles."],
]
story.append(table_grid(obj_data, [1.2 * cm, 6.5 * cm, 8.3 * cm]))
story.append(Spacer(1, 0.4 * cm))

story.append(Paragraph("3.2 Functional Scope", styles["HMSH2"]))
story.append(p(
    "The backend implements the following functional modules. Each module is "
    "represented by a dedicated controller, a set of services and one or more domain "
    "entities."
))
story.extend(bullets([
    "<b>Identity & Access:</b> registration, login, password change, profile lookup, "
    "JWT issuance and validation, role assignment (Admin, Doctor, Receptionist, Patient).",
    "<b>Patient Management:</b> CRUD on patient demographics, medical history, "
    "allergies, current medications and insurance information.",
    "<b>Doctor Management:</b> doctor profiles, specialisation, license, consultation "
    "fees and assignment to departments.",
    "<b>Appointment Management:</b> slot calculation, booking, confirmation, "
    "cancellation and completion.",
    "<b>Medical Records:</b> structured diagnoses, treatments, vital signs and "
    "lab results.",
    "<b>Prescriptions & Pharmacy:</b> creation of prescriptions, line-item items "
    "referencing medicines, dispensing workflow.",
    "<b>Billing & Payment:</b> invoice generation, partial payments, integration "
    "with Stripe, PayPal and Paymob payment gateways.",
    "<b>Departments, Rooms, Nursing Units:</b> physical resource catalogue.",
    "<b>Dashboard & Reporting:</b> aggregated KPIs for hospital management.",
    "<b>Chatbot & X-Ray AI:</b> optional AI-powered endpoints that delegate to "
    "OpenAI and an external inference service.",
]))

story.append(Paragraph("3.3 Out of Scope", styles["HMSH2"]))
story.append(p(
    "The backend does not implement the front-end rendering layer, the data-warehouse "
    "side of analytical reporting (which would typically use a separate BI tool), or "
    "the HL7 / FHIR integration with regional health information exchanges. These items "
    "are listed as future work."
))
story.append(PageBreak())


# ---------------- 4. TECHNOLOGY STACK ----------------
story.append(Paragraph("4. Technology Stack", styles["HMSH1"]))
story.append(hr())
story.append(p(
    "The platform is implemented on a mainstream Microsoft stack, augmented with a "
    "small number of widely-adopted third-party libraries. The selection emphasises "
    "long-term support, strong tooling, security and a clean separation of concerns. "
    "The following table groups the technologies by layer."
))
tech_data = [
    ["Layer", "Technology", "Version", "Purpose"],
    ["Framework",   "ASP.NET Core",            "8.0",  "Web host, routing, DI, middleware pipeline"],
    ["ORM",         "Entity Framework Core",   "8.0",  "Object-relational mapping & migrations"],
    ["Database",    "Microsoft SQL Server",    "2022", "Persistent relational storage"],
    ["Identity",    "ASP.NET Identity",        "8.0",  "User store, password hashing, role store"],
    ["Auth",        "JWT Bearer",              "8.0",  "Stateless token-based authentication"],
    ["Mapping",     "AutoMapper",              "12.0", "Entity ↔ DTO conversion"],
    ["Validation",  "FluentValidation",        "11.x", "Declarative request validation"],
    ["Logging",     "Serilog",                 "8.0",  "Structured logging to console & rolling files"],
    ["Docs",        "Swagger / Swashbuckle",   "6.6",  "Interactive OpenAPI documentation"],
    ["Rate-limit",  "AspNetCore.RateLimiting", "8.0",  "Login throttling & abuse protection"],
    ["Payments",    "Stripe / PayPal / Paymob","SDK",  "External payment gateway integration"],
    ["AI",          "OpenAI / Custom X-Ray AI","HTTP", "Optional chat and image-diagnosis endpoints"],
    ["Email",       "SMTP (SmtpClient)",       "BCL",  "Transactional emails (registration, reset)"],
    ["Container",   "Docker / docker-compose", "—",    "Reproducible local & staging deployments"],
]
story.append(table_grid(tech_data,
                        [2.6 * cm, 4.4 * cm, 2.0 * cm, 7.0 * cm]))

story.append(Spacer(1, 0.4 * cm))
story.append(Paragraph("4.1 Rationale for ASP.NET Core 8.0", styles["HMSH2"]))
story.append(p(
    "ASP.NET Core 8.0 is the latest long-term-support release of the framework at "
    "the time of writing. It offers a mature dependency-injection container, "
    "first-class support for minimal-API or controller-based programming models, "
    "performant Kestrel hosting and a rich middleware pipeline. Critically, it "
    "integrates natively with ASP.NET Identity (for the user store) and EF Core (for "
    "data access) so the entire stack can be built with a coherent set of libraries "
    "rather than gluing together heterogeneous components."
))

story.append(Paragraph("4.2 Rationale for SQL Server + EF Core", styles["HMSH2"]))
story.append(p(
    "The hospital domain is intrinsically relational. Patients have many appointments, "
    "appointments belong to exactly one doctor and one room, prescriptions link patients "
    "to medical records and so on. A relational database — modelled here with EF Core "
    "code-first migrations on top of SQL Server — naturally expresses these constraints "
    "and provides strong guarantees through foreign keys, indexes and ACID transactions."
))
story.append(PageBreak())


# ---------------- 5. SYSTEM ARCHITECTURE ----------------
story.append(Paragraph("5. System Architecture", styles["HMSH1"]))
story.append(hr())
story.append(p(
    "The HMS backend follows a classic <b>three-tier layered architecture</b>. Requests "
    "originate from a presentation layer (the browser or the Flutter app), traverse a "
    "well-defined middleware pipeline, are handled by a controller, are delegated to a "
    "service for business processing and finally reach a SQL Server database through "
    "Entity Framework Core. The diagram below summarises the layout."
))
story.extend(img("fig_architecture.png", width=13.5 * cm,
                 caption="Figure 1 — Three-tier layered architecture of the HMS backend."))

story.append(Paragraph("5.1 Layers in Detail", styles["HMSH2"]))
story.append(Paragraph("5.1.1 Presentation / Client Layer", styles["HMSH3"]))
story.append(p(
    "The client layer is intentionally thin. It consists of a React-based Single-Page "
    "Application (SPA) for desktop use, a Flutter cross-platform application for "
    "mobile use, and the Swagger UI that ships with the backend for API exploration. "
    "All three speak the same language — HTTPS with JSON payloads — and all three "
    "rely on the same JWT-based authentication scheme."
))

story.append(Paragraph("5.1.2 Application / API Layer", styles["HMSH3"]))
story.append(p(
    "This is where the bulk of the backend code lives. It is composed of:"
))
story.extend(bullets([
    "<b>Controllers</b> — thin classes that translate HTTP into method calls. Each "
    "controller is annotated with <i>[ApiController]</i> and <i>[Route(\"api/[controller]\")]</i>.",
    "<b>Services</b> — interface + implementation pairs (e.g. <i>IPatientService</i> / "
    "<i>PatientService</i>) that encapsulate business rules.",
    "<b>DTOs</b> — Data Transfer Objects that flatten and shape the data exposed to "
    "clients. They prevent over-posting and decouple the public contract from internal "
    "entities.",
    "<b>Mappings</b> — AutoMapper profiles that convert <i>Entity ↔ DTO</i> in both "
    "directions.",
    "<b>Middlewares</b> — JWT authentication, CORS, rate limiting, error handling and "
    "Serilog request logging.",
]))

story.append(Paragraph("5.1.3 Data Layer", styles["HMSH3"]))
story.append(p(
    "The data layer is composed of a single Entity Framework Core <i>DbContext</i> — "
    "<i>HospitalDbContext</i> — that exposes one <i>DbSet&lt;T&gt;</i> per domain "
    "entity. All relationships, indexes, decimal precisions and seed data are "
    "configured fluently inside <i>OnModelCreating</i>. SQL Server 2022 is the target "
    "database engine."
))

story.append(Paragraph("5.2 Cross-Cutting Concerns", styles["HMSH2"]))
story.append(p(
    "Several concerns affect every request and are therefore handled outside the "
    "controllers and services:"
))
story.extend(bullets([
    "<b>Authentication</b> — Identity + JWT, validated by <i>UseAuthentication()</i>.",
    "<b>Authorization</b> — Role-based policies declared on controller methods.",
    "<b>Validation</b> — Model state validation, augmented with FluentValidation "
    "for the more complex DTOs.",
    "<b>Logging</b> — Serilog with a daily rolling file sink under <i>logs/</i>.",
    "<b>Error handling</b> — A global <i>UseExceptionHandler</i> that returns RFC-7807 "
    "<i>ProblemDetails</i> payloads.",
    "<b>Rate limiting</b> — A token bucket on <i>POST /api/auth/login</i> to mitigate "
    "credential-stuffing.",
]))
story.append(PageBreak())


# ---------------- 5.3 Component Diagram ----------------
story.append(Paragraph("5.3 Component / Module Diagram", styles["HMSH2"]))
story.append(p(
    "Figure 10 below explodes the same architecture into the concrete components that "
    "live inside the backend project. Controllers are mapped 1-to-1 to services, and "
    "services delegate persistence to a single <i>HospitalDbContext</i>. AutoMapper "
    "sits on the side and is consumed by both controllers and services to convert "
    "between domain entities and the DTOs returned to clients."
))
story.extend(img("fig_components.png", width=13.5 * cm,
                 caption="Figure 10 — Component diagram showing controller, "
                 "service, mapping and data layers."))

story.append(Paragraph("5.4 Why a Layered Architecture", styles["HMSH2"]))
story.append(p(
    "Although the project could have been written as a single monolithic controller per "
    "feature, splitting it into <i>Controller → Service → DbContext</i> was a deliberate "
    "choice. The benefits become tangible as soon as more than one developer touches "
    "the code base:"
))
story.extend(bullets([
    "<b>Testability</b> — services depend on interfaces, so they can be unit-tested "
    "with mocked dependencies without spinning up SQL Server.",
    "<b>Reusability</b> — multiple controllers (REST, GraphQL, gRPC if needed) can "
    "share the same business logic.",
    "<b>Maintainability</b> — when business rules change, only the service layer "
    "needs to be updated. Controllers stay focused on HTTP plumbing.",
    "<b>Security</b> — DTOs prevent over-posting attacks where a malicious client "
    "tries to set internal fields such as <i>IsActive</i> or <i>UserId</i>.",
]))
story.append(Spacer(1, 0.3 * cm))


# ---------------- 6. REQUEST LIFECYCLE ----------------
story.append(Paragraph("6. Request Lifecycle & Middleware Pipeline", styles["HMSH1"]))
story.append(hr())
story.append(p(
    "Every HTTP call that arrives at the HMS backend traverses the same well-defined "
    "pipeline before its response is sent back. Understanding this pipeline is essential "
    "for debugging, performance tuning and security analysis."
))
story.extend(img("fig_request_flow.png", width=14 * cm,
                 caption="Figure 2 — Request lifecycle from the client to SQL Server."))

story.append(Paragraph("6.1 Pipeline Stages", styles["HMSH2"]))
pipeline_data = [
    ["Stage", "Component", "Responsibility"],
    ["1", "HTTPS termination (Kestrel/IIS)",
     "TLS handshake and certificate validation."],
    ["2", "Serilog request logging",
     "Captures method, path, status and duration of each request."],
    ["3", "CORS policy",
     "Approves or rejects cross-origin requests based on configured origins."],
    ["4", "Authentication",
     "Reads the <i>Authorization: Bearer</i> header and validates the JWT."],
    ["5", "Authorization",
     "Enforces role and policy attributes declared on controller methods."],
    ["6", "Rate limiting",
     "Applies the <i>auth-login</i> partition for the login endpoint."],
    ["7", "Routing & model binding",
     "Selects the correct controller action and binds the body to a DTO."],
    ["8", "FluentValidation",
     "Runs declarative rules on the bound DTO; rejects with 400 if invalid."],
    ["9", "Controller action",
     "Translates the call into a service-layer invocation."],
    ["10","Service layer",
     "Executes business rules; orchestrates several repository calls."],
    ["11","EF Core / SQL",
     "Translates LINQ to SQL, executes against SQL Server, materialises entities."],
    ["12","AutoMapper",
     "Projects the entity graph into a DTO for the response."],
    ["13","Global error handler",
     "Catches unhandled exceptions and returns a ProblemDetails payload."],
]
story.append(table_grid(pipeline_data, [1.0 * cm, 4.8 * cm, 10.0 * cm]))

story.append(Spacer(1, 0.3 * cm))
story.append(Paragraph("6.2 Example: Program.cs Excerpt", styles["HMSH2"]))
story.append(p(
    "The pipeline is configured during the application start-up inside <i>Program.cs</i>. "
    "The simplified excerpt below highlights how the cross-cutting concerns described "
    "above are stitched together:"
))
story.append(code(
    "var builder = WebApplication.CreateBuilder(args);\n"
    "\n"
    "builder.Host.UseSerilog();\n"
    "builder.Services.AddDbContext<HospitalDbContext>(o =>\n"
    "    o.UseSqlServer(builder.Configuration.GetConnectionString(\"DefaultConnection\")));\n"
    "\n"
    "builder.Services.AddIdentity<User, IdentityRole>(...)\n"
    "    .AddEntityFrameworkStores<HospitalDbContext>();\n"
    "\n"
    "builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)\n"
    "    .AddJwtBearer(opt => opt.TokenValidationParameters = ...);\n"
    "\n"
    "builder.Services.AddAutoMapper(typeof(Program));\n"
    "builder.Services.AddScoped<IPatientService, PatientService>();\n"
    "builder.Services.AddScoped<IDoctorService, DoctorService>();\n"
    "// ... all other services\n"
    "\n"
    "var app = builder.Build();\n"
    "app.UseSerilogRequestLogging();\n"
    "app.UseCors(\"AllowAll\");\n"
    "app.UseAuthentication();\n"
    "app.UseAuthorization();\n"
    "app.UseRateLimiter();\n"
    "app.MapControllers();\n"
    "app.Run();"
))
story.append(PageBreak())


# ---------------- 7. PROJECT STRUCTURE ----------------
story.append(Paragraph("7. Project Structure & Code Organization", styles["HMSH1"]))
story.append(hr())
story.append(p(
    "The backend lives in a single ASP.NET Core project named "
    "<i>Hospital Mangement System</i>. The folder structure is feature-by-layer: code "
    "is grouped first by responsibility (controllers, services, models …) and then by "
    "feature. This is the most familiar layout for .NET developers and makes it easy to "
    "locate the code corresponding to any given endpoint."
))
story.extend(img("fig_structure.png", width=12.5 * cm,
                 caption="Figure 13 — Top-level folder layout of the backend project."))

story.append(Paragraph("7.1 Key Folders", styles["HMSH2"]))
story.extend(bullets([
    "<b>Controllers/</b> — 22 REST controllers, one per resource (Auth, Patients, "
    "Doctors, Appointments, MedicalRecords, Prescriptions, Bills, Payment, Medicines, "
    "Departments, Rooms, Schedules, Dashboard, NursingUnits, Chatbot, XRayAi, Health, "
    "Features, Seed, …).",
    "<b>Services/</b> — Business logic. Each service is declared as an interface "
    "(<i>I*Service.cs</i>) and an implementation (<i>*Service.cs</i>).",
    "<b>Models/</b> — Domain entities. They inherit from a common <i>BaseEntity</i> "
    "class that provides <i>Id, CreatedAt, UpdatedAt, IsActive</i>.",
    "<b>DTOs/</b> — Lightweight contracts used to communicate with clients. Every "
    "controller action returns a DTO, never an entity directly.",
    "<b>Data/</b> — <i>HospitalDbContext</i>, <i>SeedData</i> and helpers.",
    "<b>Mappings/</b> — A single <i>AutoMapperProfile.cs</i> that registers all the "
    "Entity ↔ DTO mappings.",
    "<b>Migrations/</b> — EF Core code-first migrations.",
    "<b>Configuration/</b> — Connection-string resolver, seed-password provider.",
    "<b>Properties/</b> — <i>launchSettings.json</i> for local development.",
    "<b>logs/</b> — Daily rolling logs produced by Serilog (e.g. "
    "<i>hospital-management-20260612.txt</i>).",
]))

story.append(Paragraph("7.2 Source-Code Statistics", styles["HMSH2"]))
stats = [
    ["Metric", "Value"],
    ["Number of Controllers", "22"],
    ["Number of Services (interface + impl pairs)", "18 pairs"],
    ["Number of Domain Entities", "17"],
    ["Number of DTOs", "14"],
    ["Lines of C# Code (approx.)", "≈ 12 000"],
    ["Database Tables (including Identity)", "20+"],
    ["REST Endpoints", "80+"],
]
story.append(table_grid(stats, [10 * cm, 5 * cm]))
story.append(PageBreak())


# ---------------- 8. DATA MODEL ----------------
story.append(Paragraph("8. Data Model & Database Design", styles["HMSH1"]))
story.append(hr())
story.append(p(
    "The persistence layer is built on Entity Framework Core code-first migrations. "
    "All domain entities derive from a common <i>BaseEntity</i> class that ensures "
    "every table has an <i>Id</i>, <i>CreatedAt</i>, <i>UpdatedAt</i> and <i>IsActive</i> "
    "column. ASP.NET Identity contributes the <i>AspNetUsers</i>, <i>AspNetRoles</i> "
    "and related tables out of the box."
))

story.append(Paragraph("8.1 BaseEntity Definition", styles["HMSH2"]))
story.append(code(
    "public abstract class BaseEntity\n"
    "{\n"
    "    public int Id { get; set; }\n"
    "    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;\n"
    "    public DateTime? UpdatedAt { get; set; }\n"
    "    public bool IsActive { get; set; } = true;\n"
    "}"
))

story.append(Paragraph("8.2 Domain Entities", styles["HMSH2"]))
story.append(p(
    "The full domain consists of 17 entities. The table below summarises the most "
    "important ones and the role they play in the system:"
))
entities = [
    ["Entity", "Role"],
    ["User",          "ASP.NET Identity user — the authentication principal."],
    ["Patient",       "Person receiving care; demographics, history, insurance."],
    ["Doctor",        "Clinician with specialisation, license and consultation fee."],
    ["Staff",         "Non-clinical personnel (administrators, receptionists)."],
    ["Department",    "Hospital department (Cardiology, Pediatrics, …)."],
    ["Room",          "Physical room attached to a department; has type & capacity."],
    ["NursingUnit",   "Sub-unit inside a department for nursing assignments."],
    ["Appointment",   "Scheduled slot between patient, doctor and room."],
    ["MedicalRecord", "Diagnosis & treatment captured during/after an appointment."],
    ["Prescription",  "Header object grouping a list of <i>PrescriptionItem</i>."],
    ["PrescriptionItem","Single line of a prescription (qty, dosage, frequency)."],
    ["Medicine",      "Pharmacy item with stock, expiry, unit price."],
    ["Bill",          "Patient invoice composed of multiple <i>BillItem</i>."],
    ["BillItem",      "Line item billed to the patient (service, drug or room fee)."],
    ["Schedule",      "Doctor weekly availability pattern."],
    ["Feature",       "Feature-flag table for opt-in capabilities."],
]
story.append(table_grid(entities, [3.6 * cm, 11.4 * cm]))
story.append(PageBreak())


# ---------------- 8.3 Database Schema Overview ----------------
story.append(Paragraph("8.3 Database Schema Overview", styles["HMSH2"]))
story.append(p(
    "Figure 14 below presents a colour-coded overview of the tables that make up the "
    "HMS database. Each colour groups tables that belong to the same logical sub-domain "
    "(identity, patient/doctor master data, scheduling, clinical records, billing & "
    "pharmacy, administration)."
))
story.extend(img("fig_db_map.png", width=13.5 * cm,
                 caption="Figure 14 — Database schema overview by logical sub-domain."))

story.append(Paragraph("8.4 Important Design Decisions", styles["HMSH2"]))
story.extend(bullets([
    "<b>Soft-delete</b> via <i>IsActive</i> instead of physical removal. This preserves "
    "audit trails and prevents orphaned foreign keys.",
    "<b>DeleteBehavior.Restrict</b> on most relationships. Cascading deletes are "
    "explicitly avoided to keep medical history intact.",
    "<b>Indexes</b> on every business key (e.g. <i>NationalId</i> on Patient, "
    "<i>LicenseNumber</i> on Doctor) and on every foreign key, configured "
    "fluently in <i>HospitalDbContext.OnModelCreating</i>.",
    "<b>Decimal precision</b> explicitly set on monetary columns "
    "(<i>HasColumnType(\"decimal(18,2)\")</i>) to avoid implicit conversions.",
    "<b>Seed data</b> for the admin user, default roles, departments and rooms is "
    "loaded the first time the application starts.",
]))

story.append(Paragraph("8.5 Migrations", styles["HMSH2"]))
story.append(p(
    "Schema changes are managed exclusively through EF Core migrations. The day-to-day "
    "workflow is:"
))
story.append(code(
    "# 1) Add a migration after editing a model\n"
    "dotnet ef migrations add <DescriptiveName>\n"
    "\n"
    "# 2) Apply it to the local database\n"
    "dotnet ef database update\n"
    "\n"
    "# 3) Roll back if needed\n"
    "dotnet ef database update <PreviousMigrationName>"
))
story.append(Spacer(1, 0.3 * cm))


# ---------------- 9. ERD ----------------
story.append(Paragraph("9. Entity-Relationship Diagram", styles["HMSH1"]))
story.append(hr())
story.append(p(
    "The Entity-Relationship Diagram below depicts the persistent model of the HMS "
    "backend. It shows every primary entity, the foreign keys that bind them and the "
    "cardinality of each relationship. Identity-related tables (<i>AspNetUsers</i>, "
    "<i>AspNetRoles</i>, …) are summarised under the single <i>User</i> box for "
    "readability."
))
story.extend(img("fig_erd.png", width=14.5 * cm,
                 caption="Figure 3 — Entity-Relationship Diagram of the HMS database."))

story.append(Paragraph("9.1 Reading the Diagram", styles["HMSH2"]))
story.extend(bullets([
    "Each box represents a single database table. The dark band at the top is the "
    "table name; the body lists primary and foreign keys plus the most relevant "
    "attributes.",
    "Lines between boxes represent foreign-key relationships. Annotations such as "
    "<i>1..N</i> indicate cardinality.",
    "All foreign keys are <i>ON DELETE RESTRICT</i>; clinical history cannot be "
    "accidentally lost by deleting a parent record.",
]))
story.append(PageBreak())


# ---------------- 10. ENTITY REFERENCE ----------------
story.append(Paragraph("10. Domain Entities Reference", styles["HMSH1"]))
story.append(hr())
story.append(p(
    "This section documents the most important columns of each entity in tabular form. "
    "Only business-relevant columns are listed; <i>BaseEntity</i> columns (<i>Id, "
    "CreatedAt, UpdatedAt, IsActive</i>) are implicit on every table."
))

story.append(Paragraph("10.1 Patient", styles["HMSH2"]))
story.append(table_grid([
    ["Column", "Type", "Notes"],
    ["FirstName, LastName",     "nvarchar(100)", "Required."],
    ["NationalId",              "nvarchar(20)",  "Unique index."],
    ["Email",                   "nvarchar(256)", "Required, indexed."],
    ["PhoneNumber, PhoneNumber2","nvarchar(20)", "Primary + optional secondary."],
    ["DateOfBirth",             "datetime2",     "Used for age computation."],
    ["Gender",                  "nvarchar(10)",  "Male / Female / Other."],
    ["Address",                 "nvarchar(500)", "Free text."],
    ["EmergencyContactName/Phone","nvarchar",    "Nullable."],
    ["InsuranceProvider/Number","nvarchar",      "Nullable; supports unbilled patients."],
    ["MedicalHistory, Allergies, CurrentMedications","nvarchar(500)","Clinical context."],
    ["UserId",                  "nvarchar",      "FK to AspNetUsers."],
], [4.4 * cm, 3.2 * cm, 7.4 * cm]))

story.append(Spacer(1, 0.3 * cm))
story.append(Paragraph("10.2 Doctor", styles["HMSH2"]))
story.append(table_grid([
    ["Column", "Type", "Notes"],
    ["FirstName, LastName",  "nvarchar(100)", "Required."],
    ["LicenseNumber",        "nvarchar(50)",  "Unique."],
    ["Specialization",       "nvarchar(100)", "e.g. Cardiology."],
    ["Qualifications",       "nvarchar(500)", "Education / certifications."],
    ["ExperienceYears",      "int",           "Years of practice."],
    ["ConsultationFee",      "decimal(18,2)", "Default fee per session."],
    ["DepartmentId",         "int",           "FK to Departments."],
], [4.4 * cm, 3.2 * cm, 7.4 * cm]))

story.append(Spacer(1, 0.3 * cm))
story.append(Paragraph("10.3 Appointment", styles["HMSH2"]))
story.append(table_grid([
    ["Column", "Type", "Notes"],
    ["PatientId, DoctorId, RoomId", "int", "Foreign keys."],
    ["AppointmentDate",  "datetime2", "Calendar date of the appointment."],
    ["StartTime, EndTime","time",     "Slot boundaries."],
    ["Status", "nvarchar(20)", "Pending, Confirmed, InProgress, Completed, Cancelled, NoShow."],
    ["Notes",  "nvarchar(500)", "Optional free text."],
], [4.4 * cm, 3.2 * cm, 7.4 * cm]))

story.append(Spacer(1, 0.3 * cm))

# ---------------- 10.4 Bill / Medicine ----------------
story.append(Paragraph("10.4 Bill", styles["HMSH2"]))
story.append(table_grid([
    ["Column", "Type", "Notes"],
    ["PatientId",      "int",            "FK to Patients."],
    ["BillDate",       "datetime2",      "Issuing date."],
    ["DueDate",        "datetime2",      "Payment deadline."],
    ["TotalAmount",    "decimal(18,2)",  "Sum of BillItems."],
    ["PaidAmount",     "decimal(18,2)",  "Amount actually collected."],
    ["Status",         "nvarchar(20)",   "Unpaid / PartiallyPaid / Paid / Overdue."],
    ["PaymentMethod",  "nvarchar(20)",   "Cash / Card / Stripe / PayPal / Paymob."],
], [4.4 * cm, 3.2 * cm, 7.4 * cm]))

story.append(Spacer(1, 0.3 * cm))
story.append(Paragraph("10.5 BillItem", styles["HMSH2"]))
story.append(table_grid([
    ["Column", "Type", "Notes"],
    ["BillId",       "int",           "FK to Bills."],
    ["Description",  "nvarchar(200)", "e.g. 'Cardiology Consultation'."],
    ["Quantity",     "int",           "Number of units."],
    ["UnitPrice",    "decimal(18,2)", "Per-unit price."],
    ["TotalPrice",   "decimal(18,2)", "Quantity × UnitPrice."],
], [4.4 * cm, 3.2 * cm, 7.4 * cm]))

story.append(Spacer(1, 0.3 * cm))
story.append(Paragraph("10.6 Medicine", styles["HMSH2"]))
story.append(table_grid([
    ["Column", "Type", "Notes"],
    ["Name, GenericName",   "nvarchar(100)", "Display + chemical name."],
    ["Manufacturer",        "nvarchar(100)", "Producer / brand."],
    ["Category",            "nvarchar(50)",  "Antibiotic, Analgesic, …"],
    ["DosageForm, Strength","nvarchar(50)",  "e.g. 'Tablet 500 mg'."],
    ["UnitPrice",           "decimal(18,2)", "Retail price."],
    ["StockQuantity",       "int",           "Current inventory."],
    ["ReorderLevel",        "int",           "Threshold for reorder alerts."],
    ["ExpiryDate",          "datetime2",     "Used by expiry-tracking jobs."],
], [4.4 * cm, 3.2 * cm, 7.4 * cm]))

story.append(Spacer(1, 0.3 * cm))
story.append(Paragraph("10.7 Prescription / PrescriptionItem", styles["HMSH2"]))
story.append(table_grid([
    ["Column", "Type", "Notes"],
    ["PatientId, DoctorId, MedicalRecordId","int","Foreign keys."],
    ["PrescriptionDate",  "datetime2",     "Date written."],
    ["Status",            "nvarchar(20)",  "Pending, Dispensed, Cancelled."],
    ["Items: MedicineId", "int",           "FK to Medicines."],
    ["Items: Quantity",   "int",           "Units prescribed."],
    ["Items: Dosage",     "nvarchar(50)",  "e.g. '20 mg'."],
    ["Items: Frequency",  "nvarchar(50)",  "e.g. 'twice daily'."],
    ["Items: Duration",   "nvarchar(50)",  "e.g. '7 days'."],
], [4.4 * cm, 3.2 * cm, 7.4 * cm]))
story.append(PageBreak())


# ---------------- 11. (Component diagram already shown - skip) ----------------
# Renumber: 11 will be Class Diagram, 12 was Class, 13 use-case
# Let's reuse: Section 11 Class Diagram
story.append(Paragraph("11. Class Diagram (Domain Model)", styles["HMSH1"]))
story.append(hr())
story.append(p(
    "The class diagram below presents an object-oriented view of the domain. It shows "
    "the most important domain classes, their attributes and the behaviour they expose. "
    "All entities inherit from <i>BaseEntity</i>, which is the only abstract class in the "
    "model. Methods such as <i>Confirm()</i>, <i>Cancel()</i>, <i>Complete()</i> on "
    "<i>Appointment</i> are mapped to state transitions enforced by "
    "<i>AppointmentService</i>."
))
story.extend(img("fig_class.png", width=14.5 * cm,
                 caption="Figure 11 — Simplified class diagram of the HMS domain model."))

story.append(Paragraph("11.1 Inheritance", styles["HMSH2"]))
story.append(p(
    "Inheritance is used very sparingly. <i>BaseEntity</i> exists only to factor out "
    "auditing columns. All other relationships are expressed through composition and "
    "navigation properties rather than class hierarchies. This is intentional: deep "
    "inheritance in a persistence model tends to interact poorly with EF Core's "
    "table-per-hierarchy and table-per-type strategies."
))

story.append(Paragraph("11.2 Behaviour vs. Anaemia", styles["HMSH2"]))
story.append(p(
    "Entities in this project carry primarily data, while orchestration logic lives in "
    "the corresponding services. This is sometimes referred to as an <i>anaemic domain "
    "model</i>. The trade-off is deliberate: services are easier to unit-test and to "
    "share between several controllers, and they keep transactional boundaries explicit. "
    "When a behaviour is intrinsic to a single entity (e.g. recomputing the total of a "
    "bill after an item is added), it is implemented as a method on the entity."
))
story.append(PageBreak())


# ---------------- 12. USE-CASE ----------------
story.append(Paragraph("12. Use-Case Diagram", styles["HMSH1"]))
story.append(hr())
story.append(p(
    "The use-case diagram summarises the actors that interact with the system and the "
    "high-level capabilities exposed to each of them. The same physical user may "
    "actually own multiple actor roles (for example, a doctor who also holds a "
    "receptionist account for the front desk) — the diagram refers to roles, not "
    "individuals."
))
story.extend(img("fig_usecase.png", width=13.5 * cm,
                 caption="Figure 6 — Actors and high-level use cases of the HMS."))

story.append(Paragraph("12.1 Actors", styles["HMSH2"]))
story.extend(bullets([
    "<b>Admin</b> — Full access. Creates other users, configures departments, manages "
    "the pharmacy catalogue and reviews global KPIs from the dashboard.",
    "<b>Doctor</b> — Sees their own patients, fills in medical records, prescribes "
    "medicines and reads appointment schedules.",
    "<b>Receptionist</b> — Creates appointments and bills on behalf of patients, "
    "registers payments at the front desk.",
    "<b>Patient</b> — Self-service: registers, logs in, books appointments and views "
    "personal medical history and bills.",
]))

story.append(Paragraph("12.2 Authorization Mapping", styles["HMSH2"]))
story.append(p(
    "Each use case is implemented by one or more REST endpoints and protected with the "
    "appropriate <i>[Authorize(Roles=…)]</i> attribute. The mapping is enforced by ASP.NET "
    "Identity, which inspects the role claims contained in the JWT issued at login."
))
story.append(Spacer(1, 0.3 * cm))


# ---------------- 13. AUTH ----------------
story.append(Paragraph("13. Authentication & Authorization", styles["HMSH1"]))
story.append(hr())
story.append(Paragraph("13.1 Strategy Overview", styles["HMSH2"]))
story.append(p(
    "Authentication in HMS is stateless and based on <b>JSON Web Tokens (JWT)</b>. "
    "Credentials are presented exactly once, at the <i>POST /api/auth/login</i> "
    "endpoint, in exchange for a signed token. The token is then attached to every "
    "subsequent request through the <i>Authorization: Bearer &lt;token&gt;</i> header. "
    "No server-side session state is kept, which makes the API trivially horizontally "
    "scalable."
))

story.append(Paragraph("13.2 Identity Configuration", styles["HMSH2"]))
story.append(p(
    "User and role storage is delegated to ASP.NET Identity, which writes to the same "
    "SQL Server database used by the rest of the application. The password policy "
    "configured for HMS is:"
))
story.append(code(
    "options.Password.RequireDigit          = true;\n"
    "options.Password.RequiredLength        = 8;\n"
    "options.Password.RequireUppercase      = true;\n"
    "options.Password.RequireLowercase      = true;\n"
    "options.Password.RequireNonAlphanumeric= false;\n"
    "options.User.RequireUniqueEmail        = true;"
))

story.append(Paragraph("13.3 JWT Configuration", styles["HMSH2"]))
story.append(p(
    "JWT signing uses HS256 with a secret key sourced from configuration. Issuer, "
    "audience, lifetime and signing key are all validated on every request:"
))
story.append(code(
    "options.TokenValidationParameters = new TokenValidationParameters\n"
    "{\n"
    "    ValidateIssuer           = true,\n"
    "    ValidateAudience         = true,\n"
    "    ValidateLifetime         = true,\n"
    "    ValidateIssuerSigningKey = true,\n"
    "    ValidIssuer              = jwt[\"Issuer\"],\n"
    "    ValidAudience            = jwt[\"Audience\"],\n"
    "    IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))\n"
    "};"
))

story.append(Paragraph("13.4 Roles", styles["HMSH2"]))
story.append(p(
    "Four roles are defined and seeded the first time the application starts: <b>Admin</b>, "
    "<b>Doctor</b>, <b>Receptionist</b> and <b>Patient</b>. They are stored in the "
    "<i>AspNetRoles</i> table and assigned to users through <i>AspNetUserRoles</i>. "
    "Each role is asserted on the controller side via <i>[Authorize(Roles=\"…\")]</i>."
))
story.append(PageBreak())


# ---------------- 14. AUTH SEQUENCE ----------------
story.append(Paragraph("14. Authentication Sequence", styles["HMSH1"]))
story.append(hr())
story.append(p(
    "The sequence diagram below details the interactions that take place between the "
    "client, the AuthController, the ASP.NET Identity <i>UserManager</i>, the JWT "
    "service helper and the underlying database when a user attempts to log in. The "
    "scenario depicted is the <i>happy path</i> where the credentials match an existing "
    "active user."
))
story.extend(img("fig_auth_sequence.png", width=14 * cm,
                 caption="Figure 4 — JWT login sequence."))

story.append(Paragraph("14.1 Failure Paths", styles["HMSH2"]))
story.extend(bullets([
    "<b>Email not found.</b> <i>FindByEmailAsync</i> returns <i>null</i>; the controller "
    "responds with HTTP <i>401 Unauthorized</i> and a generic message — the exact "
    "reason is intentionally not leaked.",
    "<b>Wrong password.</b> <i>CheckPasswordAsync</i> returns <i>false</i>; same "
    "401 response.",
    "<b>Rate limit exceeded.</b> The <i>auth-login</i> token bucket rejects the request "
    "with HTTP <i>429 Too Many Requests</i> before the controller is invoked.",
    "<b>Locked account.</b> ASP.NET Identity automatically locks the account after "
    "repeated failed attempts (configurable in <i>appsettings.json</i>).",
]))

story.append(Paragraph("14.2 Token Lifetime & Renewal", styles["HMSH2"]))
story.append(p(
    "Tokens are short-lived (60 minutes by default) to mitigate the impact of a leaked "
    "token. Refresh tokens are not used; instead the front-end forces a fresh login "
    "when the token expires. This trade-off favours simplicity over UX continuity and is "
    "appropriate for a hospital intranet."
))

story.append(Paragraph("14.3 Registration Activity", styles["HMSH2"]))
story.append(p(
    "Figure 12 below shows the alternative flow for new-user registration. It follows "
    "the same validation-then-persistence pattern but performs additional role "
    "assignment afterwards."
))
story.extend(img("fig_registration_activity.png", width=12 * cm,
                 caption="Figure 12 — User registration activity diagram."))
story.append(Spacer(1, 0.3 * cm))


# ---------------- 15. APPOINTMENT BOOKING ----------------
story.append(Paragraph("15. Appointment Booking Workflow", styles["HMSH1"]))
story.append(hr())
story.append(p(
    "Booking an appointment is the most frequent write operation against the HMS "
    "backend, and also the one with the most complex validation. The system must ensure "
    "that the requested doctor is available, that the slot does not collide with an "
    "existing appointment and that the assigned room is free at the same time. "
    "All of this is encapsulated inside <i>AppointmentService.CreateAsync</i>, "
    "which is invoked by the controller after model-state validation."
))
story.extend(img("fig_appointment_sequence.png", width=14.5 * cm,
                 caption="Figure 5 — End-to-end sequence for booking an appointment."))

story.append(Paragraph("15.1 Slot Computation", styles["HMSH2"]))
story.append(p(
    "Availability is computed by intersecting the doctor's weekly <i>Schedule</i> "
    "(defined in the <i>Schedules</i> table) with the set of appointments already "
    "registered for the same day. The first 30-minute opening is offered to the client "
    "as a candidate; the client may then pick a later slot from the returned list."
))

story.append(Paragraph("15.2 Concurrency", styles["HMSH2"]))
story.append(p(
    "To avoid double-booking when two clients try to grab the same slot simultaneously, "
    "the service performs the availability check and the insertion inside a single "
    "database transaction. A SQL Server unique filtered index on "
    "(<i>DoctorId, AppointmentDate, StartTime</i>) acts as the last line of defence: "
    "even if a race condition slips through the service layer, the database will reject "
    "the duplicate."
))

story.append(Paragraph("15.3 Cancellation and Completion", styles["HMSH2"]))
story.append(p(
    "Two dedicated endpoints — <i>PUT /api/appointments/{id}/cancel</i> and "
    "<i>PUT /api/appointments/{id}/complete</i> — are exposed for status transitions. "
    "Both invoke <i>AppointmentService</i> methods that validate the current state "
    "before transitioning, in accordance with the state diagram in Section 17."
))
story.append(PageBreak())


# ---------------- 16. BILL FLOW ----------------
story.append(Paragraph("16. Billing & Payment Flowchart", styles["HMSH1"]))
story.append(hr())
story.append(p(
    "Billing is the second-most complex workflow in the platform. A bill is built "
    "incrementally as services are delivered to a patient — consultations, lab tests, "
    "drugs dispensed by the pharmacy and room charges all generate line items. The "
    "controller exposes three operations: <i>create</i>, <i>add item</i> and "
    "<i>register payment</i>. The flowchart below tracks a bill from creation through "
    "completion."
))
story.extend(img("fig_bill_flow.png", width=12 * cm,
                 caption="Figure 8 — Bill creation and payment flowchart."))

story.append(Paragraph("16.1 Payment Gateways", styles["HMSH2"]))
story.append(p(
    "Three external payment gateways are pre-integrated: <b>Stripe</b> (default), "
    "<b>PayPal</b> and <b>Paymob</b> (for local Egyptian payments). They sit behind a "
    "common <i>IPaymentGatewayService</i> interface; switching providers is a single "
    "registration line in <i>Program.cs</i>:"
))
story.append(code(
    "builder.Services.AddScoped<IPaymentGatewayService, StripePaymentService>();\n"
    "builder.Services.AddScoped<PayPalPaymentService>();\n"
    "builder.Services.AddHttpClient<PaymobPaymentService>();"
))

story.append(Paragraph("16.2 Insurance", styles["HMSH2"]))
story.append(p(
    "When a patient has an active insurance entry (<i>InsuranceProvider</i> + "
    "<i>InsuranceNumber</i>), the bill is flagged accordingly. The actual reimbursement "
    "process is delegated to the hospital's accounting back-office and is out of scope "
    "for the digital workflow described here — but the data is captured to make it "
    "straightforward in the future."
))
story.append(Spacer(1, 0.3 * cm))


# ---------------- 17. STATE DIAGRAM ----------------
story.append(Paragraph("17. Appointment State Diagram", styles["HMSH1"]))
story.append(hr())
story.append(p(
    "Appointments traverse a constrained set of states. The state machine below "
    "documents every legal transition. Any attempt to move from a state to a non-adjacent "
    "state is rejected by <i>AppointmentService</i> with a <i>400 Bad Request</i> "
    "response."
))
story.extend(img("fig_state.png", width=12.5 * cm,
                 caption="Figure 7 — Appointment state diagram."))

story.append(Paragraph("17.1 Transitions Explained", styles["HMSH2"]))
story.extend(bullets([
    "<b>Pending → Confirmed.</b> The receptionist or the doctor approves the booking.",
    "<b>Pending / Confirmed → Cancelled.</b> Triggered either by the patient (within the "
    "allowed window) or by the staff.",
    "<b>Confirmed → InProgress.</b> The patient is checked in at the front desk.",
    "<b>Confirmed → NoShow.</b> A scheduled background job promotes appointments that "
    "passed their start time without a check-in.",
    "<b>InProgress → Completed.</b> The doctor finalises the appointment, "
    "typically after writing a Medical Record and (optionally) a Prescription.",
]))

story.append(Paragraph("17.2 Side Effects", styles["HMSH2"]))
story.append(p(
    "State transitions are not just record updates; they also trigger downstream "
    "side-effects that the service layer orchestrates:"
))
story.extend(bullets([
    "Transitioning to <b>Cancelled</b> frees the room and the doctor's slot so they "
    "become available for re-booking.",
    "Transitioning to <b>Completed</b> can automatically generate a draft Bill containing "
    "the consultation fee.",
    "Transitioning to <b>NoShow</b> may flag the patient for follow-up by the front desk.",
]))
story.append(PageBreak())


# ---------------- 18. REST API REFERENCE ----------------
story.append(Paragraph("18. REST API Reference", styles["HMSH1"]))
story.append(hr())
story.append(p(
    "The backend exposes more than 80 REST endpoints across 22 controllers. Below is "
    "a curated summary of the most important ones, grouped by resource. The full, "
    "machine-readable specification is available at <i>/swagger/v1/swagger.json</i> at "
    "runtime."
))

def endpoint_table(rows):
    data = [["Method", "Route", "Auth", "Purpose"]] + rows
    t = table_grid(data, [1.6 * cm, 6.4 * cm, 2.0 * cm, 5.8 * cm], font_size=8.5)
    return t

story.append(Paragraph("18.1 Authentication", styles["HMSH2"]))
story.append(endpoint_table([
    ["POST",   "/api/auth/register",        "Public", "Register a new patient account."],
    ["POST",   "/api/auth/login",           "Public", "Authenticate and receive a JWT."],
    ["POST",   "/api/auth/change-password", "User",   "Update the current user's password."],
    ["GET",    "/api/auth/profile",         "User",   "Return the authenticated user profile."],
]))

story.append(Spacer(1, 0.2 * cm))
story.append(Paragraph("18.2 Patients", styles["HMSH2"]))
story.append(endpoint_table([
    ["GET",    "/api/patients",              "Staff",  "Paged list with optional search filter."],
    ["GET",    "/api/patients/{id}",         "Staff",  "Retrieve a patient by id."],
    ["POST",   "/api/patients",              "Staff",  "Create a new patient record."],
    ["PUT",    "/api/patients/{id}",         "Staff",  "Update an existing patient record."],
    ["DELETE", "/api/patients/{id}",         "Admin",  "Soft-delete a patient."],
]))

story.append(Spacer(1, 0.2 * cm))
story.append(Paragraph("18.3 Doctors & Departments", styles["HMSH2"]))
story.append(endpoint_table([
    ["GET",  "/api/doctors",                 "User", "List active doctors with department & rating."],
    ["POST", "/api/doctors",                 "Admin","Onboard a new doctor."],
    ["GET",  "/api/doctors/{id}/schedule",   "User", "Retrieve doctor's weekly availability."],
    ["GET",  "/api/departments",             "User", "List departments."],
    ["POST", "/api/departments",             "Admin","Create a department."],
    ["GET",  "/api/rooms",                   "Staff","List rooms (optionally filter by dept)."],
]))

story.append(Spacer(1, 0.2 * cm))
story.append(Paragraph("18.4 Appointments", styles["HMSH2"]))
story.append(endpoint_table([
    ["GET",  "/api/appointments",            "Staff","Paged list with status & date filters."],
    ["GET",  "/api/appointments/{id}",       "Staff","Retrieve a single appointment."],
    ["POST", "/api/appointments",            "User", "Book a new appointment."],
    ["PUT",  "/api/appointments/{id}",       "Staff","Reschedule / update an appointment."],
    ["PUT",  "/api/appointments/{id}/cancel","User", "Cancel an existing appointment."],
    ["PUT",  "/api/appointments/{id}/complete","Doctor","Mark as completed after the visit."],
]))
story.append(PageBreak())


# ---------------- 18.5 Medical Records / Prescriptions / Bills / Medicines ----------------
story.append(Paragraph("18.5 Medical Records & Prescriptions", styles["HMSH2"]))
story.append(endpoint_table([
    ["GET",  "/api/medicalrecords",           "Staff", "Paged list of medical records."],
    ["GET",  "/api/medicalrecords/{id}",      "Staff", "Retrieve a single medical record."],
    ["POST", "/api/medicalrecords",           "Doctor","Create a record from an appointment."],
    ["PUT",  "/api/medicalrecords/{id}",      "Doctor","Update diagnosis / treatment."],
    ["GET",  "/api/prescriptions",            "Staff", "List prescriptions (per patient/doctor)."],
    ["POST", "/api/prescriptions",            "Doctor","Issue a new prescription."],
    ["PUT",  "/api/prescriptions/{id}/dispense","Staff","Dispense an active prescription."],
]))

story.append(Spacer(1, 0.2 * cm))
story.append(Paragraph("18.6 Billing & Payment", styles["HMSH2"]))
story.append(endpoint_table([
    ["GET",  "/api/bills",                    "Staff", "Paged list with status filter."],
    ["GET",  "/api/bills/{id}",               "Staff", "Retrieve a single bill (with items)."],
    ["POST", "/api/bills",                    "Staff", "Create a new bill."],
    ["POST", "/api/bills/{id}/payment",       "Staff", "Register a (partial) payment."],
    ["POST", "/api/payment/checkout/{billId}","Patient","Open a hosted Stripe/PayPal checkout."],
    ["POST", "/api/payment/webhook",          "Public","External gateway notification endpoint."],
]))

story.append(Spacer(1, 0.2 * cm))
story.append(Paragraph("18.7 Pharmacy & Inventory", styles["HMSH2"]))
story.append(endpoint_table([
    ["GET",    "/api/medicines",             "Staff", "Paged list with low-stock filter."],
    ["GET",    "/api/medicines/{id}",        "Staff", "Retrieve a single medicine."],
    ["POST",   "/api/medicines",             "Admin", "Add a new medicine to the catalogue."],
    ["PUT",    "/api/medicines/{id}",        "Admin", "Update a medicine."],
    ["DELETE", "/api/medicines/{id}",        "Admin", "Soft-delete a medicine."],
]))

story.append(Spacer(1, 0.2 * cm))
story.append(Paragraph("18.8 Dashboard, Chatbot & X-Ray AI", styles["HMSH2"]))
story.append(endpoint_table([
    ["GET",  "/api/dashboard/stats",          "Staff","Aggregated KPIs."],
    ["GET",  "/api/dashboard/recent-appointments","Staff","Last N appointments."],
    ["GET",  "/api/dashboard/revenue",        "Admin","Revenue breakdown by service."],
    ["POST", "/api/chatbot/ask",              "User","Ask the medical chatbot."],
    ["POST", "/api/xrayai/analyze",           "Doctor","Upload an X-Ray for AI analysis."],
    ["GET",  "/api/health",                   "Public","Liveness probe."],
]))

story.append(Spacer(1, 0.3 * cm))
story.append(Paragraph("18.9 Common Response Format", styles["HMSH2"]))
story.append(p(
    "All non-error endpoints return JSON envelopes shaped as the corresponding DTO. "
    "Errors follow the RFC 7807 Problem-Details specification. A typical 400 looks like:"
))
story.append(code(
    "{\n"
    "  \"type\": \"https://tools.ietf.org/html/rfc7231#section-6.5.1\",\n"
    "  \"title\": \"One or more validation errors occurred.\",\n"
    "  \"status\": 400,\n"
    "  \"errors\": {\n"
    "    \"Email\": [ \"The Email field is required.\" ]\n"
    "  },\n"
    "  \"instance\": \"/api/auth/register\"\n"
    "}"
))
story.append(PageBreak())


# ---------------- 19. MOCK SCREENS ----------------
story.append(Paragraph("19. Mock Screens (UI consuming the API)", styles["HMSH1"]))
story.append(hr())
story.append(p(
    "Although this document focuses on the backend, it is useful to illustrate the "
    "look-and-feel of the clients that consume the REST API. This section gathers "
    "representative screen mock-ups for the most frequently used flows. Every screen "
    "corresponds to one or more API endpoints described in Section 18."
))

story.append(Paragraph("19.1 Swagger / OpenAPI Explorer", styles["HMSH2"]))
story.append(p(
    "The Swagger UI ships with the backend and is mounted at <i>/swagger</i>. It is the "
    "fastest way for developers to discover endpoints, inspect request and response "
    "schemas and try the API live with a valid JWT."
))
story.extend(img("screen_swagger.png", width=13.5 * cm,
                 caption="Screen 1 — Swagger UI listing of the public endpoints."))
story.append(Spacer(1, 0.3 * cm))


story.append(Paragraph("19.2 Login Screen", styles["HMSH2"]))
story.append(p(
    "The login screen submits credentials to <i>POST /api/auth/login</i>. Successful "
    "authentication stores the returned JWT in local storage on web and in secure "
    "storage on mobile, then redirects the user to the dashboard."
))
story.extend(img("screen_login.png", width=12.5 * cm,
                 caption="Screen 2 — Login screen (web client)."))

story.append(Spacer(1, 0.4 * cm))
story.append(Paragraph("19.3 Admin Dashboard", styles["HMSH2"]))
story.append(p(
    "The dashboard aggregates the metrics returned by <i>GET /api/dashboard/stats</i>, "
    "<i>/recent-appointments</i> and <i>/revenue</i>. Cards show counts and revenue, "
    "the bar chart visualises appointments over the last seven days, and the pie chart "
    "splits revenue by service type."
))
story.extend(img("screen_dashboard.png", width=13.5 * cm,
                 caption="Screen 3 — Hospital administrator dashboard."))
story.append(Spacer(1, 0.3 * cm))


story.append(Paragraph("19.4 Patients List", styles["HMSH2"]))
story.append(p(
    "The patients list demonstrates the standard CRUD UI offered by the platform. "
    "Server-side paging is performed by <i>GET /api/patients?page=…&amp;size=…</i> and "
    "search filters are forwarded as query-string parameters."
))
story.extend(img("screen_patients.png", width=14 * cm,
                 caption="Screen 4 — Patients management screen with search & paging."))

story.append(Paragraph("19.5 Book Appointment", styles["HMSH2"]))
story.append(p(
    "When the user picks a department and a doctor, the front-end calls "
    "<i>GET /api/doctors/{id}/schedule?date=…</i> to compute and display the available "
    "slots. Selecting a slot and submitting the form triggers a "
    "<i>POST /api/appointments</i>."
))
story.extend(img("screen_appointment.png", width=11.5 * cm,
                 caption="Screen 5 — Appointment booking form."))
story.append(Spacer(1, 0.3 * cm))


story.append(Paragraph("19.6 Billing & Invoice", styles["HMSH2"]))
story.append(p(
    "The billing screen consolidates a patient's services and items into a single "
    "invoice document. The <i>Pay Now</i> button initiates a Stripe / PayPal / Paymob "
    "checkout depending on the patient's selected method; the result is communicated "
    "back to the backend through the <i>/api/payment/webhook</i> endpoint."
))
story.extend(img("screen_billing.png", width=14 * cm,
                 caption="Screen 6 — Detailed invoice with line items and payment actions."))

story.append(Paragraph("19.7 Client / Server Interaction", styles["HMSH2"]))
story.append(p(
    "Every screen above ultimately submits an HTTP request to the backend through a "
    "thin axios (web) or Dio (mobile) HTTP client. A small interceptor attaches the "
    "JWT to outgoing requests, retries idempotent calls on transient failures and "
    "redirects to the login screen when the server returns 401. None of this is "
    "visible to the rest of the UI codebase, which always interacts with the API "
    "through a single, strongly-typed client object."
))
story.append(Spacer(1, 0.3 * cm))


# ---------------- 20. DEPLOYMENT ----------------
story.append(Paragraph("20. Deployment & Hosting", styles["HMSH1"]))
story.append(hr())
story.append(p(
    "The backend is designed to run anywhere ASP.NET Core 8.0 is supported: Windows "
    "Server, Linux containers or a managed cloud service such as Azure App Service. "
    "The reference topology used by the hospital is illustrated in Figure 9 below."
))
story.extend(img("fig_deployment.png", width=14 * cm,
                 caption="Figure 9 — Reference deployment topology."))

story.append(Paragraph("20.1 Application Server", styles["HMSH2"]))
story.append(p(
    "The application server hosts ASP.NET Core through Kestrel, behind either IIS "
    "(on Windows) or an Nginx reverse proxy (on Linux). HTTPS termination is performed "
    "by the reverse proxy. The server also writes daily rolling log files into "
    "<i>logs/</i>, which can be shipped to a centralised log aggregator (Seq, ELK, "
    "Azure Application Insights, …)."
))

story.append(Paragraph("20.2 Database Server", styles["HMSH2"]))
story.append(p(
    "SQL Server 2022 is the default database engine. Daily backups, transaction-log "
    "shipping and a regular integrity check (DBCC CHECKDB) are configured at the "
    "instance level. EF Core migrations are applied during the deployment phase "
    "(<i>dotnet ef database update</i>)."
))

story.append(Paragraph("20.3 External Services", styles["HMSH2"]))
story.extend(bullets([
    "<b>SMTP</b> — Used by the <i>EmailService</i> to deliver transactional emails. "
    "Credentials are stored in user secrets / environment variables, never in source.",
    "<b>Payment gateways</b> — Stripe, PayPal and Paymob credentials are configured "
    "per environment.",
    "<b>AI services</b> — OpenAI for the chatbot, custom inference for X-Ray. Both "
    "are optional and disabled by default.",
]))

story.append(Paragraph("20.4 Containerised Deployment", styles["HMSH2"]))
story.append(p(
    "The project ships with a <i>Dockerfile</i> and a <i>docker-compose.yml</i> that "
    "build a multi-stage image (SDK → publish → runtime) and pair it with a SQL Server "
    "container for local testing. The same image is suitable for production:"
))
story.append(code(
    "docker compose build\n"
    "docker compose up -d\n"
    "# Backend reachable at https://localhost:7102/swagger"
))
story.append(PageBreak())


# ---------------- 21. SECURITY OPS ----------------
story.append(Paragraph("21. Security, Logging & Operations", styles["HMSH1"]))
story.append(hr())
story.append(Paragraph("21.1 Defence-in-Depth", styles["HMSH2"]))
story.append(p(
    "Security inside HMS is approached as a series of overlapping layers. Even if one "
    "layer is bypassed, the others stop the attack before sensitive data leaks. The "
    "main layers are:"
))
story.extend(bullets([
    "<b>Transport.</b> TLS 1.2+ is mandatory; HTTPS is enforced by <i>UseHttpsRedirection()</i>.",
    "<b>Authentication.</b> JWT bearer tokens, signed with a symmetric key stored "
    "outside source control.",
    "<b>Authorization.</b> Role-based via <i>[Authorize(Roles=…)]</i>; sensitive endpoints "
    "also use policy-based authorization where ownership of the resource is required.",
    "<b>Input validation.</b> Both data-annotation attributes and FluentValidation "
    "rules are run on every DTO. DTOs intentionally omit sensitive fields like "
    "<i>UserId</i> to prevent over-posting.",
    "<b>Output encoding.</b> All responses are JSON; HTML injection at the API layer is "
    "structurally impossible.",
    "<b>SQL injection.</b> Prevented by EF Core parameterised queries.",
    "<b>Rate limiting.</b> The login endpoint is protected by a token-bucket policy.",
    "<b>CORS.</b> Only configured front-end origins are allowed to send credentials.",
    "<b>Secrets.</b> Connection strings and JWT secrets are pulled from environment "
    "variables or user-secrets; they are never committed to source control.",
]))

story.append(Paragraph("21.2 Logging Strategy", styles["HMSH2"]))
story.append(p(
    "Serilog is configured to write to both the console (helpful in development) and "
    "to a daily rolling file under <i>logs/</i>. Each log entry includes the HTTP "
    "verb, the path, the response status, the duration in milliseconds and, when "
    "available, the user id taken from the JWT. The same enriched events are exported to "
    "an external sink in production for long-term retention and dashboards."
))

story.append(Paragraph("21.3 Health Checks", styles["HMSH2"]))
story.append(p(
    "A lightweight <i>GET /api/health</i> endpoint returns a 200 status code with the "
    "current build hash. The endpoint is intentionally unprotected so that load "
    "balancers and orchestrators can use it without storing credentials."
))

story.append(Paragraph("21.4 Backups & Disaster Recovery", styles["HMSH2"]))
story.append(p(
    "The database is backed up nightly (full) and every fifteen minutes "
    "(transaction-log). Backups are encrypted and copied to off-site storage. "
    "The recovery point objective (RPO) is 15 minutes and the recovery time objective "
    "(RTO) is one hour. The application server is treated as cattle: a "
    "fresh deployment from CI can restore service in under ten minutes."
))
story.append(PageBreak())


# ---------------- 22. CONCLUSION ----------------
story.append(Paragraph("22. Conclusion & Future Work", styles["HMSH1"]))
story.append(hr())
story.append(Paragraph("22.1 Summary", styles["HMSH2"]))
story.append(p(
    "The Hospital Management System backend offers a complete, secure and extensible "
    "platform for the digital management of Al-Hayat Hospital. Built on ASP.NET Core "
    "8.0 and Entity Framework Core, the platform brings together identity management, "
    "appointment scheduling, electronic medical records, pharmacy inventory and a "
    "fully featured billing engine into a single coherent API. The layered architecture "
    "— controllers on top of services on top of EF Core — makes the codebase easy to "
    "test, easy to extend and easy to onboard new engineers into."
))
story.append(p(
    "The choices documented in the previous sections (JWT authentication, RESTful "
    "endpoints, AutoMapper-based DTOs, Serilog logging, Docker packaging) reflect a "
    "deliberate preference for industry-standard, low-friction tooling. They keep the "
    "system approachable to any .NET developer while leaving plenty of room for "
    "future growth."
))

story.append(Paragraph("22.2 Future Work", styles["HMSH2"]))
story.append(p(
    "A non-exhaustive backlog of improvements has emerged during the project and is "
    "tracked for upcoming iterations:"
))
story.extend(bullets([
    "<b>Refresh tokens.</b> Add a refresh-token endpoint so JWTs can be renewed "
    "transparently without forcing a new login.",
    "<b>Caching.</b> Introduce Redis to memoise expensive reads on the dashboard and "
    "the doctors / departments endpoints.",
    "<b>CQRS for reporting.</b> Split reads and writes for the dashboard to scale the "
    "two workloads independently and offer richer analytics.",
    "<b>HL7 / FHIR.</b> Expose a FHIR endpoint for interoperability with regional health "
    "information exchanges and other hospital information systems.",
    "<b>Real-time notifications.</b> Add SignalR hubs so the front-end can react "
    "instantly to appointment changes and bill updates.",
    "<b>Multi-tenancy.</b> Generalise the platform so several hospitals can share the "
    "same code base while keeping their data fully isolated.",
    "<b>Observability.</b> Wire up OpenTelemetry traces and metrics; export them to "
    "Prometheus + Grafana or Azure Application Insights.",
    "<b>Mobile push notifications.</b> Send appointment reminders to the Flutter app "
    "via Firebase Cloud Messaging.",
]))

story.append(Paragraph("22.3 Closing Remarks", styles["HMSH2"]))
story.append(p(
    "From a strategic point of view, the platform is now ready to support day-to-day "
    "operations of a mid-sized hospital and to serve as a foundation for the more "
    "ambitious data-driven initiatives outlined above. Continuous monitoring, incremental "
    "improvements and a strong test culture will keep it healthy and maintainable for "
    "many years to come."
))

story.append(Spacer(1, 1.0 * cm))
story.append(hr())
story.append(Paragraph("End of Document", styles["HMSCaption"]))
story.append(Paragraph(
    "© 2026 Al-Hayat Hospital — Hospital Management System — Backend Technical Documentation v1.0",
    styles["HMSFooter"]))


# ====================================================================
# Build PDF
# ====================================================================
doc = SimpleDocTemplate(OUT_PDF, pagesize=A4,
                        leftMargin=2 * cm, rightMargin=2 * cm,
                        topMargin=1.8 * cm, bottomMargin=2 * cm,
                        title="HMS Backend Documentation",
                        author="Al-Hayat Hospital — HMS Team")
doc.build(story, onFirstPage=on_page, onLaterPages=on_page)
print("PDF written to:", OUT_PDF)
