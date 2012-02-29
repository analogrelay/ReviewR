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

    public class ReviewDetailViewModel : ReviewSummaryViewModel, IReviewLayoutViewModel
    {
        public bool IsAuthor { get; set; }
        public FileChangeViewModel Selected { get; set; }
        public ICollection<FolderChangeViewModel> Folders { get; set; }

        ReviewDetailViewModel IReviewLayoutViewModel.Review
        {
            get { return this; }
        }
    }
}
