using AutoMapper;
using Models.Request.Create;
using Models.Request.Update;
using Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<UserRegistration, User>()
                .ForMember(destinationMember => destinationMember.UserId, memberOptions => memberOptions.Ignore())
                .ForMember(destinationMember => destinationMember.PasswordHash, memberOptions => memberOptions.Ignore())
                .ForMember(destinationMember => destinationMember.CreatedAt, memberOptions => memberOptions.Ignore())
                .ForMember(destinationMember => destinationMember.IsRestricted, memberOptions => memberOptions.Ignore())
                .ForMember(destinationMember => destinationMember.RestrictedExpiredAt, memberOptions => memberOptions.Ignore());
            CreateMap<UserUpdateRequest, User>();
            CreateMap<User, UserResponse>();
        }
    }
}
