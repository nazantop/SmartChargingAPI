using Swashbuckle.AspNetCore.Annotations;

namespace SmartChargingAPI.DTOs
{
    public class GroupResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int CapacityAmps { get; set; }
        public int CapacityInAmps { get; set; }

        public List<ChargeStationResponseDto> ChargeStations { get; set; }
    }
    public class GroupRequestDto
    {
        public string? Name { get; set; }
        public int? CapacityAmps { get; set; }

    }
}