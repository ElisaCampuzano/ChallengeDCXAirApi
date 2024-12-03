using CsvHelper.Configuration;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapper
{
    public class AirportRecordMap : ClassMap<AirportRecord>
    {
        public AirportRecordMap()
        {
            Map(m => m.id).Index(0);
            Map(m => m.ident)?.Index(1);
            Map(m => m.type)?.Index(2);
            Map(m => m.name)?.Index(3);
            Map(m => m.latitude_deg).Index(4);
            Map(m => m.longitude_deg).Index(5);
            Map(m => m.elevation_ft).Index(6).TypeConverterOption.NullValues("");
            Map(m => m.continent)?.Index(7);
            Map(m => m.iso_country).Index(8);
            Map(m => m.iso_region).Index(9);
            Map(m => m.municipality).Index(10);
            Map(m => m.scheduled_service).Index(11);
            Map(m => m.gps_code).Index(12);
            Map(m => m.iata_code).Index(13);
            Map(m => m.local_code).Index(14);
            Map(m => m.home_link).Index(15);
            Map(m => m.wikipedia_link).Index(16);
            Map(m => m.keywords).Index(17);
        }
    }
}
