using ProgrammersBlog.Shared.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entities.Concrete
{
    public class City : EntityBase, IEntity
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public string Thumbnail { get; set; }
        
        
        
        
    }
}
