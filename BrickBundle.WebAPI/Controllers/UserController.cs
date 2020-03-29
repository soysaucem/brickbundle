using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BrickBundle.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // TODO: Some form of spam prevention for Anonymous methods.
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Register([FromBody] DTO.RegisterUserDTO registerUser)
        {
            try
            {
                var user = await Model.User.Register(registerUser.Username, registerUser.EmailAddress, registerUser.Password);
                if (user != null)
                {
                    return Created("", user.Username);
                }
                return BadRequest();
            }
            catch (Model.User.UserAlreadyExistsException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Model.User.InvalidUserException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("verify")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> SendEmailVerificationCode()
        {
            var identity = new Authentication.ApiIdentity(HttpContext.User.Identity);
            var user = await Model.User.Find(identity.UserID);
            if (user != null)
            {
                if (await user.GenerateEmailVerificationCode())
                {
                    EmailHandler.SendEmailVerificationCode(user.EmailAddress, user.EmailVerificationCode);
                    return Ok(user.EmailAddress);
                }
                return BadRequest();
            }
            // Should never reach here
            return BadRequest();
        }

        [Authorize]
        [HttpPost("verify")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> VerifyEmail([FromBody] string code)
        {
            var identity = new Authentication.ApiIdentity(HttpContext.User.Identity);
            var user = await Model.User.Find(identity.UserID);
            if (user != null)
            {
                if (await user.VerifyEmail(code))
                {
                    return Ok(user.EmailAddress);
                }
                return BadRequest();
            }
            // Should never reach here
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpGet("reset/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> SendResetPasswordCode([FromRoute] string username)
        {
            var user = await Model.User.FindByUsernameOrEmail(username);
            if (user != null)
            {
                if (await user.GenerateResetPasswordCode())
                {
                    EmailHandler.SendResetPasswordCode(user.EmailAddress, user.ResetPasswordCode);
                    return Ok();
                }
                return BadRequest();
            }
            return NotFound(username);
        }

        [AllowAnonymous]
        [HttpPost("reset/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ResetPassword([FromRoute] string username, [FromBody] DTO.ResetPasswordDTO resetPassword)
        {
            var user = await Model.User.FindByUsernameOrEmail(username);
            if (user != null)
            {
                if (await user.ResetPassword(resetPassword.Code, resetPassword.Password))
                {
                    return Ok();
                }
                return BadRequest();
            }
            return NotFound(username);
        }

        [Authorize]
        [HttpGet("parts")]
        [ProducesResponseType(typeof(DTO.UserPartDTO[]), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetParts()
        {
            var identity = new Authentication.ApiIdentity(HttpContext.User.Identity);
            return Ok(await Model.User.ListPartsForUser(identity.UserID));
        }

        [Authorize]
        [HttpPost("parts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostParts([FromBody] DTO.UserPartDTO[] userParts)
        {
            var identity = new Authentication.ApiIdentity(HttpContext.User.Identity);
            var user = await Model.User.Find(identity.UserID);
            if (user != null)
            {
                if (await user.UpdateParts(userParts))
                {
                    return Ok();
                }
                return BadRequest();
            }
            // Should never reach here
            return BadRequest();
        }

        [Authorize]
        [HttpGet("buildsets")]
        [ProducesResponseType(typeof(DTO.SetListItemDTO[]), StatusCodes.Status200OK)]
        public async Task<ActionResult> ListSetsUserCanBuild()
        {
            var identity = new Authentication.ApiIdentity(HttpContext.User.Identity);
            var dto = await Model.User.ListSetsUserCanBuild(identity.UserID);
            return Ok(dto);
        }
    }
}