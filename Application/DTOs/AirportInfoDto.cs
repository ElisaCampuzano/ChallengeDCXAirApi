﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AirportInfoDto
    {
        public string? City { get; set; }
        public string? IataCode { get; set; }
        public string? AirportName { get; set; }
        public string? Country { get; set; }
        public string? Region { get; set; }
    }
}
