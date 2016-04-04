﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookFast.Api.Models;
using BookFast.Api.Models.Representations;
using BookFast.Contracts;
using BookFast.Contracts.Exceptions;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace BookFast.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "FacilityProviderOnly")]
    public class FacilitiesController : Controller
    {
        private readonly IFacilityService service;
        private readonly IFacilityMapper mapper;

        public FacilitiesController(IFacilityService service, IFacilityMapper mapper)
        {
            this.service = service;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<FacilityRepresentation>> List()
        {
            var facilities = await service.ListAsync();
            return mapper.MapFrom(facilities);
        }
        
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Find(Guid id)
        {
            try
            {
                var facility = await service.FindAsync(id);
                return new ObjectResult(mapper.MapFrom(facility));
            }
            catch (FacilityNotFoundException)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]FacilityData facilityData)
        {
            if (ModelState.IsValid)
            {
                var facility = await service.CreateAsync(mapper.MapFrom(facilityData));
                return CreatedAtAction("Find", mapper.MapFrom(facility));
            }

            return HttpBadRequest();
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody]FacilityData facilityData)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var facility = await service.UpdateAsync(id, mapper.MapFrom(facilityData));
                    return new ObjectResult(mapper.MapFrom(facility));
                }

                return HttpBadRequest();
            }
            catch (FacilityNotFoundException)
            {
                return HttpNotFound();
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await service.DeleteAsync(id);
                return new NoContentResult();
            }
            catch (FacilityNotFoundException)
            {
                return HttpNotFound();
            }
        }
    }
}