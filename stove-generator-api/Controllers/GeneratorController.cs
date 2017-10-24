using System;
using System.IO;
using System.IO.Compression;

using LibGit2Sharp;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using StoveGeneratorApi.Helpers;

namespace StoveGeneratorApi.Controllers
{
    [Route("api/generator")]
    public class GeneratorController : Controller
    {
        private readonly ILogger<GeneratorController> _logger;

        public GeneratorController(ILogger<GeneratorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("generate/{projectName}")]
        public FileContentResult Generate(string projectName)
        {
            try
            {
                DirectoryInfo directory = Directory.GetParent(Directory.GetCurrentDirectory());
                string templatePath = Path.Combine(directory.FullName, $"template-{Guid.NewGuid():N}");
                string zipPath = Path.Combine(directory.FullName, $"{projectName}.zip");

                string clonedRepoPath = Repository.Clone("yourrepsitorylik", templatePath);

                new Repository(clonedRepoPath).Dispose();

                var solutionRenamer = new SolutionRenamer(templatePath, null, "StoveProjectName", null, projectName)
                {
                    CreateBackup = false
                };
                solutionRenamer.Run();

                ZipFile.CreateFromDirectory(templatePath, zipPath);

                FileHelper.DeleteDirectory(templatePath);

                const string contentType = "application/zip";
                byte[] zipBytes = System.IO.File.ReadAllBytes(zipPath);

                System.IO.File.Delete(zipPath);

                return new FileContentResult(zipBytes, contentType)
                {
                    FileDownloadName = zipPath
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.ToString());
                throw;
            }
        }
    }
}
