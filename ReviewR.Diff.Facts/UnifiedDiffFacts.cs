using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ReviewR.Diff.Facts
{
    public class UnifiedDiffFacts
    {
        public class ReadMethod
        {
            [Fact]
            public void RequiresNonNullTextReader()
            {
                Assert.Throws<ArgumentNullException>(() => UnifiedDiff.Read(null));
            }
        }
    }
}
