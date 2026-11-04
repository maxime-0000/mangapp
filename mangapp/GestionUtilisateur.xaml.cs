
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
        private const int ID_ROLE_CLIENT = 1;   // vérifie l'Id réel dans ta table Roles
        private const int ID_ROLE_MANAGER = 2;  // cohérent avec ton EstManager()

        // Affiche la fenêtre de choix, retourne l'Id du rôle choisi (ou null si annulé)
        private int? DemanderNouveauRole(int idRoleActuel)
        {
            var combo = new ComboBox
            {
                ItemsSource = new[]
                {
            new { Id = ID_ROLE_CLIENT,  Libelle = "Client"  },
            new { Id = ID_ROLE_MANAGER, Libelle = "Manager" }
        },
                DisplayMemberPath = "Libelle",
                SelectedValuePath = "Id",
                SelectedValue = idRoleActuel,
                Margin = new Thickness(0, 5, 0, 15)
            };

            var btnOk = new Button
            {
                Content = "Valider",
                Width = 80,
                Margin = new Thickness(0, 0, 8, 0),
                IsDefault = true
            };

            var btnAnnuler = new Button
            {
                Content = "Annuler",
                Width = 80,
                IsCancel = true
            };

            var boutons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Children = { btnOk, btnAnnuler }
            };

            var contenu = new StackPanel
            {
                Margin = new Thickness(15),
                Children =
        {
            new TextBlock { Text = "Choisissez le nouveau rôle :" },
            combo,
            boutons
        }
            };

            var fenetre = new Window
            {
                Title = "Changer le rôle",
                Content = contenu,
                Width = 280,
                SizeToContent = SizeToContent.Height,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this)
            };

            int? resultat = null;

            btnOk.Click += (s, e) =>
            {
                resultat = (int?)combo.SelectedValue;
                fenetre.DialogResult = true;
            };

            fenetre.ShowDialog();
            return resultat;
        }

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
                await using (MySqlConnection connection =
                    await bdd.GetOpenConnectionAsync())
                {
                    using (MySqlCommand command = connection.CreateCommand())
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
            await using (MySqlConnection connection =
                await bdd.GetOpenConnectionAsync())
            {
                string query = @"UPDATE Utilisateurs
                         SET Id_Roles = @idRole
                         WHERE Id_Utilisateurs = @idUtilisateur";

                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.Parameters.AddWithValue("@idRole", nouvelIdRole);
                    command.Parameters.AddWithValue("@idUtilisateur", idUtilisateur);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // Affiche le bouton uniquement pour le manager
        private void BtnChangerRole_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button bouton)
            {
                bouton.Visibility = UtilisateurConnecte.EstManager()
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private async void ChangerRole_Click(object sender, RoutedEventArgs e)
        {
            // Sécurité : on revérifie même si le bouton est masqué
            if (!UtilisateurConnecte.EstManager())
            {
                MessageBox.Show("Seul un manager peut changer un rôle.");
                return;
            }

            if (sender is not Button { DataContext: Utilisateurs utilisateur })
                return;

            // Un manager ne doit pas se retirer lui-même ses droits
            if (utilisateur.Id_Utilisateur == UtilisateurConnecte.IdUtilisateur)
            {
                MessageBox.Show("Vous ne pouvez pas modifier votre propre rôle.");
                return;
            }

            int? nouvelIdRole = DemanderNouveauRole(utilisateur.Id_Roles);

            // Annulé, ou rôle identique : rien à faire
            if (nouvelIdRole == null || nouvelIdRole == utilisateur.Id_Roles)
                return;

            try
            {
                await ModifierRole(utilisateur.Id_Utilisateur, nouvelIdRole.Value);

                await ChargerUtilisateurs(
                    @"SELECT u.Id_Utilisateurs, u.nom, u.mail,
                     r.Id_Roles, r.type AS libelleRole
              FROM Utilisateurs u
              INNER JOIN Roles r ON u.Id_Roles = r.Id_Roles");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du changement de rôle : " + ex.Message);
            }
        }

        private async Task SupprimerUtilisateur(int idUtilisateur)
        {
            await using (MySqlConnection connection =
                await bdd.GetOpenConnectionAsync())
            {
                string query = @"DELETE FROM Utilisateurs
                         WHERE Id_Utilisateurs = @idUtilisateur";

                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.Parameters.AddWithValue("@idUtilisateur", idUtilisateur);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private async void SupprimerUtilisateur_Click(object sender, RoutedEventArgs e)
        {
            // Seul le manager a le droit de supprimer
            if (!UtilisateurConnecte.EstManager())
            {
                MessageBox.Show(
                    "Seul un manager peut supprimer un compte.",
                    "Accès refusé",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (LstMangas.SelectedItem is not Utilisateurs utilisateur)
            {
                MessageBox.Show("Sélectionnez un utilisateur.");
                return;
            }

            // Empêcher le manager de supprimer son propre compte
            if (utilisateur.Id_Utilisateur == UtilisateurConnecte.IdUtilisateur)
            {
                MessageBox.Show("Vous ne pouvez pas supprimer votre propre compte.");
                return;
            }

            // Confirmation
            MessageBoxResult confirmation = MessageBox.Show(
                $"Voulez-vous vraiment supprimer le compte de {utilisateur.nom} ?\n" +
                "Cette action est irréversible.",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmation != MessageBoxResult.Yes)
                return;

            try
            {
                await SupprimerUtilisateur(utilisateur.Id_Utilisateur);

                MessageBox.Show("Le compte a bien été supprimé.", "Succès",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Actualiser la liste
                await ChargerUtilisateurs(
                    @"SELECT u.Id_Utilisateurs, u.nom, u.mail,
                     r.Id_Roles, r.type AS libelleRole
              FROM Utilisateurs u
              INNER JOIN Roles r ON u.Id_Roles = r.Id_Roles");
            }
            catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.RowIsReferenced2)
            {
                // Clé étrangère : l'utilisateur est lié à d'autres données
                MessageBox.Show(
                    "Impossible de supprimer ce compte : il est encore lié à d'autres données " +
                    "(commandes, avis, favoris...).",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors de la suppression : " + ex.Message,
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

    }
}
