using System;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace mangapp
{
    // Obfuscation simple destinée à éviter une chaîne de connexion lisible directement dans l'assembly.
    // Elle ne protège pas le secret contre une personne qui analyse l'application.
    internal static class ConnexionDB
    {
        private static readonly byte[] Key = new byte[] { 0xA6, 0xC8, 0x49, 0x9E, 0xE3, 0x0D, 0x2C, 0x1B, 0xF4, 0x54, 0x81, 0x29, 0x90, 0xA1, 0x15, 0x00 };
        private static readonly byte[] Payload = new byte[] { 0xF5, 0xAC, 0x39, 0xEB, 0x82, 0x7A, 0x17, 0x2D, 0xCB, 0x6F, 0xA5, 0x13, 0xAA, 0x82, 0x2A, 0x3E, 0x8F, 0xF7, 0x69, 0xB6, 0xA7, 0x77, 0x48, 0x78, 0xD1, 0x7E, 0xA8, 0x02, 0xBA, 0x87, 0x4F, 0x7E, 0xF2, 0x88, 0x09, 0xDC, 0xB4, 0x4D, 0x37, 0x51, 0xBD, 0x13, 0xCC, 0x63, 0xCC, 0xFC, 0x00, 0x7A, 0xE5, 0x9C, 0x09, 0x90, 0xBA, 0x59, 0x74, 0x4B, 0xAD, 0x1D, 0xCB, 0x29, 0xFC, 0xFD, 0x58, 0x4C, 0x91, 0xE6, 0x79, 0xB9, 0x9A, 0x09, 0x19, 0x39, 0xE8, 0x64, 0x9E, 0x2A, 0xBB, 0x8E, 0x39, 0x65, 0xCD };

        private static string DecodeConnectionString()
        {
            var bytes = new byte[Payload.Length];
            for (var i = 0; i < Payload.Length; i++)
                bytes[i] = (byte)(Payload[i] ^ Key[i % Key.Length] ^ (byte)i);

            return Encoding.UTF8.GetString(bytes);
        }

        public static async Task<MySqlConnection> GetOpenConnectionAsync()
        {
            var connection = new MySqlConnection(DecodeConnectionString());
            try
            {
                await connection.OpenAsync();
                return connection;
            }
            catch
            {
                await connection.DisposeAsync();
                throw;
            }
        }

        public static MySqlConnection GetOpenConnection()
        {
            var connection = new MySqlConnection(DecodeConnectionString());
            try
            {
                connection.Open();
                return connection;
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }
    }
}