using LaWashSmartParking.Domain.Entities;
using System;
using Xunit;

namespace LaWashSmartParking.Domain.Tests.Entities
{
    public class ParkingUsageLogTests
    {
        [Fact]
        public void Duration_ShouldBeNull_WhenFreedAtIsNull()
        {
            var log = new ParkingUsageLog
            {
                OccupiedAt = DateTime.UtcNow
                // FreedAt = null by default
            };

            Assert.Null(log.Duration);
        }

        [Fact]
        public void Duration_ShouldCalculateDifference_WhenFreedAtIsSet()
        {
            var occupiedAt = new DateTime(2023, 1, 1, 12, 0, 0);
            var freedAt = new DateTime(2023, 1, 1, 13, 30, 0);

            var log = new ParkingUsageLog
            {
                OccupiedAt = occupiedAt,
                FreedAt = freedAt
            };

            var expected = TimeSpan.FromMinutes(90);

            Assert.Equal(expected, log.Duration);
        }
    }
}
