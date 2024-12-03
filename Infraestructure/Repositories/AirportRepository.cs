using Application.Interfaces;
using CsvHelper.Configuration;
using CsvHelper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AirportRepository : IAirportRepository
    {
        private readonly string _csvFilePath;

        public AirportRepository()
        {
            _csvFilePath = "airports.csv"; 
        }

        public Task<IEnumerable<AirportRecord>> GetAirportsFromCsvAsync()
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
