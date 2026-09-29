using System;
using System.Collections.Generic;
using System.Text;

namespace mangapp
{
    class Manga
    {
        private string nom {  get; set; }
        private int id_manga { get; set; }
        private DateTime année { get; set; }
        private double prix { get; set; }
        private int quantité { get; set; }
        private int tome {  get; set; }


        public Manga(string nom, int id_manga, DateTime année, double prix, int quantité, int tome)

        {
            this.nom = nom;
            this.id_manga = id_manga;
            this.année = année;
            this.prix = prix;
            this.quantité = quantité;
            this.tome = tome;
        }

    }
}
