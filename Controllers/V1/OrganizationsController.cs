using AutoMapper;
using Helpdesk_Backend_API.DTOs;
using Helpdesk_Backend_API.Services;
using Helpdesk_Backend_API.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk_Backend_API.Controllers.V1
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class OrganizationsController : ControllerBase
    {
        private readonly IOrganizationService organizationService;
        private readonly IMapper mapper;

        public OrganizationsController(IOrganizationService organizationService, IMapper mapper)
        {
            this.organizationService = organizationService;
            this.mapper = mapper;
        }


        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateOrganization model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }
            else
            {
                var createResult = await organizationService.AddOrganization(model, token);

                if(createResult.ResponseType != ResponseType.Success)
                {
                    ModelState.AddModelError("Failed", createResult.Message);
                    return UnprocessableEntity(ResponseBuilder.BuildResponse<object>(ModelState, null));
                }
                else
                {
                    return Ok(ResponseBuilder.BuildResponse(null, mapper.Map<GetOrganization>(createResult.Data)));
                }
            }
        }


        [HttpGet("list-all")]
        public async Task<IActionResult> ListAll(int page, int perPage, CancellationToken token)
        {
            var queryResult = organizationService.GetAll();

            var totalCount = await queryResult.CountAsync(token);

            var paginatedList = queryResult.Paginate(page, perPage);

            return Ok(ResponseBuilder.BuildResponse(null,
                Pagination.GetPagedData(mapper.Map<List<GetOrganization>>(paginatedList), page, perPage, totalCount)));
        }


        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(string organizationId, CancellationToken token)
        {
            if (string.IsNullOrEmpty(organizationId))
            {
                ModelState.AddModelError("Invalid", "input a valid query string");
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }
            else
            {
                var queryResult = await organizationService.GetOrganization(organizationId, token);

                if(queryResult.ResponseType == ResponseType.NotFound)
                {
                    ModelState.AddModelError("NotFound", queryResult.Message);
                    return NotFound(ResponseBuilder.BuildResponse<object>(ModelState, null));
                }
                else if(queryResult.ResponseType == ResponseType.Success)
                {
                    return Ok(ResponseBuilder.BuildResponse(null, mapper.Map<GetOrganization>(queryResult.Data)));
                }
                else
                {
                    ModelState.AddModelError("Failed", queryResult.Message);
                    return UnprocessableEntity(ResponseBuilder.BuildResponse<object>(ModelState, null));
                }
            }
        }
    }
}
