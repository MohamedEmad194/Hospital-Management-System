using System.Text.RegularExpressions;

namespace Hospital_Management_System.Data;

/// <summary>
/// ASCII-only login emails for doctors and patients (Arabic names stay in profile, not in email).
/// </summary>
public static class SampleEmailHelper
{
    public const string Domain = "hospital.com";

    private static readonly Regex AsciiLocalPart = new(
        @"^[a-z0-9][a-z0-9._+-]*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static string DoctorEmail(int doctorId) => $"dr.{doctorId}@{Domain}";

    public static string DoctorEmailForSeed(int departmentId, int sequenceInDept)
        => $"dr.dept{departmentId}.{sequenceInDept:D3}@{Domain}";

    public static string PatientEmail(int patientId) => $"patient.{patientId}@{Domain}";

    public static string PatientEmailForSeed(int sequence) => $"patient.{sequence:D4}@{Domain}";

    public static bool IsValidLoginEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var at = email.IndexOf('@');
        if (at <= 0 || at >= email.Length - 1)
            return false;

        var local = email[..at].ToLowerInvariant();
        var domain = email[(at + 1)..].ToLowerInvariant();

        if (domain != Domain)
            return false;

        if (local.Any(c => c > 127))
            return false;

        return AsciiLocalPart.IsMatch(local);
    }
}
