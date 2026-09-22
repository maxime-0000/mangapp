using System;
using System.Collections.Generic;
using System.Text;

namespace mangapp
{
    public class Utilisateurs
    {
        private int id_utilisateur { get; set; }
        private string nom { get; set; }
        private string mdp { get; set; }
        private string email { get; set; }

        public Utilisateurs (int id_utilisateur, string nom, string mdp, string email)
        {
            this.id_utilisateur = id_utilisateur;
            this.nom = nom;
            this.mdp = mdp;
            this.email = email;
        }
    }
}
