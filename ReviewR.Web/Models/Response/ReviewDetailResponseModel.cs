using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Models.Response
{
    public class ReviewDetailResponseModel : ReviewResponseModel
    {
        public string Description { get; set; }
        public IEnumerable<IterationModel> Iterations { get; set; }
        public IEnumerable<ParticipantModel> Participants { get; set; }
    }
}
