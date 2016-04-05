﻿using System.Collections.Generic;
using System.Threading.Tasks;
using BookFast.Contracts;
using BookFast.Contracts.Search;
using Microsoft.AspNet.Mvc;
using Swashbuckle.SwaggerGen.Annotations;

namespace BookFast.Api.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        private readonly ISearchService service;

        public SearchController(ISearchService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Search for accommodations
        /// </summary>
        /// <param name="searchText">Search terms</param>
        /// <param name="page">Page number</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("search")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, Type = typeof(IEnumerable<SearchResult>))]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, Description = "Invalid parameters")]
        public async Task<IActionResult> Search([FromQuery]string searchText, [FromQuery]int page = 1)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return HttpBadRequest();

            if (page < 1)
                return HttpBadRequest();

            var searchResults = await service.SearchAsync(searchText, page);
            return Ok(searchResults);
        }
    }
}