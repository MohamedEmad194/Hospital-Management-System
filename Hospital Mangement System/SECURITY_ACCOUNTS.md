# Account security (HMS API)

## How passwords are stored

- All login passwords are stored **only** as hashes in `AspNetUsers.PasswordHash` (ASP.NET Core Identity).
- The API **never** returns plaintext passwords in responses.
- The login page does **not** show default passwords.

## Initial passwords for seeded accounts (development)

Set via **User Secrets** or environment variables (not committed to git):

```powershell
cd "Hospital Mangement System\Hospital Mangement System"
dotnet user-secrets set "SeedOptions:InitialPasswords:Admin" "YourSecureAdminPass1!"
dotnet user-secrets set "SeedOptions:InitialPasswords:Doctor" "YourSecureDoctorPass1!"
dotnet user-secrets set "SeedOptions:InitialPasswords:Patient" "YourSecurePatientPass1!"
dotnet user-secrets set "SeedOptions:InitialPasswords:Staff" "YourSecureStaffPass1!"
```

If a role password is not configured in Development, a **strong random password** is generated once and stored as a hash only — use **Forgot password** on the login page or change it via an admin.

## Production

- Do not enable `SeedOptions:EnableSampleData` or `EnableUserProvisioning` on production hosts.
- `TestCredentials` and `QuickTest` endpoints return **404** outside Development.
- Use **Register** or admin-created accounts with strong unique passwords.
