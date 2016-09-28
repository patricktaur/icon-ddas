using AutoMapper;
using DDAS.Models.Entities.Domain;
using DDAS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDAS.API.App_Start
{
    public class AutoMapperProfileConfiguration : Profile
    {
        [Obsolete("This class is obsolete;")]
        protected override void Configure()
        {
            //CreateMap<ArtistViewModelRequest, Artist>().ReverseMap();
            //CreateMap<ArtistViewModelRequest, Artist>();
           
        }
    }
}