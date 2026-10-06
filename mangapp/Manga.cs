using System;
using System.Collections.Generic;
using System.Text;

namespace mangapp
{
    public class Manga
    {
        public string Nom { get; set; }
        public int Id { get; set; }
        public DateTime Annee { get; set; }
        public double Prix { get; set; }
        public int Quantite { get; set; }
        public int Tome { get; set; }

        public Manga() { }

        public Manga(string nom, int id, DateTime annee, double prix, int quantite, int tome)
        {
            Nom = nom;
            Id = id;
            Annee = annee;
            Prix = prix;
            Quantite = quantite;
            Tome = tome;
        }

        public override string ToString()
        {
            return $"{Nom} (Id={Id}) - {Annee:yyyy} - {Prix}€ - Qte:{Quantite} - Tome:{Tome}";
        }
    }
}
