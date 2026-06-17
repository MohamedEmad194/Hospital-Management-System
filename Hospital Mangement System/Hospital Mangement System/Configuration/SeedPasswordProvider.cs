using System.Security.Cryptography;

namespace Hospital_Management_System.Configuration;

/// <summary>
/// Initial passwords for seeded Identity accounts — read from configuration only (user secrets / env).
/// Passwords are hashed by ASP.NET Identity before storage; never returned from APIs.
/// </summary>
public static class SeedPasswordProvider
{
    public const string SectionPath = "SeedOptions:InitialPasswords";

    public static string? GetPassword(IConfiguration configuration, string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return null;

        return configuration[$"{SectionPath}:{role}"]?.Trim();
    }

    /// <summary>
    /// Returns configured password or a strong random password (logged once at Information level for dev bootstrap only).
    /// </summary>
    public static string ResolveForProvisioning(
        IConfiguration configuration,
        string role,
        Microsoft.Extensions.Logging.ILogger logger,
        bool allowGeneratedInDevelopment,
        bool isDevelopment)
    {
        var configured = GetPassword(configuration, role);
        if (!string.IsNullOrEmpty(configured))
            return configured;

        if (!allowGeneratedInDevelopment || !isDevelopment)
        {
            logger.LogWarning(
                "Skipping Identity provisioning for role {Role}: set {Path}:{Role} via user secrets or environment variables.",
                role, SectionPath, role);
            return string.Empty;
        }

        logger.LogInformation(
            "Role {Role}: no {Path}:{Role} configured. A strong random password was generated and stored as a hash only. " +
            "Use forgot-password or set the secret and re-provision.",
            role, SectionPath, role);
        return GenerateStrongPassword();
    }

    private static string GenerateStrongPassword()
    {
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghijkmnopqrstuvwxyz";
        const string digits = "23456789";
        const string special = "!@#$%&*";
        var all = upper + lower + digits + special;

        Span<char> buffer = stackalloc char[16];
        buffer[0] = upper[RandomNumberGenerator.GetInt32(upper.Length)];
        buffer[1] = lower[RandomNumberGenerator.GetInt32(lower.Length)];
        buffer[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        buffer[3] = special[RandomNumberGenerator.GetInt32(special.Length)];
        for (var i = 4; i < buffer.Length; i++)
            buffer[i] = all[RandomNumberGenerator.GetInt32(all.Length)];

        return new string(buffer);
    }
}
