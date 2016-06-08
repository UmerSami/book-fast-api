﻿using System.Threading.Tasks;
using BookFast.Contracts.Framework;
using BookFast.Data.Models;
using Accommodation = BookFast.Contracts.Models.Accommodation;
using Microsoft.EntityFrameworkCore;

namespace BookFast.Data.Commands
{
    internal class UpdateAccommodationCommand : ICommand<BookFastContext>
    {
        private readonly Accommodation accommodation;
        private readonly IAccommodationMapper mapper;

        public UpdateAccommodationCommand(Accommodation accommodation, IAccommodationMapper mapper)
        {
            this.accommodation = accommodation;
            this.mapper = mapper;
        }

        public async Task ApplyAsync(BookFastContext model)
        {
            var existingAccommodation = await model.Accommodations.FirstOrDefaultAsync(a => a.Id == accommodation.Id);
            if (existingAccommodation != null)
            {
                var dm = mapper.MapFrom(accommodation);

                existingAccommodation.Name = dm.Name;
                existingAccommodation.Description = dm.Description;
                existingAccommodation.RoomCount = dm.RoomCount;
                existingAccommodation.Images = dm.Images;

                await model.SaveChangesAsync();
            }
        }
    }
}