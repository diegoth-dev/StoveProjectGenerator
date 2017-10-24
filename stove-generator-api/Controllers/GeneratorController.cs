using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using LibGit2Sharp;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using StoveGeneratorApi.Helpers;

namespace StoveGeneratorApi.Controllers
{
    [Route("api/generator")]
    public class GeneratorController : Controller
    {
        private const string ContentType = "application/zip";
        private readonly ILogger<GeneratorController> _logger;

        public GeneratorController(ILogger<GeneratorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("generate/{placeHolder}/{projectName}/{repositoryLink}")]
        public FileContentResult Generate(string placeHolder, string projectName, string repositoryLink)
        {
            try
            {
                string repoUrl = WebUtility.UrlDecode(repositoryLink);

                DirectoryInfo directory = Directory.GetParent(Directory.GetCurrentDirectory());
                string zipPath = Path.Combine(directory.FullName, $"{projectName}.zip");

                SolutionGenerater solutionGenerater = new SolutionGenerater();
                Byte[] zipBytes = solutionGenerater.Generate(repoUrl, placeHolder, projectName);

                return new FileContentResult(zipBytes, ContentType)
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

        [HttpGet]
        [Route("generate/{placeHolder}/{projectName}/{repositoryLink}/{username}/{password}")]
        public FileContentResult Generate(string placeHolder, string projectName, string repositoryLink, string username, string password)
        {
            try
            {
                string repoUrl = WebUtility.UrlDecode(repositoryLink);
                CloneOptions cloneOptions = new CloneOptions()
                {
                    CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                    {
                        Username = username,
                        Password = password
                    }
                };

                SolutionGenerater solutionGenerater = new SolutionGenerater();
                Byte[] zipBytes = solutionGenerater.Generate(repoUrl, placeHolder, projectName, cloneOptions);


                DirectoryInfo directory = Directory.GetParent(Directory.GetCurrentDirectory());
                string zipPath = Path.Combine(directory.FullName, $"{projectName}.zip");

                return new FileContentResult(zipBytes, ContentType)
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
