using Swashbuckle.AspNetCore.Annotations;

namespace SmartChargingAPI.DTOs
{
    [SwaggerSchema("Represents the response details of a group.")]
    public class GroupResponseDto
    {
        [SwaggerSchema("The unique identifier of the group.", ReadOnly = true)]
        public string Id { get; set; }

        [SwaggerSchema("The name of the group.")]
        public string Name { get; set; }

        [SwaggerSchema("The maximum capacity of the group in amps.")]
        public int CapacityAmps { get; set; }

        [SwaggerSchema("The sum of the capacity of the charge stations in amps.")]
        public int CapacityInAmps { get; set; }

        public List<ChargeStationResponseDto> ChargeStations { get; set; }
    }

    [SwaggerSchema(Required = ["name", "capacityAmps"])]
    public class GroupRequestDto
    {
        [SwaggerSchema("The name of the group.")]
        public string Name { get; set; }

        [SwaggerSchema("The maximum capacity of the group in amps.")]
        public int CapacityAmps { get; set; }

    }
}