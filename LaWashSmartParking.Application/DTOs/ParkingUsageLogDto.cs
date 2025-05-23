public class ParkingUsageLogDto
{
    public Guid Id { get; set; }
    public Guid ParkingSpotId { get; set; }
    public Guid IoTDeviceId { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime OccupiedAt { get; set; }
    public DateTime? FreedAt { get; set; }
    public TimeSpan? Duration { get; set; }
}