using AutoMapper;
using Helpdesk_Backend_API.DTOs;
using Helpdesk_Backend_API.Services;
using Helpdesk_Backend_API.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk_Backend_API.Controllers.V1
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class StaffsController : ControllerBase
    {
        private readonly IStaffService staffService;
        private readonly IMapper mapper;

        public StaffsController(IStaffService staffService, IMapper mapper)
        {
            this.staffService = staffService;
            this.mapper = mapper;
        }


        [HttpGet("Get-ticket")]
        public async Task<IActionResult> GetStaff(string staffId, CancellationToken token)
        {
            if (string.IsNullOrEmpty(staffId))
            {
                ModelState.AddModelError("Invalid", "Input a valid query string");
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }
            else
            {
                var result = await staffService.GetStaff(staffId, token);

                if (result.ResponseType == ResponseType.NotFound)
                {
                    ModelState.AddModelError("NotFound", result.Message);
                    return NotFound(ResponseBuilder.BuildResponse<object>(ModelState, null));
                }
                else
                {
                    return Ok(ResponseBuilder.BuildResponse(null, mapper.Map<GetStaff>(result.Data)));
                }
            }
        }

        [HttpGet("list-by-organization")]
        public async Task<IActionResult> ListByOrganization(string organizationId, int page, int perPage, CancellationToken token)
        {
            if (string.IsNullOrEmpty(organizationId))
            {
                ModelState.AddModelError("Invalid", "input a valid query string");
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }

            else
            {
                var queryResult = staffService.ListByOrganization(organizationId);

                var totalCount = await queryResult.CountAsync(token);

                var paginatedList = queryResult.Paginate(page, perPage);

                return Ok(ResponseBuilder.BuildResponse(null,
                    Pagination.GetPagedData(mapper.Map<List<GetStaff>>(paginatedList), page, perPage, totalCount)));
            }
        }

        [HttpGet("list-all")]
        public async Task<IActionResult> ListAll(int page, int perPage, CancellationToken token)
        {
            var queryResult = staffService.GetAll();

            var totalCount = await queryResult.CountAsync(token);

            var paginatedList = queryResult.Paginate(page, perPage);

            return Ok(ResponseBuilder.BuildResponse(null,
                Pagination.GetPagedData(mapper.Map<List<GetStaff>>(paginatedList), page, perPage, totalCount)));
        }


        [HttpPost("Create")]
        public async Task<IActionResult> AddStaff(AddStaff model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }
            else
            {
                var createResult = await staffService.AddStaff(model, token);
                if (createResult.ResponseType != ResponseType.Success)
                {
                    ModelState.AddModelError("Failed", createResult.Message);
                    return UnprocessableEntity(ResponseBuilder.BuildResponse<object>(ModelState, null));
                }
                else
                {
                    return Ok(ResponseBuilder.BuildResponse(null, mapper.Map<GetStaff>(createResult.Data)));
                }
            }
        }

        // PUT api/<TicketsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TicketsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
