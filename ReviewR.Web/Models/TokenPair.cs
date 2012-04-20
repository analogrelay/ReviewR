using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models
{
    public class TokenPair
    {
        public string Session { get; private set; }
        public string Persistent { get; private set; }

        public TokenPair(string session, string persistent)
        {
            Session = session;
            Persistent = persistent;
        }
    }
}
