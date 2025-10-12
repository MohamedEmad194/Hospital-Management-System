Hospital Management System - React Frontend

This is the professional React frontend for the Hospital Management System backend.

Prerequisites
- Node.js 18+ and npm
- Backend running locally

Configure API Base URL
Create a `.env.local` file in this folder with:

```
REACT_APP_API_BASE_URL=http://localhost:5230
```

Adjust the URL if your backend runs on another port or host.

Install and Run
```
npm install
npm start
```

The app will open at `http://localhost:3000`.

Default Login
Use your seeded admin credentials. If you used the provided seed, try:
- Email: `admin@hms.dev`
- Password: `Admin@123`

Features Included
- Auth with JWT (login, auto-attach token, redirect on 401)
- Protected routes and basic admin layout
- Dashboard consuming `/api/Dashboard/stats`
- Patients list and search (`/api/Patients` and `/api/Patients/search`)

Project Structure (added files)
- `src/api/client.js` Axios client with base URL, token interceptors
- `src/api/auth.js` Auth endpoints (login, profile)
- `src/api/patients.js` Patient CRUD endpoints
- `src/context/AuthContext.js` Auth state and actions
- `src/components/ProtectedRoute.js` Guarded routing
- `src/components/Layout.js` Sidebar + header layout
- `src/pages/Login.js` Login screen
- `src/pages/Dashboard.js` Dashboard stats
- `src/pages/PatientsList.js` Patients table with search

Notes
- CORS is enabled in the backend (`AllowAll` policy). Ensure it remains active for local dev.
- HTTPS: if your backend enforces HTTPS, update the base URL accordingly.
- Tokens are stored in `localStorage`. Logout clears the token.


