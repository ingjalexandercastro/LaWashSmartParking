using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaWashSmartParking.Application.DTOs
{
    public class CreateParkingSpotDto
    {
        public string Location { get; set; } = string.Empty;
        public string SerialNumber { get; set; }
    }
}
