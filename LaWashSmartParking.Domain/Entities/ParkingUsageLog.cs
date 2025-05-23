

namespace LaWashSmartParking.Domain.Entities
{
    public class ParkingUsageLog
    {
        public Guid Id { get; set; }
        public Guid ParkingSpotId { get; set; }
        public Guid IoTDeviceId { get; set; }

        public DateTime OccupiedAt { get; set; }
        public DateTime? FreedAt { get; set; }

        public TimeSpan? Duration => FreedAt.HasValue ? FreedAt.Value - OccupiedAt : null;

        public ParkingSpot ParkingSpot { get; set; } = null!;
        public IoTDevice IoTDevice { get; set; } = null!;
    }
}
