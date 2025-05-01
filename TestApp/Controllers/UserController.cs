using System.Net;
using Microsoft.AspNetCore.Mvc;
using TestApp.Interfaces;

namespace TestApp.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : ControllerBase
    {
        private IExternalUserService _service;
        
        public UserController(IExternalUserService service)
        {
            _service = service;
        }
        [HttpGet("users")]
        public async Task<IActionResult> Index()
        {
            var allUsers = await this._service.GetAllUsersAsync();
            return StatusCode(200,allUsers);
        }

        [HttpGet("getuser/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await this._service.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(502, new { error = ex.Message }); 
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }
    }
}
