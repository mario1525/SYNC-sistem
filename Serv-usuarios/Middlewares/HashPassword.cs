using System;
using BCrypt.Net;

namespace Middlewares
{
    public class HashPassword
    {

        // Método para hashear una contraseña
        public string Hashpassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Método para verificar una contraseña contra un hash
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}