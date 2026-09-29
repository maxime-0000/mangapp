using System;
using System.Collections.Generic;
using System.Text;

namespace mangapp
{
    public class Zones
    {
        private int id_zone { get; set; }
        private string libele { get; set; }
        private int capacité_max { get; set; }

        public Zones (int id_zone, string libele, int capacité_max)
        {
            this.id_zone = id_zone;
            this.libele = libele;
            this.capacité_max = capacité_max;
        }
                                                
    }
}
