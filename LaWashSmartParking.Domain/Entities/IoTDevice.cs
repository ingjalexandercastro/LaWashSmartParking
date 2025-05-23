

namespace LaWashSmartParking.Domain.Entities
{
    public class IoTDevice
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        public ICollection<ParkingUsageLog> ParkingUsageLogs { get; set; } = new List<ParkingUsageLog>();
    }
}
