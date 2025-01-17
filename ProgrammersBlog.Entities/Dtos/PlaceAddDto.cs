﻿using ProgrammersBlog.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entities.Dtos
{
    public class PlaceAddDto
    {
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


        [DisplayName("Kategori")]
        [Required(ErrorMessage = "{0} alanı boş geçilmemelidir.")]
        public int CategoryName { get; set; }
        public Category Category { get; set; }

        [DisplayName("Aktif Mi?")]
        [Required(ErrorMessage = "{0} alanı boş geçilmemelidir.")]
        public bool IsActive { get; set; }
    }
}
