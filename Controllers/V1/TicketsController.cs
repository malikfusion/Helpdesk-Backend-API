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
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService ticketService;
        private readonly IMapper mapper;

        public TicketsController(ITicketService ticketService, IMapper mapper)
        {
            this.ticketService = ticketService;
            this.mapper = mapper;
        }

        
        [HttpGet("Get-ticket")]
        public async Task<IActionResult> GetTicket(string ticketId, CancellationToken token)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                ModelState.AddModelError("Invalid", "Input a valid query string");
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }
            else
            {
                var result = await ticketService.GetTicket(ticketId, token);

                if (result.ResponseType == ResponseType.NotFound)
                {
                    ModelState.AddModelError("NotFound", result.Message);
                    return NotFound(ResponseBuilder.BuildResponse<object>(ModelState, null));
                }
                else
                {
                    return Ok(ResponseBuilder.BuildResponse(null, mapper.Map<GetTicket>(result.Data)));
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
                var queryResult = ticketService.ListByOrganization(organizationId);

                var totalCount = await queryResult.CountAsync(token);

                var paginatedList = queryResult.Paginate(page, perPage);

                return Ok(ResponseBuilder.BuildResponse(null,
                    Pagination.GetPagedData(mapper.Map<List<GetTicket>>(paginatedList), page, perPage, totalCount)));
            }
        }

        [HttpGet("list-by-staff")]
        public async Task<IActionResult> ListByStaff(string staffId, int page, int perPage, CancellationToken token)
        {
            if (string.IsNullOrEmpty(staffId))
            {
                ModelState.AddModelError("Invalid", "input a valid query string");
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }

            else
            {
                var queryResult = ticketService.ListByStaff(staffId);

                var totalCount = await queryResult.CountAsync(token);

                var paginatedList = queryResult.Paginate(page, perPage);

                return Ok(ResponseBuilder.BuildResponse(null,
                    Pagination.GetPagedData(mapper.Map<List<GetTicket>>(paginatedList), page, perPage, totalCount)));
            }
        }

        [HttpGet("list-all")]
        public async Task<IActionResult> ListAll(int page, int perPage, CancellationToken token)
        {
            var queryResult = ticketService.GetAll();

            var totalCount = await queryResult.CountAsync(token);

            var paginatedList = queryResult.Paginate(page, perPage);

            return Ok(ResponseBuilder.BuildResponse(null,
                Pagination.GetPagedData(mapper.Map<List<GetTicket>>(paginatedList), page, perPage, totalCount)));
        }


        [HttpPost("Create")]
        public async Task<IActionResult> AddTicket(AddTicket model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }
            else
            {
                var createResult = await ticketService.AddTicket(model, token);
                if(createResult.ResponseType != ResponseType.Success)
                {
                    ModelState.AddModelError("Failed", createResult.Message);
                    return UnprocessableEntity(ResponseBuilder.BuildResponse<object>(ModelState, null));
                }
                else
                {
                    return Ok(ResponseBuilder.BuildResponse(null, mapper.Map<GetTicket>(createResult.Data)));
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
