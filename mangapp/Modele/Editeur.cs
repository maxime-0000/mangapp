using System;
using System.Collections.Generic;
using System.Text;

namespace mangapp
{
    public class Editeur
    {
        private int id_Editeur { get; set; }
        private string libelle { get; set;}

        public Editeur (int id_Editeur, string libelle)
        {
            this.id_Editeur = id_Editeur;
            this.libelle = libelle;
        }
    }
}
