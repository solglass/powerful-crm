using AutoMapper;
using powerful_crm.API.Models.InputModels;
using powerful_crm.API.Models.MiddleModels;
using powerful_crm.API.Models.OutputModels;
using powerful_crm.Core;
using powerful_crm.Core.Enums;
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
                .ForMember(dest => dest.BirthDate, opts => opts.MapFrom(src => DateTime.ParseExact(src.BirthDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None)))
                .ForMember(dest=> dest.Role, opts=>opts.MapFrom(src=> Role.Client));

            CreateMap<CityDto, CityOutputModel>();
            CreateMap<CityInputModel, CityDto>();
            CreateMap<UpdateLeadInputModel, LeadDto>()
                .ForMember(dest => dest.BirthDate, opts => opts.MapFrom(src => DateTime.ParseExact(src.BirthDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None)))
             .ForMember(dest => dest.City, opts => opts.MapFrom(src => new CityDto { Id = src.CityId }));

            CreateMap<BalanceInputModel, BalanceOutputModel>();
            CreateMap<SearchLeadInputModel, SearchLeadDto>()
            .ForMember(dest => dest.StartBirthDate, opts => opts.MapFrom(src => DateTime.ParseExact(src.StartBirthDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None)))
            .ForMember(dest => dest.EndBirthDate, opts => opts.MapFrom(src => DateTime.ParseExact(src.EndBirthDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None)))
         .ForMember(dest => dest.City, opts => opts.MapFrom(src=> new CityDto() {Name = src.CityName }));
            
            CreateMap<TransactionInputModel, TransactionMiddleModel>()
                .ForMember(dest => dest.CurrencyPair, opt => opt.MapFrom(c => c.CurrentCurrency + c.AccountCurrency));
            CreateMap<TransferInputModel, TransferMiddleModel>()
                .ForMember(dest => dest.CurrencyPair, opt => opt.MapFrom(c => c.CurrentCurrency + c.AccountCurrency));
        }
    }
}