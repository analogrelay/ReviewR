using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Security;

namespace ReviewR.Web.Models
{
    public class AuthenticationToken
    {
        public const int CurrentVersion = 1;

        public int Version { get; private set; }
        public Guid TokenId { get; set; }
        public DateTimeOffset Expires { get; set; }

        public AuthenticationToken(Guid tokenId, DateTimeOffset expires): this(CurrentVersion, tokenId, expires) { }
        public AuthenticationToken(int version, Guid tokenId, DateTimeOffset expires)
        {
            Version = version;
            TokenId = tokenId;
            Expires = expires;
        }

        public byte[] EncodeToken()
        {
            // Write the unencrypted token
            MemoryStream strm = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(strm);
            writer.Write(Version);
            writer.Write(TokenId.ToByteArray());
            writer.Write(Expires.UtcTicks);
            byte[] buf = strm.ToArray();
            writer.Dispose();
            return buf;
        }

        public static AuthenticationToken FromEncodedToken(byte[] encoded)
        {
            // Read the token
            MemoryStream strm = new MemoryStream(encoded);
            BinaryReader reader = new BinaryReader(strm);
            int version = reader.ReadInt32();
            if (version != CurrentVersion)
            {
                throw new SecurityException("Unknown token version");
            }
            Guid tokenId = new Guid(reader.ReadBytes(16));
            DateTimeOffset expires = new DateTimeOffset(reader.ReadInt64(), TimeSpan.Zero);
            return new AuthenticationToken(tokenId, expires);
        }
    }
}