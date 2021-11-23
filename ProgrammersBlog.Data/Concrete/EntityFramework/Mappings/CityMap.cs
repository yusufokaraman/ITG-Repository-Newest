using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgrammersBlog.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Data.Concrete.EntityFramework.Mappings
{
    public class CityMap : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.Name).HasMaxLength(70);
            builder.Property(c => c.Content).IsRequired();
            builder.Property(a => a.Content).HasColumnType("NVARCHAR(MAX)");
            builder.Property(a => a.Thumbnail).IsRequired();
            builder.Property(a => a.Thumbnail).HasMaxLength(250);
            builder.Property(c => c.CreatedByName).IsRequired();
            builder.Property(c => c.CreatedByName).HasMaxLength(50);
            builder.Property(c => c.ModifiedByName).IsRequired();
            builder.Property(c => c.ModifiedByName).HasMaxLength(50);
            builder.Property(c => c.CreatedDate).IsRequired();
            builder.Property(c => c.ModifiedDate).IsRequired();
            builder.Property(c => c.IsActive).IsRequired();
            builder.Property(c => c.IsDeleted).IsRequired();
            builder.Property(c => c.Note).HasMaxLength(500);
            builder.ToTable("Cities");
            builder.HasData(new City
            {
                Id = 1,
                Name = "Adana",
                Content = "Adana, Türkiye'nin bir ili ve en kalabalık altıncı şehridir. 2019 yılı verilerine göre 2.258.718 nüfusa sahiptir. İlin yüz ölçümü 13.844 km²dir. İlde km²ye 160 kişi düşmektedir. 01.02.2018 TÜİK verilerine göre 5'i merkez ilçe olmak üzere toplam 15 ilçesi ve belediyesi vardır. Bu ilçelerde 831 mahalle bulunmaktadır.",
                Thumbnail = "Default.jpg",
                IsActive = true,
                IsDeleted = false,
                CreatedByName = "InitialCreate",
                CreatedDate = DateTime.Now,
                ModifiedByName = "InitialCreate",
                ModifiedDate = DateTime.Now,
                Note = "01 Plakalı il Adana.",
            },
           new City
           {
               Id = 2,
               Name = "Adıyaman",
               Content = "Adıyaman, aynı isimli ilin merkez ilçesidir. Adıyaman merkez ilçesinin nüfusu 2020 istatistiklerine 310.644'dür. ",
               Thumbnail = "Default.jpg",
               IsActive = true,
               IsDeleted = false,
               CreatedByName = "InitialCreate",
               CreatedDate = DateTime.Now,
               ModifiedByName = "InitialCreate",
               ModifiedDate = DateTime.Now,
               Note = "02 Plakalı il Adıyaman."
           },
           new City
           {
               Id = 3,
               Name = "Afyon",
               Content = "Afyonkarahisar veya eski ve halk arasındaki ismiyle Afyon, aynı isimli ilin merkezidir. Mermercilik ve gıda sektöründe Türkiye içinde ve dışında isim yapmıştır. Şehrin Afyon olan ismi, 2005 yılında Afyonkarahisar olarak değiştirilmiştir. ",
               Thumbnail = "Default.jpg",
               IsActive = true,
               IsDeleted = false,
               CreatedByName = "InitialCreate",
               CreatedDate = DateTime.Now,
               ModifiedByName = "InitialCreate",
               ModifiedDate = DateTime.Now,
               Note = "03 Plakalı il Afyon."
           }
           );
        }
    }
}
