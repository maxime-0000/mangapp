using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace mangapp
{
    internal static class bdd
    {
        private static string? _connectionString;

        public static void Configure(string connectionString)
        {
            _connectionString = connectionString
                ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public static void ConfigureFromConfiguration(
            IConfiguration configuration,
            string name = "mangapp")
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var cs = configuration.GetConnectionString(name)
                ?? configuration[$"ConnectionStrings:{name}"];

            if (string.IsNullOrEmpty(cs))
                throw new InvalidOperationException(
                    $"Chaîne de connexion '{name}' introuvable.");

            Configure(cs);
        }

        public static void ConfigureFromEnvironment(
            string envVarName = "DEFAULT_CONN")
        {
            var cs = Environment.GetEnvironmentVariable(envVarName);

            if (string.IsNullOrEmpty(cs))
                throw new InvalidOperationException(
                    $"Variable d'environnement '{envVarName}' introuvable ou vide.");

            Configure(cs);
        }

        public static async Task<MySqlConnection> GetOpenConnectionAsync()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException(
                    "Chaîne de connexion non configurée.");

            var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            return conn;
        }

        public static MySqlConnection GetOpenConnection()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException(
                    "Chaîne de connexion non configurée.");

            var conn = new MySqlConnection(_connectionString);
            conn.Open();

            return conn;
        }
    }
}
