using Api.Dtos;
using Api.Models;
using AutoMapper;

namespace Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Mapper.CreateMap<OperationsDto, Operations>();
            Mapper.CreateMap<Operations, OperationsDto>();

            Mapper.CreateMap<NamesDto, Names>();
            Mapper.CreateMap<Names, NamesDto>();

            Mapper.CreateMap<NamesTelegramDto, Names>();
            Mapper.CreateMap<Names, NamesTelegramDto>();

            Mapper.CreateMap<NamesDtos, Names>();
            Mapper.CreateMap<Names, NamesDtos>();


            Mapper.CreateMap<NamesDtos, NamesDto>();
            Mapper.CreateMap<NamesDto, NamesDtos>();

            Mapper.CreateMap<NamesTelegramDto, NamesDto>();
            Mapper.CreateMap<NamesDto, NamesTelegramDto>();

            Mapper.CreateMap<NamesDtos, NamesTelegramDto>();
            Mapper.CreateMap<NamesTelegramDto, NamesDtos>();

            Mapper.CreateMap<SenderDto, Senders>();
            Mapper.CreateMap<Senders, SenderDto>();

            Mapper.CreateMap<SenderDtos, Senders>();
            Mapper.CreateMap<Senders, SenderDtos>();

            Mapper.CreateMap<SenderDtos, SendersUsers>();
            Mapper.CreateMap<SendersUsers, SenderDtos>();

            Mapper.CreateMap<SenderDto, SenderDtos>();
            Mapper.CreateMap<SenderDtos, SenderDto>();

            Mapper.CreateMap<ListsDto, Lists>();
            Mapper.CreateMap<Lists, ListsDto>();

            Mapper.CreateMap<NamesListsDto, NamesLists>();
            Mapper.CreateMap<NamesLists, NamesListsDto>();


            Mapper.CreateMap<UsersDto, Users>();
            Mapper.CreateMap<Users, UsersDto>();

            Mapper.CreateMap<SendersUsersDto, SendersUsers>();
            Mapper.CreateMap<SendersUsers, SendersUsersDto>();

            Mapper.CreateMap<BulletinsDtos, Bulletins>();
            Mapper.CreateMap<Bulletins, BulletinsDtos>();

            Mapper.CreateMap<BulletinsDtos, BulletinsDto>();
            Mapper.CreateMap<BulletinsDto, BulletinsDtos>();

            Mapper.CreateMap<BulletinsDto, Bulletins>();
            Mapper.CreateMap<Bulletins, BulletinsDto>();

            Mapper.CreateMap<NotificationsDto, Notifications>();
            Mapper.CreateMap<Notifications, NotificationsDto>();

            Mapper.CreateMap<TemporaryValidateTableDto, TemporaryValidateTable>();
            Mapper.CreateMap<TemporaryValidateTable, TemporaryValidateTableDto>();

            Mapper.CreateMap<UserPermitsDto, UserPermits>();
            Mapper.CreateMap<UserPermits, UserPermitsDto>();

            Mapper.CreateMap<VisureDocumentType, VisureDocumentTypeDto>();
            Mapper.CreateMap<VisureDocumentTypeDto, VisureDocumentType>();

            Mapper.CreateMap<GEDDto, GED>();
            Mapper.CreateMap<GEDDto, GED>();

            Mapper.CreateMap<PropertyDto, Property>();
            Mapper.CreateMap<Property, PropertyDto>();

            Mapper.CreateMap<LoghiDto, Loghi>();
            Mapper.CreateMap<Loghi, LoghiDto>();

            Mapper.CreateMap<Logs, LogsDto>();
            Mapper.CreateMap<LogsDto, Logs>();

        }
    }
}