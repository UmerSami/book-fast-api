﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BookFast.Api.Models.Representations
{
    public class BookingRepresentation
    {
        /// <summary>
        /// Booking ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Accommodation ID
        /// </summary>
        public Guid AccommodationId { get; set; }

        /// <summary>
        /// Accommodation name
        /// </summary>
        public string AccommodationName { get; set; }

        /// <summary>
        /// Facility ID
        /// </summary>
        public Guid FacilityId { get; set; }

        /// <summary>
        /// Facility name
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// Facility street address
        /// </summary>
        public string StreetAddress { get; set; }

        /// <summary>
        /// Booking start date
        /// </summary>
        public DateTimeOffset FromDate { get; set; }

        /// <summary>
        /// Booking end date
        /// </summary>
        public DateTimeOffset ToDate { get; set; }
    }
}