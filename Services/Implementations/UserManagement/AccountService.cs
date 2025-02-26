﻿using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.Request.Create;
using Services.Interfaces.UserManagement;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations.UserManagement
{
    public class AccountService : BaseService, IAccountService
    {
        public AccountService(Models.AppContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public async Task<Result<string>> Login(LoginRequest loginRequest, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginRequest.Username, cancellationToken);
            if (user == null)
            {
                return Result.Fail("User is not existed");
            }
            if (user.IsRestricted)
            {
                return Result.Fail($"User is restricted to {user.RestrictedExpiredAt}");
            }
            if (user.PasswordHash != _appExtension.CreateHashPassword(loginRequest.Password))
            {
                return Result.Fail("Password is incorrect");
            }
            var token = await GenerateJWTKey(user);
            return Result.Ok(token.Value);
        }
        private async Task<Result<string>> GenerateJWTKey(User user)
        {
            if(user == null)
            {
                return Result.Fail<string>("User is not existed");
            }
            if(user.IsRestricted)
            {
                return Result.Fail<string>($"User is restricted to {user.RestrictedExpiredAt}");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSetting.Value.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("ID", user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string Token = tokenHandler.WriteToken(token);
            await _context.SaveChangesAsync();
            return Result.Ok(Token);
        }
        private async Task<Result<Guid?>> ExtractJWTKeyToGetId(string token)
        {
            if(string.IsNullOrEmpty(token))
            {
                return Result.Fail<Guid?>("Token is empty");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            if(!tokenHandler.CanReadToken(token))
            {
                return Result.Fail<Guid?>("Token is invalid");
            }
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var id = jwtToken.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            if (string.IsNullOrEmpty(id))
            {
                return Result.Fail<Guid?>("Token is invalid");
            }
            return Result.Ok((Guid?)Guid.Parse(id));
        }
    }
}
