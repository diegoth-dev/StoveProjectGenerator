using Microsoft.AspNetCore.Mvc;

namespace StoveGeneratorApi.Controllers
{
    [Route("")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HelpController : Controller
    {
        [Route("")]
        [HttpGet]
        public RedirectResult Index()
        {
            return Redirect($"{Request.Scheme}://{Request.Host.ToUriComponent()}/swagger");
        }
    }
}
