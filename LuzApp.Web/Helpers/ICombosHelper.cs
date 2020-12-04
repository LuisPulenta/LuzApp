using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace LuzApp.Web.Helpers
{
    public interface ICombosHelper
    {
        IEnumerable<SelectListItem> GetComboDepartments();

        IEnumerable<SelectListItem> GetComboCities(int departmentId);

        IEnumerable<SelectListItem> GetComboNeighborhoods(int cityId);

    }
}