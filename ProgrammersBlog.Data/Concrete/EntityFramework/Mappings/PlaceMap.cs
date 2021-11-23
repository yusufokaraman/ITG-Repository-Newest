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
            builder.HasOne<City>(p => p.City).WithMany(c => c.Places).HasForeignKey(p => p.CityId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne<Category>(p => p.Category).WithMany(c => c.Places).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.NoAction);
            builder.ToTable("Places");
            builder.HasData(new Place
            {
                Id = 1,
                Name = "Adana Kebapçısı",
                Address = "Adana Merkez,Adana Kebapçısı",
                PlacePicture = "Default.jpg",
                IsActive = true,
                IsDeleted = false,
                CreatedByName = "InitialCreate",
                CreatedDate = DateTime.Now,
                ModifiedByName = "InitialCreate",
                ModifiedDate = DateTime.Now,
                Note = "Adana'da yer alan kebapçı",
                CityId = 1,
                CategoryId = 1

            },
            new Place
            {
                Id = 2,
                Name = "Adıyaman Ev Yemekleri",
                Address = "Adıyaman Ev Yemekler, Merkez-Adıyaman",
                PlacePicture = "Default.jpg",
                IsActive = true,
                IsDeleted = false,
                CreatedByName = "InitialCreate",
                CreatedDate = DateTime.Now,
                ModifiedByName = "InitialCreate",
                ModifiedDate = DateTime.Now,
                Note = "Adıyaman'da faaliyer gösteren ev yemekleri restoranı.",
                CityId = 2,
                CategoryId = 1
            },
            new Place
            {
                Id = 3,
                Name = "Adana Varda Köprüsü",
                Address = "Adana Varda Köprüsü,Merkez Adana",
                PlacePicture = "Default.jpg",
                IsActive = true,
                IsDeleted = false,
                CreatedByName = "InitialCreate",
                CreatedDate = DateTime.Now,
                ModifiedByName = "InitialCreate",
                ModifiedDate = DateTime.Now,
                Note = "Adana'da bulunan tarihi Varda Köprüsü.",
                CategoryId = 2,
                CityId = 3
            }
            );
        }
    }
}
