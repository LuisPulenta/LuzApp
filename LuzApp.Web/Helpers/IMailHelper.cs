using LuzApp.Common.Responses;

namespace LuzApp.Web.Helpers
{
    public interface IMailHelper
    {
        Response SendMail(string to, string subject, string body);
    }
}