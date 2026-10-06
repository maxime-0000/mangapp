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
    public partial class Statistique : Page
    {
        public Statistique()
        {
            InitializeComponent();
        }


        // ============================================================
        // VUE GLOBALE DES MANGAS
        // ============================================================

        private async void AfficherMangas_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                var mangas = await bdd.GetAllMangaAsync();
                
                dgMangas.ItemsSource = mangas;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors du chargement des mangas : "
                    + ex.Message
                );
            }
        }


        // ============================================================
        // STOCK D'UN MANGA PRÉCIS
        // ============================================================

        private async void VerifierManga_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtIdManga.Text, out int idManga))
                {
                    MessageBox.Show(
                        "Veuillez saisir un ID de manga valide."
                    );

                    return;
                }


                string stock =
                    await bdd.GetStockMangaAsync(idManga);


                lblStockManga.Text =
                    "État du stock : " + stock;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors de la vérification du stock : "
                    + ex.Message
                );
            }
        }


        // ============================================================
        // VUE GLOBALE DES ZONES
        // ============================================================

        private async void AfficherZones_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                var zones = await bdd.GetAllZonesAsync();

                dgZones.ItemsSource = zones;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors du chargement des zones : "
                    + ex.Message
                );
            }
        }


        // ============================================================
        // STOCK D'UNE ZONE PRÉCISE
        // ============================================================

        private async void VerifierZone_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtIdZone.Text, out int idZone))
                {
                    MessageBox.Show(
                        "Veuillez saisir un ID de zone valide."
                    );

                    return;
                }


                string stock =
                    await bdd.GetStockZoneAsync(idZone);


                lblStockZone.Text =
                    "État du stock : " + stock;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors de la vérification du stock : "
                    + ex.Message
                );
            }
        }
    }
}
