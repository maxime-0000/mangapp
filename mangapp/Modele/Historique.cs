using System;
using System.Collections.Generic;
using System.Text;

namespace mangapp
{
    public class Historique
    {
        public int Id_Historique { get; set; }
        public DateTime Date { get; set; }
        public int Quantite { get; set; }
        public string Type_mouvement { get; set; } = "";
        public int Id_Utilisateur { get; set; }
        public string NomManager { get; set; } = "";
        public int Id_Manga { get; set; }
        public string NomManga { get; set; } = "";
    }
}
