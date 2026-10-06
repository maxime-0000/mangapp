using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
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
    /// Logique d'interaction pour Rechercher.xaml
    /// </summary>
    public partial class Rechercher : Page
    {
        public Rechercher()
        {
            InitializeComponent();
        }

        private async void BtnAfficherTous_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var list = await bdd.GetAllMangaAsync();
                LstMangas.Items.Clear();
                if (list.Count == 0)
                {
                    MessageBox.Show("Aucun manga trouvé.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    foreach (var m in list)
                    {
                        LstMangas.Items.Add(m.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la récupération des mangas : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

		private async void BoutonRechercher_Click(object sender, RoutedEventArgs e)
		{
			// 1. Récupération du texte saisi par l'utilisateur
			string texteSaisi = TxtRechercher.Text.Trim();

			// 2. Préparation de la liste (on la vide pour ne pas cumuler avec l'ancienne recherche)
			List<Manga> list = new List<Manga>();

			// 3. Connexion et requête SQL avec paramètre et l'opérateur LIKE
			// Remplacez la chaîne de connexion par vos informations réelles ou centralisez-la dans la configuration.
			string connectionString = "Server=172.16.119.3;Database=mangapp;User=etudiant;Password=etudiant;";

			// Utiliser des noms de colonnes sans accents pour éviter des problèmes SQL fréquents.
			string query = "SELECT Id_Manga, nom, année, prix, tome, quantité FROM Manga WHERE nom LIKE @recherche;";

			using (MySqlConnection connection = new MySqlConnection(connectionString))
			using (MySqlCommand command = new MySqlCommand(query, connection))
			{
				// 4. Injection sécurisée de la valeur en entourant le texte de '%' pour le LIKE
				command.Parameters.AddWithValue("@recherche", "%" + texteSaisi + "%");

				try
				{
					await connection.OpenAsync();
					using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
					{
						// Fonctions locales pour des lectures robustes (acceptent VARCHAR numériques)
						int SafeGetInt(MySqlDataReader r, string name)
						{
							int idx;
							try { idx = r.GetOrdinal(name); } catch { return 0; }
							if (r.IsDBNull(idx)) return 0;
							var v = r.GetValue(idx);
							if (v is int ii) return ii;
							if (v is long ll) return Convert.ToInt32(ll);
							if (v is short ss) return Convert.ToInt32(ss);
							if (v is decimal dec) return Convert.ToInt32(dec);
							if (v is double dd) return Convert.ToInt32(dd);
							var s = Convert.ToString(v)?.Trim();
							if (string.IsNullOrEmpty(s)) return 0;
							if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var pi)) return pi;
							if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var pd)) return Convert.ToInt32(pd);
							if (int.TryParse(s, NumberStyles.Integer, CultureInfo.CurrentCulture, out pi)) return pi;
							if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out pd)) return Convert.ToInt32(pd);
							return 0;
						}

						double SafeGetDouble(MySqlDataReader r, string name)
						{
							int idx;
							try { idx = r.GetOrdinal(name); } catch { return 0.0; }
							if (r.IsDBNull(idx)) return 0.0;
							var v = r.GetValue(idx);
							if (v is double dd) return dd;
							if (v is float ff) return Convert.ToDouble(ff);
							if (v is decimal dec) return Convert.ToDouble(dec);
							if (v is int ii) return Convert.ToDouble(ii);
							var s = Convert.ToString(v)?.Trim();
							if (string.IsNullOrEmpty(s)) return 0.0;
							if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var pd)) return pd;
							if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out pd)) return pd;
							return 0.0;
						}

						string SafeGetString(MySqlDataReader r, string name)
						{
							int idx;
							try { idx = r.GetOrdinal(name); } catch { return string.Empty; }
							if (r.IsDBNull(idx)) return string.Empty;
							return Convert.ToString(r.GetValue(idx)) ?? string.Empty;
						}

						while (await reader.ReadAsync())
						{
							// Lecture avec conversions robustes (gère VARCHAR contenant des nombres)
							string nom = SafeGetString(reader, "nom");
							int id = SafeGetInt(reader, "Id_Manga");
							int anneeInt = SafeGetInt(reader, "année");
							if (anneeInt == 0) anneeInt = DateTime.Now.Year;
							DateTime anneeDateTime = new DateTime(anneeInt, 1, 1);
							double prix = SafeGetDouble(reader, "prix");
							int qte = SafeGetInt(reader, "quantité");
							int tome = SafeGetInt(reader, "tome");

							var m = new Manga(nom, id, anneeDateTime, prix, qte, tome);
							list.Add(m);
						}
					}

					// 5. Mettre à jour l'affichage de la ListBox
					LstMangas.Items.Clear();
					foreach (var item in list)
					{
						LstMangas.Items.Add(item.ToString());
					}

				}
				catch (Exception ex)
				{
					MessageBox.Show($"Erreur lors de la recherche : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private async void BoutonRechercherAuteur_Click(object sender, RoutedEventArgs e)
		{
			// 1. Récupération du texte saisi par l'utilisateur
			var tbAuteur = this.FindName("TxtAuteur") as TextBox;
			string texteSaisi = tbAuteur?.Text.Trim() ?? string.Empty;

			// 2. Préparation de la liste
			List<Manga> list = new List<Manga>();

			string connectionString = "Server=172.16.119.3;Database=mangapp;User=etudiant;Password=etudiant;";

			// 3. REQUÊTE MODIFIÉE : On lie la table Manga et la table Auteur pour chercher sur le NOM de l'auteur
			string query = @"SELECT m.Id_Manga, m.nom, m.année, m.prix, m.tome, m.quantité 
                     FROM Manga m
                     INNER JOIN Auteur a ON m.Id_Auteur = a.Id_Auteur
                     WHERE a.nom LIKE @recherche;";

			using (MySqlConnection connection = new MySqlConnection(connectionString))
			using (MySqlCommand command = new MySqlCommand(query, connection))
			{
				// 4. Injection sécurisée de la valeur
				command.Parameters.AddWithValue("@recherche", "%" + texteSaisi + "%");

				try
				{
					await connection.OpenAsync();
					using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
					{
						// Vos fonctions locales d'origine (conservées à l'identique)
						int SafeGetInt(MySqlDataReader r, string name)
						{
							int idx;
							try { idx = r.GetOrdinal(name); } catch { return 0; }
							if (r.IsDBNull(idx)) return 0;
							var v = r.GetValue(idx);
							if (v is int ii) return ii;
							if (v is long ll) return Convert.ToInt32(ll);
							if (v is short ss) return Convert.ToInt32(ss);
							if (v is decimal dec) return Convert.ToInt32(dec);
							if (v is double dd) return Convert.ToInt32(dd);
							var s = Convert.ToString(v)?.Trim();
							if (string.IsNullOrEmpty(s)) return 0;
							if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var pi)) return pi;
							if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var pd)) return Convert.ToInt32(pd);
							if (int.TryParse(s, NumberStyles.Integer, CultureInfo.CurrentCulture, out pi)) return pi;
							if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out pd)) return Convert.ToInt32(pd);
							return 0;
						}

						double SafeGetDouble(MySqlDataReader r, string name)
						{
							int idx;
							try { idx = r.GetOrdinal(name); } catch { return 0.0; }
							if (r.IsDBNull(idx)) return 0.0;
							var v = r.GetValue(idx);
							if (v is double dd) return dd;
							if (v is float ff) return Convert.ToDouble(ff);
							if (v is decimal dec) return Convert.ToDouble(dec);
							if (v is int ii) return Convert.ToDouble(ii);
							var s = Convert.ToString(v)?.Trim();
							if (string.IsNullOrEmpty(s)) return 0.0;
							if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var pd)) return pd;
							if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out pd)) return pd;
							return 0.0;
						}

						string SafeGetString(MySqlDataReader r, string name)
						{
							int idx;
							try { idx = r.GetOrdinal(name); } catch { return string.Empty; }
							if (r.IsDBNull(idx)) return string.Empty;
							return Convert.ToString(r.GetValue(idx)) ?? string.Empty;
						}

						while (await reader.ReadAsync())
						{
							// Les colonnes sélectionnées ayant les mêmes noms, cette logique reste identique
							string nom = SafeGetString(reader, "nom");
							int id = SafeGetInt(reader, "Id_Manga");
							int anneeInt = SafeGetInt(reader, "année");
							if (anneeInt == 0) anneeInt = DateTime.Now.Year;
							DateTime anneeDateTime = new DateTime(anneeInt, 1, 1);
							double prix = SafeGetDouble(reader, "prix");
							int qte = SafeGetInt(reader, "quantité");
							int tome = SafeGetInt(reader, "tome");

							var m = new Manga(nom, id, anneeDateTime, prix, qte, tome);
							list.Add(m);
						}
					}

					// 5. Mise à jour de l'affichage
					LstMangas.Items.Clear();
					foreach (var item in list)
					{
						LstMangas.Items.Add(item.ToString());
					}

				}
				catch (Exception ex)
				{
					MessageBox.Show($"Erreur lors de la recherche : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

	}
}
