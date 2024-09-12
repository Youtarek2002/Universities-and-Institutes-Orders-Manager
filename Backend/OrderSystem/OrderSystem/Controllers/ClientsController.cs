using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.DTOs;
using OrderSystem.Models;
using OrderSystem.Validators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
//C: \Users\youssef\source\repos\OrderSystem\OrderSystem\Controllers\ClientsController.cs
namespace OrderSystem.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {

        private readonly IMapper _mapper;

        private readonly Trainig02Context _context;
        public ClientsController(IMapper mapper)
        {
            _mapper = mapper;
            _context = new Trainig02Context();
        }




        [HttpGet("id")]
        [Authorize]
        public Response<InfoDTO> GetClientInfo(int id)
        {
            if (ModelState.IsValid)
            {
                var client = _context.Clients.FirstOrDefault(c=> c.ClientId == id && !c.IsDeleted);
                int TotalOrders = _context.Orders.Where(o=>o.ClientId == id && !o.IsDeleted).Count();
                int ApprovedOrders = _context.Orders.Where(o=> o.ClientId == id && !o.IsDeleted && o.StatusId == 2).Count();
                int ApprovedSerials = _context.Serials.Where(s => s.ClientId == id && !s.IsDeleted && s.StatusId == 2).Count();
                InfoDTO data = new()
                {
                    TotalOrders = TotalOrders,
                    ApprovedOrders = ApprovedOrders,
                    ApprovedSerials = ApprovedSerials

                };
                return new Response<InfoDTO>
                {
                    Data = data,
                    Message = "Data Retreived successfully",
                    StatusCode = 200,
                    Success = true

                };

            }
            Response<InfoDTO> response2 = new()
            {
                Success = false,
                Message = "Bad Request",
                StatusCode = 400,
                Data = null
            };
            return response2;

        }

        [HttpGet]
        [Authorize]
        public Response<List<GetDtO>> GetUniClients()
        {
            if (ModelState.IsValid)
            {
                var requestToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var handler = new JwtSecurityTokenHandler().ReadToken(requestToken) as JwtSecurityToken;
                var claims = handler.Claims;
                var orgid = 0;
                var temporgid = int.TryParse(claims.FirstOrDefault(claim => claim.Type == "orgid")?.Value, out orgid);
                var userid = Int32.Parse(claims.FirstOrDefault(claim => claim.Type == "userid").Value);
                var roleClaim = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value;
                List<GetDtO> list = new List<GetDtO>();
                var clients = _context.Clients.Where(c => !c.IsDeleted && c.OrgId == 1).ToList();
                
                list = _mapper.Map<List<GetDtO>>(clients);
                if (list.Count > 0)
                {
                    Response<List<GetDtO>> response1 = new()
                    {
                        Success = true,
                        Message = "Retreived all Universities successfully",
                        StatusCode = 200,
                        Data = list
                    };
                    return response1;
                }
                Response<List<GetDtO>> response3 = new()
                {
                    Success = false,
                    Message = "No Universities found",
                    StatusCode = 404,
                    Data = null
                };
                return response3;
            }
            Response<List<GetDtO>> response2 = new()
            {
                Success = false,
                Message = "Bad Request",
                StatusCode = 400,
                Data = null
            };
            return response2;


        }



        [HttpGet]
        [Authorize]
        public Response<List<GetDtO>> GetInstClients()
        {
            if (ModelState.IsValid)
            {

                var requestToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var handler = new JwtSecurityTokenHandler().ReadToken(requestToken) as JwtSecurityToken;
                var claims = handler.Claims;
                var orgid = 0;
                var temporgid = int.TryParse(claims.FirstOrDefault(claim => claim.Type == "orgid")?.Value, out orgid);
                var userid = Int32.Parse(claims.FirstOrDefault(claim => claim.Type == "userid").Value);
                var roleClaim = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value;
                List<GetDtO> list = new List<GetDtO>();
                var clients = _context.Clients.Where(c => !c.IsDeleted && c.OrgId == 2).ToList();
                
                list = _mapper.Map<List<GetDtO>>(clients);
                if (list.Count > 0)
                {
                    Response<List<GetDtO>> response1 = new()
                    {
                        Success = true,
                        Message = "Retreived all Institutes successfully",
                        StatusCode = 200,
                        Data = list
                    };
                    return response1;
                }
                Response<List<GetDtO>> response3 = new()
                {
                    Success = false,
                    Message = "No clients found",
                    StatusCode = 404,
                    Data = null
                };
                return response3;
            }
            Response<List<GetDtO>> response2 = new()
            {
                Success = false,
                Message = "Bad Request",
                StatusCode = 400,
                Data = null
            };
            return response2;


        }

        [HttpPost]
        [Authorize(Roles = "Super-Admin,University-Admin,Institute-Admin")]
        public Response<AddEditDTO> AddClient([FromBody] AddEditDTO client, IValidator<AddEditDTO> validator)
        {
            if (ModelState.IsValid)
            {
                var validationResult = validator.Validate(client);
                if (validationResult.IsValid)
                {
                    Client cli = _mapper.Map<Client>(client);
                    _context.Clients.Add(cli);
                    _context.SaveChanges();
                    Response<AddEditDTO> response1 = new()
                    {
                        Success = true,
                        Data = client,
                        StatusCode = 200,
                        Message = "Client Successfully added"
                    };
                    return response1;
                }
                Response<AddEditDTO> response2 = new()
                {
                    Success = false,
                    Data = client,
                    StatusCode = 400,
                    Message = "Failed to add client\n" + validationResult.ToString()
                };
                return response2;

            }
            Response<AddEditDTO> response3 = new()
            {
                Success = false,
                Data = null,
                Message = "Bad Request",
                StatusCode = 400

            };
            return response3;
        }

        [HttpPost("id")]
        [Authorize(Roles = "Super-Admin,University-Admin,Institute-Admin")]
        public Response<AddEditDTO> EditClient(int id, [FromBody] AddEditDTO cli, IValidator<AddEditDTO> validator)
        {
            if (ModelState.IsValid)
            {
                var validationResult = validator.Validate(cli);
                if (validationResult.IsValid)
                {
                    var client = _context.Clients.FirstOrDefault(u => u.ClientId == id && !u.IsDeleted);
                    if (client != null)
                    {
                        client.ClientName = cli.Name;
                        client.FixedPart = cli.FixedPart;
                        _context.SaveChanges();
                        Response<AddEditDTO> response1 = new()
                        {
                            Success = true,
                            Message = "Client edited successfully",
                            Data = cli,
                            StatusCode = 200
                        };
                        return response1;
                    }
                }
                Response<AddEditDTO> response2 = new()
                {
                    Success = false,
                    Message = "Couldn't edit Client\n" + validationResult.ToString(),
                    Data = cli,
                    StatusCode = 400
                };
                return response2;
            }
            Response<AddEditDTO> response3 = new()
            {
                Success = false,
                Data = null,
                Message = "Bad Request",
                StatusCode = 400

            };
            return response3;

        }
        [HttpDelete("id")]
        [Authorize(Roles = "Super-Admin,University-Admin,Institute-Admin")]
        public Response<Client> DeleteClient(int id)
        {
            if (ModelState.IsValid)
            {
                var client = _context.Clients.FirstOrDefault(u => u.ClientId == id && !u.IsDeleted);
                if (client != null)
                {
                    client.IsDeleted = true;
                    _context.SaveChanges();
                    Response<Client> response1 = new()
                    {
                        Data = client,
                        StatusCode = 200,
                        Message = "Client Deleted Successfully",
                        Success = true
                    };
                    return response1;

                }

            }
            Response<Client> response2 = new()
            {
                Data = null,
                Message = "Couldn't Delete client",
                Success = false,
                StatusCode = 400
            };
            return response2;
        }


    }
}
