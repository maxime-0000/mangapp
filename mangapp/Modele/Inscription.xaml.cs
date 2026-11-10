using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace mangapp
{   
    public static class PasswordHasher
    {
        private const int SaltSize = 16;        
        private const int KeySize = 32;         
        private const int Iterations = 600_000; 
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

        public static string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

            // Format stocké : iterations.sel.hash (en base64)
            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string password, string storedHash)
        {
            var parts = storedHash.Split('.');
            if (parts.Length != 3) return false;

            int iterations = int.Parse(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] expectedHash = Convert.FromBase64String(parts[2]);

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algorithm, expectedHash.Length);

            // Comparaison en temps constant (évite les attaques par timing)
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
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

        private bool MailValide(string mail)
        {
            return Regex.IsMatch(mail, @"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$");
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

            if (!MailValide(mail))
            {
                MessageBox.Show("Veuillez saisir une adresse mail valide (exemple : nom@domaine.fr).");
                return;
            }

            if (mdp != confirmation)
            {
                MessageBox.Show("Les mots de passe ne correspondent pas.");
                return;
            }

            try
            {
                string mdpHashe = await Task.Run(() => PasswordHasher.Hash(mdp));
                Utilisateurs utilisateur = new Utilisateurs(nom, mdpHashe, mail);

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
