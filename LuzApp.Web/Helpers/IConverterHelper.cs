using LuzApp.Common.Responses;
using LuzApp.Web.Data.Entities;
using System.Threading.Tasks;

namespace LuzApp.Web.Helpers
{
    public interface IConverterHelper
    {
        UserResponse ToUserResponse(User user);

        Task<LuminaryResponse> ToLuminaryResponse(Luminary luminary);
    }
}