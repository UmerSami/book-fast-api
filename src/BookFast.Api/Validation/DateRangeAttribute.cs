﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BookFast.Api.Validation
{
    public class DateRangeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var booking = value as CreateBookingViewModel;
            var now = DateTime.Now.Date;
            if (booking == null || booking.FromDate.Date < now || booking.ToDate.Date < now)
                return true;

            return booking.ToDate >= booking.FromDate;
        }
    }
}