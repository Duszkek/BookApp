using AutoMapper;
using BookApp.DTO;
using BookApp.Models;

namespace BookApp.Utils;

public class DTOMapper
{
    #region Members

    private readonly IMapper Mapper;

    #endregion

    #region .Ctor

    public DTOMapper(bool checkMapping = true)
    {
        Mapper = CreateMapper(checkMapping);
    }

    #endregion

    #region Methods

    private IMapper CreateMapper(bool checkMapping)
    {
        MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<DtoBook, BookModel>()
                .ForMember(dest => dest.LocalId, opt => opt.Ignore())
                .ForMember(dest => dest.ApiId, opt => opt.MapFrom(src => src.Id)) 
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.VolumeInfo.Title))
                .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.VolumeInfo.Authors != null 
                    ? string.Join(", ", src.VolumeInfo.Authors) 
                    : "Unknown"))
                .ForMember(dest => dest.PublishedDate, opt => opt.MapFrom(src => src.VolumeInfo.PublishedDate))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.VolumeInfo.Description))
                .ForMember(dest => dest.Thumbnail, opt => opt.MapFrom(src => EnsureHttps(src.VolumeInfo.ImageLinks.Thumbnail)))
                .ForMember(dest => dest.SmallThumbnail, opt => opt.MapFrom(src => EnsureHttps(src.VolumeInfo.ImageLinks.SmallThumbnail)))
                .ForMember(dest => dest.PageCount, opt => opt.MapFrom(src => src.VolumeInfo.PageCount))
                .ForMember(dest => dest.IsSaved, opt => opt.MapFrom(src => false));
        });

        if (checkMapping)
        {
            try
            {
                mapperConfiguration.AssertConfigurationIsValid();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        return mapperConfiguration.CreateMapper();
    }

    public TResult Get<TResult, TSource>(TSource source)
        where TResult : class
        where TSource : class
    {
        try
        {
            if (source == null)
            {
                return default;
            }

            TResult result = Mapper.Map<TResult>(source);

            if (result == null)
            {
                throw new NullReferenceException(nameof(result));
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return default;
        }
    }

    public List<TResult> Get<TResult, TSource>(ICollection<TSource> source)
        where TResult : class
        where TSource : class
    {
        if (source?.Count > 0)
        {
            List<TResult> resultList = new();

            foreach (TSource item in source)
            {
                TResult result = Get<TResult, TSource>(item);

                if (result != null)
                {
                    resultList.Add(result);
                }
            }

            return resultList;
        }

        return new List<TResult>();
    }
    
    private string EnsureHttps(string url)
    {
        if (string.IsNullOrEmpty(url))
            return string.Empty;

        return url.StartsWith("http://") ? url.Replace("http://", "https://") : url;
    }

    #endregion
}
