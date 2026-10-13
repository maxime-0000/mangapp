
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace mangapp
{
    public partial class GestionUtilisateur : Page
    {
        private readonly string connectionString =
            "Server=172.16.119.2;Database=mangapp;User=etudiant;Password=etudiant;";

        public GestionUtilisateur()
        {
            InitializeComponent();
        }

        // ==============================================
        // AFFICHER TOUS LES UTILISATEURS
        // ==============================================

        private async void BtnAfficherTous_Click(object sender, RoutedEventArgs e)
        {
            await ChargerUtilisateurs(
                @"SELECT u.Id_Utilisateurs, u.nom, u.mail,
                         r.Id_Roles, r.type AS libelleRole
                  FROM Utilisateurs u
                  INNER JOIN Roles r
                  ON u.Id_Roles = r.Id_Roles");
        }

        // ==============================================
        // RECHERCHER UN UTILISATEUR PAR NOM
        // ==============================================

        private async void BoutonRechercher_Click(object sender, RoutedEventArgs e)
        {
            string texteSaisi = TxtRechercher.Text.Trim();

            await ChargerUtilisateurs(
                @"SELECT u.Id_Utilisateurs, u.nom, u.mail,
                         r.Id_Roles, r.type AS libelleRole
                  FROM Utilisateurs u
                  INNER JOIN Roles r
                  ON u.Id_Roles = r.Id_Roles
                  WHERE u.nom LIKE @recherche",
                "%" + texteSaisi + "%");
        }

        // ==============================================
        // RECHERCHER PAR LIBELLE DU ROLE
        // ==============================================

        private async void BoutonRechercherAuteur_Click(object sender, RoutedEventArgs e)
        {
            string texteSaisi = TxtAuteur.Text.Trim();

            await ChargerUtilisateurs(
                @"SELECT u.Id_Utilisateurs, u.nom, u.mail,
                         r.Id_Roles, r.type AS libelleRole
                  FROM Utilisateurs u
                  INNER JOIN Roles r
                  ON u.Id_Roles = r.Id_Roles
                  WHERE r.type LIKE @role",
                "%" + texteSaisi + "%");
        }

        // ==============================================
        // CHARGER LES UTILISATEURS DEPUIS LA BDD
        // ==============================================

        private async Task ChargerUtilisateurs(
            string query,
            string? recherche = null)
        {
            List<Utilisateurs> list = new List<Utilisateurs>();

            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(connectionString))
                {
                    using (MySqlCommand command =
                        new MySqlCommand(query, connection))
                    {
                        // Paramètres de recherche
                        if (recherche != null)
                        {
                            if (query.Contains("@recherche"))
                            {
                                command.Parameters.AddWithValue(
                                    "@recherche", recherche);
                            }

                            if (query.Contains("@role"))
                            {
                                command.Parameters.AddWithValue(
                                    "@role", recherche);
                            }
                        }

                        await connection.OpenAsync();

                        using (var reader =
                            await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                // Informations utilisateur
                                int id = Convert.ToInt32(
                                    reader["Id_Utilisateurs"]);

                                string nom = Convert.ToString(
                                    reader["nom"]) ?? "";

                                string mail = Convert.ToString(
                                    reader["mail"]) ?? "";

                                // Informations du rôle
                                int idRole = Convert.ToInt32(
                                    reader["Id_Roles"]);

                                string libelle = Convert.ToString(
                                    reader["libelleRole"]) ?? "";

                                // Création de l'objet Roles
                                Roles role = new Roles(
                                    idRole,
                                    libelle
                                );

                                // Création de l'objet Utilisateurs
                                Utilisateurs utilisateur =
                                    new Utilisateurs(
                                        id,
                                        nom,
                                        mail,
                                        role
                                    );

                                // Ajout dans la liste
                                list.Add(utilisateur);
                            }
                        }
                    }
                }

                // ==============================================
                // AFFICHAGE DANS LA LISTBOX
                // ==============================================

                LstMangas.Items.Clear();

                foreach (Utilisateurs utilisateur in list)
                {
                    LstMangas.Items.Add(
                        utilisateur.ToString()
                    );
                }

                if (list.Count == 0)
                {
                    MessageBox.Show(
                        "Aucun utilisateur trouvé.",
                        "Information",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors de la récupération des utilisateurs : "
                    + ex.Message,
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
