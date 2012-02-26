using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReviewR.Web.Models;
using ReviewR.Web.ViewModels;

namespace ReviewR.Web.Services
{
    public static class FileChangeViewModelMapper
    {
        public static ICollection<FolderChangeViewModel> MapFiles(ICollection<FileChange> collection)
        {
            return collection.GroupBy(GetFolderName).Select(ProcessFolder).ToList();
        }

        private static FolderChangeViewModel ProcessFolder(IGrouping<string, FileChange> arg)
        {
            int idx = arg.Key.Length;
            return new FolderChangeViewModel()
            {
                Name = arg.Key,
                Files = arg.Select(chg =>
                {
                    int lastSlash = chg.FileName.LastIndexOf('/');
                    string name = (lastSlash >= 0 && lastSlash < chg.FileName.Length - 1) ?
                                  chg.FileName.Substring(lastSlash + 1) :
                                  chg.FileName;
                    return new FileChangeViewModel()
                    {
                        Id = chg.Id,
                        ChangeType = chg.ChangeType,
                        FileName = name
                    };
                }).ToList()
            };
        }

        private static string GetFolderName(FileChange arg)
        {
            int lastSlash = arg.FileName.LastIndexOf('/');
            return lastSlash > 0 ? arg.FileName.Substring(0, lastSlash) : "/";
        }

        private static string GetOldFileName(FileChange f)
        {
            FileModification mod = f as FileModification;
            if (mod != null)
            {
                return mod.NewFileName;
            }
            else
            {
                return null;
            }
        }
    }
}