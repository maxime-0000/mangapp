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
using static System.Collections.Specialized.BitVector32;

namespace mangapp
{
    /// <summary>
    /// Logique d'interaction pour Connexion.xaml
    /// </summary>
    public partial class Connexion : Page
    {
        public Connexion()
        {
            InitializeComponent();
        }

        private void Mail_Utilisateur(object sender, TextChangedEventArgs e)
        {

        }

        private void MDP_Utilisateur(object sender, RoutedEventArgs e)
        {

        }

        private async Task<bool> ConnecterUtilisateur(string mail, string mdp)
        {
            await using var conn = await bdd.GetOpenConnectionAsync();
            await using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
        SELECT Id_Utilisateurs
        FROM Utilisateurs
        WHERE mail = @mail AND mdp = @mdp;
    ";

            cmd.Parameters.AddWithValue("@mail", mail);
            cmd.Parameters.AddWithValue("@mdp", mdp);

            object? resultat = await cmd.ExecuteScalarAsync();

            return resultat != null;
        }

        private async void Button_Valider(object sender, RoutedEventArgs e)
        {
            string mail = Mail.Text.Trim();
            string mdp = Mot_de_Passe.Password;

            if (string.IsNullOrWhiteSpace(mail) ||
                string.IsNullOrWhiteSpace(mdp))
            {
                MessageBox.Show("Veuillez remplir tous les champs.");
                return;
            }

            try
            {
                bool ok = await ConnecterUtilisateur(mail, mdp);

                if (ok)
                {
                    MessageBox.Show("Connexion réussie !");
                    NavigationService?.Navigate(new Connexion());
                }
                    
                else
                    MessageBox.Show("Utilisateur ou mot de passe incorrect.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur de connexion à la base : " + ex.Message);
            }
        }
    }
}
