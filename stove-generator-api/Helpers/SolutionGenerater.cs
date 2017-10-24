using System;
using System.IO;
using System.IO.Compression;
using LibGit2Sharp;

namespace StoveGeneratorApi.Helpers
{
    public class SolutionGenerater
    {
        public byte[] Generate(string repositoryLink, string placeHolder, string projectName, CloneOptions cloneOptions = null)
        {
            DirectoryInfo directory = Directory.GetParent(Directory.GetCurrentDirectory());
            string templatePath = Path.Combine(directory.FullName, $"template-{Guid.NewGuid():N}");
            string zipPath = Path.Combine(directory.FullName, $"{projectName}.zip");

            string clonedRepoPath = Repository.Clone(repositoryLink, templatePath, cloneOptions);

            new Repository(clonedRepoPath).Dispose();

            var solutionRenamer = new SolutionRenamer(templatePath, null, placeHolder, null, projectName)
            {
                CreateBackup = false
            };
            solutionRenamer.Run();

            ZipFile.CreateFromDirectory(templatePath, zipPath);

            FileHelper.DeleteDirectory(templatePath);

            byte[] zipBytes = System.IO.File.ReadAllBytes(zipPath);

            System.IO.File.Delete(zipPath);

            return zipBytes;
        }
    }
}