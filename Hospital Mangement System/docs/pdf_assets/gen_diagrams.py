# -*- coding: utf-8 -*-
"""Generate diagrams and mock screens for HMS backend documentation."""

import os
import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
from matplotlib.patches import FancyBboxPatch, FancyArrowPatch, Rectangle, Polygon
from matplotlib.lines import Line2D

OUT = os.path.dirname(os.path.abspath(__file__))

# Color palette (medical theme)
NAVY = "#0E3B5F"
TEAL = "#0E8A8A"
BLUE = "#1E6FB8"
LIGHT_BLUE = "#E3F2FD"
ACCENT = "#E74C3C"
GREEN = "#2E8B57"
ORANGE = "#F39C12"
GRAY = "#7F8C8D"
LIGHT_GRAY = "#ECF0F1"
WHITE = "#FFFFFF"
DARK = "#2C3E50"


def box(ax, x, y, w, h, text, fc=LIGHT_BLUE, ec=NAVY, fontcolor=DARK,
        fontsize=10, fontweight="bold", radius=0.04):
    p = FancyBboxPatch((x, y), w, h,
                       boxstyle=f"round,pad=0.02,rounding_size={radius}",
                       linewidth=1.4, edgecolor=ec, facecolor=fc)
    ax.add_patch(p)
    ax.text(x + w / 2, y + h / 2, text, ha="center", va="center",
            fontsize=fontsize, color=fontcolor, fontweight=fontweight, wrap=True)


def arrow(ax, x1, y1, x2, y2, color=NAVY, style="->", lw=1.6, text=None,
          text_offset=(0, 0.15), fontsize=8):
    arrowstyle = style
    linestyle = "-"
    if style == "-->":
        arrowstyle = "->"
        linestyle = "--"
    a = FancyArrowPatch((x1, y1), (x2, y2),
                        arrowstyle=arrowstyle, mutation_scale=14,
                        color=color, lw=lw, linestyle=linestyle)
    ax.add_patch(a)
    if text:
        mx, my = (x1 + x2) / 2 + text_offset[0], (y1 + y2) / 2 + text_offset[1]
        ax.text(mx, my, text, ha="center", va="center",
                fontsize=fontsize, color=color, fontstyle="italic")


def title(ax, t):
    ax.set_title(t, fontsize=14, fontweight="bold", color=NAVY, pad=12)


def setup(ax, xlim, ylim):
    ax.set_xlim(*xlim)
    ax.set_ylim(*ylim)
    ax.set_aspect("equal")
    ax.axis("off")


def save(fig, name):
    fig.tight_layout()
    path = os.path.join(OUT, name)
    fig.savefig(path, dpi=170, bbox_inches="tight", facecolor=WHITE)
    plt.close(fig)
    print("Saved", path)


# ---------------- Architecture (Layered) ----------------
def diagram_architecture():
    fig, ax = plt.subplots(figsize=(11, 8))
    setup(ax, (0, 12), (0, 10))
    title(ax, "Figure 1 — Three-Tier Layered Architecture")

    # Client layer
    box(ax, 0.5, 8.0, 11, 1.5, "PRESENTATION / CLIENT LAYER", fc=NAVY,
        fontcolor=WHITE, fontsize=11)
    box(ax, 1.0, 8.2, 2.8, 1.1, "Web Browser\n(React.js SPA)", fc=LIGHT_BLUE, fontsize=9)
    box(ax, 4.2, 8.2, 2.8, 1.1, "Mobile App\n(Flutter)", fc=LIGHT_BLUE, fontsize=9)
    box(ax, 7.4, 8.2, 2.8, 1.1, "Swagger UI\n(API Explorer)", fc=LIGHT_BLUE, fontsize=9)

    # API layer
    box(ax, 0.5, 4.5, 11, 3.0, "APPLICATION / API LAYER  —  ASP.NET Core 8.0", fc=TEAL,
        fontcolor=WHITE, fontsize=11)
    box(ax, 1.0, 6.0, 2.3, 0.9, "Controllers\n(REST endpoints)", fc=WHITE, fontsize=8.5)
    box(ax, 3.6, 6.0, 2.3, 0.9, "JWT Auth /\nIdentity", fc=WHITE, fontsize=8.5)
    box(ax, 6.2, 6.0, 2.3, 0.9, "Middlewares\n(CORS, Rate Limit)", fc=WHITE, fontsize=8.5)
    box(ax, 8.8, 6.0, 2.4, 0.9, "Swagger /\nOpenAPI", fc=WHITE, fontsize=8.5)
    box(ax, 1.0, 4.8, 2.3, 0.9, "Services\n(Business Logic)", fc=WHITE, fontsize=8.5)
    box(ax, 3.6, 4.8, 2.3, 0.9, "DTOs +\nAutoMapper", fc=WHITE, fontsize=8.5)
    box(ax, 6.2, 4.8, 2.3, 0.9, "Validation\n(FluentValidation)", fc=WHITE, fontsize=8.5)
    box(ax, 8.8, 4.8, 2.4, 0.9, "Serilog\nLogging", fc=WHITE, fontsize=8.5)

    # Data layer
    box(ax, 0.5, 1.0, 11, 3.0, "DATA LAYER  —  Entity Framework Core + SQL Server",
        fc=BLUE, fontcolor=WHITE, fontsize=11)
    entities = ["Patients", "Doctors", "Appointments", "Bills", "Medicines",
                "Departments", "Rooms", "Prescriptions", "MedicalRecords",
                "Schedules", "Users", "NursingUnits"]
    for i, name in enumerate(entities):
        col = i % 6
        row = i // 6
        box(ax, 1.0 + col * 1.7, 1.4 + (1 - row) * 1.0, 1.6, 0.8, name,
            fc=WHITE, fontsize=8)

    # External services
    box(ax, 0.5, -0.3, 3.5, 0.9, "Payment Gateways\n(Stripe, PayPal, Paymob)",
        fc=ORANGE, fontcolor=WHITE, fontsize=9)
    box(ax, 4.2, -0.3, 3.6, 0.9, "AI Services\n(OpenAI Chatbot, X-Ray AI)",
        fc=ACCENT, fontcolor=WHITE, fontsize=9)
    box(ax, 8.0, -0.3, 3.5, 0.9, "Email Service\n(SMTP)",
        fc=GREEN, fontcolor=WHITE, fontsize=9)

    # Arrows
    arrow(ax, 6.0, 8.0, 6.0, 7.5, color=NAVY, lw=2)
    arrow(ax, 6.0, 4.5, 6.0, 4.0, color=NAVY, lw=2)
    arrow(ax, 2.5, 0.6, 2.5, 1.0, color=ORANGE, lw=1.6)
    arrow(ax, 6.0, 0.6, 6.0, 1.0, color=ACCENT, lw=1.6)
    arrow(ax, 9.5, 0.6, 9.5, 1.0, color=GREEN, lw=1.6)

    save(fig, "fig_architecture.png")


# ---------------- Request flow ----------------
def diagram_request_flow():
    fig, ax = plt.subplots(figsize=(12, 4.5))
    setup(ax, (0, 14), (0, 5))
    title(ax, "Figure 2 — HTTP Request Lifecycle Through the Backend")

    stages = [
        (0.2, "Client\n(Browser /\nMobile)", LIGHT_BLUE),
        (2.4, "Middleware\nPipeline\n(CORS, Auth,\nRate Limit)", TEAL),
        (5.0, "Controller\n(Route +\nModel Binding)", BLUE),
        (7.6, "Service\n(Business\nLogic)", NAVY),
        (10.2, "DbContext\n(EF Core)", ORANGE),
        (12.4, "SQL\nServer", ACCENT),
    ]
    text_colors = {LIGHT_BLUE: DARK, TEAL: WHITE, BLUE: WHITE, NAVY: WHITE,
                   ORANGE: WHITE, ACCENT: WHITE}
    for x, t, fc in stages:
        box(ax, x, 2.0, 1.8, 1.4, t, fc=fc, fontcolor=text_colors[fc],
            fontsize=8.5)

    # forward arrows
    pts = [stages[i][0] + 1.8 for i in range(len(stages) - 1)]
    starts = [stages[i + 1][0] for i in range(len(stages) - 1)]
    labels = ["HTTPS", "Validate", "Invoke", "Map", "SQL"]
    for p, s, lab in zip(pts, starts, labels):
        arrow(ax, p, 2.9, s, 2.9, color=NAVY, lw=1.8, text=lab,
              text_offset=(0, 0.25))

    # return arrow
    arrow(ax, 13.2, 1.6, 1.5, 1.6, color=GREEN, lw=1.8,
          text="JSON response (DTO)", text_offset=(0, -0.3))

    save(fig, "fig_request_flow.png")


# ---------------- ERD ----------------
def diagram_erd():
    fig, ax = plt.subplots(figsize=(12, 9))
    setup(ax, (0, 14), (0, 11))
    title(ax, "Figure 3 — Entity-Relationship Diagram (Core Entities)")

    ents = {
        "User":        (6.0, 9.2, 2.4, 1.2, ["Id (PK)", "Email", "PasswordHash", "Role"]),
        "Patient":     (0.2, 6.4, 2.6, 2.0, ["Id (PK)", "FirstName", "LastName",
                                             "NationalId", "DateOfBirth", "Gender",
                                             "Allergies", "Insurance"]),
        "Doctor":      (5.6, 6.4, 2.6, 2.0, ["Id (PK)", "DepartmentId (FK)",
                                             "Specialization", "LicenseNo",
                                             "ConsultFee", "Experience"]),
        "Department":  (11.2, 6.4, 2.6, 2.0, ["Id (PK)", "Name", "Location",
                                              "HeadOfDept", "PhoneExt"]),
        "Room":        (11.2, 3.5, 2.6, 1.7, ["Id (PK)", "DepartmentId (FK)",
                                              "RoomNumber", "Type", "Capacity"]),
        "Appointment": (0.2, 3.0, 2.6, 2.2, ["Id (PK)", "PatientId (FK)",
                                             "DoctorId (FK)", "RoomId (FK)",
                                             "Date", "Status", "Notes"]),
        "MedicalRecord":(3.0, 3.0, 2.4, 2.2, ["Id (PK)", "PatientId (FK)",
                                              "DoctorId (FK)", "AppointmentId (FK)",
                                              "Diagnosis", "Treatment",
                                              "VitalSigns"]),
        "Prescription":(5.6, 3.0, 2.4, 2.2, ["Id (PK)", "PatientId (FK)",
                                             "DoctorId (FK)", "MedRecordId (FK)",
                                             "Status", "Date"]),
        "Bill":        (8.2, 3.0, 2.4, 2.2, ["Id (PK)", "PatientId (FK)",
                                             "BillDate", "TotalAmount",
                                             "PaidAmount", "PaymentMethod",
                                             "Status"]),
        "Medicine":    (8.2, 0.4, 2.4, 2.0, ["Id (PK)", "Name", "Generic",
                                             "Category", "Strength", "Stock",
                                             "UnitPrice", "ExpiryDate"]),
        "PrescItem":   (5.6, 0.4, 2.4, 2.0, ["Id (PK)", "PrescriptionId (FK)",
                                             "MedicineId (FK)", "Qty",
                                             "Dosage", "Frequency"]),
        "BillItem":    (11.0, 0.4, 2.6, 2.0, ["Id (PK)", "BillId (FK)",
                                              "Description", "Qty",
                                              "UnitPrice", "Total"]),
    }

    for name, (x, y, w, h, fields) in ents.items():
        # header
        box(ax, x, y + h - 0.35, w, 0.35, name, fc=NAVY, fontcolor=WHITE,
            fontsize=9.5, radius=0.02)
        # body
        body = "\n".join(fields)
        box(ax, x, y, w, h - 0.35, body, fc=WHITE, fontsize=7.5,
            fontweight="normal", radius=0.02)

    def link(a, b, text=None):
        ax1, ay1, aw, ah, _ = ents[a]
        ax2, ay2, bw, bh, _ = ents[b]
        x1 = ax1 + aw / 2
        y1 = ay1 + ah / 2
        x2 = ax2 + bw / 2
        y2 = ay2 + bh / 2
        arrow(ax, x1, y1, x2, y2, color=TEAL, lw=1.2,
              style="-", text=text, text_offset=(0, 0.18), fontsize=7)

    link("User", "Patient", "1..1")
    link("User", "Doctor", "1..1")
    link("Department", "Doctor", "1..N")
    link("Department", "Room", "1..N")
    link("Patient", "Appointment", "1..N")
    link("Doctor", "Appointment", "1..N")
    link("Room", "Appointment", "1..N")
    link("Patient", "MedicalRecord", "1..N")
    link("Doctor", "MedicalRecord", "1..N")
    link("Patient", "Prescription", "1..N")
    link("MedicalRecord", "Prescription")
    link("Patient", "Bill", "1..N")
    link("Prescription", "PrescItem", "1..N")
    link("Medicine", "PrescItem", "1..N")
    link("Bill", "BillItem", "1..N")

    save(fig, "fig_erd.png")


# ---------------- Auth Sequence ----------------
def diagram_auth_sequence():
    fig, ax = plt.subplots(figsize=(11, 8))
    setup(ax, (0, 11), (0, 9))
    title(ax, "Figure 4 — JWT Authentication Sequence")

    actors = ["Client", "AuthController", "UserManager", "JWT Service", "Database"]
    xs = [1, 3.2, 5.4, 7.6, 9.8]
    for x, name in zip(xs, actors):
        box(ax, x - 0.8, 7.8, 1.6, 0.7, name, fc=NAVY, fontcolor=WHITE,
            fontsize=9)
        # lifeline
        ax.add_line(Line2D([x, x], [0.5, 7.8], color=GRAY,
                            linestyle="--", lw=1))

    msgs = [
        (0, 1, 7.2, "POST /api/auth/login {email, password}"),
        (1, 2, 6.6, "FindByEmailAsync(email)"),
        (2, 4, 6.0, "SELECT * FROM AspNetUsers WHERE Email=?"),
        (4, 2, 5.4, "User entity"),
        (2, 1, 4.8, "Verified user"),
        (1, 2, 4.2, "CheckPasswordAsync(user, pwd)"),
        (2, 1, 3.6, "True / False"),
        (1, 3, 3.0, "GenerateToken(user, roles)"),
        (3, 1, 2.4, "Signed JWT"),
        (1, 0, 1.8, "200 OK { token, user }"),
    ]
    colors = {True: BLUE, False: GREEN}
    for src, dst, y, label in msgs:
        forward = dst > src
        color = BLUE if forward else GREEN
        x1, x2 = xs[src], xs[dst]
        arrow(ax, x1, y, x2, y, color=color, lw=1.4,
              style="->" if forward else "-->", text=label,
              text_offset=(0, 0.15), fontsize=7.5)

    save(fig, "fig_auth_sequence.png")


# ---------------- Appointment booking sequence ----------------
def diagram_appointment_sequence():
    fig, ax = plt.subplots(figsize=(12, 8))
    setup(ax, (0, 12), (0, 9))
    title(ax, "Figure 5 — Appointment Booking Sequence")

    actors = ["Client", "AppointmentsCtrl", "AppointmentService",
              "DoctorService", "DbContext", "SQL Server"]
    xs = [0.9, 3.0, 5.2, 7.4, 9.4, 11.2]
    for x, name in zip(xs, actors):
        box(ax, x - 0.85, 7.8, 1.7, 0.7, name, fc=TEAL, fontcolor=WHITE,
            fontsize=8.5)
        ax.add_line(Line2D([x, x], [0.4, 7.8], color=GRAY,
                            linestyle="--", lw=1))

    msgs = [
        (0, 1, 7.2, "POST /api/appointments"),
        (1, 2, 6.6, "Create(dto)"),
        (2, 3, 6.0, "GetAvailability(doctorId, date)"),
        (3, 4, 5.4, "Query Schedule + existing Appointments"),
        (4, 5, 4.8, "SELECT ..."),
        (5, 4, 4.2, "Results"),
        (4, 3, 3.6, "Slot info"),
        (3, 2, 3.0, "Available / Conflict"),
        (2, 4, 2.4, "INSERT Appointment"),
        (4, 5, 1.8, "Persist"),
        (2, 1, 1.2, "AppointmentDto"),
        (1, 0, 0.6, "201 Created"),
    ]
    for src, dst, y, label in msgs:
        forward = dst > src
        color = BLUE if forward else GREEN
        arrow(ax, xs[src], y, xs[dst], y, color=color, lw=1.4,
              style="->" if forward else "-->", text=label,
              text_offset=(0, 0.15), fontsize=7)

    save(fig, "fig_appointment_sequence.png")


# ---------------- Use case diagram ----------------
def diagram_usecase():
    fig, ax = plt.subplots(figsize=(11, 8.5))
    setup(ax, (0, 12), (0, 10))
    title(ax, "Figure 6 — Use-Case Diagram (Actors & System Boundary)")

    # System boundary
    sys = Rectangle((3, 0.6), 6, 9, linewidth=1.8, edgecolor=NAVY,
                    facecolor="#F8FBFF")
    ax.add_patch(sys)
    ax.text(6, 9.3, "Hospital Management System", ha="center",
            fontsize=11, color=NAVY, fontweight="bold")

    # Actors (stick figures)
    def actor(x, y, name):
        ax.plot(x, y + 0.45, marker="o", markersize=14,
                markerfacecolor=NAVY, markeredgecolor=NAVY)
        ax.plot([x, x], [y + 0.40, y - 0.05], color=NAVY, lw=2)
        ax.plot([x - 0.25, x + 0.25], [y + 0.20, y + 0.20], color=NAVY, lw=2)
        ax.plot([x, x - 0.25], [y - 0.05, y - 0.45], color=NAVY, lw=2)
        ax.plot([x, x + 0.25], [y - 0.05, y - 0.45], color=NAVY, lw=2)
        ax.text(x, y - 0.85, name, ha="center", fontsize=10,
                fontweight="bold", color=DARK)

    actor(1.2, 8.0, "Admin")
    actor(1.2, 5.0, "Doctor")
    actor(1.2, 2.0, "Patient")
    actor(10.8, 5.0, "Receptionist")

    use_cases_admin = ["Manage Users", "Manage Departments",
                       "Manage Medicines", "View Dashboard"]
    use_cases_doctor = ["Manage Patients", "Create Medical Records",
                        "Create Prescriptions"]
    use_cases_patient = ["Register / Login", "Book Appointment",
                         "View Bills"]
    use_cases_recep = ["Manage Appointments", "Manage Bills",
                       "Issue Payments"]

    def uc(x, y, name):
        e = matplotlib.patches.Ellipse((x, y), 2.2, 0.6, edgecolor=TEAL,
                                       facecolor=LIGHT_BLUE, lw=1.4)
        ax.add_patch(e)
        ax.text(x, y, name, ha="center", va="center", fontsize=8.5,
                color=DARK, fontweight="bold")

    for i, n in enumerate(use_cases_admin):
        uc(5.0, 8.7 - i * 0.55, n)
        arrow(ax, 1.5, 8.0, 4.0, 8.7 - i * 0.55, color=GRAY, lw=0.8, style="-")

    for i, n in enumerate(use_cases_doctor):
        uc(5.0, 5.5 - i * 0.6, n)
        arrow(ax, 1.5, 5.0, 4.0, 5.5 - i * 0.6, color=GRAY, lw=0.8, style="-")

    for i, n in enumerate(use_cases_patient):
        uc(5.0, 2.5 - i * 0.6, n)
        arrow(ax, 1.5, 2.0, 4.0, 2.5 - i * 0.6, color=GRAY, lw=0.8, style="-")

    for i, n in enumerate(use_cases_recep):
        uc(7.5, 6.0 - i * 0.7, n)
        arrow(ax, 10.5, 5.0, 8.6, 6.0 - i * 0.7, color=GRAY, lw=0.8, style="-")

    save(fig, "fig_usecase.png")


# ---------------- State diagram ----------------
def diagram_state():
    fig, ax = plt.subplots(figsize=(10, 6.5))
    setup(ax, (0, 11), (0, 7))
    title(ax, "Figure 7 — Appointment Status State Diagram")

    states = {
        "Start": (0.8, 5.5, 1.3, 0.7),
        "Pending": (3.2, 5.5, 1.7, 0.9),
        "Confirmed": (6.0, 5.5, 1.8, 0.9),
        "InProgress": (8.7, 5.5, 1.8, 0.9),
        "Completed": (8.7, 2.8, 1.8, 0.9),
        "Cancelled": (3.2, 2.8, 1.7, 0.9),
        "NoShow": (6.0, 2.8, 1.7, 0.9),
        "End": (0.8, 2.8, 1.3, 0.7),
    }
    colors = {"Start": GRAY, "End": GRAY, "Pending": ORANGE,
              "Confirmed": BLUE, "InProgress": TEAL,
              "Completed": GREEN, "Cancelled": ACCENT, "NoShow": ACCENT}
    for name, (x, y, w, h) in states.items():
        fc = colors[name]
        box(ax, x, y, w, h, name, fc=fc, fontcolor=WHITE, fontsize=10)

    arrows = [
        ("Start", "Pending", "create"),
        ("Pending", "Confirmed", "confirm"),
        ("Pending", "Cancelled", "cancel"),
        ("Confirmed", "InProgress", "check-in"),
        ("Confirmed", "Cancelled", "cancel"),
        ("Confirmed", "NoShow", "patient absent"),
        ("InProgress", "Completed", "finish"),
        ("Cancelled", "End", ""),
        ("NoShow", "End", ""),
        ("Completed", "End", ""),
    ]
    for a, b, label in arrows:
        ax1, ay1, aw, ah = states[a]
        bx1, by1, bw, bh = states[b]
        x1, y1 = ax1 + aw / 2, ay1 + ah / 2
        x2, y2 = bx1 + bw / 2, by1 + bh / 2
        arrow(ax, x1, y1, x2, y2, color=NAVY, lw=1.4, text=label,
              text_offset=(0, 0.2), fontsize=8)

    save(fig, "fig_state.png")


# ---------------- Bill creation flowchart ----------------
def diagram_bill_flow():
    fig, ax = plt.subplots(figsize=(8.5, 11))
    setup(ax, (0, 9), (0, 13))
    title(ax, "Figure 8 — Bill Creation & Payment Flowchart")

    def oval(x, y, w, h, t):
        e = matplotlib.patches.Ellipse((x + w / 2, y + h / 2), w, h,
                                       edgecolor=NAVY,
                                       facecolor=LIGHT_BLUE, lw=1.6)
        ax.add_patch(e)
        ax.text(x + w / 2, y + h / 2, t, ha="center", va="center",
                fontsize=10, fontweight="bold")

    def rect(x, y, w, h, t, fc=WHITE):
        box(ax, x, y, w, h, t, fc=fc, fontsize=9.5)

    def diamond(x, y, w, h, t):
        cx, cy = x + w / 2, y + h / 2
        p = Polygon([(cx, y + h), (x + w, cy), (cx, y), (x, cy)],
                    edgecolor=NAVY, facecolor=ORANGE, lw=1.6)
        ax.add_patch(p)
        ax.text(cx, cy, t, ha="center", va="center", fontsize=9,
                fontweight="bold", color=WHITE)

    oval(3.0, 11.8, 3.0, 0.9, "Start")
    rect(2.5, 10.4, 4.0, 0.9, "Select Patient", fc=LIGHT_BLUE)
    rect(2.5, 9.0, 4.0, 0.9, "Add Bill Items\n(service / drug / room)", fc=LIGHT_BLUE)
    diamond(2.7, 7.5, 3.6, 1.2, "More items?")
    rect(0.2, 7.7, 2.0, 0.7, "Add item", fc=LIGHT_BLUE)
    rect(2.5, 6.0, 4.0, 0.9, "Calculate Total\n(items + tax - discount)", fc=LIGHT_BLUE)
    rect(2.5, 4.6, 4.0, 0.9, "Set Due Date / Status", fc=LIGHT_BLUE)
    diamond(2.7, 3.0, 3.6, 1.2, "Valid?")
    rect(0.2, 3.2, 2.0, 0.7, "Show\nErrors", fc=ACCENT)
    rect(2.5, 1.5, 4.0, 0.9, "Persist Bill\n+ Items", fc=GREEN)
    rect(2.5, 0.2, 4.0, 0.9, "Generate Invoice\nNumber & Notify", fc=GREEN)

    arrow(ax, 4.5, 11.8, 4.5, 11.3)
    arrow(ax, 4.5, 10.4, 4.5, 9.9)
    arrow(ax, 4.5, 9.0, 4.5, 8.7)
    arrow(ax, 2.7, 8.1, 2.2, 8.1, text="Yes", text_offset=(0, 0.1))
    arrow(ax, 1.2, 7.7, 1.2, 8.5)
    arrow(ax, 1.2, 8.7, 2.5, 9.4)
    arrow(ax, 4.5, 7.5, 4.5, 6.9, text="No", text_offset=(0.1, 0))
    arrow(ax, 4.5, 6.0, 4.5, 5.5)
    arrow(ax, 4.5, 4.6, 4.5, 4.2)
    arrow(ax, 2.7, 3.6, 2.2, 3.6, text="No", text_offset=(0, 0.15))
    arrow(ax, 4.5, 3.0, 4.5, 2.4, text="Yes", text_offset=(0.1, 0))
    arrow(ax, 4.5, 1.5, 4.5, 1.1)

    save(fig, "fig_bill_flow.png")


# ---------------- Deployment diagram ----------------
def diagram_deployment():
    fig, ax = plt.subplots(figsize=(12, 7))
    setup(ax, (0, 14), (0, 8))
    title(ax, "Figure 9 — Deployment Diagram")

    # Client
    box(ax, 0.5, 2.5, 3.0, 3.5, "End-User Device", fc=LIGHT_GRAY)
    box(ax, 0.8, 4.5, 2.4, 1.0, "Browser\n(React SPA)", fc=WHITE, fontsize=9)
    box(ax, 0.8, 3.0, 2.4, 1.0, "Mobile App\n(Flutter)", fc=WHITE, fontsize=9)

    # App server
    box(ax, 4.5, 1.5, 4.5, 5.0, "Application Server", fc=LIGHT_GRAY)
    box(ax, 4.8, 5.0, 3.9, 1.0, "Kestrel / IIS\nHTTPS", fc=NAVY, fontcolor=WHITE,
        fontsize=9)
    box(ax, 4.8, 3.5, 3.9, 1.3, "ASP.NET Core 8.0\nWeb API\n(HMS Backend)",
        fc=TEAL, fontcolor=WHITE, fontsize=9)
    box(ax, 4.8, 1.9, 3.9, 1.4, "Log Files (Serilog)\nMigrations / Seed", fc=WHITE,
        fontsize=9)

    # DB server
    box(ax, 9.8, 2.5, 3.8, 3.5, "Database Server", fc=LIGHT_GRAY)
    box(ax, 10.1, 3.5, 3.3, 1.8, "SQL Server 2022\nHospitalManagementSystem DB",
        fc=BLUE, fontcolor=WHITE, fontsize=9)

    # External
    box(ax, 0.5, 0.2, 4.0, 1.0, "External SMTP\n(Gmail/Outlook)", fc=GREEN,
        fontcolor=WHITE, fontsize=9)
    box(ax, 5.0, 0.2, 4.0, 1.0, "Payment Gateways\nStripe / PayPal / Paymob",
        fc=ORANGE, fontcolor=WHITE, fontsize=9)
    box(ax, 9.5, 0.2, 4.0, 1.0, "AI Services\nOpenAI / X-Ray Inference", fc=ACCENT,
        fontcolor=WHITE, fontsize=9)

    arrow(ax, 3.5, 5.0, 4.5, 5.5, text="HTTPS", text_offset=(0, 0.25))
    arrow(ax, 3.5, 3.5, 4.5, 5.0, text="HTTPS", text_offset=(0, 0.25))
    arrow(ax, 9.0, 4.3, 9.8, 4.3, text="TCP 1433", text_offset=(0, 0.25))
    arrow(ax, 6.7, 1.9, 4.0, 1.2, text="SMTP")
    arrow(ax, 6.7, 1.9, 7.0, 1.2, text="REST")
    arrow(ax, 6.7, 1.9, 10.0, 1.2, text="HTTPS")

    save(fig, "fig_deployment.png")


# ---------------- Component diagram ----------------
def diagram_components():
    fig, ax = plt.subplots(figsize=(12, 8))
    setup(ax, (0, 14), (0, 9))
    title(ax, "Figure 10 — Component / Module Diagram")

    # Controllers row
    controllers = ["Auth", "Patients", "Doctors", "Appointments", "MedicalRec",
                   "Prescriptions", "Bills", "Payment", "Dashboard", "Chatbot",
                   "XRayAi", "Medicines"]
    box(ax, 0.5, 6.8, 13, 1.6, "Controllers Layer", fc=NAVY, fontcolor=WHITE,
        fontsize=10)
    for i, c in enumerate(controllers):
        r = i // 6
        col = i % 6
        box(ax, 0.8 + col * 2.15, 6.95 + (1 - r) * 0.6, 2.05, 0.55, c, fc=WHITE,
            fontsize=8)

    # Services row
    services = ["PatientSvc", "DoctorSvc", "AppointmentSvc", "MedRecSvc",
                "PrescriptionSvc", "BillSvc", "MedicineSvc", "DeptSvc",
                "RoomSvc", "ScheduleSvc", "ChatbotSvc", "EmailSvc",
                "StripeSvc", "PayPalSvc", "PaymobSvc", "OpenAISvc",
                "XRayAiSvc", "NursingSvc"]
    box(ax, 0.5, 3.4, 13, 2.6, "Services Layer (Business Logic)", fc=TEAL,
        fontcolor=WHITE, fontsize=10)
    for i, s in enumerate(services):
        r = i // 6
        col = i % 6
        box(ax, 0.8 + col * 2.15, 3.55 + (2 - r) * 0.55, 2.05, 0.5, s,
            fc=WHITE, fontsize=7.5)

    # Data row
    box(ax, 0.5, 1.6, 6.5, 1.5, "HospitalDbContext\n(EF Core)", fc=BLUE,
        fontcolor=WHITE, fontsize=10)
    box(ax, 7.2, 1.6, 6.3, 1.5, "AutoMapper Profiles\n(Models ↔ DTOs)", fc=ORANGE,
        fontcolor=WHITE, fontsize=10)

    box(ax, 0.5, 0.1, 13, 1.0, "SQL Server Database", fc=ACCENT, fontcolor=WHITE,
        fontsize=10)

    arrow(ax, 7, 6.8, 7, 6.0, color=NAVY, lw=2)
    arrow(ax, 4, 3.4, 4, 3.1, color=NAVY, lw=2)
    arrow(ax, 10, 3.4, 10, 3.1, color=NAVY, lw=2)
    arrow(ax, 4, 1.6, 4, 1.1, color=NAVY, lw=2)
    save(fig, "fig_components.png")


# ---------------- Class diagram ----------------
def diagram_class():
    fig, ax = plt.subplots(figsize=(12, 9))
    setup(ax, (0, 14), (0, 10))
    title(ax, "Figure 11 — Simplified Class Diagram (Domain Model)")

    def cls(x, y, w, h, name, attrs, methods):
        box(ax, x, y + h - 0.4, w, 0.4, name, fc=NAVY, fontcolor=WHITE,
            fontsize=9, radius=0.02)
        ax.add_patch(Rectangle((x, y), w, h - 0.4, edgecolor=NAVY,
                                facecolor=WHITE, lw=1.2))
        # attributes
        ax.text(x + 0.1, y + h - 0.55, "Attributes", fontsize=7.5,
                color=GRAY, fontweight="bold")
        for i, a in enumerate(attrs):
            ax.text(x + 0.15, y + h - 0.80 - i * 0.20, "• " + a,
                    fontsize=7)
        # methods
        ymid = y + h - 0.80 - len(attrs) * 0.20 - 0.15
        ax.add_line(Line2D([x, x + w], [ymid, ymid], color=NAVY, lw=0.8))
        ax.text(x + 0.1, ymid - 0.25, "Methods", fontsize=7.5, color=GRAY,
                fontweight="bold")
        for i, m in enumerate(methods):
            ax.text(x + 0.15, ymid - 0.50 - i * 0.20, "+ " + m,
                    fontsize=7)

    cls(0.2, 7.0, 3.3, 2.6, "BaseEntity",
        ["Id : int", "CreatedAt : DateTime", "UpdatedAt : DateTime",
         "IsActive : bool"],
        [])

    cls(4.2, 7.0, 4.0, 2.6, "Patient : BaseEntity",
        ["FirstName, LastName", "NationalId", "Email, Phone",
         "DateOfBirth, Gender", "Allergies, Insurance"],
        ["AddAppointment()", "AddPrescription()"])

    cls(9.0, 7.0, 4.5, 2.6, "Doctor : BaseEntity",
        ["Specialization", "LicenseNo", "ConsultationFee",
         "Experience, DepartmentId"],
        ["GetSchedule()", "GetPatients()"])

    cls(0.2, 3.5, 4.2, 3.0, "Appointment : BaseEntity",
        ["PatientId, DoctorId, RoomId", "Date, StartTime, EndTime",
         "Status (enum)", "Notes"],
        ["Confirm()", "Cancel()", "Complete()"])

    cls(4.8, 3.5, 4.5, 3.0, "MedicalRecord : BaseEntity",
        ["PatientId, DoctorId", "AppointmentId", "Diagnosis, Treatment",
         "VitalSigns, LabResults"],
        ["AttachPrescription()"])

    cls(9.7, 3.5, 3.8, 3.0, "Prescription : BaseEntity",
        ["PatientId, DoctorId", "MedicalRecordId", "Date, Status",
         "Notes"],
        ["Dispense()", "AddItem()"])

    cls(0.2, 0.2, 4.2, 3.0, "Bill : BaseEntity",
        ["PatientId, BillDate, DueDate", "TotalAmount, PaidAmount",
         "PaymentMethod, Status"],
        ["RegisterPayment()", "AddItem()"])

    cls(4.8, 0.2, 4.5, 3.0, "Medicine : BaseEntity",
        ["Name, Generic, Category", "Strength, DosageForm",
         "UnitPrice, Stock, ExpiryDate"],
        ["Reorder()", "Dispense()"])

    cls(9.7, 0.2, 3.8, 3.0, "Department : BaseEntity",
        ["Name, Description", "Location, PhoneExt",
         "HeadOfDepartment"],
        ["AssignDoctor()", "AssignRoom()"])

    save(fig, "fig_class.png")


# ---------------- Activity diagram (registration) ----------------
def diagram_registration_activity():
    fig, ax = plt.subplots(figsize=(8.5, 11))
    setup(ax, (0, 9), (0, 13))
    title(ax, "Figure 12 — User Registration Activity Diagram")

    def rect(x, y, w, h, t, fc=LIGHT_BLUE):
        box(ax, x, y, w, h, t, fc=fc, fontsize=9.5)

    def diamond(x, y, w, h, t):
        cx, cy = x + w / 2, y + h / 2
        p = Polygon([(cx, y + h), (x + w, cy), (cx, y), (x, cy)],
                    edgecolor=NAVY, facecolor=ORANGE, lw=1.6)
        ax.add_patch(p)
        ax.text(cx, cy, t, ha="center", va="center", fontsize=9,
                fontweight="bold", color=WHITE)

    def oval(x, y, w, h, t, fc=LIGHT_BLUE):
        e = matplotlib.patches.Ellipse((x + w / 2, y + h / 2), w, h,
                                       edgecolor=NAVY, facecolor=fc, lw=1.6)
        ax.add_patch(e)
        ax.text(x + w / 2, y + h / 2, t, ha="center", va="center",
                fontsize=10, fontweight="bold")

    oval(3.2, 11.8, 2.7, 0.8, "Start")
    rect(2.0, 10.4, 5.0, 0.9, "User opens Register page")
    rect(2.0, 9.0, 5.0, 0.9, "Submit form (POST /api/auth/register)")
    rect(2.0, 7.6, 5.0, 0.9, "Validate DTO + ModelState")
    diamond(2.8, 6.0, 3.4, 1.1, "Valid?")
    rect(7.0, 6.1, 1.9, 0.8, "Return 400", fc=ACCENT)
    rect(2.0, 4.5, 5.0, 0.9, "UserManager.CreateAsync()")
    diamond(2.8, 3.0, 3.4, 1.1, "Created?")
    rect(7.0, 3.1, 1.9, 0.8, "Return\nIdentityErrors", fc=ACCENT)
    rect(2.0, 1.6, 5.0, 0.9, "Assign default role (Patient)")
    rect(2.0, 0.4, 5.0, 0.9, "Generate JWT + return 200 OK", fc=GREEN)

    arrow(ax, 4.5, 11.8, 4.5, 11.3)
    arrow(ax, 4.5, 10.4, 4.5, 9.9)
    arrow(ax, 4.5, 9.0, 4.5, 8.5)
    arrow(ax, 4.5, 7.6, 4.5, 7.1)
    arrow(ax, 6.2, 6.55, 7.0, 6.5, text="No", text_offset=(0, 0.15))
    arrow(ax, 4.5, 6.0, 4.5, 5.4, text="Yes", text_offset=(0.1, 0))
    arrow(ax, 4.5, 4.5, 4.5, 4.1)
    arrow(ax, 6.2, 3.55, 7.0, 3.5, text="No", text_offset=(0, 0.15))
    arrow(ax, 4.5, 3.0, 4.5, 2.5, text="Yes", text_offset=(0.1, 0))
    arrow(ax, 4.5, 1.6, 4.5, 1.3)

    save(fig, "fig_registration_activity.png")


# ---------------- Project structure ----------------
def diagram_structure():
    fig, ax = plt.subplots(figsize=(11, 9))
    setup(ax, (0, 12), (0, 11))
    title(ax, "Figure 13 — Project Folder Structure")

    items = [
        (1, 10.2, "Hospital Management System/", NAVY, WHITE),
        (1.5, 9.7, "Program.cs", DARK, LIGHT_BLUE),
        (1.5, 9.2, "appsettings.json", DARK, LIGHT_BLUE),
        (1.5, 8.7, "Hospital Mangement System.csproj", DARK, LIGHT_BLUE),
        (1.5, 8.2, "Controllers/  (22 REST controllers)", NAVY, "#FFF4E0"),
        (1.5, 7.7, "Services/     (interface + implementation)", NAVY, "#FFF4E0"),
        (1.5, 7.2, "Models/       (17 domain entities)", NAVY, "#FFF4E0"),
        (1.5, 6.7, "DTOs/         (14 transfer objects)", NAVY, "#FFF4E0"),
        (1.5, 6.2, "Data/         (HospitalDbContext + Seed)", NAVY, "#FFF4E0"),
        (1.5, 5.7, "Mappings/     (AutoMapperProfile.cs)", NAVY, "#FFF4E0"),
        (1.5, 5.2, "Migrations/   (EF Core migrations)", NAVY, "#FFF4E0"),
        (1.5, 4.7, "Configuration/(JWT, ConnectionString resolver)", NAVY, "#FFF4E0"),
        (1.5, 4.2, "Properties/   (launchSettings.json)", DARK, LIGHT_BLUE),
        (1.5, 3.7, "logs/         (Serilog daily rolling logs)", DARK, LIGHT_BLUE),
    ]
    for x, y, t, fc, bg in items:
        box(ax, x, y, 9, 0.45, t, fc=bg, fontcolor=fc, fontsize=10,
            radius=0.02)

    save(fig, "fig_structure.png")


# ---------------- Mock screen: Swagger UI ----------------
def screen_swagger():
    fig, ax = plt.subplots(figsize=(11, 8))
    setup(ax, (0, 14), (0, 10))
    title(ax, "Screen 1 — Swagger / OpenAPI Explorer")

    # browser chrome
    box(ax, 0.2, 9.0, 13.6, 0.8, "https://localhost:7102/swagger/index.html",
        fc=LIGHT_GRAY, fontcolor=DARK, fontsize=10)
    # green dot + dropdown icons
    ax.plot(0.45, 9.4, "o", color=ACCENT, markersize=10)
    ax.plot(0.80, 9.4, "o", color=ORANGE, markersize=10)
    ax.plot(1.15, 9.4, "o", color=GREEN, markersize=10)

    # title bar
    box(ax, 0.2, 8.0, 13.6, 0.9, "Hospital Management System API   v1.0   |   /swagger/v1/swagger.json",
        fc=NAVY, fontcolor=WHITE, fontsize=10)

    endpoints = [
        ("POST",  "/api/auth/login",         GREEN,  "User login (returns JWT)"),
        ("POST",  "/api/auth/register",      GREEN,  "Register a new patient account"),
        ("GET",   "/api/patients",           BLUE,   "List all patients (paged)"),
        ("POST",  "/api/patients",           GREEN,  "Create new patient"),
        ("GET",   "/api/patients/{id}",      BLUE,   "Get patient by id"),
        ("PUT",   "/api/patients/{id}",      ORANGE, "Update patient"),
        ("DELETE","/api/patients/{id}",      ACCENT, "Soft-delete patient"),
        ("GET",   "/api/doctors",            BLUE,   "List doctors"),
        ("POST",  "/api/appointments",       GREEN,  "Book appointment"),
        ("PUT",   "/api/appointments/{id}/cancel", ORANGE, "Cancel appointment"),
        ("GET",   "/api/dashboard/stats",    BLUE,   "Dashboard counters & KPIs"),
        ("POST",  "/api/bills",              GREEN,  "Create bill"),
    ]
    for i, (verb, path, color, desc) in enumerate(endpoints):
        y = 7.4 - i * 0.55
        box(ax, 0.3, y, 0.9, 0.4, verb, fc=color, fontcolor=WHITE, fontsize=9)
        box(ax, 1.3, y, 5.5, 0.4, path, fc=WHITE, fontcolor=DARK,
            fontsize=9.5, fontweight="normal")
        box(ax, 6.9, y, 6.9, 0.4, desc, fc=LIGHT_BLUE, fontcolor=DARK,
            fontsize=8.5, fontweight="normal")

    save(fig, "screen_swagger.png")


# ---------------- Mock screen: Login ----------------
def screen_login():
    fig, ax = plt.subplots(figsize=(11, 8))
    setup(ax, (0, 14), (0, 10))
    title(ax, "Screen 2 — Authentication Screen (Login)")

    box(ax, 0.2, 9.0, 13.6, 0.8, "https://hms.alhayat.com/login", fc=LIGHT_GRAY,
        fontcolor=DARK, fontsize=10)
    box(ax, 0.2, 0.2, 13.6, 8.7, "", fc="#F4F8FB", radius=0.02)

    # Logo + title
    box(ax, 5.0, 7.4, 4.0, 0.9, "Al-Hayat Hospital", fc=NAVY, fontcolor=WHITE,
        fontsize=14)
    ax.text(7.0, 6.9, "Hospital Management System", ha="center",
            fontsize=11, color=DARK)

    # Card
    box(ax, 4.0, 1.5, 6.0, 5.0, "", fc=WHITE)
    ax.text(7.0, 6.1, "Sign in to your account", ha="center", fontsize=12,
            fontweight="bold", color=NAVY)

    # Email field
    ax.text(4.4, 5.5, "Email", fontsize=9.5, color=DARK)
    box(ax, 4.4, 4.85, 5.2, 0.5, "admin@hospital.com", fc=LIGHT_GRAY,
        fontcolor=DARK, fontsize=9.5, fontweight="normal")

    ax.text(4.4, 4.5, "Password", fontsize=9.5, color=DARK)
    box(ax, 4.4, 3.85, 5.2, 0.5, "••••••••", fc=LIGHT_GRAY,
        fontcolor=DARK, fontsize=9.5, fontweight="normal")

    ax.text(4.4, 3.4, "Role", fontsize=9.5, color=DARK)
    box(ax, 4.4, 2.75, 5.2, 0.5, "Admin  ▾", fc=LIGHT_GRAY,
        fontcolor=DARK, fontsize=9.5, fontweight="normal")

    box(ax, 4.4, 1.95, 5.2, 0.55, "LOG IN", fc=TEAL, fontcolor=WHITE,
        fontsize=11)

    ax.text(7.0, 1.0, "Forgot password?  |  Create account",
            ha="center", fontsize=9, color=BLUE)

    save(fig, "screen_login.png")


# ---------------- Mock screen: Dashboard ----------------
def screen_dashboard():
    fig, ax = plt.subplots(figsize=(13, 8))
    setup(ax, (0, 16), (0, 10))
    title(ax, "Screen 3 — Admin Dashboard")

    # Sidebar
    box(ax, 0.2, 0.2, 2.6, 9.5, "", fc=NAVY)
    sidebar = ["Dashboard", "Patients", "Doctors", "Appointments",
               "Medical Records", "Prescriptions", "Bills",
               "Medicines", "Departments", "Reports"]
    for i, s in enumerate(sidebar):
        y = 8.6 - i * 0.65
        bg = TEAL if s == "Dashboard" else NAVY
        box(ax, 0.4, y, 2.2, 0.5, s, fc=bg, fontcolor=WHITE, fontsize=9)

    # Topbar
    box(ax, 3.0, 9.0, 12.8, 0.8, "  Welcome back, Dr. Ahmed   |   Today: 12 Jun 2026   |    Admin ▾",
        fc=WHITE, fontcolor=DARK, fontsize=10)

    # KPI cards
    kpis = [
        ("1,247", "Patients", BLUE),
        ("64",    "Doctors", TEAL),
        ("38",    "Today's Appts", ORANGE),
        ("$24K",  "Revenue", GREEN),
    ]
    for i, (val, label, color) in enumerate(kpis):
        x = 3.1 + i * 3.2
        box(ax, x, 7.0, 3.0, 1.6, "", fc=WHITE)
        ax.text(x + 1.5, 7.95, val, ha="center", fontsize=22,
                color=color, fontweight="bold")
        ax.text(x + 1.5, 7.3, label, ha="center", fontsize=10, color=DARK)

    # Chart placeholders
    box(ax, 3.1, 3.6, 6.4, 3.0, "", fc=WHITE)
    ax.text(6.3, 6.3, "Appointments — last 7 days", ha="center", fontsize=10,
            fontweight="bold", color=DARK)
    # Fake bar chart
    bar_vals = [18, 27, 22, 31, 25, 35, 38]
    for i, v in enumerate(bar_vals):
        ax.add_patch(Rectangle((3.5 + i * 0.85, 4.0), 0.55, v * 0.05,
                                facecolor=TEAL))
        ax.text(3.78 + i * 0.85, 3.85, ["Mon","Tue","Wed","Thu","Fri","Sat","Sun"][i],
                fontsize=8, color=DARK, ha="center")

    box(ax, 9.7, 3.6, 6.1, 3.0, "", fc=WHITE)
    ax.text(12.7, 6.3, "Revenue distribution", ha="center", fontsize=10,
            fontweight="bold", color=DARK)
    # Pie
    wedges, _ = ax.pie([45, 25, 20, 10],
                       colors=[BLUE, ORANGE, GREEN, ACCENT],
                       center=(11.4, 4.7), radius=0.9, startangle=90)
    ax.legend(["Consultations", "Pharmacy", "Lab", "Rooms"],
              loc="center right", bbox_to_anchor=(0.99, 0.45),
              fontsize=8)

    # Recent table
    box(ax, 3.1, 0.4, 12.7, 3.1, "", fc=WHITE)
    ax.text(9.4, 3.2, "Recent Appointments", ha="center", fontsize=10,
            fontweight="bold", color=DARK)
    headers = ["Patient", "Doctor", "Department", "Time", "Status"]
    rows = [
        ["Ali Hassan",  "Dr. Sarah",  "Cardiology",  "09:30", "Confirmed"],
        ["Mona Adel",   "Dr. Khaled", "Pediatrics",  "10:00", "Completed"],
        ["Omar Tarek",  "Dr. Layla",  "Orthopedics", "10:30", "Pending"],
        ["Nour Sami",   "Dr. Hany",   "Dermatology", "11:00", "Cancelled"],
    ]
    col_x = [3.3, 5.4, 7.5, 10.0, 12.0]
    for cx, h in zip(col_x, headers):
        ax.text(cx, 2.8, h, fontsize=9, color=NAVY, fontweight="bold")
    for r, row in enumerate(rows):
        for cx, val in zip(col_x, row):
            color = DARK
            if val == "Confirmed": color = BLUE
            if val == "Completed": color = GREEN
            if val == "Pending":   color = ORANGE
            if val == "Cancelled": color = ACCENT
            ax.text(cx, 2.3 - r * 0.45, val, fontsize=9, color=color)

    save(fig, "screen_dashboard.png")


# ---------------- Mock screen: Patient list ----------------
def screen_patients():
    fig, ax = plt.subplots(figsize=(13, 8))
    setup(ax, (0, 16), (0, 10))
    title(ax, "Screen 4 — Patients Management")

    box(ax, 0.2, 0.2, 2.6, 9.5, "", fc=NAVY)
    sidebar = ["Dashboard", "Patients", "Doctors", "Appointments",
               "Medical Records", "Prescriptions", "Bills"]
    for i, s in enumerate(sidebar):
        y = 8.6 - i * 0.65
        bg = TEAL if s == "Patients" else NAVY
        box(ax, 0.4, y, 2.2, 0.5, s, fc=bg, fontcolor=WHITE, fontsize=9)

    box(ax, 3.0, 9.0, 12.8, 0.8, "  Patients   /   Manage",
        fc=WHITE, fontcolor=DARK, fontsize=10)
    box(ax, 3.1, 7.8, 5.5, 0.7, "  Search by name, national id…",
        fc=WHITE, fontcolor=GRAY, fontsize=9.5, fontweight="normal")
    box(ax, 8.8, 7.8, 1.6, 0.7, "Filter ▾", fc=WHITE, fontcolor=DARK, fontsize=9.5)
    box(ax, 13.9, 7.8, 1.9, 0.7, "+ New Patient", fc=TEAL, fontcolor=WHITE,
        fontsize=9.5)

    # Table
    box(ax, 3.1, 0.4, 12.7, 7.1, "", fc=WHITE)
    headers = ["ID", "Name", "National ID", "Gender", "Phone", "Insurance",
               "Actions"]
    col_x = [3.3, 4.4, 7.0, 9.2, 10.6, 13.0, 15.2]
    for cx, h in zip(col_x, headers):
        ax.text(cx, 7.0, h, fontsize=9.5, fontweight="bold", color=NAVY)

    sample = [
        ("P-1001", "Ali Hassan",     "29812345678901", "Male",   "010-1234567", "MediPlus"),
        ("P-1002", "Mona Adel",      "29603456789012", "Female", "011-2345678", "Bupa"),
        ("P-1003", "Omar Tarek",     "29005678910123", "Male",   "012-3456789", "—"),
        ("P-1004", "Nour Sami",      "29701234567890", "Female", "010-9876543", "MetLife"),
        ("P-1005", "Khaled Mansour", "28912345678901", "Male",   "011-4567890", "Allianz"),
        ("P-1006", "Salma Reda",     "29207654321098", "Female", "012-5556677", "GIG"),
        ("P-1007", "Hany Adel",      "28503216549870", "Male",   "010-1112233", "AXA"),
        ("P-1008", "Yara Mahmoud",   "29805671230456", "Female", "011-3334455", "MediPlus"),
        ("P-1009", "Tarek Ezzat",    "27704561237890", "Male",   "012-7778899", "—"),
        ("P-1010", "Reem Salah",     "29509832145678", "Female", "010-2223344", "Bupa"),
    ]
    for r, row in enumerate(sample):
        y = 6.5 - r * 0.55
        for cx, val in zip(col_x[:-1], row):
            ax.text(cx, y, val, fontsize=8.5, color=DARK)
        # action buttons
        box(ax, 15.0, y - 0.1, 0.35, 0.35, "✎", fc=BLUE, fontcolor=WHITE,
            fontsize=8)
        box(ax, 15.4, y - 0.1, 0.35, 0.35, "X", fc=ACCENT, fontcolor=WHITE,
            fontsize=8)

    # Pagination
    ax.text(9.5, 0.7, "‹  1  2  3  4  ›", ha="center", fontsize=10,
            color=NAVY)

    save(fig, "screen_patients.png")


# ---------------- Mock screen: Appointment form ----------------
def screen_appointment_form():
    fig, ax = plt.subplots(figsize=(11, 8))
    setup(ax, (0, 14), (0, 10))
    title(ax, "Screen 5 — Book Appointment Form")

    box(ax, 0.2, 9.0, 13.6, 0.8, "  Appointments   /   New", fc=WHITE,
        fontcolor=DARK, fontsize=10)

    # form card
    box(ax, 2.5, 0.7, 9.0, 8.1, "", fc=WHITE)
    ax.text(7.0, 8.3, "Schedule New Appointment", ha="center", fontsize=12,
            fontweight="bold", color=NAVY)

    fields = [
        ("Patient",          "Ali Hassan (P-1001) ▾"),
        ("Department",       "Cardiology ▾"),
        ("Doctor",           "Dr. Sarah Mansour ▾"),
        ("Appointment Date", "2026-06-18"),
        ("Available Slots",  "09:00 - 09:30   |   10:00 - 10:30 ✓   |   11:00 - 11:30"),
        ("Room",             "Room 204 ▾"),
        ("Notes",            "Patient reports chest tightness on exertion."),
    ]
    for i, (lbl, val) in enumerate(fields):
        y = 7.5 - i * 0.85
        ax.text(3.0, y + 0.35, lbl, fontsize=9.5, color=DARK)
        box(ax, 3.0, y, 8.0, 0.45, val, fc=LIGHT_GRAY, fontcolor=DARK,
            fontsize=9, fontweight="normal")

    box(ax, 9.0, 1.0, 2.0, 0.6, "Save", fc=TEAL, fontcolor=WHITE, fontsize=10)
    box(ax, 7.0, 1.0, 1.8, 0.6, "Cancel", fc=GRAY, fontcolor=WHITE, fontsize=10)

    save(fig, "screen_appointment.png")


# ---------------- Mock screen: Billing ----------------
def screen_billing():
    fig, ax = plt.subplots(figsize=(13, 8))
    setup(ax, (0, 16), (0, 10))
    title(ax, "Screen 6 — Billing & Payments")

    box(ax, 0.2, 0.2, 2.6, 9.5, "", fc=NAVY)
    sidebar = ["Dashboard", "Patients", "Doctors", "Appointments", "Bills"]
    for i, s in enumerate(sidebar):
        y = 8.6 - i * 0.65
        bg = TEAL if s == "Bills" else NAVY
        box(ax, 0.4, y, 2.2, 0.5, s, fc=bg, fontcolor=WHITE, fontsize=9)

    box(ax, 3.0, 9.0, 12.8, 0.8, "  Bills   /   Invoice #INV-2026-0421",
        fc=WHITE, fontcolor=DARK, fontsize=10)

    box(ax, 3.1, 5.8, 12.7, 3.0, "", fc=WHITE)
    ax.text(3.4, 8.4, "Patient", fontsize=10, color=GRAY)
    ax.text(3.4, 8.0, "Ali Hassan (P-1001)", fontsize=11,
            fontweight="bold", color=DARK)
    ax.text(3.4, 7.4, "Insurance: MediPlus", fontsize=9.5, color=DARK)
    ax.text(3.4, 6.9, "Doctor: Dr. Sarah Mansour", fontsize=9.5, color=DARK)
    ax.text(3.4, 6.4, "Date: 2026-06-12 09:30", fontsize=9.5, color=DARK)

    ax.text(11.0, 8.4, "Status", fontsize=10, color=GRAY)
    box(ax, 11.0, 7.6, 1.8, 0.5, "Partially Paid", fc=ORANGE, fontcolor=WHITE,
        fontsize=10)
    ax.text(11.0, 7.0, "Due: 2026-07-12", fontsize=9.5, color=DARK)
    ax.text(11.0, 6.5, "Method: Stripe", fontsize=9.5, color=DARK)

    # items
    box(ax, 3.1, 1.5, 12.7, 4.1, "", fc=WHITE)
    headers = ["Description", "Qty", "Unit Price", "Total"]
    col_x = [3.4, 9.0, 11.3, 14.0]
    for cx, h in zip(col_x, headers):
        ax.text(cx, 5.1, h, fontsize=10, fontweight="bold", color=NAVY)
    items = [
        ("Cardiology Consultation",        "1",  "$ 80",   "$ 80"),
        ("ECG Test",                       "1",  "$ 45",   "$ 45"),
        ("Echocardiogram",                 "1",  "$ 120",  "$ 120"),
        ("Atorvastatin 20mg (30 tabs)",   "1",  "$ 22",   "$ 22"),
        ("Aspirin 100mg (60 tabs)",       "1",  "$ 8",    "$ 8"),
    ]
    for r, row in enumerate(items):
        y = 4.5 - r * 0.45
        for cx, val in zip(col_x, row):
            ax.text(cx, y, val, fontsize=9.5, color=DARK)

    ax.text(11.3, 2.0, "Subtotal", fontsize=10, color=DARK)
    ax.text(14.0, 2.0, "$ 275", fontsize=10, color=DARK)
    ax.text(11.3, 1.6, "Total", fontsize=11, color=NAVY, fontweight="bold")
    ax.text(14.0, 1.6, "$ 275", fontsize=11, color=NAVY, fontweight="bold")

    box(ax, 11.8, 0.5, 2.0, 0.6, "Pay Now", fc=GREEN, fontcolor=WHITE,
        fontsize=10)
    box(ax, 13.9, 0.5, 1.8, 0.6, "Print", fc=BLUE, fontcolor=WHITE,
        fontsize=10)

    save(fig, "screen_billing.png")


# ---------------- DB tables map ----------------
def diagram_db_map():
    fig, ax = plt.subplots(figsize=(11, 7))
    setup(ax, (0, 14), (0, 9))
    title(ax, "Figure 14 — Database Schema Overview")

    tables = [
        ("AspNetUsers",       NAVY,    [(0.5, 7.0, 2.3, 1.4)]),
        ("AspNetRoles",       NAVY,    [(3.0, 7.0, 2.3, 1.4)]),
        ("Patients",          BLUE,    [(0.5, 5.0, 2.3, 1.6)]),
        ("Doctors",           TEAL,    [(3.0, 5.0, 2.3, 1.6)]),
        ("Departments",       TEAL,    [(5.5, 5.0, 2.3, 1.6)]),
        ("Rooms",             TEAL,    [(8.0, 5.0, 2.3, 1.6)]),
        ("Appointments",      ORANGE,  [(0.5, 3.0, 2.3, 1.6)]),
        ("MedicalRecords",    ORANGE,  [(3.0, 3.0, 2.3, 1.6)]),
        ("Prescriptions",     ORANGE,  [(5.5, 3.0, 2.3, 1.6)]),
        ("PrescriptionItems", ORANGE,  [(8.0, 3.0, 2.3, 1.6)]),
        ("Bills",             ACCENT,  [(0.5, 1.0, 2.3, 1.6)]),
        ("BillItems",         ACCENT,  [(3.0, 1.0, 2.3, 1.6)]),
        ("Medicines",         GREEN,   [(5.5, 1.0, 2.3, 1.6)]),
        ("Schedules",         GREEN,   [(8.0, 1.0, 2.3, 1.6)]),
        ("Staff",             GRAY,    [(10.5, 7.0, 2.6, 1.4)]),
        ("Features",          GRAY,    [(10.5, 5.0, 2.6, 1.6)]),
        ("NursingUnits",      GRAY,    [(10.5, 3.0, 2.6, 1.6)]),
    ]

    for name, color, rects in tables:
        for x, y, w, h in rects:
            box(ax, x, y, w, h, name, fc=color, fontcolor=WHITE, fontsize=10)

    save(fig, "fig_db_map.png")


if __name__ == "__main__":
    diagram_architecture()
    diagram_request_flow()
    diagram_erd()
    diagram_auth_sequence()
    diagram_appointment_sequence()
    diagram_usecase()
    diagram_state()
    diagram_bill_flow()
    diagram_deployment()
    diagram_components()
    diagram_class()
    diagram_registration_activity()
    diagram_structure()
    diagram_db_map()
    screen_swagger()
    screen_login()
    screen_dashboard()
    screen_patients()
    screen_appointment_form()
    screen_billing()
    print("Done.")
