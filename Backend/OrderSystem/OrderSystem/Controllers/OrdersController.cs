using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.DTOs;
using OrderSystem.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Security.Claims;
using System.Security.Cryptography;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;

using PdfSharp.Drawing;
using ClosedXML.Excel; 
using DocumentFormat.OpenXml.EMMA;
using QRCoder;
using System.Drawing;
//using IronBarCode;  // IronQR
using ZXing;
using ZXing.Windows.Compatibility;
using DocumentFormat.OpenXml.ExtendedProperties;
//using IronBarCode;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OrderSystem.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly Trainig02Context _context;
        public OrdersController(IMapper mapper)
        {
            _mapper = mapper;
            _context = new Trainig02Context();
        }
        [HttpPost]
        [Authorize]
        public Response<IEnumerable<GetOrderDTO>> FilterOrder([FromBody] FilterOrderDTO list)
        {
           if(ModelState.IsValid)
            {
                var requestToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var handler = new JwtSecurityTokenHandler().ReadToken(requestToken) as JwtSecurityToken;
                var claims = handler.Claims;
                var filteredList = (IEnumerable<GetOrderDTO>)null;
                var list2 = list.OrderList.ToList();
                Console.WriteLine(list.ClientName);
                Console.WriteLine(list.StatusName);
                foreach(var orderr in list2)
                {
                    Console.WriteLine(orderr.ClientName == list.ClientName);
                }
                if (list.ClientName != null && list.StatusName != null)
                {

                     filteredList = from order in list2 where order.ClientName == list.ClientName && order.OrderStatus == list.StatusName select order;
                }
                else if(list.ClientName != null)
                {
                     filteredList = from order in list2 where order.ClientName == list.ClientName select order;

                }
                else if (list.StatusName != null)
                {
                     filteredList = from order in list2 where order.OrderStatus == list.StatusName select order;

                }
                if (filteredList.Count() > 0)
                {
                    
                    Response<IEnumerable<GetOrderDTO>> response1 = new()
                    {
                        Data = filteredList,
                        Message = "Retreived all orders successfully",
                        StatusCode = 200,
                        Success = true
                    };
                    return response1;
                }
                else
                {
                    Response<IEnumerable<GetOrderDTO>> response2 = new()
                    {
                        Data = null,
                        Message = "No orders found",
                        StatusCode = 400,
                        Success = false
                    };
                    return response2;
                }
            }
            Response<IEnumerable<GetOrderDTO>> response3 = new()
            {
                Data = null,
                Message = "Bad request",
                StatusCode = 200,
                Success = true
            };
            return response3;
        }

        [HttpGet][ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        public Response<List<GetOrderDTO>> GetOrdersbyID()
        {
            if (ModelState.IsValid)
            {
                var requestToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var handler = new JwtSecurityTokenHandler().ReadToken(requestToken) as JwtSecurityToken;
                var claims = handler.Claims;
                var userid = Int32.Parse(claims.FirstOrDefault(claim => claim.Type == "userid").Value);
                var orders = _context.Orders.Include(s => s.Status).Include(c=>c.Client).Include(u => u.User).Where(o => o.UserId == userid && !o.IsDeleted).ToList();
                List<GetOrderDTO> list = _mapper.Map<List<GetOrderDTO>>(orders);
                if (list.Count > 0)
                {
                    Response<List<GetOrderDTO>> response1 = new()
                    {
                        Data = list,
                        Message = "Retreived all orders successfully",
                        StatusCode = 200,
                        Success = true
                    };
                    return response1;
                }
                Response<List<GetOrderDTO>> response2 = new()
                {
                    Data = null,
                    Message = "No orders found",
                    StatusCode = 400,
                    Success = false
                };
                return response2;

            }
            Response<List<GetOrderDTO>> response3 = new()
            {
                Data = null,
                Message = "Bad request",
                StatusCode = 200,
                Success = true
            };
            return response3;
        }









        [HttpGet]
        [Authorize]
        public Response<List<GetOrderDTO>> GetOrdersbyRole()
        {
            if (ModelState.IsValid)
            {
                var requestToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var handler = new JwtSecurityTokenHandler().ReadToken(requestToken) as JwtSecurityToken;
                var claims = handler.Claims;
                var userid = Int32.Parse(claims.FirstOrDefault(claim => claim.Type == "userid").Value);
                var orgid = 0;
                var temporgid = int.TryParse(claims.FirstOrDefault(claim => claim.Type == "orgid")?.Value, out orgid);
                var roleClaim = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value;
                var orders = new List<Order>();
                if (roleClaim.Contains("Super"))
                {
                    orders = _context.Orders.Include(s => s.Status).Include(c => c.Client).Include(u=>u.User).Where(o => !o.IsDeleted).ToList();
                }
                else if (roleClaim.Contains("Admin"))
                {
                    orders = _context.Orders.Include(s => s.Status).Include(c => c.Client).Include(u=>u.User).Where(o => o.OrgId == orgid && !o.IsDeleted).ToList();

                }
                
                List<GetOrderDTO> list = _mapper.Map<List<GetOrderDTO>>(orders);
                if (list.Count > 0)
                {
                    Response<List<GetOrderDTO>> response1 = new()
                    {
                        Data = list,
                        Message = "Retreived all orders successfully",
                        StatusCode = 200,
                        Success = true
                    };
                    return response1;
                }
                Response<List<GetOrderDTO>> response2 = new()
                {
                    Data = null,
                    Message = "No orders found",
                    StatusCode = 400,
                    Success = false
                };
                return response2;

            }
            Response<List<GetOrderDTO>> response3 = new()
            {
                Data = null,
                Message = "Bad request",
                StatusCode = 200,
                Success = true
            };
            return response3;

        }
        [HttpPost]
        [Authorize]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Response<AddEDITOrderDTO> AddOrder([FromForm] AddEDITOrderDTO Info,[FromForm] IFormFile pdf, [FromForm] IFormFile? xml, IValidator<AddEDITOrderDTO> validator)
        {
            if (ModelState.IsValid)
            {
                var ValidationResult = validator.Validate(Info);

                if (ValidationResult.IsValid)
                {
                    var requestToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                    var handler = new JwtSecurityTokenHandler().ReadToken(requestToken) as JwtSecurityToken;
                    var claims = handler.Claims;
                    var userid = Int32.Parse(claims.FirstOrDefault(claim => claim.Type == "userid").Value);
                    Order order = _mapper.Map<Order>(Info);
                    order.ClientId = Info.ClientID;
                    order.UserId = userid;
                    order.OrgId = Info.OrgId;
                    _context.Orders.Add(order);
                    _context.SaveChanges();
                    order.PdfName = order.OrderId.ToString() + "_" + pdf.FileName;
                    if (pdf != null && pdf.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var filePath = Path.Combine(uploadsFolder, order.OrderId.ToString()+"_"+ pdf.FileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            pdf.CopyTo(fileStream);
                        }
                    }
                    if (xml != null)
                    {
                        order.XLName = order.OrderId.ToString() + "_" + xml.FileName;
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var filePath = Path.Combine(uploadsFolder, order.OrderId.ToString() + "_" + xml.FileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            xml.CopyTo(fileStream);
                        }
                    }
                    
                    

                        
                    
                    _context.SaveChanges();
                    Response<AddEDITOrderDTO> response1 = new()
                    {
                        Data = Info,
                        Message = "Order placed successfully!",
                        StatusCode = 200,
                        Success = true,
                    };
                    return response1;


                }
                Response<AddEDITOrderDTO> response2 = new()
                {
                    Success = false,
                    StatusCode = 400,
                    Message = ValidationResult.ToString(),
                    Data = Info
                };
                return response2;


            }
            Response<AddEDITOrderDTO> response3 = new()
            {
                Message = "Bad Request",
                StatusCode = 400,
                Data = null,
                Success = false,

            };
            return response3;
        }
        //[HttpPost("id")]
        //[Authorize]
        //public Response<AddEDITOrderDTO> EditOrder(int orderid, [FromBody] AddEDITOrderDTO Info, IValidator<AddEDITOrderDTO> validator)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var ValidationResult = validator.Validate(Info);
        //        if (ValidationResult.IsValid)
        //        {
        //            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderid && !o.IsDeleted);
        //            if (order.StatusId > 2)
        //            {
        //                Response<AddEDITOrderDTO> response1 = new()
        //                {
        //                    Data = null,
        //                    Message = "Can't edit order. Order is already " + _context.Statuses.FirstOrDefault(s => s.StatusId == order.StatusId).StatusName,
        //                    StatusCode = 400,
        //                    Success = false,
        //                };
        //                return response1;

        //            }
        //            order.OrderName = Info.OrderName;
        //            order.OrderDate = Info.OrderDate;
        //            order.OrderAmount = Info.OrderAmount;
        //            order.NumberOfCopies = Info.NumberOfCopies;
        //            _context.SaveChanges();
        //            Response<AddEDITOrderDTO> response2 = new()
        //            {
        //                Data = Info,
        //                Success = true,
        //                StatusCode = 200,
        //                Message = "Order Edited Successfully"
        //            };
        //            return response2;

        //        }
        //        Response<AddEDITOrderDTO> response3 = new()
        //        {
        //            Data = null,
        //            Message = ValidationResult.ToString(),
        //            StatusCode = 400,
        //            Success = false,
        //        };
        //        return response3;

        //    }
        //    Response<AddEDITOrderDTO> response4 = new()
        //    {
        //        Data = null,
        //        Message = "Bad Request",
        //        StatusCode = 400,
        //        Success = false,
        //    };
        //    return response4;
        //}
        [HttpDelete("id")]
        [Authorize (Roles="Super-Admin,University-Admin,Institute-Admin")]
        public Response<String> DeleteOrder(int id)
        {
            if (ModelState.IsValid)
            {
                int orderid = id;
                var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderid && !o.IsDeleted);
                order.IsDeleted = true;
                var serials = _context.Serials.Where(s => s.OrderId == orderid && !s.IsDeleted).ToList();
                foreach(var serial in serials)
                {
                    serial.IsDeleted = true;
                }
                _context.SaveChanges();
                Response<String> response1 = new()
                {
                    Data = null,
                    Message = "Order Deleted Successfully",
                    StatusCode = 200,
                    Success = true
                };
                return response1;

            }
            Response<String> response2 = new()
            {
                Data = null,
                Message = "Bad Request",
                StatusCode = 400,
                Success = false,
            };
            return response2;

        }

        [HttpPost("id")]
        [Authorize]
        public Response<String> CancelOrder(int orderid) 
        {
            if (ModelState.IsValid) 
            {
                var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderid && !o.IsDeleted);
                var serials = _context.Serials.Where(s => s.OrderId==orderid && !s.IsDeleted).ToList();
                if(order.StatusId<3)
                {
                    order.StatusId = 5;
                    foreach(var serial in serials)
                    {
                    serial.StatusId = 5;
                    }
                    _context.SaveChanges();
                    Response<String> response1 = new()
                    {
                        Data = null,
                        Message = "Order cancelled successfully",
                        StatusCode = 200,
                        Success = true
                    };
                    return response1;
                }
                Response<String> response2 = new()
                {
                    Data = null,
                    Message = "Can't cancel order.Order is already " + _context.Statuses.FirstOrDefault(s => s.StatusId == order.StatusId).StatusName,
                    StatusCode = 400,
                    Success = false,
                };
                return response2;
                
            }
            Response<String> response3 = new()
            {
                Data = null,
                Message = "Bad Request",
                StatusCode = 400,
                Success = false,
            };
            return response3;
        }
        [HttpPost("id")]
        [Authorize(Roles = "Super-Admin,University-Admin,Institute-Admin")]
        public Response<String> ApproveOrder(int id)
        {
            if (ModelState.IsValid)
            {
                int orderid = id;
                var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderid && !o.IsDeleted);
                order.StatusId = 2;


                //var serials = _context.Serials.Where(s => s.OrderId == orderid && !s.IsDeleted).ToList();
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                string existingFilePath = Path.Combine(uploadsFolder, order.PdfName);
                PdfDocument document = PdfReader.Open(existingFilePath, PdfDocumentOpenMode.Modify);
                var workbook = (XLWorkbook)null;
                if(order.XLName != null)
                {
                    workbook = new XLWorkbook(Path.Combine(uploadsFolder,order.XLName));
                }
                string column = "B";
                int j = 0;
                var lastserial = _context.Serials.OrderBy(s => s.SerialId).LastOrDefault(s => s.ClientId == order.ClientId);
                if (lastserial != null)
                {
                    String gettingSerial = lastserial.SerialNumber;
                    String lastSerial = string.Concat(gettingSerial.Where(char.IsDigit));
                    j = Int32.Parse(lastSerial) + 1;
                }

                for (int i = 0; i < document.PageCount; i++)
                {
                    Serial serial = new Serial();
                    PdfPage page = document.Pages[i];
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XFont font = new XFont("Arial", 12, XFontStyleEx.Bold);
                    if(workbook!=null)
                    {
                        var worksheet = workbook.Worksheet(1);
                        string temp = string.Concat("b" + (i + 2).ToString());
                        string value1 = worksheet.Cell(temp).GetValue<string>();
                        //string value1 = "heloo";

                        //var barcodeWriter = new BarcodeWriter();
                        //barcodeWriter.Format = BarcodeFormat.QR_CODE;
                        //barcodeWriter.Options.Width = 200;
                        //barcodeWriter.Options.Height = 200;
                        //// Encode the text and generate the QR code
                        //var barcodeBitmap = barcodeWriter.Write(value1);



                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(value1,
                        QRCodeGenerator.ECCLevel.Q);
                        QRCode qrCode = new QRCode(qrCodeData);
                        Bitmap qrCodeImage = qrCode.GetGraphic(2);

                        //var qrCode = QRCodeWriter.CreateQrCode(value1,50);  // 300px QR code
                        //Bitmap qrCodeBitmap = qrCode.ToBitmap();
                        XImage image = XImage.FromGdiPlusImage(qrCodeImage); //you can use XImage.FromGdiPlusImage to get the bitmap object as image (not a stream)
                        gfx.DrawImage(image, 450, 670);
                        

                    }
                    serial.ClientId = order.ClientId;
                    serial.OrderId = order.OrderId;
                    serial.StatusId = order.StatusId;
                    serial.SerialNumber = _context.Clients?.FirstOrDefault(c => c.ClientId == order.ClientId && !c.IsDeleted)?.FixedPart + j.ToString();
                    gfx.DrawString(serial.SerialNumber, font, XBrushes.Black,
                        new XRect(470, 750, page.Width, page.Height),
                        XStringFormats.TopLeft);
                    j++;
                    _context.Serials.Add(serial);
                }
                
                //foreach (var serial in serials)
                //{
                    
                //    serial.StatusId = 2;
                //}

                string processedFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "processed");
                if (!Directory.Exists(processedFolder))
                {
                    Directory.CreateDirectory(processedFolder);
                }

                // Save the modified document as a new PDF file in the processed folder
                string newFilePath = Path.Combine(processedFolder, order.PdfName);
                document.Save(newFilePath);


                _context.SaveChanges();
                Response<String> response1 = new()
                {
                    Data = null,
                    Message = "Order Approved Successfully",
                    StatusCode = 200,
                    Success = true
                };
                return response1;

            }
            Response<String> response2 = new()
            {
                Data = null,
                Message = "Bad Request",
                StatusCode = 400,
                Success = false,
            };
            return response2;

        }


        [HttpGet("id")]
        [Authorize]
        public IActionResult Download(int id)
        {
            // Specify the path to the file
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id && !o.IsDeleted);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "processed", order.PdfName);

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            // Return the file as an attachment
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/pdf", order.PdfName);
        }

    }



}
