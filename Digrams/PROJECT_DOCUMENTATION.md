## Hospital Management System - Comprehensive Documentation

Version: 1.0
Maintainers: HMS Team

### How to Use This Document
- This single file is organized for quick skim and deep dives.
- Each top-level section has a concise summary followed by details.
- Use the Table of Contents links to jump to areas of interest.

---

### Table of Contents
1. Executive Summary
2. System Overview
3. Architecture & Design
4. Backend (.NET) Services
5. Data Model & Database
6. API Endpoints Cheat Sheet
7. Authentication & Authorization
8. Frontend (React) Application
9. Internationalization (Arabic/English) & RTL
10. Error Handling & Logging
11. Environment Configuration
12. Local Development
13. Building & Deployment
14. Observability & Diagnostics
15. Security Considerations
16. Performance & Scalability
17. Data Integrity & Backups
18. Accessibility & UX
19. Testing Strategy
20. Troubleshooting Guide
21. Roadmap & Future Work
22. Glossary

---

### 1) Executive Summary
The Hospital Management System (HMS) is a full-stack application that streamlines operational workflows for hospitals and clinics. It provides modules for Patients, Doctors, Departments, Appointments, Medicines, Bills, Records, and Admin dashboards. The backend is built on ASP.NET Core with Entity Framework Core and SQL Server; the frontend is built with React. The app supports bilingual UI (Arabic/English) with automatic Right-To-Left support for Arabic.

Goals:
- Unified operational view of hospital data
- Fast administrative workflows (scheduling, billing, inventory)
- Secure, auditable, and scalable foundation
- Great UX with multilingual support

Non-Goals for v1:
- Clinical decision support (CDS)
- HL7/FHIR interoperability (see roadmap)

---

### 2) System Overview
High-level modules:
- Identity & Auth
- Patients & Doctors
- Departments & Rooms
- Appointments & Schedules
- Medicines & Inventory
- Billing & Payments
- Dashboard & Reporting

Data flow (simplified):
```
[React UI] → [API Client] → [ASP.NET Controllers] → [Services] → [EF Core] → [SQL Server]
                                                     ↘ [Logs]
```

---

### 3) Architecture & Design

Patterns & principles:
- Clean layering: Controllers → Services → Data (DbContext/EF)
- DTO mapping via AutoMapper
- Dependency Injection (built-in ASP.NET Core DI)
- Configuration via `appsettings.json` per environment

Logical view:
```
┌─────────────────────────────────────────────────────────┐
│                       React Frontend                    │
│  Routing, Context/Auth, Pages, Components, i18n, RTL    │
└───────────────▲───────────────────────────────▲────────┘
                │                               │
                │ HTTP/JSON                     │ Auth state
                │                               │ (tokens/session)
┌───────────────┴───────────────────────────────┴────────┐
│                    ASP.NET Core API                     │
│ Controllers → Services → EF Core → SQL Server          │
│ Auth, Validation, Logging, Mapping                      │
└─────────────────────────────────────────────────────────┘
```

Deployment view:
```
Frontend (static build)  →  CDN / Web server
Backend (ASP.NET Core)   →  Container / IIS / Kestrel
Database                 →  SQL Server (on-prem or managed)
```

---

### 4) Backend (.NET) Services
Project: `Hospital Mangement System/Hospital Mangement System`

Key directories:
- `Controllers/` Appointments, Auth, Bills, Dashboard, Departments, Doctors, MedicalRecords, Medicines, Patients, Prescriptions, Rooms, Schedules
- `Services/` Business logic per domain
- `Data/` `HospitalDbContext.cs`, `SeedData.cs`
- `DTOs/` Transfer models
- `Mappings/` AutoMapper profile
- `Models/` Entity definitions

Request lifecycle:
1) HTTP hits Controller action
2) Controller calls Service
3) Service uses DbContext (EF) to query/update
4) AutoMapper maps between Entities and DTOs
5) Action returns `ActionResult<T>` with proper status codes

---

### 5) Data Model & Database
Primary entities (not exhaustive):
- `User`, `Staff`
- `Patient`, `Doctor`, `Department`, `Room`
- `Appointment`, `Schedule`
- `Medicine`, `MedicalRecord`, `Prescription`, `PrescriptionItem`
- `Bill`, `BillItem`

Relational notes:
- `Appointment` links `Patient` ↔ `Doctor` (+ time slots)
- `Bill` references `Patient` and aggregates `BillItem`
- `Prescription` references `Patient`/`Doctor` and includes items

Migrations folder contains schema history; EF Core handles creation/updates.

---

### 6) API Endpoints Cheat Sheet
Examples (refer to `API_DOCUMENTATION.md` for full details):
- `GET /Dashboard/stats`
- `GET /Patients`, `POST /Patients`
- `GET /Doctors`, `POST /Doctors`
- `GET /Appointments`, `POST /Appointments`
- `GET /Medicines`, `POST /Medicines`
- `GET /Bills`, `POST /Bills`

Conventions:
- JSON body for create/update
- Standard HTTP status codes
- Validation errors returned as problem details / error object

---

### 7) Authentication & Authorization
Auth controller provides login. The frontend stores session via `AuthContext`. Protected routes use `ProtectedRoute` to guard pages. Roles and claims can be extended in the Services/Auth layer.

Session flow (frontend):
```
Login form → Auth API → store user/session in context → guard routes
```

---

### 8) Frontend (React) Application
Project: `Hospital frontend/my-app`

Structure:
- `src/pages/` Dashboards and CRUD screens
- `src/components/` `Layout`, `ProtectedRoute`
- `src/api/` API client modules
- `src/context/AuthContext.js` auth state
- `src/i18n.js` i18next initialization and resources

Routing (in `App.js`):
```
/login
/(protected) /, /patients, /doctors, /appointments, /bills, /departments, /medicines
```

Styling approach: lightweight inline styles + CSS variables.

---

### 9) Internationalization (Arabic/English) & RTL
- Libraries: `i18next`, `react-i18next`, `i18next-browser-languagedetector`
- Resources defined in `src/i18n.js` for `en` and `ar`
- Direction handled globally:
  - `document.documentElement.lang = lng`
  - `document.documentElement.dir = 'rtl' | 'ltr'`
- Language switcher in `Layout` header
- Pages updated to use `useTranslation()` and `t()` for user-visible text

---

### 10) Error Handling & Logging
Backend:
- Centralized logging to `logs/` with environment-specific files
- Services return domain errors; controllers map to HTTP codes

Frontend:
- User-friendly messages on network/API errors
- Common errors translated (login/network/SSL)

---

### 11) Environment Configuration
Backend:
- `appsettings.json`, `appsettings.Development.json`
- Connection strings, logging levels, CORS

Frontend:
- `.env` (optional) for `REACT_APP_*` and `PUBLIC_URL`
- Public assets served from `/public`

---

### 12) Local Development
Prerequisites:
- Node.js LTS, .NET SDK, SQL Server (local/Dev container)

Steps:
1) Backend: `dotnet restore && dotnet run`
2) Frontend: `npm install && npm start` in `Hospital frontend/my-app`
3) Ensure API base URL in `src/api/client.js` is correct

Test data:
- `Data/SeedData.cs` seeds minimal records for quick start

---

### 13) Building & Deployment
Frontend:
- `npm run build` → `build/` static assets
- Serve via any static web server or CDN

Backend:
- Containerize via provided `Dockerfile`/`docker-compose.yml`
- Or deploy to IIS/Kestrel on Windows Server/Linux

Database:
- Run EF migrations on startup or via CLI prior to first run

---

### 14) Observability & Diagnostics
Logging:
- Application logs under `logs/`
Metrics (future):
- Add ASP.NET Core metrics, OpenTelemetry exporters

---

### 15) Security Considerations
- Use HTTPS end-to-end
- Validate inputs; leverage model validation attributes
- Restrict CORS to trusted origins
- Store secrets outside source control (Key Vault/Secret Manager)
- Principle of least privilege for DB and app identities

---

### 16) Performance & Scalability
- Use pagination on list endpoints
- Add indexes for frequent queries (e.g., `Appointments` by date/doctor)
- Consider read replicas/reporting DB for heavy dashboards
- Frontend: memoization, virtualization for large tables (future)

---

### 17) Data Integrity & Backups
- Enable SQL Server backups and point-in-time recovery
- Consider soft-deletes for critical entities
- Add foreign keys and cascade rules thoughtfully

---

### 18) Accessibility & UX
- High-contrast colors for key actions
- Text alternatives for icons/images
- Keyboard navigation on forms and lists (progressive)
- Proper directionality for RTL texts

---

### 19) Testing Strategy
Backend (future):
- Unit tests for Services and Mappers
- Integration tests for Controllers using TestServer

Frontend (future):
- Component tests via React Testing Library
- Basic e2e via Playwright/Cypress

---

### 20) Troubleshooting Guide
Symptoms → Checks:
- Frontend cannot reach API → Verify API base URL, CORS, HTTPS cert
- Login fails → Check user seed, HTTPS vs HTTP mixed content
- Database errors → Check connection string, migrations applied
- Arabic not RTL → Confirm `documentElement.dir` switching and `i18n` config
- Hero image not showing → Ensure `public/hospital-hero.jpg` exists and `PUBLIC_URL` path

---

### 21) Roadmap & Future Work
- Roles & fine-grained permissions
- Inventory thresholds & alerts
- HL7/FHIR interoperability
- Reporting exports (CSV/PDF) and BI integration
- Notifications (email/SMS) for appointments and bills

---

### 22) Glossary
- HMS: Hospital Management System
- DTO: Data Transfer Object
- EF Core: Entity Framework Core
- RTL: Right-To-Left
- PII: Personally Identifiable Information

---

Appendix A: ASCII Diagrams
```
Frontend Build
  src → transpile → optimize → build/static → served by web server

Backend Request
  Client → Controller → Service → DbContext → SQL → map → JSON
```

Appendix C: Entity-Relationship (ER) Diagram (simplified)
```
Patient (id) 1---* Appointment *---1 Doctor (id)
Patient (id) 1---* Bill (id) 1---* BillItem (id)
Doctor (id)  *---1 Department (id)
Patient (id) 1---* MedicalRecord (id)
Patient (id) 1---* Prescription (id) 1---* PrescriptionItem (id)
Medicine (id) 1---* PrescriptionItem (id)
```

Appendix D: Example Sequence Diagrams

1) Create Appointment
```
User → Frontend: fill form
Frontend → API: POST /Appointments {patientId, doctorId, date, time}
API Controller → Service: validate, business rules
Service → DbContext: insert Appointment
DbContext → SQL Server: save
Service → Controller: result DTO
Controller → Frontend: 201 Created + DTO
Frontend → User: success toast, navigate to list
```

2) Create Bill with Items
```
User → Frontend: add items
Frontend → API: POST /Bills {patientId, billItems:[...]}
Controller → Service: validate totals
Service → DbContext: insert Bill + BillItems (transaction)
DbContext → SQL: commit
Controller → Frontend: 201 + bill summary
```

---

Expanded API Reference (selected)

Patients
- GET /Patients → list (paginate optional)
- POST /Patients → create

Doctors
- GET /Doctors → list
- POST /Doctors → create

Appointments
- GET /Appointments → list (filters: date, doctorId, patientId, status)
- POST /Appointments → create

Bills
- GET /Bills → list
- POST /Bills → create { patientId, billDate, dueDate, billItems:[{description, quantity, unitPrice}] }

Medicines
- GET /Medicines → list
- POST /Medicines → create

Departments
- GET /Departments → list
- POST /Departments → create

Notes:
- All POST endpoints expect JSON body and return created DTO.
- Validation errors return 400 with error details.

Appendix B: Directory Map (selected)
```
Hospital frontend/my-app
  ├─ public/
  │  └─ hospital-hero.jpg
  └─ src/
     ├─ api/
     ├─ components/
     ├─ context/
     ├─ pages/
     └─ i18n.js

Hospital Mangement System/Hospital Mangement System
  ├─ Controllers/
  ├─ Services/
  ├─ Data/
  ├─ DTOs/
  ├─ Models/
  └─ Mappings/
```

End of document.
