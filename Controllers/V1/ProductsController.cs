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
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly IMapper mapper;

        public ProductsController(IProductService productService, IMapper mapper)
        {
            this.productService = productService;
            this.mapper = mapper;
        }


        [HttpPost("create")]
        public async Task<IActionResult> Create(AddProduct model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }
            else
            {
                var createResult = await productService.AddProduct(model, token);

                if (createResult.ResponseType != ResponseType.Success)
                {
                    ModelState.AddModelError("Failed", createResult.Message);
                    return UnprocessableEntity(ResponseBuilder.BuildResponse<object>(ModelState, null));
                }
                else
                {
                    return Ok(ResponseBuilder.BuildResponse(null, mapper.Map<GetProduct>(createResult.Data)));
                }
            }
        }


        [HttpGet("get-product")]
        public async Task<IActionResult> GetProduct(string productId, CancellationToken token)
        {
            if (string.IsNullOrEmpty(productId))
            {
                ModelState.AddModelError("Invalid", "input a valid query string");
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }
            else
            {
                var result = await productService.GetProduct(productId, token);

                if(result.ResponseType == ResponseType.NotFound)
                {
                    ModelState.AddModelError("NotFound", result.Message);
                    return NotFound(ResponseBuilder.BuildResponse<object>(ModelState, null));
                }
                else
                {
                    return Ok(ResponseBuilder.BuildResponse(null, mapper.Map<GetProduct>(result.Data)));
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
                var queryResult = productService.ListByOrganization(organizationId);

                var totalCount = await queryResult.CountAsync(token);

                var paginatedList = queryResult.Paginate(page, perPage);

                return Ok(ResponseBuilder.BuildResponse(null,
                    Pagination.GetPagedData(mapper.Map<List<GetProduct>>(paginatedList), page, perPage, totalCount)));
            }
        }


        [HttpGet("list-by-project")]
        public async Task<IActionResult> ListByProject(string projectId, int page, int perPage, CancellationToken token)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                ModelState.AddModelError("Invalid", "input a valid query string");
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }

            else
            {
                var queryResult = productService.ListByProject(projectId);

                var totalCount = await queryResult.CountAsync(token);

                var paginatedList = queryResult.Paginate(page, perPage);

                return Ok(ResponseBuilder.BuildResponse(null,
                    Pagination.GetPagedData(mapper.Map<List<GetProduct>>(paginatedList), page, perPage, totalCount)));
            }
        }


        [HttpGet("list-all")]
        public async Task<IActionResult> ListAll(int page, int perPage, CancellationToken token)
        {
            var queryResult = productService.GetAll();

            var totalCount = await queryResult.CountAsync(token);

            var paginatedList = queryResult.Paginate(page, perPage);

            return Ok(ResponseBuilder.BuildResponse(null,
                Pagination.GetPagedData(mapper.Map<List<GetProduct>>(paginatedList), page, perPage, totalCount)));
        }
    }
}
