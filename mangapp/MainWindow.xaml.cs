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

namespace mangapp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void BtnRechercher(object sender, RoutedEventArgs e)
		{
			MainFrame.Navigate(new Rechercher());
		}

		private void BtnAccueil(object sender, RoutedEventArgs e)
		{
			MainFrame.Content = null;
		}

		private void BtnStats(object sender, RoutedEventArgs e)
		{
			if (!UtilisateurConnecte.EstManager())
			{
				MessageBox.Show("Cette page est réservée aux managers.");
				return;
			}

			MainFrame.Navigate(new Statistique());
		}		

		private void BtnInscription(object sender, RoutedEventArgs e)
		{
			MainFrame.Navigate(new Inscription());
		}

        private void BtnConnexion(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Connexion());
        }

		public void ActualiserUtilisateur()
		{
			Utilisateurtxt.Text = "Utilisateur : " + UtilisateurConnecte.Mail;
		}


	}
}