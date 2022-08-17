using Helpdesk_Backend_API.DTOs.ControllerDtos;
using Helpdesk_Backend_API.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Helpdesk_Backend_API.Controllers.v1
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        public RoleManager<IdentityRole> RoleManager { get; }

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            RoleManager = roleManager;
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateRole(CreateRoleDto roleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));

            }

            if (await RoleManager.FindByNameAsync(roleDto.RoleName) is not null)
            {
                ModelState.AddModelError("BadRequest", "Role Already Exists");
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }

            var result = await RoleManager.CreateAsync(new IdentityRole(roleDto.RoleName));
            if (result.Succeeded)
            {
                return Ok(ResponseBuilder.BuildResponse<object>(null, "Completed"));
            }


            ModelState.AddModelError("BadRequest", "Unable To Create Role");
            return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
        }


        [HttpGet("list-roles")]
        public async Task<IActionResult> ViewAllRoles(CancellationToken token)
        {
            var roles = await RoleManager.Roles.Select(c => c.Name).ToListAsync(token);

            return Ok(ResponseBuilder.BuildResponse<object>(null, roles));
        }


        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteRole([Required] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                ModelState.AddModelError("BadRequest", "Role Cannot Be Null Exists");
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }

            var role = await RoleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                ModelState.AddModelError("NotFound", "Role Not Found");
                return NotFound(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }

            var result = await RoleManager.DeleteAsync(role); 
            if (result.Succeeded)
            {
                return Ok(ResponseBuilder.BuildResponse<object>(null, "Completed"));
            }

            ModelState.AddModelError("BadRequest", "Unable To Delete Role");
            return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));

        }
    }
}
