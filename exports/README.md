# Hospital Data Exports

Static CSV dumps of the hospital seed data. Open any of these directly in Excel
(double-click → opens as a worksheet).

## Files

| File | Rows | Source |
| --- | --- | --- |
| `departments.csv` | 19 | All hospital departments |
| `rooms.csv` | 29 | All rooms with type, floor, building, department |
| `medicines.csv` | 38 | Pharmacy stock with prices and stock levels |
| `doctors.csv` | 3 | Department heads (hardcoded seed only) |
| `patients.csv` | 5 | Sample patients (hardcoded seed only) |
| `features.csv` | 3 | Bilingual feature cards shown on the dashboard |

## Convert to a single .xlsx workbook

In Excel:

1. Open `departments.csv` (it loads as Sheet1, renames it "Departments").
2. From Excel's File menu choose **Save As → Excel Workbook (.xlsx)**.
3. Repeat for the other CSVs, or use **Data → From Text/CSV** to import each
   one as a new sheet inside the same workbook.

## Why the doctor / patient files are small

The backend's `SeedData.cs` adds a few explicit rows by hand and then bulk
generates the rest at runtime:

- Doctors: 3 hardcoded + bulk-generated up to **8 per department** (≈ 152 total)
- Patients: 5 hardcoded + bulk-generated up to **300 total**
- Appointments: bulk-generated up to **500**
- Prescriptions: bulk-generated up to **200**
- Bills: bulk-generated up to **250**
- Medical records: bulk-generated up to **200**
- Schedules: 3 weekday shifts per doctor

The bulk-generated rows only exist after the backend starts and runs the seeder,
so they aren't in these static CSVs. To export the full live data, run the
backend and call the live export endpoint:

```
GET /api/Export/{entity}?format=csv
```

Example URLs once the backend is running on `http://localhost:5230`:

- `http://localhost:5230/api/Export/patients`
- `http://localhost:5230/api/Export/doctors`
- `http://localhost:5230/api/Export/appointments`
- `http://localhost:5230/api/Export/bills`
- `http://localhost:5230/api/Export/medicines`
- `http://localhost:5230/api/Export/departments`
- `http://localhost:5230/api/Export/rooms`

Each returns a UTF-8 BOM CSV that Excel opens cleanly (including any Arabic
characters in legacy rows).
