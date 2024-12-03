using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAirportRepository
    {
        Task<IEnumerable<AirportRecord>> GetAirportsFromCsvAsync();

        public async Task<AirportInfoDto> GetAirportInfoByIataCode(string iataCode)
        {
            var airports = await GetAirportsFromCsvAsync();
            var airport = airports.FirstOrDefault(a => a.iata_code == iataCode);
            if (airport == null) return null;

            return new AirportInfoDto
            {
                City = airport.municipality,
                IataCode = airport.iata_code,
                AirportName = airport.name,
                Country = airport.iso_country,
                Region = airport.iso_region
            };
        }
    }
}
