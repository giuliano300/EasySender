using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WasySender_2_0.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        Mapper.CreateMap<Recipient, Names>();
        Mapper.CreateMap<Names, Recipient>();

        Mapper.CreateMap<Recipient, NamesTelegram>();
        Mapper.CreateMap<NamesTelegram, Recipient>();
    }
}
