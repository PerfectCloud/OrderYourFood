using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthenticationPlugin;
using LetsOrderFoods.Data;
using LetsOrderFoods.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LetsOrderFoods.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private IConfiguration  _configuration;
        private readonly AuthService _auth;

        public AccountController(ApplicationDbContext context,
            IConfiguration configuration,
            AuthService auth)
        {
            _context = context;
            _configuration = configuration;
            _auth = auth;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(User user)
        {
            var EmailExistDeja = _context.Users.SingleOrDefault(
                x => x.Email == user.Email);
            if (EmailExistDeja != null) return BadRequest("Cette Adresse EMail utilisateur Exist Deja ");

            var utilisateurObj = new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = SecurePasswordHasherHelper.Hash(user.Password),
                Role = "User"
            };

            _context.Users.Add(utilisateurObj);
            await _context.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created);

        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(User user)
        {
            var Email = _context.Users.FirstOrDefault(x => x.Email == user.Email);

            if (Email == null) return StatusCode(StatusCodes.Status404NotFound);

            var motdepasse = Email.Password;
            if (SecurePasswordHasherHelper.Verify(user.Password, motdepasse))
                return Unauthorized();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Name,user.Email),
                new Claim(ClaimTypes.Role,user.Role)
            };

            var montoken = _auth.GenerateAccessToken(claims);
            return new ObjectResult(new
            {
                access_token = montoken.AccessToken,
                token_type = montoken.TokenType,
                user_Id = Email.Id,
                user_name = Email.Name,
                expires_in = montoken.ExpiresIn,
                creation_time = montoken.ValidFrom,
                expiration_time = montoken.ValidTo
            });
        }
    }
}