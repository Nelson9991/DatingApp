using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using API.Models.Dtos.User;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public AccountController(ApplicationDbContext context, IMapper mapper, ITokenService tokenService)
        {
            _context = context;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserReadDto>> Register(UserRegisterDto userRegisterDto)
        {
            if (await UserExsist(userRegisterDto.UserName))
            {
                return BadRequest("Username is Taken");
            }

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = userRegisterDto.UserName.ToLower(),
                Password = hmac.ComputeHash(Encoding.UTF8.GetBytes(userRegisterDto.Password)),
                PasswordSalt = hmac.Key
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserReadDto>(user);

            userDto.Token = _tokenService.CreateToken(user);

            return userDto;
        }

        private async Task<bool> UserExsist(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserReadDto>> Login(UserLoginDto userLoginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == userLoginDto.UserName);

            if (user == null)
            {
                return Unauthorized("Invalid username");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userLoginDto.Password));

            if (computedHash.Where((t, i) => t != user.Password[i]).Any())
            {
                return Unauthorized("Invalid password");
            }

            var userDto = _mapper.Map<UserReadDto>(user);

            userDto.Token = _tokenService.CreateToken(user);

            return userDto;
        }
    }
}