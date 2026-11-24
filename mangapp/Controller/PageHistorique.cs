using mangapp;
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
using System.Windows.Shapes;
using System.Xml.Linq;

namespace mangapp
{
    public partial class PageHistorique : Page
{
    public PageHistorique()
    {
        InitializeComponent();
        Loaded += async (s, e) => await ChargerAsync();
    }

    private async void Actualiser_Click(object sender, RoutedEventArgs e) => await ChargerAsync();

    private async System.Threading.Tasks.Task ChargerAsync()
    {
        try { dgHistorique.ItemsSource = await bdd.GetHistoriqueAsync(); }
        catch (Exception ex) { MessageBox.Show("Erreur : " + ex.Message); }
    }
}
}
