using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using VibrantUtils;

namespace ReviewR.Web.Services
{
    public class HashService
    {
        private const int SaltLength = 16;
        private RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();
        public KeyedHashAlgorithm Algorithm { get; set; }

        public HashService()
            : this(new HMACSHA256())
        {
        }

        public HashService(KeyedHashAlgorithm algorithm)
        {
            Algorithm = algorithm;
        }

        public virtual string GenerateHash(string value, string salt)
        {
            Requires.NotNullOrEmpty(value, "value");
            Requires.NotNullOrEmpty(salt, "salt");

            // Get the data
            byte[] saltData = Convert.FromBase64String(salt);
            byte[] valueData = Encoding.UTF8.GetBytes(value);

            // Hash it
            Algorithm.Key = saltData;
            byte[] hashed = Algorithm.ComputeHash(valueData);

            // Return as base-64 string
            return Convert.ToBase64String(hashed);
        }

        public virtual string GenerateSalt()
        {
            byte[] saltData = new byte[SaltLength];
            _rng.GetBytes(saltData);
            return Convert.ToBase64String(saltData);
        }
    }
}