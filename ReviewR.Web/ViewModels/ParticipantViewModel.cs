using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.ViewModels
{
    public class ParticipantViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
    }
}
