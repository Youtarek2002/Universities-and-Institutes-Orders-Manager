using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OrderSystem.DTOs;
using OrderSystem.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrderSystem.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        private readonly Trainig02Context _context;

        public UsersController(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _context = new Trainig02Context();
            _configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        public Response<String> Login([FromBody] LoginDTO Info, IValidator<LoginDTO> validator)
        {
            if (ModelState.IsValid)
            {
                var validationResult = validator.Validate(Info);
                if (validationResult.IsValid)
                {
                    var user = _context.Users.FirstOrDefault(u => u.Email == Info.Email && !u.IsDeleted);
                    if (user != null)
                    {
                        byte[] data = Encoding.ASCII.GetBytes(Info.Password);
                        data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                        String hash = Encoding.ASCII.GetString(data);
                        if (hash == user.Password)
                        {
                            var token = GenerateJwtToken(user.Email, user.UserName,user.UserFirstName,user.UserLastName, user.UserId, user.OrgId, _context.Roles.FirstOrDefault(r => r.RoleId == user.RoleId && !r.IsDeleted).RoleName);
                            Response<String> response1 = new()
                            {
                                StatusCode = 200,
                                Success = true,
                                Message = "Logged in",
                                Data = token

                            };
                            return response1;
                        }


                    }

                    Response<String> response2 = new()
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = "Incorrect Email or Password",
                        Data = validationResult.ToString()
                    };
                    return response2;
                }

            }
            Response<String> response3 = new()
            {
                StatusCode = 400,
                Success = false,
                Message = "Bad Request",
                Data = null
            };
            return response3;
        }
        [HttpPost]
        [AllowAnonymous]
        public Response<String> Register([FromBody] RegisterDTO Info, IValidator<RegisterDTO> validator)
        {
            if (ModelState.IsValid)
            {
                var validationResult = validator.Validate(Info);
                if (validationResult.IsValid)
                {
                    var user = _context.Users.FirstOrDefault(u => u.Email == Info.Email && !u.IsDeleted);
                    if (user != null)
                    {
                        Response<String> response1 = new()
                        {
                            StatusCode = 400,
                            Success = false,
                            Data = Info.Email,
                            Message = "Email already in use!"
                        };
                        return response1;
                    }
                    User newuser = _mapper.Map<User>(Info);
                    byte[] data = Encoding.ASCII.GetBytes(Info.Password);
                    data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                    String hash = Encoding.ASCII.GetString(data);
                    newuser.Password = hash;
                    _context.Users.Add(newuser);
                    _context.SaveChanges();
                    Response<String> response2 = new()
                    {
                        StatusCode = 200,
                        Success = true,
                        Data = null,
                        Message = "User registered successfully!"
                    };
                    return response2;

                }
                Response<String> response3 = new()
                {
                    StatusCode = 400,
                    Success = false,
                    Data = null,
                    Message = validationResult.ToString()
                };
                return response3;




            }


            Response<String> response4 = new()
            {
                StatusCode = 400,
                Success = false,
                Message = "Bad Request",
                Data = null
            };
            return response4;
        }

        [HttpPost]
        [Authorize]
        public Response<EditInfo> EditInfo([FromBody] EditInfo Info, IValidator<EditInfo> validator)
        {
            if (ModelState.IsValid)
            {
                var ValidationResult = validator.Validate(Info);
                if (ValidationResult.IsValid)
                {
                    byte[] data = Encoding.ASCII.GetBytes(Info.Password);
                    data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                    String hash = Encoding.ASCII.GetString(data);
                    var requestToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                    var handler = new JwtSecurityTokenHandler().ReadToken(requestToken) as JwtSecurityToken;
                    var claims = handler.Claims;
                    var email = claims.FirstOrDefault(claim => claim.Type == "sub").Value;
                    var user = _context.Users.FirstOrDefault(u => u.Email == email && !u.IsDeleted);
                    if (hash == user.Password)
                    {
                        user.UserLastName = Info.LastName;
                        user.UserFirstName = Info.FirstName;
                        user.UserName = Info.UserName;
                        _context.SaveChanges();
                        Response<EditInfo> response1 = new()
                        {
                            Data = Info,
                            Message = "User updated successfully",
                            StatusCode = 200,
                            Success = true,
                        };
                        return response1;
                    }
                    else
                    {
                        Response<EditInfo> response2 = new()
                        {
                            Data = null,
                            Message = "Incorrect Password!",
                            StatusCode = 400,
                            Success = false,
                        };
                        return response2;
                    }

                }
                Response<EditInfo> response3 = new()
                {
                    Data = Info,
                    Message = ValidationResult.ToString(),
                    Success = false,
                    StatusCode = 400
                };
                return response3;


            }
            Response<EditInfo> response4 = new()
            {
                StatusCode = 400,
                Success = false,
                Message = "Bad Request",
                Data = null
            };
            return response4;
        }

        [HttpPost]
        [Authorize]
        public Response<String> EditPassword([FromBody] EditPassword Info, IValidator<EditPassword> validator)
        {
            if (ModelState.IsValid)
            {
                var ValidationResult = validator.Validate(Info);
                if (ValidationResult.IsValid)
                {
                    var requestToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                    var handler = new JwtSecurityTokenHandler().ReadToken(requestToken) as JwtSecurityToken;
                    var claims = handler.Claims;
                    var email = claims.FirstOrDefault(claim => claim.Type == "sub").Value;

                    byte[] data = Encoding.ASCII.GetBytes(Info.Password);
                    data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                    String hash = Encoding.ASCII.GetString(data);

                    var user = _context.Users.FirstOrDefault(u => u.Email == email && !u.IsDeleted);
                    if (user.Password == hash)
                    {
                        byte[] datanew = Encoding.ASCII.GetBytes(Info.NewPassword);
                        datanew = new System.Security.Cryptography.SHA256Managed().ComputeHash(datanew);
                        String hashnew = Encoding.ASCII.GetString(datanew);
                        user.Password = hashnew;
                        _context.SaveChanges();
                        Response<String> response1 = new()
                        {
                            StatusCode = 200,
                            Data = null,
                            Message = "Password updated successfully",
                            Success = true,
                        };
                        return response1;
                    }
                    else
                    {
                        Response<String> response2 = new()
                        {
                            StatusCode = 400,
                            Data = null,
                            Message = "Incorrect old password!",
                            Success = false,
                        };
                        return response2;
                    }
                }
                Response<String> response3 = new()
                {
                    StatusCode = 400,
                    Data = ValidationResult.ToString(),
                    Message = "Please enter a new valid password",
                    Success = false,
                };
                return response3;
            }
            Response<String> response4 = new()
            {
                StatusCode = 400,
                Success = false,
                Message = "Bad Request",
                Data = null
            };
            return response4;
        }

        [HttpDelete]
        [Authorize]
        public Response<String> DeleteUser([FromBody] String password)
        {
            if (ModelState.IsValid)
            {
                byte[] data = Encoding.ASCII.GetBytes(password);
                data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                String hash = Encoding.ASCII.GetString(data);

                var requestToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var handler = new JwtSecurityTokenHandler().ReadToken(requestToken) as JwtSecurityToken;
                var claims = handler.Claims;
                var email = claims.FirstOrDefault(claim => claim.Type == "sub").Value;

                var user = _context.Users.FirstOrDefault(u => u.Email == email && !u.IsDeleted);
                if (user.Password == hash)
                {
                    user.IsDeleted = true;
                    _context.SaveChanges();
                    Response<String> response1 = new()
                    {
                        Data = null,
                        Message = "User deleted successfully",
                        StatusCode = 200,
                        Success = true
                    };
                    return response1;
                }
                Response<String> response2 = new()
                {
                    Data = null,
                    Message = "Incorrect password!",
                    StatusCode = 400,
                    Success = false
                };
                return response2;
            }
            Response<String> response3 = new()
            {
                StatusCode = 400,
                Success = false,
                Message = "Bad Request",
                Data = null
            };
            return response3;

        }

        private string GenerateJwtToken(string email, string username,string firstname,string lastname, int userid, int? orgid, string rolename)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("username", username),
            new Claim("firstname", firstname),
            new Claim("lastname", lastname),
            new Claim("userid", userid.ToString()),
            new Claim("orgid",orgid.ToString()),
            new Claim(ClaimTypes.Role,rolename),
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
