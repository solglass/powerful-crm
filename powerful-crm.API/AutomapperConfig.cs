using AutoMapper;
using powerful_crm.API.Models.InputModels;
using powerful_crm.API.Models.OutputModels;
using powerful_crm.Core;
using powerful_crm.Core.Models;
using System;
using System.Globalization;



namespace EducationSystem.API
{
    public class AutomapperConfig : Profile
    {
        public AutomapperConfig()
        {
            CreateMap<LeadDto, LeadOutputModel>()
                .ForMember(dest => dest.BirthDate, opts => opts.MapFrom(src => src.BirthDate.ToString(Constants.DATE_FORMAT)));
            CreateMap<LeadInputModel, LeadDto>()
                .ForMember(dest => dest.City, opts => opts.MapFrom(src => new CityDto { Id = src.CityId }))
                .ForMember(dest => dest.BirthDate, opts => opts.MapFrom(src => DateTime.ParseExact(src.BirthDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None)));
            CreateMap<CityDto, CityOutputModel>();
            CreateMap<CityInputModel, CityDto>();
            CreateMap<UpdateLeadInputModel, LeadDto>()
                .ForMember(dest => dest.BirthDate, opts => opts.MapFrom(src => DateTime.ParseExact(src.BirthDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None)))
             .ForMember(dest => dest.City, opts => opts.MapFrom(src => new CityDto { Id = src.CityId }));

            CreateMap<BalanceInputModel, BalanceOutputModel>();
            CreateMap<TransactionInputModel, TransactionOutputModel>()
                .ForMember(dest => dest.Timestamp, opts => opts.MapFrom(src => src.Timestamp.ToString(Constants.DATE_FORMAT_WITH_TIME)));
            CreateMap<TransferInputModel, TransferOutputModel>()
                .ForMember(dest => dest.Timestamp, opts => opts.MapFrom(src => src.Timestamp.ToString(Constants.DATE_FORMAT_WITH_TIME)));
        }
    }
}