using AutoMapper;
using SmartChargingAPI.DTOs;
using SmartChargingAPI.Models;

namespace SmartChargingAPI.Helpers.Mappers;
    public class CommonProfile : Profile
    {
        public CommonProfile()
        {
            CreateMap<Group, GroupResponseDto>()
                .ForMember(dest => dest.ChargeStations, opt => opt.MapFrom(src => src.ChargeStations));

            CreateMap<GroupRequestDto, Group>();

            CreateMap<ChargeStation, ChargeStationResponseDto>()
                .ForMember(dest => dest.Connectors, opt => opt.MapFrom(src => src.Connectors));

            CreateMap<ChargeStationRequestDto, ChargeStation>();
            
            CreateMap<Connector, ConnectorResponseDto>();

            CreateMap<ConnectorRequestDto, Connector>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }