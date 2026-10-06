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




	}
}
