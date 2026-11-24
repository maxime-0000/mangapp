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
        // RECHERCHE AVEC FILTRE
        // ==============================================

        private async void BoutonRechercher_Click(
            object sender, RoutedEventArgs e)
        {
            string texteSaisi = TxtRechercher.Text.Trim();

            if (string.IsNullOrWhiteSpace(texteSaisi))
            {
                MessageBox.Show(
                    "Veuillez saisir une recherche.");

                return;
            }

            string query;

            object recherche = "%" + texteSaisi + "%";

            switch (CmbFiltre.SelectedIndex)
            {
                // RECHERCHE PAR NOM

                case 0:

                    query = @"
                        SELECT Id_Manga, nom, année,
                               prix, tome, quantité
                        FROM Manga
                        WHERE nom LIKE @recherche";

                    break;

                // RECHERCHE PAR AUTEUR

                case 1:

                    query = @"
                        SELECT m.Id_Manga, m.nom, m.année,
                               m.prix, m.tome, m.quantité
                        FROM Manga m
                        INNER JOIN Auteur a
                            ON m.Id_Auteur = a.Id_Auteur
                        WHERE a.nom LIKE @recherche";

                    break;

                // RECHERCHE PAR TOME

                case 2:

                    if (!int.TryParse(texteSaisi, out int tome)
                        || tome <= 0)
                    {
                        MessageBox.Show(
                            "Veuillez saisir un tome valide.");

                        return;
                    }

                    recherche = tome;

                    query = @"
                        SELECT Id_Manga, nom, année,
                               prix, tome, quantité
                        FROM Manga
                        WHERE tome = @recherche";

                    break;

                // RECHERCHE PAR GENRE

                case 3:

                    query = @"
                        SELECT m.Id_Manga, m.nom, m.année,
                        m.prix, m.tome, m.quantité,
                        m.Id_Genre, g.`type`
                        FROM Manga m
                        INNER JOIN Genre g
                        ON m.Id_Genre = g.Id_Genre
                    WHERE g.type LIKE @recherche";

                    break;

                // RECHERCHE PAR EDITEUR

                case 4:

                    query = @"
                         SELECT m.Id_Manga, m.nom, m.année,
                        m.prix, m.tome, m.quantité,
                        m.Id_Genre, g.libelle
                        FROM Manga m
                        INNER JOIN Editeur g
                        ON m.Id_Genre = g.Id_Editeur
                    WHERE g.libelle LIKE @recherche";

                    break;

                // RECHERCHE PAR ANNEE

                case 5:

                    if (!int.TryParse(texteSaisi, out int annee)
                        || annee < 1 || annee > 9999)
                    {
                        MessageBox.Show(
                            "Veuillez saisir une année valide.");

                        return;
                    }

                    recherche = annee;

                    query = @"
                        SELECT Id_Manga, nom, année,
                               prix, tome, quantité
                        FROM Manga
                        WHERE année = @recherche";

                    break;

                // AUCUN FILTRE SELECTIONNE

                default:

                    MessageBox.Show(
                        "Veuillez sélectionner un filtre.");

                    return;
            }

            await ChargerMangas(query, recherche);
        }

        // ==============================================
        // CHARGER LES MANGAS DEPUIS MYSQL
        // ==============================================

        private async Task ChargerMangas(
            string query,
            object? recherche = null)
        {
            List<Manga> list = new List<Manga>();

            try
            {
                await using (MySqlConnection connection =
                    await bdd.GetOpenConnectionAsync())
                {
                    using (MySqlCommand command =
                        connection.CreateCommand())
                    {
                        command.CommandText = query;

                        // Paramètre de recherche

                        if (recherche != null)
                        {
                            command.Parameters.AddWithValue(
                                "@recherche", recherche);
                        }

                        // Exécution de la requête

                        using (var reader =
                            await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
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
                // AFFICHAGE DES RESULTATS
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
        // CONVERSION SECURISEE EN INT
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
        // CONVERSION SECURISEE EN DOUBLE
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
        // CONVERSION SECURISEE EN STRING
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