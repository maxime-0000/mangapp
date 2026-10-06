using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace mangapp
{
    internal static class bdd
    {
        private static string? _connectionString;

        // Configure la chaîne de connexion au démarrage de l'application.
        public static void Configure(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        // Configure à partir d'un IConfiguration (appsettings.json, variables d'env, etc.).
        public static void ConfigureFromConfiguration(IConfiguration configuration, string name = "mangapp")
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var cs = configuration.GetConnectionString(name) ?? configuration[$"ConnectionStrings:{name}"];
            if (string.IsNullOrEmpty(cs))
                throw new InvalidOperationException($"Chaîne de connexion '{name}' introuvable dans la configuration.");

            Configure(cs);
        }

        // Configure à partir d'une variable d'environnement contenant directement la chaîne de connexion.
        public static void ConfigureFromEnvironment(string envVarName = "DEFAULT_CONN")
        {
            var cs = Environment.GetEnvironmentVariable(envVarName);
            if (string.IsNullOrEmpty(cs))
                throw new InvalidOperationException($"Variable d'environnement '{envVarName}' introuvable ou vide.");

            Configure(cs);
        }

        // Retourne et ouvre une connexion MySql asynchrone. L'appelant doit disposer (Dispose) la connexion.
        public static async Task<MySqlConnection> GetOpenConnectionAsync()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Chaîne de connexion non configurée. Appelez bdd.Configure(...) au démarrage.");

            var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();
            return conn;
        }

        // Récupère tous les mangas depuis la table 'manga'.
        public static async Task<System.Collections.Generic.List<Manga>> GetAllMangaAsync()
        {
            var list = new System.Collections.Generic.List<Manga>();

            await using var conn = await GetOpenConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Manga";

            await using var reader = await cmd.ExecuteReaderAsync();

            // Construire un mapping des colonnes disponibles
            var columnMap = new System.Collections.Generic.Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnMap[reader.GetName(i)] = i;
            }

            while (await reader.ReadAsync())
            {
				string nom = TryGetString(reader, columnMap, "nom") ?? TryGetString(reader, columnMap, "title") ?? "";
				int id = TryGetInt(reader, columnMap, "Id_Manga") ?? TryGetInt(reader, columnMap, "id") ?? 0;

                // 1. On essaie de lire un DateTime directement, sinon on lit une année (int) et on construit un DateTime (1er janvier)
                DateTime? anneeDt = TryGetDateTime(reader, columnMap, "année") ?? TryGetDateTime(reader, columnMap, "annee");
                if (!anneeDt.HasValue)
                {
                    int anneeInt = TryGetInt(reader, columnMap, "année") ?? TryGetInt(reader, columnMap, "annee") ?? DateTime.Now.Year;
                    try
                    {
                        anneeDt = new DateTime(anneeInt, 1, 1);
                    }
                    catch
                    {
                        // Si l'année est invalide, fallback sur l'année courante
                        anneeDt = new DateTime(DateTime.Now.Year, 1, 1);
                    }
                }
                DateTime anneeDateTime = anneeDt.Value;

				double prix = TryGetDouble(reader, columnMap, "prix") ?? 0.0;
				int qte = TryGetInt(reader, columnMap, "quantité") ?? TryGetInt(reader, columnMap, "quantite") ?? TryGetInt(reader, columnMap, "quantity") ?? 0;
				int tome = TryGetInt(reader, columnMap, "tome") ?? 0;

				// 3. On passe maintenant l'objet DateTime attendu par votre constructeur
				var m = new Manga(nom, id, anneeDateTime, prix, qte, tome);
				list.Add(m);
			}

            return list;
        }

        private static string? TryGetString(MySqlDataReader r, System.Collections.Generic.IDictionary<string, int> map, string key)
        {
            if (map.TryGetValue(key, out var idx) && !r.IsDBNull(idx)) return r.GetString(idx);
            return null;
        }

        private static int? TryGetInt(MySqlDataReader r, System.Collections.Generic.IDictionary<string, int> map, string key)
        {
            if (map.TryGetValue(key, out var idx) && !r.IsDBNull(idx)) return Convert.ToInt32(r.GetValue(idx));
            return null;
        }

        private static double? TryGetDouble(MySqlDataReader r, System.Collections.Generic.IDictionary<string, int> map, string key)
        {
            if (map.TryGetValue(key, out var idx) && !r.IsDBNull(idx)) return Convert.ToDouble(r.GetValue(idx));
            return null;
        }

        private static DateTime? TryGetDateTime(MySqlDataReader r, System.Collections.Generic.IDictionary<string, int> map, string key)
        {
            if (map.TryGetValue(key, out var idx) && !r.IsDBNull(idx))
            {
                var v = r.GetValue(idx);
                if (v is DateTime dt) return dt;
                if (DateTime.TryParse(Convert.ToString(v), out var parsed)) return parsed;
            }
            return null;
        }

        // Version synchrone si nécessaire.
        public static MySqlConnection GetOpenConnection()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Chaîne de connexion non configurée. Appelez bdd.Configure(...) au démarrage.");

            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
