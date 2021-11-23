using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgrammersBlog.Entities.Concrete;

namespace ProgrammersBlog.Data.Concrete.EntityFramework.Mappings
{
   public class CategoryMap:IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.Name).HasMaxLength(70);
            builder.Property(c => c.Description).HasMaxLength(500);
            builder.Property(c => c.CreatedByName).IsRequired();
            builder.Property(c => c.CreatedByName).HasMaxLength(50);
            builder.Property(c => c.ModifiedByName).IsRequired();
            builder.Property(c => c.ModifiedByName).HasMaxLength(50);
            builder.Property(c => c.CreatedDate).IsRequired();
            builder.Property(c => c.ModifiedDate).IsRequired();
            builder.Property(c => c.IsActive).IsRequired();
            builder.Property(c => c.IsDeleted).IsRequired();
            builder.Property(c => c.Note).HasMaxLength(500);
            builder.HasOne<City>(c => c.City).WithMany(c => c.Categories).HasForeignKey(a => a.CityId).OnDelete(DeleteBehavior.NoAction);
            builder.ToTable("Categories");

            builder.HasData(
                 new Category
                 {
                     Id = 1,
                     Name = "Yemek",
                     CityId = 1,
                     Description = "Yemek yenilebilecek yerler ile ilgili oluşturulmuş kategoridir.",
                     IsActive = true,
                     IsDeleted = false,
                     CreatedByName = "InitialCreate",
                     CreatedDate = DateTime.Now,
                     ModifiedByName = "InitialCreate",
                     ModifiedDate = DateTime.Now,
                     Note = "Yemek Turist Rehberi Kategorisi",


                 },
             new Category
             {
                 Id = 2,
                 Name = "Tarihi Gezi",
                 CityId = 1,
                 Description = "Müze ve tarihsel yerler için oluşturulmuş kategoridir.",
                 IsActive = true,
                 IsDeleted = false,
                 CreatedByName = "InitialCreate",
                 CreatedDate = DateTime.Now,
                 ModifiedByName = "InitialCreate",
                 ModifiedDate = DateTime.Now,
                 Note = "Tarihi Gezi Turist Rehberi Kategorisi",


             },
             new Category
             {
                 Id = 3,
                 Name = "Doğa Gezisi",
                 CityId = 1,
                 Description = "Doğal Parklar için oluşturulmuş kategoridir.",
                 IsActive = true,
                 IsDeleted = false,
                 CreatedByName = "InitialCreate",
                 CreatedDate = DateTime.Now,
                 ModifiedByName = "InitialCreate",
                 ModifiedDate = DateTime.Now,
                 Note = "Doğal Parklar Turist Rehberi Kategorisi",

             }


             );
        }
    }
}
