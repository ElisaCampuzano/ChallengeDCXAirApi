using Application.DTOs;
using Application.Interfaces;
using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Services
{
    public class AirportService : IAirportService
    {
        private readonly string _csvFilePath = "airports.csv";

        public async Task<AirportInfoDto> GetAirportInfoByCity(string city)
        {
            var airports = await GetAirportsFromCsvAsync();
            var airport = airports
                .Where(a => a.municipality.Equals(city, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(a.iata_code))
                .FirstOrDefault();

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

        private Task<IEnumerable<AirportRecord>> GetAirportsFromCsvAsync()
        {
            using var reader = new StreamReader(_csvFilePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HeaderValidated = null,
                MissingFieldFound = null
            });

            var records = csv.GetRecords<AirportRecord>();
            return Task.FromResult<IEnumerable<AirportRecord>>(records.ToList());
        }
    }
}
