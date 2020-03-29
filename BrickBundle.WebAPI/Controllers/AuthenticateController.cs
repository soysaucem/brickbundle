using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BrickBundle.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Login([FromBody]DTO.LoginUserDTO loginUser)
        {
            try
            {
                var user = await Model.User.Login(loginUser.Username, loginUser.Password);
                if (user == null)
                {
                    return BadRequest("invalid username or password");
                }
                return Created("", new
                {
                    token = Authentication.GenerateToken(user.ID, user.Username)
                }); ;
            }
            catch (Model.User.InvalidUsernameOrPasswordException)
            {
                return BadRequest("invalid username or password");
            }
        }
    }
}
