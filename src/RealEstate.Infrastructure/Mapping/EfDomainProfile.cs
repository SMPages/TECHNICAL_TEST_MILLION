using System; // por DateTime?
using AutoMapper;

// EF
using EProperty = RealEstate.Infrastructure.Persistence.EF.Models.Property;
using EPropertyImage = RealEstate.Infrastructure.Persistence.EF.Models.PropertyImage;
using EPropertyTrace = RealEstate.Infrastructure.Persistence.EF.Models.PropertyTrace;

// Dominio
using DProperty = RealEstate.Domain.Entities.Property;
using DPropertyImage = RealEstate.Domain.Entities.PropertyImage;
using DPropertyTrace = RealEstate.Domain.Entities.PropertyTrace;

namespace RealEstate.Infrastructure.Mapping;

public class EfDomainProfile : Profile
{
    public EfDomainProfile()
    {
        // EF -> Dominio (Property)
        CreateMap<EProperty, DProperty>()
            .ConstructUsing(src => new DProperty(
                src.CodeInternal!,   // codeInternal
                src.Name!,           // name
                src.Address!,        // address
                src.IdOwner,         // idOwner
                src.Price            // price
            ))
            .AfterMap((src, dst) =>
            {
                dst.UpdateBasicInfo(
                    src.Name!,
                    src.Address!,
                    src.City,
                    src.Year.HasValue ? (short?)src.Year.Value : null,
                    src.Bedrooms.HasValue ? (byte?)src.Bedrooms.Value : null,
                    src.Bathrooms.HasValue ? (byte?)src.Bathrooms.Value : null,
                    src.AreaSqFt.HasValue ? (double?)src.AreaSqFt.Value : null
                );
            })
            // Navegaciones del dominio que NO queremos que AutoMapper valide
            .ForMember(d => d.Owner, o => o.Ignore())
            .ForMember(d => d.Images, o => o.Ignore())
            .ForMember(d => d.Traces, o => o.Ignore());

        // Dominio -> EF (Property)
        CreateMap<DProperty, EProperty>()
            .ForMember(d => d.IdProperty, o => o.Ignore())
            .ForMember(d => d.IdOwnerNavigation, o => o.Ignore())
            .ForMember(d => d.PropertyImages, o => o.Ignore())
            .ForMember(d => d.PropertyTraces, o => o.Ignore());

        // EF -> Dominio (PropertyImage)
        CreateMap<EPropertyImage, DPropertyImage>()
            .ConstructUsing(src => new DPropertyImage(
                src.IdProperty,
                src.FileUrl!,
                src.IsMain,
                src.Caption,
                src.SortOrder
            ))
            .ForMember(d => d.Property, o => o.Ignore()); // navegación dominio

        // Dominio -> EF (PropertyImage)
        CreateMap<DPropertyImage, EPropertyImage>()
            .ForMember(d => d.IdPropertyImage, o => o.Ignore())
            .ForMember(d => d.IdPropertyNavigation, o => o.Ignore());

        // EF -> Dominio (PropertyTrace)
        CreateMap<EPropertyTrace, DPropertyTrace>()
            .ConstructUsing(src => new DPropertyTrace(
                src.IdProperty,
                src.Name!,
                src.Value,
                src.Tax,
                (DateTime?)null // si tienes DateSale en EF cámbialo aquí
                                // ej.: src.DateSale.HasValue ? src.DateSale.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null
            ))
            .ForMember(d => d.Property, o => o.Ignore())
            .ForMember(d => d.DateSale, o => o.Ignore()); // evita validación extra

        // Dominio -> EF (PropertyTrace)
        CreateMap<DPropertyTrace, EPropertyTrace>()
            .ForMember(d => d.IdPropertyTrace, o => o.Ignore())
            .ForMember(d => d.IdPropertyNavigation, o => o.Ignore())
            .ForMember(d => d.DateSale, o => o.Ignore()); // si EF tiene DateSale y no quieres mapearlo
    }
}
