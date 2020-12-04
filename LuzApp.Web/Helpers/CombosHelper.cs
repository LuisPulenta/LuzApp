using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LuzApp.Common.Entities;
using LuzApp.Web.Data;
using System.Collections.Generic;
using System.Linq;

namespace LuzApp.Web.Helpers
{
    public class CombosHelper : ICombosHelper
    {
        private readonly DataContext _context;

        public CombosHelper(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetComboDepartments()
        {
            List<SelectListItem> list = _context.Departments.Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = $"{t.Id}"
            })
                .OrderBy(t => t.Text)
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione una Provincia...]",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboCities(int departmentId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            Department department = _context.Departments
                .Include(c => c.Cities)
                .FirstOrDefault(c => c.Id == departmentId);
            if (department != null)
            {
                list = department.Cities.Select(t => new SelectListItem
                {
                    Text = t.Name,
                    Value = $"{t.Id}"
                })
                    .OrderBy(t => t.Text)
                    .ToList();
            }

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione una Ciudad...]",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboNeighborhoods(int cityId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            City city = _context.Cities
                .Include(d => d.Neighborhoods)
                .FirstOrDefault(d => d.Id == cityId);
            if (city != null)
            {
                list = city.Neighborhoods.Select(t => new SelectListItem
                {
                    Text = t.Name,
                    Value = $"{t.Id}"
                })
                    .OrderBy(t => t.Text)
                    .ToList();
            }

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione un Barrio...]",
                Value = "0"
            });

            return list;
        }
    }
}