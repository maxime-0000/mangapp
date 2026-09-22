using System;
using System.Collections.Generic;
using System.Text;

namespace mangapp
{
    public class Roles
    {
        private int id_role { get; set; }
        private string libelle { get; set; }

        public Roles (int id_role, string libelle)
        {
            this.id_role = id_role;
            this.libelle = libelle;
        }
    }
}
