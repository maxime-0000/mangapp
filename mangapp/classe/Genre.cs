using System;
using System.Collections.Generic;
using System.Text;

namespace mangapp
{
    public class Genre
    {
        private int id_genre { get; set; }
        private string libelle { get; set; }

        public Genre (int id_genre, string libelle)
        {
            this.id_genre = id_genre;
            this.libelle = libelle;
        }
    }
}
