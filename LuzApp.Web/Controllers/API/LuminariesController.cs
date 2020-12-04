using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuzApp.Common.Entities;
using LuzApp.Web.Data;
using LuzApp.Web.Helpers;
using System.Collections.Generic;
using LuzApp.Common.Responses;
using LuzApp.Web.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using LuzApp.Common.Helpers;
using LuzApp.Common.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace LuzApp.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LuminariesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;

        public LuminariesController(DataContext context, IConverterHelper converterHelper, IImageHelper imageHelper)
        {
            _context = context;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
        }

        // GET: api/Luminaries
        [HttpGet("{Neighborhood}")]

        public async Task<IActionResult> GetLuminaries([FromRoute] string Neighborhood)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var luminaries = await _context.Luminaries
                .Include(u => u.User)
                .Include(i=>i.LuminaryImages)
                .OrderBy(t => t.Address)
                .Where(o => (o.Neighborhood.Name == Neighborhood))
                .ToListAsync();

            if (luminaries == null)
            {
                return BadRequest("No hay Luminarias Relevadas.");
            }


            List<LuminaryResponse> luminaryResponses = new List<LuminaryResponse>();

            foreach (Luminary luminary in luminaries)
            {
                LuminaryResponse luminaryResponse = await _converterHelper.ToLuminaryResponse(luminary);
                luminaryResponses.Add(luminaryResponse);
            }

            return Ok(luminaryResponses);
        }


        

        


        

        // POST: api/Luminaries

        [HttpPost]
        public async Task<IActionResult> PostLuminary([FromBody] LuminaryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Foto1
            var imageUrl1 = string.Empty;
            if (request.BasePhotoArray != null && request.BasePhotoArray.Length > 0)
            {
                var stream = new MemoryStream(request.BasePhotoArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "wwwroot\\images\\Luminaries";
                var fullPath = $"~/images/Luminaries/{file}";
                var response = FilesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    imageUrl1 = fullPath;
                }
            }
            //Foto2
            var imageUrl2 = string.Empty;
            if (request.TopPhotoArray != null && request.TopPhotoArray.Length > 0)
            {
                var stream = new MemoryStream(request.TopPhotoArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "wwwroot\\images\\Luminaries";
                var fullPath = $"~/images/Luminaries/{file}";
                var response = FilesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    imageUrl2 = fullPath;
                }
            }
            //Foto3
            var imageUrl3 = string.Empty;
            if (request.FullPhotoArray != null && request.FullPhotoArray.Length > 0)
            {
                var stream = new MemoryStream(request.FullPhotoArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "wwwroot\\images\\Luminaries";
                var fullPath = $"~/images/Luminaries/{file}";
                var response = FilesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    imageUrl3 = fullPath;
                }
            }

            User User2 = await _context.Users
                .Include (n=> n.Neighborhood)
                .FirstOrDefaultAsync(o => o.Id == request.UserId);


            var luminary = new Luminary
            {
                Date = request.Date,
                Id = request.Id,
                Address = request.Address,
                BasePhoto = imageUrl1,
                TopPhoto = imageUrl2,
                FullPhoto = imageUrl3,
                Type = request.Tipo,
                Remarks = request.Remarks,
                User = User2,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Neighborhood= User2.Neighborhood,
                State = request.State,

            };

            _context.Luminaries.Add(luminary);
            await _context.SaveChangesAsync();
            return Ok(_converterHelper.ToLuminaryResponse(luminary));
            //return NoContent();
        }




        // PUT: api/Luminaries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLuminary([FromRoute] int id, [FromBody] LuminaryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != request.Id)
            {
                return BadRequest();
            }

            var oldLuminary = await _context.Luminaries.FindAsync(request.Id);
            if (oldLuminary == null)
            {
                return BadRequest("La Luminaria no existe.");
            }

            var imageUrl1 = oldLuminary.BasePhoto;
            if (request.BasePhotoArray != null && request.BasePhotoArray.Length > 0)
            {
                var stream = new MemoryStream(request.BasePhotoArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "wwwroot\\images\\Luminaries";
                var fullPath = $"~/images/Luminaries/{file}";
                var response = FilesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    imageUrl1 = fullPath;
                }
            }

            var imageUrl2 = oldLuminary.TopPhoto;
            if (request.TopPhotoArray != null && request.TopPhotoArray.Length > 0)
            {
                var stream = new MemoryStream(request.TopPhotoArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "wwwroot\\images\\Luminaries";
                var fullPath = $"~/images/Luminaries/{file}";
                var response = FilesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    imageUrl2 = fullPath;
                }
            }

            var imageUrl3 = oldLuminary.FullPhoto;
            if (request.FullPhotoArray != null && request.FullPhotoArray.Length > 0)
            {
                var stream = new MemoryStream(request.FullPhotoArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "wwwroot\\images\\Luminaries";
                var fullPath = $"~/images/Luminaries/{file}";
                var response = FilesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    imageUrl3 = fullPath;
                }
            }

            oldLuminary.Address = request.Address;
            oldLuminary.BasePhoto = imageUrl1;
            oldLuminary.TopPhoto = imageUrl2;
            oldLuminary.FullPhoto = imageUrl3;
            oldLuminary.Date = request.Date;
            oldLuminary.Id = request.Id;
            oldLuminary.Latitude = request.Latitude;
            oldLuminary.Longitude = request.Longitude;
            oldLuminary.Type = request.Tipo;
            oldLuminary.State = request.State;
            oldLuminary.Remarks = request.Remarks;
            oldLuminary.User = await _context.Users.FindAsync(request.UserId);


            _context.Luminaries.Update(oldLuminary);
            await _context.SaveChangesAsync();
            return Ok();
        }
















        // DELETE: api/Luminaries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLuminary([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            var luminary = await _context.Luminaries
                .FirstOrDefaultAsync(p => p.Id == id);
            if (luminary == null)
            {
                return this.NotFound();
            }



            _context.Luminaries.Remove(luminary);
            await _context.SaveChangesAsync();
            return Ok("Luminaria borrada");
        }

        private bool LuminaryExists(int id)
        {
            return _context.Luminaries.Any(e => e.Id == id);
        }
    }
}