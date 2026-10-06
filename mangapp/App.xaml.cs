using System;
using System.Windows;
using Microsoft.Extensions.Configuration;

namespace mangapp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                // Appel direct à bdd.Configure avec chaîne de connexion en dur (exemple).
                // Remplacez par vos valeurs réelles ou utilisez une variable d'environnement en production.
                bdd.Configure("Server=172.16.119.3;Port=3306;Database=mangapp;User=etudiant;Password=etudiant;");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de configuration : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }

}
