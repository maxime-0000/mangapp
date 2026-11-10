<<<<<<< HEAD
﻿
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MySqlConnector;
=======
﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
>>>>>>> f6c204a4c8d3e8c1a1fbc867f54e5faf5e3411b3

namespace mangapp
{
    public partial class Statistique : Page
    {
        public Statistique()
        {
            InitializeComponent();
        }

<<<<<<< HEAD
=======

>>>>>>> f6c204a4c8d3e8c1a1fbc867f54e5faf5e3411b3
        // ============================================================
        // VUE GLOBALE DES MANGAS
        // ============================================================

        private async void AfficherMangas_Click(
<<<<<<< HEAD
            object sender, RoutedEventArgs e)
        {
            try
            {
                var mangas = await GetAllMangaAsync();
=======
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                var mangas = await bdd.GetAllMangaAsync();
                
>>>>>>> f6c204a4c8d3e8c1a1fbc867f54e5faf5e3411b3
                dgMangas.ItemsSource = mangas;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors du chargement des mangas : "
<<<<<<< HEAD
                    + ex.Message);
            }
        }

        private async Task<List<Manga>> GetAllMangaAsync()
        {
            var list = new List<Manga>();

            await using var conn =
                await bdd.GetOpenConnectionAsync();

            await using var cmd = conn.CreateCommand();

            cmd.CommandText = "SELECT * FROM Manga";

            await using var reader =
                await cmd.ExecuteReaderAsync();

            var columnMap = new Dictionary<string, int>(
                StringComparer.OrdinalIgnoreCase);

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
                        anneeDt = new DateTime(
                            DateTime.Now.Year, 1, 1);
                    }
                }

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

                var manga = new Manga(
                    nom,
                    id,
                    anneeDt.Value,
                    prix,
                    qte,
                    tome);

                list.Add(manga);
            }

            return list;
        }
=======
                    + ex.Message
                );
            }
        }

>>>>>>> f6c204a4c8d3e8c1a1fbc867f54e5faf5e3411b3

        // ============================================================
        // STOCK D'UN MANGA PRÉCIS
        // ============================================================

        private async void VerifierManga_Click(
<<<<<<< HEAD
            object sender, RoutedEventArgs e)
        {
            try
            {
                string nom = txtIdManga.Text?.Trim() ?? "";
=======
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                string nom = txtIdManga.Text?.Trim();
>>>>>>> f6c204a4c8d3e8c1a1fbc867f54e5faf5e3411b3

                if (string.IsNullOrEmpty(nom))
                {
                    MessageBox.Show(
<<<<<<< HEAD
                        "Veuillez saisir le nom du manga.");
                    return;
                }

                string stock = await GetStockMangaAsync(nom);
=======
                        "Veuillez saisir le nom du manga."
                    );

                    return;
                }


                string stock =
                    await bdd.GetStockMangaAsync(nom);

>>>>>>> f6c204a4c8d3e8c1a1fbc867f54e5faf5e3411b3

                lblStockManga.Text =
                    "État du stock : " + stock;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors de la vérification du stock : "
<<<<<<< HEAD
                    + ex.Message);
            }
        }

        private async Task<string> GetStockMangaAsync(string nom)
        {
            await using var conn =
                await bdd.GetOpenConnectionAsync();

            await using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                SELECT `quantité`
                FROM Manga
                WHERE nom = @nom;
            ";

            cmd.Parameters.AddWithValue("@nom", nom);

            object? resultat = await cmd.ExecuteScalarAsync();

            if (resultat == null || resultat == DBNull.Value)
                return "Manga introuvable";

            int quantite = Convert.ToInt32(resultat);

            if (quantite == 0)
                return "Rupture de stock";

            if (quantite <= 3)
                return "Stock faible";

            return "Stock suffisant";
        }

        // ============================================================
        // RÉCUPÉRER LE MANGA SÉLECTIONNÉ DANS LE TABLEAU
        // ============================================================

        private int? GetSelectedMangaId()
        {
            if (dgMangas.SelectedItem is not Manga manga)
            {
                MessageBox.Show(
                    "Veuillez sélectionner un manga dans le tableau.");
                return null;
            }

            // On recherche la propriété de l'identifiant.
            // Cela accepte Id ou Id_Manga.
            var propriete = manga.GetType().GetProperty(
                "Id",
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.IgnoreCase);

            if (propriete == null)
            {
                propriete = manga.GetType().GetProperty(
                    "Id_Manga",
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.IgnoreCase);
            }

            if (propriete == null)
            {
                MessageBox.Show(
                    "Impossible de récupérer l'ID du manga.");
                return null;
            }

            object? valeur = propriete.GetValue(manga);

            if (valeur == null)
                return null;

            return Convert.ToInt32(valeur);
        }

        // ============================================================
        // AJOUTER DES EXEMPLAIRES AU STOCK
        // ============================================================

        private async void AjouterStock_Click(
            object sender, RoutedEventArgs e)
        {
            int? id = GetSelectedMangaId();

            if (!id.HasValue)
                return;

            if (!int.TryParse(
                txtGestionQuantite.Text, out int quantite)
                || quantite <= 0)
            {
                MessageBox.Show(
                    "Veuillez saisir une quantité positive.");
                return;
            }

            try
            {
                await using var conn =
                    await bdd.GetOpenConnectionAsync();

                await using var cmd = conn.CreateCommand();

                cmd.CommandText = @"
                    UPDATE Manga
                    SET `quantité` = `quantité` + @quantite
                    WHERE Id_Manga = @id;
                ";

                cmd.Parameters.AddWithValue("@quantite", quantite);
                cmd.Parameters.AddWithValue("@id", id.Value);

                int lignes = await cmd.ExecuteNonQueryAsync();

                if (lignes == 0)
                {
                    MessageBox.Show("Manga introuvable.");
                    return;
                }

                MessageBox.Show(
                    "Exemplaires ajoutés avec succès !");

                await ActualiserMangasAsync(id.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors de l'ajout du stock : "
                    + ex.Message);
            }
        }

        // ============================================================
        // RETIRER DES EXEMPLAIRES DU STOCK
        // ============================================================

        private async void RetirerStock_Click(
            object sender, RoutedEventArgs e)
        {
            int? id = GetSelectedMangaId();

            if (!id.HasValue)
                return;

            if (!int.TryParse(
                txtGestionQuantite.Text, out int quantite)
                || quantite <= 0)
            {
                MessageBox.Show(
                    "Veuillez saisir une quantité positive.");
                return;
            }

            try
            {
                await using var conn =
                    await bdd.GetOpenConnectionAsync();

                await using var cmd = conn.CreateCommand();

                cmd.CommandText = @"
                    UPDATE Manga
                    SET `quantité` = `quantité` - @quantite
                    WHERE Id_Manga = @id
                    AND `quantité` >= @quantite;
                ";

                cmd.Parameters.AddWithValue("@quantite", quantite);
                cmd.Parameters.AddWithValue("@id", id.Value);

                int lignes = await cmd.ExecuteNonQueryAsync();

                if (lignes == 0)
                {
                    MessageBox.Show(
                        "Stock insuffisant ou manga introuvable.");
                    return;
                }

                MessageBox.Show(
                    "Exemplaires retirés avec succès !");

                await ActualiserMangasAsync(id.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors du retrait du stock : "
                    + ex.Message);
            }
        }

        // ============================================================
        // MODIFIER LES INFORMATIONS D'UN MANGA
        // ============================================================

        private async void ModifierManga_Click(
            object sender, RoutedEventArgs e)
        {
            int? id = GetSelectedMangaId();

            if (!id.HasValue)
                return;

            try
            {
                string nom;
                DateTime annee;
                decimal prix;
                int tome;

                // Récupération des informations du manga
                // directement dans la base de données.

                await using (var conn =
                    await bdd.GetOpenConnectionAsync())
                {
                    await using var cmd = conn.CreateCommand();

                    cmd.CommandText = @"
                        SELECT nom, `année`, prix, tome
                        FROM Manga
                        WHERE Id_Manga = @id;
                    ";

                    cmd.Parameters.AddWithValue("@id", id.Value);

                    await using var reader =
                        await cmd.ExecuteReaderAsync();

                    if (!await reader.ReadAsync())
                    {
                        MessageBox.Show("Manga introuvable.");
                        return;
                    }

                    nom = Convert.ToString(reader["nom"]) ?? "";

                    int anneeInt = Convert.ToInt32(reader["année"]);

                    annee = new DateTime(anneeInt, 1, 1);

                    prix = Convert.ToDecimal(reader["prix"]);

                    tome = Convert.ToInt32(reader["tome"]);
                }

                // Création de la fenêtre de modification.

                var fenetre = new Window
                {
                    Title = "Modifier un manga",
                    Width = 360,
                    Height = 400,
                    ResizeMode = ResizeMode.NoResize,
                    WindowStartupLocation =
                        WindowStartupLocation.CenterOwner,
                    Background = System.Windows.Media.Brushes.White
                };

                Window? parent = Window.GetWindow(this);

                if (parent != null)
                    fenetre.Owner = parent;

                var panel = new StackPanel
                {
                    Margin = new Thickness(20)
                };

                // Méthode locale pour ajouter un champ.

                TextBox AjouterChamp(string label, string valeur)
                {
                    panel.Children.Add(new TextBlock
                    {
                        Text = label,
                        FontSize = 14,
                        Margin = new Thickness(0, 0, 0, 4)
                    });

                    var champ = new TextBox
                    {
                        Text = valeur,
                        Height = 28,
                        FontSize = 14,
                        Margin = new Thickness(0, 0, 0, 12),
                        VerticalContentAlignment =
                            VerticalAlignment.Center
                    };

                    panel.Children.Add(champ);

                    return champ;
                }

                // Champs préremplis.

                var txtNom = AjouterChamp("Nom :", nom);

                var txtAnnee = AjouterChamp(
                    "Date (jj/mm/aaaa) :",
                    annee.ToString("dd/MM/yyyy"));

                var txtPrix = AjouterChamp(
                    "Prix :",
                    prix.ToString(
                        CultureInfo.GetCultureInfo("fr-FR")));

                var txtTome = AjouterChamp(
                    "Tome :",
                    tome.ToString());

                // Bouton d'enregistrement.

                var btnEnregistrer = new Button
                {
                    Content = "Enregistrer les modifications",
                    Height = 35,
                    Margin = new Thickness(0, 8, 0, 0)
                };

                panel.Children.Add(btnEnregistrer);

                fenetre.Content = panel;

                // Enregistrement des nouvelles informations.

                btnEnregistrer.Click += async (s, args) =>
                {
                    string nouveauNom = txtNom.Text.Trim();

                    if (string.IsNullOrWhiteSpace(nouveauNom))
                    {
                        MessageBox.Show(
                            "Le nom du manga est obligatoire.");
                        return;
                    }

                    if (!DateTime.TryParseExact(
                        txtAnnee.Text.Trim(),
                        "dd/MM/yyyy",
                        CultureInfo.GetCultureInfo("fr-FR"),
                        DateTimeStyles.None,
                        out DateTime nouvelleAnnee))
                    {
                        MessageBox.Show(
                            "Date invalide. Format : jj/mm/aaaa.");
                        return;
                    }

                    string textePrix = txtPrix.Text.Trim();

                    bool prixValide = decimal.TryParse(
                        textePrix,
                        NumberStyles.Number,
                        CultureInfo.GetCultureInfo("fr-FR"),
                        out decimal nouveauPrix);

                    if (!prixValide)
                    {
                        prixValide = decimal.TryParse(
                            textePrix,
                            NumberStyles.Number,
                            CultureInfo.InvariantCulture,
                            out nouveauPrix);
                    }

                    if (!prixValide || nouveauPrix < 0)
                    {
                        MessageBox.Show(
                            "Veuillez saisir un prix valide.");
                        return;
                    }

                    if (!int.TryParse(
                        txtTome.Text,
                        out int nouveauTome)
                        || nouveauTome <= 0)
                    {
                        MessageBox.Show(
                            "Veuillez saisir un numéro de tome valide.");
                        return;
                    }

                    try
                    {
                        btnEnregistrer.IsEnabled = false;

                        await using var conn =
                            await bdd.GetOpenConnectionAsync();

                        await using var cmd = conn.CreateCommand();

                        cmd.CommandText = @"
                            UPDATE Manga
                            SET nom = @nom,
                                `année` = @annee,
                                prix = @prix,
                                tome = @tome
                            WHERE Id_Manga = @id;
                        ";

                        cmd.Parameters.AddWithValue(
                            "@nom", nouveauNom);

                        cmd.Parameters.AddWithValue(
                            "@annee", nouvelleAnnee.Year);

                        cmd.Parameters.AddWithValue(
                            "@prix", nouveauPrix);

                        cmd.Parameters.AddWithValue(
                            "@tome", nouveauTome);

                        cmd.Parameters.AddWithValue(
                            "@id", id.Value);

                        int lignes =
                            await cmd.ExecuteNonQueryAsync();

                        if (lignes == 0)
                        {
                            // Une mise à jour identique peut
                            // retourner zéro ligne modifiée.
                            await using var verification =
                                conn.CreateCommand();

                            verification.CommandText = @"
                                SELECT COUNT(*)
                                FROM Manga
                                WHERE Id_Manga = @id;
                            ";

                            verification.Parameters.AddWithValue(
                                "@id", id.Value);

                            int existe = Convert.ToInt32(
                                await verification.ExecuteScalarAsync());

                            if (existe == 0)
                            {
                                MessageBox.Show("Manga introuvable.");
                                return;
                            }
                        }

                        fenetre.DialogResult = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Erreur de modification : "
                            + ex.Message);
                    }
                    finally
                    {
                        btnEnregistrer.IsEnabled = true;
                    }
                };

                // Affichage de la fenêtre.

                if (fenetre.ShowDialog() == true)
                {
                    await ActualiserMangasAsync(id.Value);

                    MessageBox.Show(
                        "Manga modifié avec succès !");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur : " + ex.Message);
            }
        }

        // ============================================================
        // ACTUALISER LE TABLEAU DES MANGAS
        // ============================================================

        private async Task ActualiserMangasAsync(int id)
        {
            var mangas = await GetAllMangaAsync();

            dgMangas.ItemsSource = mangas;

            // Retrouver le manga précédemment sélectionné.

            foreach (var manga in mangas)
            {
                var propriete = manga.GetType().GetProperty(
                    "Id",
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.IgnoreCase);

                if (propriete == null)
                {
                    propriete = manga.GetType().GetProperty(
                        "Id_Manga",
                        BindingFlags.Public |
                        BindingFlags.Instance |
                        BindingFlags.IgnoreCase);
                }

                if (propriete != null &&
                    Convert.ToInt32(
                        propriete.GetValue(manga)) == id)
                {
                    dgMangas.SelectedItem = manga;
                    break;
                }
            }
        }
=======
                    + ex.Message
                );
            }
        }

>>>>>>> f6c204a4c8d3e8c1a1fbc867f54e5faf5e3411b3

        // ============================================================
        // VUE GLOBALE DES ZONES
        // ============================================================

        private async void AfficherZones_Click(
<<<<<<< HEAD
            object sender, RoutedEventArgs e)
        {
            try
            {
                var zones = await GetAllZonesAsync();
=======
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                var zones = await bdd.GetAllZonesAsync();
>>>>>>> f6c204a4c8d3e8c1a1fbc867f54e5faf5e3411b3

                dgZones.ItemsSource = zones;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors du chargement des zones : "
<<<<<<< HEAD
                    + ex.Message);
=======
                    + ex.Message
                );
>>>>>>> f6c204a4c8d3e8c1a1fbc867f54e5faf5e3411b3
            }
        }


<<<<<<< HEAD
        private async Task<List<Zones>> GetAllZonesAsync()
        {
            var zones = new List<Zones>();

            await using var conn =
                await bdd.GetOpenConnectionAsync();

            await using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
        SELECT
            z.Id_Zones,
            z.libelle,
            z.capacité_max,
            COALESCE(SUM(m.`quantité`), 0) AS NombreMangas
        FROM Zones z
        LEFT JOIN Manga m
            ON m.Id_Zones = z.Id_Zones
        GROUP BY z.Id_Zones, z.libelle, z.capacité_max
        ORDER BY z.Id_Zones;
    ";

            await using var reader =
                await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                int idZone = Convert.ToInt32(
                    reader["Id_Zones"]);

                string libelle = Convert.ToString(
                    reader["libelle"]) ?? "";

                int capaciteMax = Convert.ToInt32(
                    reader["capacité_max"]);

                int nombreMangas = Convert.ToInt32(
                    reader["NombreMangas"]);

                var zone = new Zones(
                    idZone,
                    libelle,
                    capaciteMax);

                zone.NombreMangas = nombreMangas;

                zones.Add(zone);
            }

            return zones;
        }


=======
>>>>>>> f6c204a4c8d3e8c1a1fbc867f54e5faf5e3411b3
        // ============================================================
        // STOCK D'UNE ZONE PRÉCISE
        // ============================================================

        private async void VerifierZone_Click(
<<<<<<< HEAD
            object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(
                    txtIdZone.Text,
                    out int idZone))
                {
                    MessageBox.Show(
                        "Veuillez saisir un ID de zone valide.");
                    return;
                }

                string stock =
                    await GetStockZoneAsync(idZone);
=======
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtIdZone.Text, out int idZone))
                {
                    MessageBox.Show(
                        "Veuillez saisir un ID de zone valide."
                    );

                    return;
                }


                string stock =
                    await bdd.GetStockZoneAsync(idZone);

>>>>>>> f6c204a4c8d3e8c1a1fbc867f54e5faf5e3411b3

                lblStockZone.Text =
                    "État du stock : " + stock;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors de la vérification du stock : "
<<<<<<< HEAD
                    + ex.Message);
            }
        }

        private async Task<string> GetStockZoneAsync(int idZone)
        {
            await using var conn =
                await bdd.GetOpenConnectionAsync();

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

            await using var reader =
                await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return "Zone introuvable";

            int capaciteMax = Convert.ToInt32(
                reader["capacité_max"]);

            int stockActuel = Convert.ToInt32(
                reader["stock_actuel"]);

            if (stockActuel == 0)
                return "Zone vide";

            if (stockActuel <= capaciteMax * 0.25)
                return "Stock faible";

            return "Stock suffisant";
        }

        // ============================================================
        // MÉTHODES UTILITAIRES
        // ============================================================

        private static string? TryGetString(
            MySqlDataReader r,
            IDictionary<string, int> map,
            string key)
        {
            if (map.TryGetValue(key, out var idx)
                && !r.IsDBNull(idx))
            {
                return r.GetString(idx);
            }

            return null;
        }

        private static int? TryGetInt(
            MySqlDataReader r,
            IDictionary<string, int> map,
            string key)
        {
            if (map.TryGetValue(key, out var idx)
                && !r.IsDBNull(idx))
            {
                var v = r.GetValue(idx);

                if (v is int i)
                    return i;

                if (v is long l)
                    return Convert.ToInt32(l);

                if (v is short s)
                    return Convert.ToInt32(s);

                if (v is decimal dec)
                    return Convert.ToInt32(dec);

                if (v is double d)
                    return Convert.ToInt32(d);

                var str = Convert.ToString(v)?.Trim();

                if (string.IsNullOrEmpty(str))
                    return null;

                if (int.TryParse(
                    str,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out var parsedInt))
                {
                    return parsedInt;
                }

                if (double.TryParse(
                    str,
                    NumberStyles.Float |
                    NumberStyles.AllowThousands,
                    CultureInfo.InvariantCulture,
                    out var parsedDouble))
                {
                    return Convert.ToInt32(parsedDouble);
                }
            }

            return null;
        }

        private static double? TryGetDouble(
            MySqlDataReader r,
            IDictionary<string, int> map,
            string key)
        {
            if (map.TryGetValue(key, out var idx)
                && !r.IsDBNull(idx))
            {
                var v = r.GetValue(idx);

                if (v is double dd)
                    return dd;

                if (v is float f)
                    return Convert.ToDouble(f);

                if (v is decimal dec)
                    return Convert.ToDouble(dec);

                if (v is int ii)
                    return Convert.ToDouble(ii);

                var str = Convert.ToString(v)?.Trim();

                if (string.IsNullOrEmpty(str))
                    return null;

                if (double.TryParse(
                    str,
                    NumberStyles.Float |
                    NumberStyles.AllowThousands,
                    CultureInfo.InvariantCulture,
                    out var parsed))
                {
                    return parsed;
                }

                if (double.TryParse(
                    str,
                    NumberStyles.Float |
                    NumberStyles.AllowThousands,
                    CultureInfo.CurrentCulture,
                    out parsed))
                {
                    return parsed;
                }
            }

            return null;
        }

        private static DateTime? TryGetDateTime(
            MySqlDataReader r,
            IDictionary<string, int> map,
            string key)
        {
            if (map.TryGetValue(key, out var idx)
                && !r.IsDBNull(idx))
            {
                var v = r.GetValue(idx);

                if (v is DateTime dt)
                    return dt;

                if (DateTime.TryParse(
                    Convert.ToString(v),
                    out var parsed))
                {
                    return parsed;
                }
            }

            return null;
        }
=======
                    + ex.Message
                );
            }
        }
>>>>>>> f6c204a4c8d3e8c1a1fbc867f54e5faf5e3411b3
    }
}
