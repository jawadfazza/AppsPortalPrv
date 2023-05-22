using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AppsPortal.Extensions
{
    public class HashingHelper
    {
        // Pepper value used on source code to be added to the user password 
        // to splite the encryption keys into two pieces on the db and one on the application source code
        // to avoid hacking the password by having the db.
        //WARNING: PEPPER VALUE SHOULD NOT BE CHENAGED, THE PASSWORFDS WILL NOT BE DECODED AGAIN!
        private static readonly string Pepper = "AYCHIHSHMAMAJAFA";//16 Letter


        public static string EncryptPassword(string Password, string UserGUID) //Salt = UserGUID
        {
            // Generate random bytes as salt.
            // Prepend the salt and compute the hash along with the password
            // Then prepend the salt to the hashed result

            byte[] salt = Encoding.UTF8.GetBytes(UserGUID); // Get a random array of bytes
            byte[] pepper = Encoding.UTF8.GetBytes(Pepper);
            byte[] enteredPassword = Encoding.UTF8.GetBytes(Password); // Get the password bytes

            List<byte> passwordByteList = new List<byte>(); // Prepend the salt to the password

            passwordByteList.AddRange(salt);
            passwordByteList.AddRange(pepper);
            passwordByteList.AddRange(enteredPassword);

            byte[] passwordHashResult;

            using (SHA512 shaM = new SHA512Managed())
            {
                passwordHashResult = shaM.ComputeHash(passwordByteList.ToArray()); // Get the hash
            }

            return Convert.ToBase64String(passwordHashResult);
        }
    }
}