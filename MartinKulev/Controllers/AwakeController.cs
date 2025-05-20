using Microsoft.AspNetCore.Mvc;

namespace MartinKulev.Controllers
{
    [Route("awake")]  // Base route: /awake
    public class AwakeController : Controller
    {
        [HttpGet]
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return Content("AwakeController Index", "text/plain");
        }
    }
}
