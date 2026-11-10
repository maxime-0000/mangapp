
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace mangapp
{
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
                SELECT Utilisateurs.Id_Utilisateurs, Utilisateurs.Id_Roles,
               Utilisateurs.nom, Utilisateurs.mail, Roles.type
				FROM Utilisateurs
				INNER JOIN Roles ON Utilisateurs.Id_Roles = Roles.Id_Roles
				WHERE Utilisateurs.mail = @mail AND Utilisateurs.mdp = @mdp;"";
            ";

			cmd.Parameters.AddWithValue("@mail", mail);
			cmd.Parameters.AddWithValue("@mdp", mdp);

			await using var reader = await cmd.ExecuteReaderAsync();

			if (await reader.ReadAsync())
			{
				UtilisateurConnecte.IdUtilisateur =
					Convert.ToInt32(reader["Id_Utilisateurs"]);

				UtilisateurConnecte.IdRole =
					Convert.ToInt32(reader["Id_Roles"]);

				UtilisateurConnecte.type =
					reader["type"].ToString() ?? "";

				UtilisateurConnecte.Mail = mail;

                UtilisateurConnecte.Nom = reader["nom"].ToString() ?? "";  

                return true;
			}

			return false;
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
					MainWindow? fenetre =
						Application.Current.MainWindow as MainWindow;

					fenetre?.ActualiserUtilisateur();

					MessageBox.Show("Connexion réussie !");
				}
				else
				{
					MessageBox.Show(
						"Utilisateur ou mot de passe incorrect."
					);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					"Erreur de connexion à la base : " + ex.Message
				);
			}
		}
	}
}
