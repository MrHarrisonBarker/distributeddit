using AutoMapper;
using BuildingBlocks.Models;

namespace UserService.Maps
{
    public class UserMaps : Profile
    {
        public UserMaps()
        {
            CreateMap<Post, PostViewModel>();
        }
    }
}