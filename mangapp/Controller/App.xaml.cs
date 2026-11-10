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
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.Local.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();

                var connectionString = Environment.GetEnvironmentVariable("MANGAPP_DB_CONNECTION")
                    ?? configuration.GetConnectionString("mangapp");

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException(
                        "Configurez ConnectionStrings:mangapp dans appsettings.Local.json " +
                        "ou la variable d'environnement MANGAPP_DB_CONNECTION.");

                bdd.Configure(connectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de configuration : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }

}
