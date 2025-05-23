

using LaWashSmartParking.Domain.Enum;

namespace LaWashSmartParking.Domain.Entities
{
    public class ParkingSpot
    {
        public Guid Id { get; set; }
        public string Location { get; set; } = string.Empty;
        public ParkingStatus Status { get; set; } = ParkingStatus.Free;
        public Guid? AssignedDeviceId { get; set; }

        public ICollection<ParkingUsageLog> ParkingUsageLogs { get; set; } = new List<ParkingUsageLog>();
    }
}
    