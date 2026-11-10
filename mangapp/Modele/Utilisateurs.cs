using System;
using System.Data;

namespace mangapp
{
    public class Utilisateurs
    {
        public int Id_Utilisateur { get; set; }
        public int Id_Roles { get; set; }
        public string nom { get; set; }
        public string mdp { get; set; }
        public string email { get; set; }
        public int type_utilisateur { get; set; }
        public Roles Role { get; set; }

        // Constructeur complet
        public Utilisateurs(int Id_Utilisateur, int Id_Roles,
                            string nom, string mdp,
                            string email, int type_utilisateur, Roles role)
        {
            this.Id_Utilisateur = Id_Utilisateur;
            this.Id_Roles = Id_Roles;
            this.nom = nom;
            this.mdp = mdp;
            this.email = email;
            this.type_utilisateur = type_utilisateur;
            this.Role = role;
            this.Id_Roles = role.id_role;
        }

        // Constructeur utilisé lors de l'inscription (nom, mdp, mail)
        public Utilisateurs(string nom, string mdp, string email)
        {
            this.nom = nom;
            this.mdp = mdp;
            this.email = email;
            this.type_utilisateur = 0;
            this.Id_Roles = 0;
            this.Role = null;
        }

        // Constructeur utilisé pour afficher un utilisateur avec son rôle (id, nom, mail, Roles)
        public Utilisateurs(int id, string nom, string email, Roles role)
        {
            this.Id_Utilisateur = id;
            this.nom = nom;
            this.email = email;
            this.Role = role;
            this.Id_Roles = role?.id_role ?? 0;
            this.type_utilisateur = this.Id_Roles;
        }

        // Constructeur compatible avec les classes dérivées qui appellent base(...) sans Roles
        public Utilisateurs(int Id_Utilisateur, int Id_Roles,
                            string nom, string mdp,
                            string email, int type_utilisateur)
        {
            this.Id_Utilisateur = Id_Utilisateur;
            this.Id_Roles = Id_Roles;
            this.nom = nom;
            this.mdp = mdp;
            this.email = email;
            this.type_utilisateur = type_utilisateur;
            this.Role = null;
        }

        // Constructeur pour récupérer un utilisateur
        public Utilisateurs(string nom, int id, string email, int idRole)
        {
            this.nom = nom;
            this.Id_Utilisateur = id;
            this.email = email;
            this.Id_Roles = idRole;
            this.type_utilisateur = idRole;
        }

        public override string ToString()
        {
            return $"Utilisateur : {nom} - Rôle : {Role?.libelle}";
        }


    }
}