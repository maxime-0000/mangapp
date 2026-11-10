namespace mangapp
{
    public class Roles
    {
        public int id_role { get; set; }
        public string libelle { get; set; }

        public Roles(int IdRoles, string libelle)
        {
            this.id_role = IdRoles;
            this.libelle = libelle;
        }
    }
}