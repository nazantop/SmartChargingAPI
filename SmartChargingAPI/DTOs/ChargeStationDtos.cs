namespace SmartChargingAPI.DTOs
{
    public class ChargeStationRequestDto
    {
        public string Name { get; set; }

        public List<ConnectorRequestDto>? Connectors { get; set; }
    }

    public class ChargeStationResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int CapacityInAmps { get; set; }

        public List<ConnectorResponseDto> Connectors { get; set; }
    }

    public class ChargeStationUpdateRequestDto
    {
        public string Name { get; set; }

    }
}