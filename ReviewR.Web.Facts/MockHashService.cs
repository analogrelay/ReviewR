using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReviewR.Web.Services;

namespace ReviewR.Web.Facts
{
    public class MockHashService : HashService
    {
        public override string GenerateHash(string value, string salt)
        {
            return new String(String.Concat(salt, "|", value).Reverse().ToArray());
        }

        public override string GenerateSalt()
        {
            return "salt";
        }
    }
}
