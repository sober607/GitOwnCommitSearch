using LibGit2Sharp;
using System;
using System.Linq;

namespace GitOwnCommitSearch
{
	class Program
	{
		static void Main(string[] args)
		{
			// List of your repos to search
			var repoList = new string[] { 
				@"C:\repo"
			};

			// The branch to search in
			var branchName = "master";

			// The search term to look for in commit messages
			var commitMessageSearchTerm = "MS-";

			// The search term to look for in commit diffs
			var diffSearchTerm = "Exception";

			foreach(var repoPath in repoList)
			{
				using (var repo = new Repository(repoPath))
				{
					Commands.Checkout(repo, branchName);

					var commits = repo.Commits.QueryBy(new CommitFilter { IncludeReachableFrom = branchName });

					foreach (var commit in commits)
					{
						if (commit.Message.Contains(commitMessageSearchTerm))
						{
							var parentCommit = commit.Parents.FirstOrDefault();
							if (parentCommit != null)
							{
								var patch = repo.Diff.Compare<Patch>(parentCommit.Tree, commit.Tree);

								if (patch.Content.Contains(diffSearchTerm))
								{
									Console.WriteLine($"Commit: {commit.Sha}");
									Console.WriteLine($"Message: {commit.Message.Substring(0, 80)}");

									Console.WriteLine(new string('-', 80));
									Console.WriteLine(Environment.NewLine);
								}
							}
						}
					}
				}
			}
		}
	}
}
