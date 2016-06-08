﻿using BookFast.Api.Models.Representations;
using BookFast.Contracts.Exceptions;
using BookFast.Files.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;
using System;
using System.Threading.Tasks;

namespace BookFast.Api.Controllers
{
    [Authorize(Policy = "Facility.Write")]
    [SwaggerResponseRemoveDefaults]
    public class FileAccessController : Controller
    {
        private readonly IFileAccessService fileService;
        private readonly IFileAccessMapper mapper;

        public FileAccessController(IFileAccessService fileService, IFileAccessMapper mapper)
        {
            this.fileService = fileService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Get a write access token for a new facility image
        /// </summary>
        /// <param name="id">Facility ID</param>
        /// <param name="originalFileName">Image file name</param>
        /// <returns></returns>
        [HttpGet("/api/facilities/{id}/image-token")]
        [SwaggerOperation("get-facility-image-upload-token")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, Type = typeof(FileAccessTokenRepresentation))]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, Description = "Invalid parameters")]
        [SwaggerResponse(System.Net.HttpStatusCode.NotFound, Description = "Facility not found")]
        public async Task<IActionResult> GetFacilityImageUploadToken(Guid id, [FromQuery]string originalFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(originalFileName))
                    throw new ArgumentNullException(nameof(originalFileName));

                var token = await fileService.IssueFacilityImageUploadTokenAsync(id, originalFileName);
                return Ok(mapper.MapFrom(token));
            }
            catch(ArgumentNullException)
            {
                return BadRequest();
            }
            catch (FacilityNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Get a write access token for a new accommodation image
        /// </summary>
        /// <param name="id">Accommodation ID</param>
        /// <param name="originalFileName">Image file name</param>
        /// <returns></returns>
        [HttpGet("/api/accommodations/{id}/image-token")]
        [SwaggerOperation("get-accommodation-image-upload-token")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, Type = typeof(FileAccessTokenRepresentation))]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, Description = "Invalid parameters")]
        [SwaggerResponse(System.Net.HttpStatusCode.NotFound, Description = "Accommodation not found")]
        public async Task<IActionResult> GetAccommodationImageUploadToken(Guid id, [FromQuery]string originalFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(originalFileName))
                    throw new ArgumentNullException(nameof(originalFileName));

                var token = await fileService.IssueAccommodationImageUploadTokenAsync(id, originalFileName);
                return Ok(mapper.MapFrom(token));
            }
            catch (ArgumentNullException)
            {
                return BadRequest();
            }
            catch (AccommodationNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
