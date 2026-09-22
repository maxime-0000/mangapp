using System;
using System.Collections.Generic;
using System.Text;

namespace mangapp
{
    public class Manager : Utilisateurs
    {
        public Manager(int id_utilisateur, string nom, string mdp, string email) : base(id_utilisateur, nom, mdp, email)
        {
        }
    }
}
