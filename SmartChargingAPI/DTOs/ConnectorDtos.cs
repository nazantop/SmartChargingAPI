namespace SmartChargingAPI.DTOs
{
    public class ConnectorRequestDto
    {
        public int MaxCurrentAmps { get; set; }
    }

    public class ConnectorResponseDto
    {
        public int Id { get; set; }
        public int MaxCurrentAmps { get; set; }
    }
}