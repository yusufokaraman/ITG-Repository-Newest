using ProgrammersBlog.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammersBlog.Mvc.Areas.Admin.Models
{
    public class CityAddAjaxViewModel
    {
        public CityAddDto CityAddDto { get; set; }
        public string CityAddPartial { get; set; }
        public CityDto CityDto { get; set; }
    }
}
