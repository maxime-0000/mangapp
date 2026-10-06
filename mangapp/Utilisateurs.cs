using System;
using System.Collections.Generic;
using System.Text;

namespace mangapp
{
    public class Utilisateurs
    {
        private int Id_Utilisateur { get; set; }
        public string nom { get; set; }
        public string mdp { get; set; }
        public string email { get; set; }
        private int type_utilisateur { get; set; }

        public Utilisateurs(string nom, string mdp, string email)
        {
            this.Id_Utilisateur = Id_Utilisateur;
            this.nom = nom;
            this.mdp = mdp;
            this.email = email;
            this.type_utilisateur = 1;
        }
    }
}
