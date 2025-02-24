using AutoMapper;
using Models.Request.Create;
using Models.Request.Update;
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
            CreateMap<UserRegistration, User>();
            CreateMap<UserUpdateRequest, User>();
        }
    }
}
