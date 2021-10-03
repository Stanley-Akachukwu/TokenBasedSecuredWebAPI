using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using UCare.Models.Entities;
using UCare.Models.UserViewModels;

namespace UCare.Models.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserViewModel>().ReverseMap();
            //CreateMap<Talk, TalkModel>()
            //    .ReverseMap()
            //    .ForMember(t => t.Camp, opt => opt.Ignore())
            //    .ForMember(t => t.Speaker, opt => opt.Ignore());
            //CreateMap<Speaker, SpeakerModel>()
            //    .ReverseMap();
        }
    }
}
