using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;
using LibGit2Sharp;
using VibrantCommandLine;

namespace PackR.Commands
{
    [Export(typeof(Command))]
    [Command("pack", "Package changes from the current repository for review")]
    public class PackCommand : Command
    {
        public override int ExecuteCommand()
        {
            // Do we have a revision-spec?
            if (Arguments.Count > 0)
            {
                throw new NotImplementedException();
            }

            // Get the current repo
            string repoPath = Repository.Discover(Environment.CurrentDirectory);
            if (String.IsNullOrEmpty(repoPath))
            {
                throw new InvalidOperationException("Cannot find a git repo!");
            }
            Repository repo = new Repository(repoPath);

            // What branch are we on?
            Branch head = repo.Head;

            // What branch are we starting from?
            Branch start = repo.Branches["master"];
            if (start == null)
            {
                throw new InvalidOperationException("No 'master' branch and a branch wasn't specified on the command line...");
            }

            // Calculate changes
            IEnumerable<Change> changes = ChangeCalculator.Calculate(start.Commits.FirstOrDefault(), head.Commits.FirstOrDefault());
            return 0;
        }
    }
}
