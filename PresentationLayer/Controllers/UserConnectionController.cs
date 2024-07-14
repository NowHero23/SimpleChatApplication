using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserConnectionController : Controller
    {
        private IUserConnectionService _connectionService;

        public UserConnectionController(IUserConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
        [HttpGet("getAll")]
        public IActionResult GetAllUserConnections()
        {
            var userConnection = _connectionService.FindAll().ToList();
            return Ok(userConnection);
        }
    }
}
