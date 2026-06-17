using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Hospital_Management_System.Configuration;

/// <summary>
/// Builds a SQL connection string safely (handles special characters in passwords)
/// and merges it into configuration so EF and health checks see one canonical value.
/// </summary>
public static class ConnectionStringResolver
{
    /// <summary>
    /// If <c>Database:Server</c>, <c>Database:Name</c> (or Database), <c>Database:User</c> (or UserId), and <c>Database:Password</c>
    /// are all set, builds via <see cref="SqlConnectionStringBuilder"/> and sets <c>ConnectionStrings:DefaultConnection</c>.
    /// Otherwise, if <c>ConnectionStrings:DefaultConnection</c> is set, re-parses it through the builder to normalize escaping.
    /// Call early in <c>Program.cs</c> before <c>AddDbContext</c>.
    /// </summary>
    public static void Apply(Microsoft.Extensions.Configuration.ConfigurationManager config)
    {
        var dbSection = config.GetSection("Database");
        var server = dbSection["Server"]?.Trim();
        var database = (dbSection["Name"] ?? dbSection["Database"])?.Trim();
        var user = (dbSection["User"] ?? dbSection["UserId"])?.Trim();
        var password = dbSection["Password"];

        if (!string.IsNullOrWhiteSpace(server) &&
            !string.IsNullOrWhiteSpace(database) &&
            !string.IsNullOrWhiteSpace(user) &&
            !string.IsNullOrWhiteSpace(password))
        {
            var built = BuildFromParts(dbSection, server, database, user, password);
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = built
            });
            return;
        }

        var existing = config.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(existing))
            return;

        try
        {
            var normalized = new SqlConnectionStringBuilder(existing).ConnectionString;
            if (!string.Equals(normalized, existing, StringComparison.Ordinal))
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = normalized
                });
            }
        }
        catch
        {
            // Leave original string if parser cannot handle it
        }
    }

    private static string BuildFromParts(IConfigurationSection dbSection, string server, string database, string user, string password)
    {
        var b = new SqlConnectionStringBuilder
        {
            DataSource = server,
            InitialCatalog = database,
            UserID = user,
            Password = password,
            MultipleActiveResultSets = dbSection.GetValue<bool>("MultipleActiveResultSets", true),
            Encrypt = dbSection.GetValue<bool>("Encrypt", false),
            TrustServerCertificate = dbSection.GetValue<bool>("TrustServerCertificate", true),
            ConnectTimeout = dbSection.GetValue<int>("ConnectTimeout", 30)
        };

        return b.ConnectionString;
    }
}
