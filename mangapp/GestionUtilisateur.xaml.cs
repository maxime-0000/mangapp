
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
                        // ==============================================
                        // PARAMÈTRES DE RECHERCHE
                        // ==============================================

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

                        // ==============================================
                        // RÉCUPÉRATION DES UTILISATEURS
                        // ==============================================

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

                LstMangas.ItemsSource = null;
                LstMangas.ItemsSource = list;

                // ==============================================
                // VÉRIFICATION DES RÉSULTATS
                // ==============================================

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


        private async Task ModifierRole(int idUtilisateur, int nouvelIdRole)
        {
            using (MySqlConnection connection =
                new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = @"UPDATE Utilisateurs
                         SET Id_Roles = @idRole
                         WHERE Id_Utilisateurs = @idUtilisateur";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idRole", nouvelIdRole);
                    command.Parameters.AddWithValue("@idUtilisateur", idUtilisateur);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private async void ModifierRole_Click(object sender, RoutedEventArgs e)
        {
            if (LstMangas.SelectedItem is Utilisateurs utilisateur)
            {
                string saisie = Microsoft.VisualBasic.Interaction.InputBox(
                    "Entrez le nouvel ID du rôle :",
                    "Modifier le rôle",
                    utilisateur.Id_Roles.ToString()
                );

                if (int.TryParse(saisie, out int nouvelIdRole))
                {
                    await ModifierRole(
                        utilisateur.Id_Utilisateur,
                        nouvelIdRole
                    );

                    // Actualiser la liste
                    await ChargerUtilisateurs(
                    @"SELECT u.Id_Utilisateurs, u.nom, u.mail,
                      r.Id_Roles, r.type AS libelleRole
                      FROM Utilisateurs u
                      INNER JOIN Roles r ON u.Id_Roles = r.Id_Roles");
                }
                else if (!string.IsNullOrWhiteSpace(saisie))
                {
                    MessageBox.Show("Veuillez saisir un ID de rôle valide.");
                }
            }
            else
            {
                MessageBox.Show("Sélectionnez un utilisateur.");
            }
        }

    }
}
