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

		private void BtnAccueil(object sender, RoutedEventArgs e)
		{
            // Retour à la page précédente si possible
            if (this.NavigationService != null && this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
            else
            {
                // Si aucune page précédente, fermer la fenêtre principale ou naviguer vers une page par défaut
                // Exemple : naviguer vers une nouvelle instance de MainWindow n'est pas adapté ici (MainWindow est une Window)
                // On laisse l'action vide pour éviter l'erreur de compilation
            }
		}


	}
}
