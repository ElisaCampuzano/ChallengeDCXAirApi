using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChallengeDCXAirApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DCXAirController : ControllerBase
    {
        private readonly IFlightRepository _flightRepository;

        public DCXAirController(IFlightRepository flightRepository)
        {
            _flightRepository = flightRepository;
        }

        /// <summary>
        /// Retrieves all available flights.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a list of all flights.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var flights = await _flightRepository.GetAllFlightsAsync();
            return Ok(flights);
        }

        
    }
}
