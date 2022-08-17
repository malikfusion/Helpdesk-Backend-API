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
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService projectService;
        private readonly IMapper mapper;
        public ProjectsController(IProjectService projectService, IMapper mapper)
        {
            this.projectService = projectService;
            this.mapper = mapper;
        }



        [HttpPost("create")]
        public async Task<IActionResult> AddProject(AddProject model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }
            else
            {
                var createResult = await projectService.AddProject(model, token);

                if (createResult.ResponseType != ResponseType.Success)
                {
                    ModelState.AddModelError("Failed", createResult.Message);
                    return UnprocessableEntity(ResponseBuilder.BuildResponse<object>(ModelState, null));
                }
                else
                {
                    return Ok(ResponseBuilder.BuildResponse(null, mapper.Map<GetProject>(createResult.Data)));
                }
            }
        }



        [HttpGet("get-project")]
        public async Task<IActionResult> GetProject(string projectId, CancellationToken token)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                ModelState.AddModelError("Invalid", "input a valid query string");
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }
            else
            {
                var result = await projectService.GetProject(projectId, token);

                if (result.ResponseType == ResponseType.NotFound)
                {
                    ModelState.AddModelError("NotFound", result.Message);
                    return NotFound(ResponseBuilder.BuildResponse<object>(ModelState, null));
                }
                else
                {
                    return Ok(ResponseBuilder.BuildResponse(null, mapper.Map<GetProject>(result.Data)));
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
                var queryResult = projectService.ListByOrganization(organizationId);

                var totalCount = await queryResult.CountAsync(token);

                var paginatedList = queryResult.Paginate(page, perPage);

                return Ok(ResponseBuilder.BuildResponse(null,
                    Pagination.GetPagedData(mapper.Map<List<GetProject>>(paginatedList), page, perPage, totalCount)));
            }
        }


        [HttpGet("list-all")]
        public async Task<IActionResult> ListAll(int page, int perPage, CancellationToken token)
        {
            var queryResult = projectService.GetAll();

            var totalCount = await queryResult.CountAsync(token);

            var paginatedList = queryResult.Paginate(page, perPage);

            return Ok(ResponseBuilder.BuildResponse(null,
                Pagination.GetPagedData(mapper.Map<List<GetProject>>(paginatedList), page, perPage, totalCount)));
        }
    }
}
