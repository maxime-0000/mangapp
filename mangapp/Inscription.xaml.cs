using System;
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
using MySqlConnector;

namespace mangapp
{
    /// <summary>
    /// Logique d'interaction pour Inscription.xaml
    /// </summary>
    public partial class Inscription : Page
    {
        public Inscription()
        {
            InitializeComponent();
        }
        private void Nom_Utilisateur(object sender, TextChangedEventArgs e)
        {

        }
        private void MDP_Utilisateur(object sender, RoutedEventArgs e)
        {

        }
        private void Confirmation_Utilisateur(object sender, RoutedEventArgs e)
        {

        }
        private void Mail_Utilisateur(object sender, TextChangedEventArgs e)
        {

        }

        private async Task AjouterUtilisateur(Utilisateurs utilisateur)
        {
            await using var conn = await bdd.GetOpenConnectionAsync();
            await using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
        INSERT INTO Utilisateurs (nom, mdp, mail, Id_Roles)
        SELECT @nom, @mdp, @mail, Id_Roles
        FROM Roles
        WHERE type = 'client';
    ";

            cmd.Parameters.AddWithValue("@nom", utilisateur.nom);
            cmd.Parameters.AddWithValue("@mdp", utilisateur.mdp);
            cmd.Parameters.AddWithValue("@mail", utilisateur.email);

            await cmd.ExecuteNonQueryAsync();
        }

        private async void Button_Creer(object sender, RoutedEventArgs e)
        {
            string nom = Nom.Text.Trim();
            string mail = Mail.Text.Trim();
            string mdp = Mot_de_Passe.Password;
            string confirmation = Confirmation.Password;

            if (string.IsNullOrWhiteSpace(nom) ||
                string.IsNullOrWhiteSpace(mail) ||
                string.IsNullOrWhiteSpace(mdp) ||
                string.IsNullOrWhiteSpace(confirmation))
            {
                MessageBox.Show("Veuillez remplir tous les champs.");
                return;
            }

            if (mdp != confirmation)
            {
                MessageBox.Show("Les mots de passe ne correspondent pas.");
                return;
            }

            try
            {
                Utilisateurs utilisateur = new Utilisateurs(nom, mdp, mail);

                await AjouterUtilisateur(utilisateur);

                MessageBox.Show("Inscription réussie !");

                Nom.Clear();
                Mail.Clear();
                Mot_de_Passe.Clear();
                Confirmation.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'inscription : " + ex.Message);
            }
        }
    }
}
