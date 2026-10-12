
namespace mangapp
{
	public static class UtilisateurConnecte
	{
		public static int IdUtilisateur { get; set; }
		public static int IdRole { get; set; }
		public static string Mail { get; set; } = "";
		public static string type { get; set; } = "";

		public static bool EstManager()
		{
			return IdRole == 2;
		}

		public static void Deconnecter()
		{
			IdUtilisateur = 0;
			IdRole = 0;
			Mail = "";
			type = "";
		}
	}
}
