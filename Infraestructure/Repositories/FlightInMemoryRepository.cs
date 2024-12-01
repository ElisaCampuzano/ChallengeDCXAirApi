using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class FlightInMemoryRepository : IFlightRepository
    {
        private readonly ConcurrentBag<Flight> _flights = new();

        public Task<IEnumerable<Flight>> GetAllFlightsAsync()
        {
            return Task.FromResult(_flights.AsEnumerable());
        }

        public Task AddFlightsAsync(IEnumerable<Flight> flights)
        {
            foreach (var flight in flights)
            {
                _flights.Add(flight);
            }
            return Task.CompletedTask;
        }
    }
}
