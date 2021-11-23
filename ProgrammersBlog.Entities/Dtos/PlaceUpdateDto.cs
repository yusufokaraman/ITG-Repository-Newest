using ProgrammersBlog.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entities.Dtos
{
    public class PlaceUpdateDto
    {
        [Required]
        public int Id { get; set; }
        [DisplayName("Mekan Adı")]
        [Required(ErrorMessage = "{0} alanı boş geçilmemelidir.")]
        [MaxLength(70, ErrorMessage = "{0} alanı {1} karakterden uzun olmamalıdır.")]
        [MinLength(5, ErrorMessage = "{0} alanı {1} karakterden kısa olmamalıdır.")]
        public string Name { get; set; }

        [DisplayName("Adres")]
        [Required(ErrorMessage = "{0} alanı boş geçilmemelidir.")]
        [MaxLength(500, ErrorMessage = "{0} alanı {1} karakterden uzun olmamalıdır.")]
        [MinLength(10, ErrorMessage = "{0} alanı {1} karakterden kısa olmamalıdır.")]
        public string Address { get; set; }

        [DisplayName("Mekan Görsel")]
        [Required(ErrorMessage = "{0} alanı boş geçilmemelidir.")]
        [MaxLength(250, ErrorMessage = "{0} alanı {1} karakterden uzun olmamalıdır.")]
        [MinLength(5, ErrorMessage = "{0} alanı {1} karakterden kısa olmamalıdır.")]
        public string PlacePicture { get; set; }

        [DisplayName("Şehir")]
        [Required(ErrorMessage = "{0} alanı boş geçilmemelidir.")]
        public int CityName { get; set; }
        public City City { get; set; }

        [DisplayName("Kategori")]
        [Required(ErrorMessage = "{0} alanı boş geçilmemelidir.")]
        public int CategoryName { get; set; }
        public Category Category { get; set; }

        [DisplayName("Aktif mi?")]
        [Required(ErrorMessage = "{0} alanı boş geçilmemelidir.")]
        public bool IsActive { get; set; }

        [DisplayName("Silinsin mi?")]
        [Required(ErrorMessage = "{0} alanı boş geçilmemelidir.")]
        public bool IsDeleted { get; set; }
    }
}
