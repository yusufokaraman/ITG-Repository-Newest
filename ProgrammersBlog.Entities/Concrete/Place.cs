using ProgrammersBlog.Shared.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entities.Concrete
{
    public class Place: EntityBase,IEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PlacePicture { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<Article> Articles { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
