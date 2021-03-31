using AutoMapper;
using powerful_crm.API.Models.InputModels;
using powerful_crm.API.Models.OutputModels;
using powerful_crm.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

using System.Linq;
using System.Threading.Tasks;

namespace EducationSystem.API
{
    public class AutomapperConfig : Profile
    {
        private const string _dateFormat = "dd.MM.yyyy";
        public AutomapperConfig()
        {
            CreateMap<LeadDto, LeadOutputModel>()
                .ForMember(dest => dest.BirthDate, opts => opts.MapFrom(src => src.BirthDate.ToString(_dateFormat)));
            CreateMap<LeadInputModel, LeadDto>()
                .ForMember(dest => dest.BirthDate, opts => opts.MapFrom(src => DateTime.ParseExact(src.BirthDate, _dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None)));
        }
    }
}