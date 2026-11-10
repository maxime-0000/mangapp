using System;
using System.Collections.Generic;
using System.Text;

namespace mangapp
{
    public class Manager : Utilisateurs
    {
        public Manager(int Id_utilisateur, string nom, string mdp, string email, int type_utilisateur, int Id_Roles) : base(Id_utilisateur, Id_Roles, nom, mdp, email, type_utilisateur)
        {
            this.Id_Utilisateur = Id_utilisateur;
            this.Id_Roles = Id_Roles;
            this.nom = nom;
            this.mdp = mdp;
            this.email = email;
            this.type_utilisateur = type_utilisateur;
        }
    }
}

