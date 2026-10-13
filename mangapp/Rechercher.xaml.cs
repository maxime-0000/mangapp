
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MySqlConnector;

namespace mangapp
{
    public partial class Rechercher : Page
    {
        private readonly string connectionString =
            "Server=172.16.119.2;Port=3306;Database=mangapp;User=etudiant;Password=etudiant;Connection Timeout=10;";

        public Rechercher()
        {
            InitializeComponent();
        }

        // ==============================================
        // AFFICHER TOUS LES MANGAS
        // ==============================================

        private async void BtnAfficherTous_Click(
            object sender, RoutedEventArgs e)
        {
            string query = @"
                SELECT Id_Manga, nom, année,
                       prix, tome, quantité
                FROM Manga";

            await ChargerMangas(query);
        }

        // ==============================================
        // RECHERCHER UN MANGA PAR NOM
        // ==============================================

        private async void BoutonRechercher_Click(
            object sender, RoutedEventArgs e)
        {
            string texteSaisi = TxtRechercher.Text.Trim();

            string query = @"
                SELECT Id_Manga, nom, année,
                       prix, tome, quantité
                FROM Manga
                WHERE nom LIKE @recherche";

            await ChargerMangas(
                query,
                "%" + texteSaisi + "%"
            );
        }

        // ==============================================
        // RECHERCHER UN MANGA PAR AUTEUR
        // ==============================================

        private async void BoutonRechercherAuteur_Click(
            object sender, RoutedEventArgs e)
        {
            string texteSaisi = TxtAuteur.Text.Trim();

            string query = @"
                SELECT m.Id_Manga, m.nom, m.année,
                       m.prix, m.tome, m.quantité
                FROM Manga m
                INNER JOIN Auteur a
                    ON m.Id_Auteur = a.Id_Auteur
                WHERE a.nom LIKE @recherche";

            await ChargerMangas(
                query,
                "%" + texteSaisi + "%"
            );
        }

        // ==============================================
        // CHARGER LES MANGAS DEPUIS MYSQL
        // ==============================================

        private async Task ChargerMangas(
            string query,
            string? recherche = null)
        {
            List<Manga> list = new List<Manga>();

            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(connectionString))
                {
                    using (MySqlCommand command =
                        new MySqlCommand(query, connection))
                    {
                        // Paramètre de recherche
                        if (recherche != null)
                        {
                            command.Parameters.AddWithValue(
                                "@recherche", recherche);
                        }

                        // Ouverture de la connexion
                        await connection.OpenAsync();

                        // Exécution de la requête
                        using (var reader =
                            await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                // Informations du manga
                                int id = SafeGetInt(
                                    reader, "Id_Manga");

                                string nom = SafeGetString(
                                    reader, "nom");

                                int annee = SafeGetInt(
                                    reader, "année");

                                if (annee < 1 || annee > 9999)
                                {
                                    annee = DateTime.Now.Year;
                                }

                                DateTime dateSortie =
                                    new DateTime(annee, 1, 1);

                                double prix = SafeGetDouble(
                                    reader, "prix");

                                int quantite = SafeGetInt(
                                    reader, "quantité");

                                int tome = SafeGetInt(
                                    reader, "tome");

                                // Création de l'objet Manga
                                Manga manga = new Manga(
                                    nom,
                                    id,
                                    dateSortie,
                                    prix,
                                    quantite,
                                    tome
                                );

                                list.Add(manga);
                            }
                        }
                    }
                }

                // ==============================================
                // AFFICHAGE DES RÉSULTATS
                // ==============================================

                LstMangas.ItemsSource = null;
                LstMangas.ItemsSource = list;

                if (list.Count == 0)
                {
                    MessageBox.Show(
                        "Aucun manga trouvé.",
                        "Information",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    "Erreur MySQL : " + ex.Message,
                    "Erreur de connexion ou de requête",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors de la recherche : " + ex.Message,
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ==============================================
        // CONVERSION SÉCURISÉE EN INT
        // ==============================================

        private int SafeGetInt(
            MySqlDataReader reader, string colonne)
        {
            int index = reader.GetOrdinal(colonne);

            if (reader.IsDBNull(index))
                return 0;

            object valeur = reader.GetValue(index);

            if (int.TryParse(
                Convert.ToString(valeur),
                out int resultat))
            {
                return resultat;
            }

            return 0;
        }

        // ==============================================
        // CONVERSION SÉCURISÉE EN DOUBLE
        // ==============================================

        private double SafeGetDouble(
            MySqlDataReader reader, string colonne)
        {
            int index = reader.GetOrdinal(colonne);

            if (reader.IsDBNull(index))
                return 0;

            object valeur = reader.GetValue(index);

            if (valeur is IConvertible &&
                !(valeur is string))
            {
                return Convert.ToDouble(valeur);
            }

            string texte = Convert.ToString(valeur) ?? "";

            if (double.TryParse(
                texte,
                NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture,
                out double resultat))
            {
                return resultat;
            }

            if (double.TryParse(
                texte,
                NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.CurrentCulture,
                out resultat))
            {
                return resultat;
            }

            return 0;
        }

        // ==============================================
        // CONVERSION SÉCURISÉE EN STRING
        // ==============================================

        private string SafeGetString(
            MySqlDataReader reader, string colonne)
        {
            int index = reader.GetOrdinal(colonne);

            if (reader.IsDBNull(index))
                return "";

            return Convert.ToString(
                reader.GetValue(index)) ?? "";
        }
    }
}
