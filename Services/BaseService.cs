using AutoMapper;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Settings;

namespace Services
{
    public class BaseService
    {
        protected readonly Models.AppContext _context;
        protected readonly IMapper _mapper;
        protected readonly IConfiguration _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        protected readonly IOptions<MailSetting> _mailSetting;
        protected readonly IOptions<JWTSetting> _jwtSetting;
        protected readonly AppExtension _appExtension;
        public BaseService(Models.AppContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _mailSetting = Options.Create(_configuration.GetSection("MailSettings").Get<MailSetting>());
            _jwtSetting = Options.Create(_configuration.GetSection("AppSettings").Get<JWTSetting>());
            _appExtension = new AppExtension(_configuration, _mailSetting);
        }
    }
}
