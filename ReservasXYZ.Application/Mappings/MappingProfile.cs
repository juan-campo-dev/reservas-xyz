using AutoMapper;
using ReservasXYZ.Application.DTOs;
using ReservasXYZ.Domain.Entities;

namespace ReservasXYZ.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Site, SiteDto>()
            .ForMember(d => d.AccommodationCount, o => o.MapFrom(s => s.Accommodations.Count));
        CreateMap<CreateSiteDto, Site>();
        CreateMap<UpdateSiteDto, Site>();

        CreateMap<Accommodation, AccommodationDto>()
            .ForMember(d => d.SiteName, o => o.MapFrom(s => s.Site != null ? s.Site.Name : string.Empty));
        CreateMap<CreateAccommodationDto, Accommodation>();
        CreateMap<UpdateAccommodationDto, Accommodation>();

        CreateMap<Room, RoomDto>()
            .ForMember(d => d.AccommodationName, o => o.MapFrom(s => s.Accommodation != null ? s.Accommodation.Name : string.Empty))
            .ForMember(d => d.SiteName, o => o.MapFrom(s => s.Accommodation != null && s.Accommodation.Site != null ? s.Accommodation.Site.Name : string.Empty));
        CreateMap<CreateRoomDto, Room>();
        CreateMap<UpdateRoomDto, Room>();

        CreateMap<Season, SeasonDto>();
        CreateMap<CreateSeasonDto, Season>();
        CreateMap<UpdateSeasonDto, Season>();

        CreateMap<Rate, RateDto>()
            .ForMember(d => d.RoomNumber, o => o.MapFrom(s => s.Room != null ? s.Room.RoomNumber : string.Empty))
            .ForMember(d => d.SeasonName, o => o.MapFrom(s => s.Season != null ? s.Season.Name : string.Empty));
        CreateMap<CreateRateDto, Rate>();
        CreateMap<UpdateRateDto, Rate>();

        CreateMap<Reservation, ReservationDto>();
        CreateMap<ReservationDetail, ReservationDetailDto>()
            .ForMember(d => d.RoomNumber, o => o.MapFrom(s => s.Room != null ? s.Room.RoomNumber : string.Empty))
            .ForMember(d => d.AccommodationName, o => o.MapFrom(s => s.Room != null && s.Room.Accommodation != null ? s.Room.Accommodation.Name : string.Empty))
            .ForMember(d => d.SiteName, o => o.MapFrom(s => s.Room != null && s.Room.Accommodation != null && s.Room.Accommodation.Site != null ? s.Room.Accommodation.Site.Name : string.Empty));
    }
}
