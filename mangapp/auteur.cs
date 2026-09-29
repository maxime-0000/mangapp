using System;
using System.Collections.Generic;
using System.Text;

namespace mangapp
{
    class auteur
    {
        private string nom { get; set; }
        private string description { get; set; }
        private int id_auteur { get; set; }

        public auteur(string nom, string description, int id_auteur)
        {
            this.nom = nom;
            this.description = description;
            this.id_auteur = id_auteur;
        }
    }

   
}
