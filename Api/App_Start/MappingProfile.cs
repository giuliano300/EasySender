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

            Mapper.CreateMap<NamesDtos, NameInsertDto>();
            Mapper.CreateMap<NameInsertDto, NamesDtos>();

            Mapper.CreateMap<NamesTelegramDto, NamesDto>();
            Mapper.CreateMap<NamesDto, NamesTelegramDto>();

            Mapper.CreateMap<NamesDtos, NamesTelegramDto>();
            Mapper.CreateMap<NamesTelegramDto, NamesDtos>();

            Mapper.CreateMap<SenderDto, Senders>();
            Mapper.CreateMap<Senders, SenderDto>();

            Mapper.CreateMap<SenderDtos, Senders>();
            Mapper.CreateMap<Senders, SenderDtos>();

            Mapper.CreateMap<SenderDtos, SenderInsertDto>();
            Mapper.CreateMap<SenderInsertDto, SenderDtos>();

            Mapper.CreateMap<SenderDto, SenderDtos>();
            Mapper.CreateMap<SenderDtos, SenderDto>();

            Mapper.CreateMap<NamesDmDto, NamesDm>();
            Mapper.CreateMap<NamesDm, NamesDmDto>();

            Mapper.CreateMap<DmDto, Dm>();
            Mapper.CreateMap<Dm, DmDto>();

            Mapper.CreateMap<ListsDto, Lists>();
            Mapper.CreateMap<Lists, ListsDto>();

            Mapper.CreateMap<NamesListsDto, NamesLists>();
            Mapper.CreateMap<NamesLists, NamesListsDto>();

            Mapper.CreateMap<NamesDm, NamesLists>();
            Mapper.CreateMap<NamesLists, NamesDm>();

            Mapper.CreateMap<NamesDmDto, NamesDm>();
            Mapper.CreateMap<NamesDm, NamesDmDto>();

            Mapper.CreateMap<DmConfigurationsDto, DmConfigurations>();
            Mapper.CreateMap<DmConfigurations, DmConfigurationsDto>();

            Mapper.CreateMap<UsersDto, Users>();
            Mapper.CreateMap<Users, UsersDto>();

            Mapper.CreateMap<DmProductsDto, DmProducts>();
            Mapper.CreateMap<DmProducts, DmProductsDto>();

            Mapper.CreateMap<SmsUsersDto, SmsUsers>();
            Mapper.CreateMap<SmsUsers, SmsUsersDto>();

            Mapper.CreateMap<SmsListsDto, SmsLists>();
            Mapper.CreateMap<SmsLists, SmsListsDto>();

            Mapper.CreateMap<SendersUsersDto, SendersUsers>();
            Mapper.CreateMap<SendersUsers, SendersUsersDto>();

            Mapper.CreateMap<BulletinsDtos, Bulletins>();
            Mapper.CreateMap<Bulletins, BulletinsDtos>();

            Mapper.CreateMap<BulletinsDtos, BulletinsDto>();
            Mapper.CreateMap<BulletinsDto, BulletinsDtos>();

            Mapper.CreateMap<BulletinsDto, Bulletins>();
            Mapper.CreateMap<Bulletins, BulletinsDto>();

            Mapper.CreateMap<BulletinsDtos, BulletinsApiDto>();
            Mapper.CreateMap<BulletinsApiDto, BulletinsDtos>();

            Mapper.CreateMap<BulletinsApiDto, BulletinsDto>();
            Mapper.CreateMap<BulletinsDto, BulletinsApiDto>();

            Mapper.CreateMap<BulletinsApiDto, Bulletins>();
            Mapper.CreateMap<Bulletins, BulletinsApiDto>();

            Mapper.CreateMap<NotificationsDto, Notifications>();
            Mapper.CreateMap<Notifications, NotificationsDto>();

            Mapper.CreateMap<TemporaryValidateTableDto, TemporaryValidateTable>();
            Mapper.CreateMap<TemporaryValidateTable, TemporaryValidateTableDto>();

        }
    }
}