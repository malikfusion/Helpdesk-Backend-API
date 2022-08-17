using Helpdesk_Backend_API.DTOs;
using Helpdesk_Backend_API.Services;
using Helpdesk_Backend_API.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk_Backend_API.Controllers.V1
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IFusionAdminService fusionAdminService;
        public AuthController(IUserService userService, IFusionAdminService fusionAdminService)
        {
            this.userService = userService;
            this.fusionAdminService = fusionAdminService;
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestModel model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }
            else
            {
                if(await fusionAdminService.IsUserNameTaken(model.UserName, token) is false)
                {
                    ModelState.AddModelError("Notfound", "User account not found");
                    return UnprocessableEntity(ResponseBuilder.BuildResponse<object>(ModelState, null));
                }
                else
                {
                    var loginResult = await userService.Login(model);

                    if(loginResult is null)
                    {
                        ModelState.AddModelError("Failed", "failed to login");
                        return UnprocessableEntity(ResponseBuilder.BuildResponse<object>(ModelState, null));
                    }
                    else
                    {
                        return Ok(ResponseBuilder.BuildResponse(null, loginResult));
                    }
                }
            }
        }
    }
}
