using System;
using System.Collections.Generic;
using System.Text;

namespace mangapp
{
    public class Roles
    {
        private int id_role { get; set; }
        private string libelle { get; set; }

        public Roles (int Id_Roles, string libelle)
        {
            this.id_role = Id_Roles;
            this.libelle = libelle;
        }
    }
}
