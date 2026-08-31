using AutoMapper;
using Music.bisLog.Dtos;
using Music.DataAccess.Models;
using Music.DataAccess.Utils;

namespace Music.bisLog;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Song, SongDto>()
            .ForMember(d => d.Authors, opt => opt.MapFrom(s =>
                string.Join(", ", s.Authors.Select(a => a.Name))))
            .ForMember(d => d.Genres, opt => opt.MapFrom(s =>
                string.Join(", ", s.Genres.Select(g => g.Name))))
            .ForMember(d => d.UploadDate, opt => opt.MapFrom(s => s.CreatedAt))
            .ForMember(d => d.UploadedBy, opt => opt.MapFrom(s => s.User != null ? s.User.Username : ""));

        CreateMap<Song, SongDetailDto>()
            .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.User != null ? s.User.Id : 0))
            .ForMember(d => d.Authors, opt => opt.MapFrom(s =>
                s.Authors.Select(a => new AuthorDto { Id = a.Id, Name = a.Name, Country = a.Country, Description = a.Description }).ToList()))
            .ForMember(d => d.Genres, opt => opt.MapFrom(s =>
                s.Genres.Select(g => new GenreDto { Id = g.Id, Name = g.Name, Description = g.Description }).ToList()))
            .ForMember(d => d.AuthorNames, opt => opt.Ignore())
            .ForMember(d => d.GenreNames, opt => opt.Ignore())
            .ForMember(d => d.DurationFormatted, opt => opt.MapFrom(s =>
                s.Duration > 0 ? $"{s.Duration / 60}:{s.Duration % 60:D2}" : ""))
            .ForMember(d => d.UploadDate, opt => opt.MapFrom(s => s.CreatedAt));

        CreateMap<User, UserDto>()
            .ForMember(d => d.Role, opt => opt.MapFrom(u =>
                u.Roles.Select(r => r.Name).FirstOrDefault() ?? RoleNames.User));

        CreateMap<Genre, GenreDto>();

        CreateMap<Author, AuthorDto>();
    }
}