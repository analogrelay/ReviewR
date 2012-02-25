using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace ReviewR.Web.Facts.Controllers
{
    public class TestPostedFile : HttpPostedFileBase
    {
        private Stream _content;

        public override int ContentLength
        {
            get
            {
                return (int)_content.Length;
            }
        }

        public override string ContentType
        {
            get
            {
                return "text/plain";
            }
        }

        public override string FileName
        {
            get
            {
                return "TestFile.txt";
            }
        }

        public override Stream InputStream
        {
            get
            {
                return _content;
            }
        }

        public TestPostedFile() : this(Stream.Null) { }
        public TestPostedFile(string content) : this(CreateStream(content)) { }

        public TestPostedFile(Stream strm)
        {
            _content = strm;
        }

        public override void SaveAs(string filename)
        {
            throw new NotImplementedException();
        }

        private static Stream CreateStream(string content)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(content));
        }
    }
}
