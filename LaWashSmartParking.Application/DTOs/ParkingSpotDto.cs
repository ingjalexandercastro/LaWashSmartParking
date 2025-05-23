using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaWashSmartParking.Domain.Enum;

namespace LaWashSmartParking.Application.DTOs
{
    public class ParkingSpotDto
    {
        public Guid Id { get; set; }
        public string Location { get; set; } = string.Empty;
        public ParkingStatus Status { get; set; } = ParkingStatus.Free;
        public Guid? AssignedDeviceId { get; set; }
        public string StatusString => Status == ParkingStatus.Free ? "free" : "occupied";
    }
}
