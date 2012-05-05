using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models.Response
{
    public class IterationModel
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public bool Published { get; set; }
        public string Description { get; set; }
    }
}
