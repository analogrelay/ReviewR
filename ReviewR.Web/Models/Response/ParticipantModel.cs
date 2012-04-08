using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReviewR.Web.Models.Data;

namespace ReviewR.Web.Models.Response
{
    public class ParticipantModel
    {
        public UserModel User { get; set; }
        public ParticipantStatus Status { get; set; }
        public bool Required { get; set; }
    }
}
