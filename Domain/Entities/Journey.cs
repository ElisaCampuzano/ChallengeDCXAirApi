using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Journey
    {
        public List<Flight> Flights { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public double Price { get; set; }
    }
}
