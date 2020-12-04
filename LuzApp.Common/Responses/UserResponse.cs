using LuzApp.Common.Entities;
using LuzApp.Common.Enums;

namespace LuzApp.Common.Responses
{
    public class UserResponse
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Document { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string ImagePath { get; set; }

        public string ImageFullPath => string.IsNullOrEmpty(ImagePath)
            ? $"http://keypress.serveftp.net:88/LuzAppApi/images/Users/nouser.png"
            : $"http://keypress.serveftp.net:88/LuzAppApi{ImagePath.Substring(1)}";


        public UserType UserType { get; set; }

        public Neighborhood Neighborhood { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string FullNameWithDocument => $"{FirstName} {LastName} - {Document}";
    }
}