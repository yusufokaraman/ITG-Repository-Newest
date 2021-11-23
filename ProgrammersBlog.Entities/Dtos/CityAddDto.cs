using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entities.Dtos
{
    public class CityAddDto
    {
        [DisplayName("Şehir Adı")]
        [Required(ErrorMessage = "{0} Boş Geçilmemelidir.")]
        [MaxLength(70, ErrorMessage = "{0} {1} karakterden uzun olmamalıdır.")]
        [MinLength(3, ErrorMessage = "{0} {1} karakterden kısa olmamalıdır.")]
        public string Name { get; set; }

        [DisplayName("İçerik")]
        [MinLength(3, ErrorMessage = "{0} {1} karakterden kısa olmamalıdır.")]
        public string Content { get; set; }

        [DisplayName("Görsel")]
        [MaxLength(250, ErrorMessage = "{0} {1} karakterden uzun olmamalıdır.")]
        [MinLength(3, ErrorMessage = "{0} {1} karakterden kısa olmamalıdır.")]
        public string Thumbnail { get; set; }

        [DisplayName("Aktif mi?")]
        [Required(ErrorMessage = "{0} Boş Geçilmemelidir.")]
        public bool IsActive { get; set; }
    }
}
