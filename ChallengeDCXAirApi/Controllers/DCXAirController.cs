using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace ChallengeDCXAirApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DCXAirController : ControllerBase
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IAirportService _airportService;
        private readonly IAirportRepository _airportRepository;


        public DCXAirController(IFlightRepository flightRepository, IAirportService airportService, IAirportRepository airportRepository)
        {
            _flightRepository = flightRepository;
            _airportService = airportService;
            _airportRepository = airportRepository;
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

        [HttpGet("journey")]
        public async Task<IActionResult> GetJourney(string origin, string destination, string routeType = "oneway", string currency = "USD")
        {
            if (routeType != "oneway" && routeType != "roundtrip")
            {
                return BadRequest("Invalid route type. Allowed values are 'oneway' or 'roundtrip'.");
            }

            var originIata = await _airportService.GetAirportInfoByCity(origin);
            var destinationIata = await _airportService.GetAirportInfoByCity(destination);
            if (originIata != null && destinationIata != null)
            {
                origin = originIata.IataCode;
                destination = destinationIata.IataCode;
            }
            else
            {
                return NotFound("No outbound flights found for the specified origin and destination.");
            }

            var allFlights = await _flightRepository.GetAllFlightsAsync();

            if (routeType == "oneway")
            {
                var outboundJourneys = GetMultiLegRoutes(allFlights, origin, destination);
                if (!outboundJourneys.Any())
                {
                    return NotFound("No outbound flights found for the specified origin and destination.");
                }

                if (currency != "USD") 
                {
                    outboundJourneys = await ConvertPrices((List<JourneyDto>)outboundJourneys, currency);
                }

                return Ok(outboundJourneys);
            }
            else if (routeType == "roundtrip")
            {
                var outboundJourneys = GetMultiLegRoutes(allFlights, origin, destination);
                var returnJourneys = GetMultiLegRoutes(allFlights, destination, origin);

                if (!outboundJourneys.Any() || !returnJourneys.Any())
                {
                    return NotFound("No roundtrip flights found for the specified origin and destination.");
                }

                var roundtripJourneys = new List<JourneyDto>();

                foreach (var outbound in outboundJourneys)
                {
                    foreach (var inbound in returnJourneys)
                    {
                        roundtripJourneys.Add(new JourneyDto
                        {
                            Origin = outbound.Origin,
                            Destination = outbound.Destination,
                            Price = outbound.Price + inbound.Price,
                            Currency = currency,
                            Flights = outbound.Flights.Concat(inbound.Flights).ToList()
                        });
                    }
                }

                if (currency != "USD") 
                {
                    roundtripJourneys = await ConvertPrices(roundtripJourneys, currency);
                }

                return Ok(roundtripJourneys);
            }

            return BadRequest("Unexpected route type.");
        }

        [HttpGet("{city}")]
        public async Task<IActionResult> GetIataCode(string city)
        {
            var airportInfo = await _airportService.GetAirportInfoByCity(city);
            if (airportInfo == null)
            {
                return NotFound(new { message = "Aeropuerto no encontrado o sin código IATA" });
            }
            return Ok(airportInfo);
        }

        private IEnumerable<JourneyDto> GetMultiLegRoutes(IEnumerable<Flight> allFlights, string origin, string destination)
        {
            var results = new List<JourneyDto>();

            // BFS (Búsqueda en anchura) para buscar rutas de múltiples vuelos
            var queue = new Queue<List<Flight>>();
            foreach (var flight in allFlights.Where(f => f.Origin == origin))
            {
                queue.Enqueue(new List<Flight> { flight });
            }

            while (queue.Count > 0)
            {
                var currentRoute = queue.Dequeue();
                var lastFlight = currentRoute.Last();

                if (lastFlight.Destination == destination)
                {
                    var flightsWithInfo = currentRoute.Select(f => new FlightDto
                    {
                        Id = Guid.NewGuid().ToString(),
                        Origin = f.Origin,
                        Destination = f.Destination,
                        Price = (double)f.Price,
                        Transport = new TransportDto
                        {
                            FlightCarrier = f.Transport.FlightCarrier,
                            FlightNumber = f.Transport.FlightNumber
                        },
                        OriginInfo = GetAirportInfo(f.Origin), 
                        DestinationInfo = GetAirportInfo(f.Destination) 
                    }).ToList();

                    results.Add(new JourneyDto
                    {
                        Origin = origin,
                        Destination = destination,
                        Price = currentRoute.Sum(f => f.Price),
                        Flights = flightsWithInfo
                    });
                }
                else
                {
                    foreach (var nextFlight in allFlights.Where(f => f.Origin == lastFlight.Destination &&
                                                                     !currentRoute.Any(c => c.Origin == f.Destination)))
                    {
                        var newRoute = new List<Flight>(currentRoute) { nextFlight };
                        queue.Enqueue(newRoute);
                    }
                }
            }

            return results;
        }

        private async Task<List<JourneyDto>> ConvertPrices(List<JourneyDto> journeys, string currency)
        {
            var currencyConverter = new CurrencyConverterService(new HttpClient());
            foreach (var journey in journeys)
            {
                journey.Price = await currencyConverter.ConvertCurrency("USD", currency, (double)journey.Price);
                for (int i = 0; i < journey.Flights?.Count; i++)
                {
                    FlightDto? flight = journey.Flights[i];
                    flight.Price = await currencyConverter.ConvertCurrency("USD", currency, flight.Price);
                }
            }

            return journeys;
        }

        private AirportInfoDto? GetAirportInfo(string iataCode)
        {
            var airport = _airportRepository.GetAirportInfoByIataCode(iataCode).Result;
            return airport == null ? null : new AirportInfoDto
            {
                City = airport.City,
                IataCode = airport.IataCode,
                AirportName = airport.AirportName,
                Country = airport.Country,
                Region = airport.Region
            };
        }
    }

}
