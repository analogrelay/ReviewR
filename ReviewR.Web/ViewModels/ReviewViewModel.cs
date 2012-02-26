using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewR.Web.ViewModels
{
    public class ReviewSummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ReviewDetailViewModel : ReviewSummaryViewModel
    {
        public FileChangeViewModel Selected { get; set; }
        public ICollection<FolderChangeViewModel> Folders { get; set; }
    }
}
