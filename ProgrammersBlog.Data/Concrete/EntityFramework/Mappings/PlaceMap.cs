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
    public class PlaceMap : IEntityTypeConfiguration<Place>
    {
        public void Configure(EntityTypeBuilder<Place> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Name).IsRequired();
            builder.Property(p => p.Name).HasMaxLength(70);
            builder.Property(p => p.Address).IsRequired();
            builder.Property(p => p.Address).HasMaxLength(500);
            builder.Property(a => a.Date).IsRequired();
            builder.Property(a => a.SeoAuthor).IsRequired();
            builder.Property(a => a.SeoAuthor).HasMaxLength(50);
            builder.Property(a => a.SeoDescription).HasMaxLength(150);
            builder.Property(a => a.SeoDescription).IsRequired();
            builder.Property(a => a.SeoTags).IsRequired();
            builder.Property(a => a.SeoTags).HasMaxLength(70);
            builder.Property(a => a.ViewCount).IsRequired();
            builder.Property(a => a.CommentCount).IsRequired();
            builder.Property(p => p.PlacePicture).IsRequired();
            builder.Property(p => p.PlacePicture).HasMaxLength(250);
            builder.Property(p => p.CreatedByName).IsRequired();
            builder.Property(p => p.CreatedByName).HasMaxLength(50);
            builder.Property(p => p.ModifiedByName).IsRequired();
            builder.Property(p => p.ModifiedByName).HasMaxLength(50);
            builder.Property(p => p.CreatedDate).IsRequired();
            builder.Property(p => p.ModifiedDate).IsRequired();
            builder.Property(p => p.IsActive).IsRequired();
            builder.Property(p => p.IsDeleted).IsRequired();
            builder.Property(p => p.Note).HasMaxLength(500);
            builder.HasOne<Category>(p => p.Category).WithMany(c => c.Places).HasForeignKey(p => p.CategoryId);
            builder.HasOne<User>(p => p.User).WithMany(u => u.Places).HasForeignKey(p => p.UserId);
            builder.ToTable("Places");
            builder.HasData(new Place
            {
                Id = 1,
                Name = "Adana Kebapçısı",
                Address = "Adana Merkez,Adana Kebapçısı",
                PlacePicture = "Default.jpg",
                SeoDescription = "Adana Yemek Kültürü",
                SeoTags = "Adana, Kebap, Yemek",
                SeoAuthor = "Yusuf Karaman",
                Date = DateTime.Now,
                IsActive = true,
                IsDeleted = false,
                CreatedByName = "InitialCreate",
                CreatedDate = DateTime.Now,
                ModifiedByName = "InitialCreate",
                ModifiedDate = DateTime.Now,
                Note = "Adana'da yer alan kebapçı",
                UserId=1,
                CategoryId = 1,
                ViewCount = 100,
                CommentCount = 1

            },
            new Place
            {
                Id = 2,
                Name = "Adıyaman Ev Yemekleri",
                Address = "Adıyaman Ev Yemekler, Merkez-Adıyaman",
                PlacePicture = "Default.jpg",
                SeoDescription = "Adıyaman Yemek Kültürü",
                SeoTags = "Adıyaman, Kebap, Yemek",
                SeoAuthor = "Yusuf Karaman",
                Date = DateTime.Now,
                IsActive = true,
                IsDeleted = false,
                CreatedByName = "InitialCreate",
                CreatedDate = DateTime.Now,
                ModifiedByName = "InitialCreate",
                ModifiedDate = DateTime.Now,
                Note = "Adıyaman'da faaliyer gösteren ev yemekleri restoranı.",
                UserId=1,
                CategoryId = 1,
                ViewCount = 100,
                CommentCount = 1
            },
            new Place
            {
                Id = 3,
                Name = "Adana Varda Köprüsü",
                Address = "Adana Varda Köprüsü,Merkez Adana",
                PlacePicture = "Default.jpg",
                SeoDescription = "Adana Tarihi Yerler",
                SeoTags = "Adana, Kültür,Tarih,Vanda,Kebap",
                SeoAuthor = "Yusuf Karaman",
                Date = DateTime.Now,
                IsActive = true,
                IsDeleted = false,
                CreatedByName = "InitialCreate",
                CreatedDate = DateTime.Now,
                ModifiedByName = "InitialCreate",
                ModifiedDate = DateTime.Now,
                Note = "Adana'da bulunan tarihi Varda Köprüsü.",
                UserId=1,
                CategoryId = 2,
                ViewCount = 100,
                CommentCount = 1

            }
            );
        }
    }
}
