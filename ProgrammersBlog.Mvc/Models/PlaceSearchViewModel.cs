using ProgrammersBlog.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammersBlog.Mvc.Models
{
    public class PlaceSearchViewModel
    {
        public PlaceListDto PlaceListDto { get; set; }
        public string Keyword { get; set; }
    }
}
