using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibGit2Sharp;

namespace PackR.Commands
{
    public class Change
    {
        public Blob Original { get; set; }
        public Blob Modified { get; set; }
        public ChangeType ChangeType
        {
            get
            {
                if (Original == null && Modified != null)
                {
                    return ChangeType.Added;
                }
                else if (Original != null && Modified == null)
                {
                    return ChangeType.Removed;
                }
                else
                {
                    return ChangeType.Modified;
                }
            }
        }

        public Change(Blob original, Blob modified)
        {
            Original = original;
            Modified = modified;
        }
    }
}
