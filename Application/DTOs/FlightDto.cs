using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class FlightDto
    {
        public string? Id { get; set; }
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public double Price { get; set; }
        public TransportDto? Transport { get; set; }
        public AirportInfoDto? OriginInfo { get; set; } 
        public AirportInfoDto? DestinationInfo { get; set; }
    }
}
