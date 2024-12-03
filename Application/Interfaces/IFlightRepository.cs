using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IFlightRepository
    {
        Task<IEnumerable<Flight>> GetAllFlightsAsync();
        Task AddFlightsAsync(IEnumerable<Flight> flights);
        Task<IEnumerable<Flight>> GetFlightsByOriginAndDestinationAsync(string origin, string destination);
    }
}
