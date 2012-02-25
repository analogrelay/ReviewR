using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ReviewR.Diff;
using ReviewR.Web.ViewModels;

namespace ReviewR.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Upload(string callback, string format)
        {
            using (StreamReader reader = new StreamReader(Request.Files[0].InputStream))
            {
                string json = 
                    new JavaScriptSerializer().Serialize(
                        ConvertDiff(UnifiedDiff.Read(reader)));
                
                //return Content(json, "application/json");
                    
                ViewBag.Callback = callback;
                ViewBag.Diff = new HtmlString(HttpUtility.JavaScriptStringEncode(json));
                return View();
            }
        }

        private DiffViewModel ConvertDiff(Diff.Diff diff)
        {
            return new DiffViewModel()
            {
                Files = new Dictionary<string, DiffFileViewModel>()
                {
                    {
                        diff.OriginalFile,
                        new DiffFileViewModel() {
                            Deletions = diff.Hunks.Sum(h => h.Lines.Where(l => l.Type == DiffLineType.Removed).Count()),
                            Insertions = diff.Hunks.Sum(h => h.Lines.Where(l => l.Type == DiffLineType.Added).Count()),
                            Binary = false,
                            DiffLines = diff.Hunks.SelectMany(ConvertHunk).ToList()
                        }
                    }
                }
            };
        }

        private IEnumerable<DiffLineViewModel> ConvertHunk(DiffHunk arg)
        {
            return arg.Lines.Select((l, i) => new DiffLineViewModel()
            {
                LeftLine = arg.OriginalLocation.Line + i,
                RightLine = arg.ModifiedLocation.Line + i,
                Text = l.Content,
                Type = l.Type
            });
        }
    }
}
