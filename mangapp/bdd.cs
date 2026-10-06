using System;
using System.Threading.Tasks;
using System.Globalization;
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


        public static async Task<MySqlConnection> GetOpenConnectionAsync()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException(
                    "Chaîne de connexion non configurée. Appelez bdd.Configure(...) au démarrage."
                );

            var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            return conn;
        }


        public static async Task<System.Collections.Generic.List<Manga>> GetAllMangaAsync()
        {
            var list = new System.Collections.Generic.List<Manga>();

            await using var conn = await GetOpenConnectionAsync();
            await using var cmd = conn.CreateCommand();

            cmd.CommandText = "SELECT * FROM Manga";

            await using var reader = await cmd.ExecuteReaderAsync();

            // Construire un mapping des colonnes disponibles
            var columnMap =
                new System.Collections.Generic.Dictionary<string, int>(
                    StringComparer.OrdinalIgnoreCase
                );

            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnMap[reader.GetName(i)] = i;
            }

            while (await reader.ReadAsync())
            {
                string nom =
                    TryGetString(reader, columnMap, "nom")
                    ?? TryGetString(reader, columnMap, "title")
                    ?? "";

                int id =
                    TryGetInt(reader, columnMap, "Id_Manga")
                    ?? TryGetInt(reader, columnMap, "id")
                    ?? 0;

                // On essaie de lire un DateTime directement,
                // sinon on lit une année et on construit un DateTime.
                DateTime? anneeDt =
                    TryGetDateTime(reader, columnMap, "année")
                    ?? TryGetDateTime(reader, columnMap, "annee");

                if (!anneeDt.HasValue)
                {
                    int anneeInt =
                        TryGetInt(reader, columnMap, "année")
                        ?? TryGetInt(reader, columnMap, "annee")
                        ?? DateTime.Now.Year;

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

                double prix =
                    TryGetDouble(reader, columnMap, "prix")
                    ?? 0.0;

                int qte =
                    TryGetInt(reader, columnMap, "quantité")
                    ?? TryGetInt(reader, columnMap, "quantite")
                    ?? TryGetInt(reader, columnMap, "quantity")
                    ?? 0;

                int tome =
                    TryGetInt(reader, columnMap, "tome")
                    ?? 0;

                var m = new Manga(
                    nom,
                    id,
                    anneeDateTime,
                    prix,
                    qte,
                    tome
                );

                list.Add(m);
            }

            return list;
        }



        public static async Task<string> GetStockMangaAsync(int idManga)
        {
            await using var conn = await GetOpenConnectionAsync();
            await using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                SELECT quantité
                FROM Manga
                WHERE Id_Manga = @idManga;
            ";

            cmd.Parameters.AddWithValue("@idManga", idManga);

            object? resultat = await cmd.ExecuteScalarAsync();

            // Le manga n'existe pas
            if (resultat == null || resultat == DBNull.Value)
            {
                return "Manga introuvable";
            }

            int quantite = Convert.ToInt32(resultat);

            // Plus aucun exemplaire
            if (quantite == 0)
            {
                return "Rupture de stock";
            }

            // Entre 1 et 3 exemplaires
            if (quantite <= 3)
            {
                return "Stock faible";
            }

            // Plus de 3 exemplaires
            return "Stock suffisant";
        }

        // ============================================================
        // VUE GLOBALE DES ZONES
        // ============================================================

        public static async Task<System.Collections.Generic.List<Zones>> GetAllZonesAsync()
        {
            var zones = new System.Collections.Generic.List<Zones>();

            await using var conn = await GetOpenConnectionAsync();
            await using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                SELECT Id_Zones, libelle, capacité_max
                FROM Zones;
            ";

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                int idZone =
                    Convert.ToInt32(reader["Id_Zones"]);

                string libelle =
                    Convert.ToString(reader["libelle"]) ?? "";

                int capaciteMax =
                    Convert.ToInt32(reader["capacité_max"]);

                var zone = new Zones(
                    idZone,
                    libelle,
                    capaciteMax
                );

                zones.Add(zone);
            }

            return zones;
        }

        // ============================================================
        // STOCK D'UNE ZONE PRÉCISE
        // ============================================================

        public static async Task<string> GetStockZoneAsync(int idZone)
        {
            await using var conn = await GetOpenConnectionAsync();
            await using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                SELECT
                    z.capacité_max,
                    COALESCE(SUM(m.quantité), 0) AS stock_actuel
                FROM Zones z
                LEFT JOIN Manga m
                    ON m.Id_Zones = z.Id_Zones
                WHERE z.Id_Zones = @idZone
                GROUP BY z.Id_Zones, z.capacité_max;
            ";

            cmd.Parameters.AddWithValue("@idZone", idZone);

            await using var reader = await cmd.ExecuteReaderAsync();

            // La zone n'existe pas
            if (!await reader.ReadAsync())
            {
                return "Zone introuvable";
            }

            int capaciteMax =
                Convert.ToInt32(reader["capacité_max"]);

            int stockActuel =
                Convert.ToInt32(reader["stock_actuel"]);

            // Aucun manga dans la zone
            if (stockActuel == 0)
            {
                return "Zone vide";
            }

            // La zone est remplie à 25 % ou moins
            if (stockActuel <= capaciteMax * 0.25)
            {
                return "Stock faible";
            }

            return "Stock suffisant";
        }

        // ============================================================
        // MÉTHODES UTILITAIRES
        // ============================================================

        private static string? TryGetString(
            MySqlDataReader r,
            System.Collections.Generic.IDictionary<string, int> map,
            string key)
        {
            if (map.TryGetValue(key, out var idx) && !r.IsDBNull(idx))
                return r.GetString(idx);

            return null;
        }

        private static int? TryGetInt(
            MySqlDataReader r,
            System.Collections.Generic.IDictionary<string, int> map,
            string key)
        {
            if (map.TryGetValue(key, out var idx) && !r.IsDBNull(idx))
            {
                var v = r.GetValue(idx);

                if (v is int i) return i;
                if (v is long l) return Convert.ToInt32(l);
                if (v is short s) return Convert.ToInt32(s);
                if (v is decimal dec) return Convert.ToInt32(dec);
                if (v is double d) return Convert.ToInt32(d);

                var str = Convert.ToString(v)?.Trim();
                if (string.IsNullOrEmpty(str)) return null;

                // Try strict integer parse (invariant culture)
                if (int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedInt))
                    return parsedInt;

                // Try parsing as floating value then convert to int
                if (double.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var parsedDouble))
                    return Convert.ToInt32(parsedDouble);
            }

            return null;
        }

        private static double? TryGetDouble(
            MySqlDataReader r,
            System.Collections.Generic.IDictionary<string, int> map,
            string key)
        {
            if (map.TryGetValue(key, out var idx) && !r.IsDBNull(idx))
            {
                var v = r.GetValue(idx);

                if (v is double dd) return dd;
                if (v is float f) return Convert.ToDouble(f);
                if (v is decimal dec) return Convert.ToDouble(dec);
                if (v is int ii) return Convert.ToDouble(ii);

                var str = Convert.ToString(v)?.Trim();
                if (string.IsNullOrEmpty(str)) return null;

                // Try parse with invariant culture (accepts '.' as decimal)
                if (double.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var parsed))
                    return parsed;

                // Fallback to current culture (some DBs return numbers with comma)
                if (double.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out parsed))
                    return parsed;
            }

            return null;
        }

        private static DateTime? TryGetDateTime(
            MySqlDataReader r,
            System.Collections.Generic.IDictionary<string, int> map,
            string key)
        {
            if (map.TryGetValue(key, out var idx) && !r.IsDBNull(idx))
            {
                var v = r.GetValue(idx);

                if (v is DateTime dt)
                    return dt;

                if (DateTime.TryParse(Convert.ToString(v), out var parsed))
                    return parsed;
            }

            return null;
        }

        // Version synchrone si nécessaire.
        public static MySqlConnection GetOpenConnection()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException(
                    "Chaîne de connexion non configurée. Appelez bdd.Configure(...) au démarrage."
                );

            var conn = new MySqlConnection(_connectionString);
            conn.Open();

            return conn;
        }

    }
}
