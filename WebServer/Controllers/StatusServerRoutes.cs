using EggLink.DanhengServer.WebServer.Handler;
using Microsoft.AspNetCore.Mvc;

namespace EggLink.DanhengServer.WebServer.Controllers
{
    [ApiController]
    [Route("/")]
    public class StatusServerRoutes
    {
        [HttpGet("/status/server")]
        public ActionResult<string> StatusServer()
        {
            var responseJson = new StatusServerHandler().ResponseJson;
            return Ok(responseJson);
        }
    }
}
