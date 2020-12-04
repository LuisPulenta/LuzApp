using LuzApp.Common.Responses;
using LuzApp.Web.Data;
using LuzApp.Web.Data.Entities;
using System.Threading.Tasks;

namespace LuzApp.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly DataContext _context;

        public ConverterHelper(DataContext context)
        {
            _context = context;
        }

        public UserResponse ToUserResponse(User user)
        {
            if (user == null)
            {
                return null;
            }

            return new UserResponse
            {
                Address = user.Address,
                Document = user.Document,
                Email = user.Email,
                FirstName = user.FirstName,
                Id = user.Id,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ImagePath = user.ImagePath,
                UserType = user.UserType,
                Neighborhood=user.Neighborhood,
             };
        }

        public async Task<LuminaryResponse> ToLuminaryResponse(Luminary luminary)
        {
            return new LuminaryResponse
            {
                Address = luminary.Address,
                BasePhoto = luminary.BasePhoto,
                TopPhoto = luminary.TopPhoto,
                FullPhoto = luminary.FullPhoto,
                Date = luminary.Date,
                Id = luminary.Id,
                Remarks = luminary.Remarks,
                User = ToUserResponse(await _context.Users.FindAsync(luminary.User.Id)),
                Latitude = luminary.Latitude,
                Longitude = luminary.Longitude,
                Tipo=luminary.Type,
            };
        }
    }
}