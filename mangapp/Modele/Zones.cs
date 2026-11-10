
using System;

namespace mangapp
{
    public class Zones
    {
        public int id_zone { get; private set; }
        public string libele { get; private set; }
        public int capacité_max { get; private set; }

        // Nombre total d'exemplaires dans la zone
        public int NombreMangas { get; set; }

        public Zones(int id_zone, string libele, int capacité_max)
        {
            this.id_zone = id_zone;
            this.libele = libele;
            this.capacité_max = capacité_max;

            NombreMangas = 0;
        }
    }
}

