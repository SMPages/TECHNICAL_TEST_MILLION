using AutoMapper;
// Aliases EF
using EProperty = RealEstate.Infrastructure.Persistence.EF.Models.Property;
using EPropertyImage = RealEstate.Infrastructure.Persistence.EF.Models.PropertyImage;
using EPropertyTrace = RealEstate.Infrastructure.Persistence.EF.Models.PropertyTrace;
// Aliases Dominio
using DProperty = RealEstate.Domain.Entities.Property;
using DPropertyImage = RealEstate.Domain.Entities.PropertyImage;
using DPropertyTrace = RealEstate.Domain.Entities.PropertyTrace;
namespace RealEstate.Infrastructure.Mapping;

public class EfDomainProfile : Profile
{
    public EfDomainProfile()
    {
        CreateMap<EProperty, DProperty>().ReverseMap();
        CreateMap<EPropertyImage, DPropertyImage>().ReverseMap();
        CreateMap<EPropertyTrace, DPropertyTrace>().ReverseMap();
    }
}