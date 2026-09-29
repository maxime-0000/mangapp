using System;
using System.Collections.Generic;
using System.Text;

namespace mangapp
{
    public class Client : Utilisateurs
    {
        public Client (int id_utilisateur,string nom, string prenom, string email, string mdp, int type_utilisateur) : base(id_utilisateur,nom, email, mdp, type_utilisateur)
        {

        }
    }
}
