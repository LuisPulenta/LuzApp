using LuzApp.Common.Entities;
using LuzApp.Common.Enums;
using LuzApp.Web.Data;
using LuzApp.Web.Data.Entities;
using LuzApp.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuzApp.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _userHelper = userHelper;
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckDepartmentsAsync();
            await CheckRolesAsync();
            await CheckUserAsync("1010", "Luis", "Nuñez", "luisalbertonu@gmail.com", "156814963", "Espora 2052", UserType.Admin);

        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task<User> CheckUserAsync(
    string document,
    string firstName,
    string lastName,
    string email,
    string phone,
    string address,
    UserType userType)
        {
            User user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    Neighborhood = _context.Neighborhoods.FirstOrDefault(),
                    UserType = userType
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);

            }

            return user;
        }



        private async Task CheckDepartmentsAsync()
        {
            if (!_context.Departments.Any())
            {
                _context.Departments.Add(new Department
                {
                    Name = "Córdoba",
                    Cities = new List<City>
                {
                    new City
                    {
                        Name = "Córdoba Capital" ,
                        Neighborhoods = new List<Neighborhood>
                        {
                            new Neighborhood { Name = "Rosedal" },
                            new Neighborhood { Name = "Los Naranjos" },
                            new Neighborhood { Name = "Balcarce" }
                        }
                    }
                }
                });
                _context.Departments.Add(new Department
                {
                    Name = "Buenos Aires",
                    Cities = new List<City>
                {
                    new City
                    {
                        Name = "Capital Federal",
                        Neighborhoods = new List<Neighborhood>
                        {
                            new Neighborhood { Name = "La Boca" },
                            new Neighborhood { Name = "Núñez" },
                            new Neighborhood { Name = "Caballito" }
                        }
                    }
                }
                });
                await _context.SaveChangesAsync();
            }
        }
    }
}