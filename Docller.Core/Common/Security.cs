using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using Docller.Core.Models;

namespace Docller.Core.Common
{
    public class Security
    {
        public static string GeneratePassword()
        {
            return Membership.GeneratePassword(8, 1);
        }

        public static void PopulatePassword(User user)
        {
            user.PasswordSalt = CreateSalt();
            user.Password = CreatePasswordHash(user.Password, user.PasswordSalt);
        }

        public static User GetUserWithTempPassword(string userName)
        {
            User user = new User {UserName = userName, Password = GetRandomString()};
            return user;
        }

        public static string CreateSalt()
        {
            return CreateSalt(5);
        }

        public static string CreateSalt(int size)
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number.
            return Convert.ToBase64String(buff);
        }

        public static string CreatePasswordHash(string password, string salt)
        {
            string saltAndPwd = String.Concat(password, salt);
            string hashedPwd =
                FormsAuthentication.HashPasswordForStoringInConfigFile(
                saltAndPwd, "sha1");
            return hashedPwd;
        }

        public static string GetRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path;

        }

        public static string Encrypt(string value)
        {
            byte[] data = MachineKey.Protect(Encoding.UTF8.GetBytes(value));
            return Convert.ToBase64String(data);
        }

        public static T Decrypt<T>(string value)
        {
            byte[] dataToDecrypt = Convert.FromBase64String(value);
            byte[] data = MachineKey.Unprotect(dataToDecrypt);
            if (data != null)
            {
                string val = Encoding.UTF8.GetString(data);
                if (!string.IsNullOrEmpty(val))
                {
                    return (T)ValueSerializer.Deserialize(typeof(T), val);
                }
            }
            return default (T);
        }
    }
}
