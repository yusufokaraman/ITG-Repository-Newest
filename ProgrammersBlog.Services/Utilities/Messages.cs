﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Services.Utilities
{
    public static class Messages
    {
        public static class General
        {
            public static string ValidationError()
            {
                return $"Bir veya daha fazla validasyon hatası ile karşılaşıldı.";
            }
        }
        public static class Category
        {
            public static string NotFound(bool isPlural)
            {
                if (isPlural) return "Hiç bir kategori bulunamadı.";
                return "Böyle bir kategori bulunamadı.";
            }
            public static string NotFoundById(int categoryId)
            {
                return $"{categoryId} kategori koduna ait bir kategori bulunamadı.";
            }
            public static string Add(string categoryName)
            {
                return $"{categoryName} adlı kategori başarıyla eklenmiştir.";
            }

            public static string Update(string categoryName)
            {
                return $"{categoryName} adlı kategori başarıyla güncellenmiştir.";
            }
            public static string Delete(string categoryName)
            {
                return $"{categoryName} adlı kategori başarıyla silinmiştir.";
            }
            public static string HardDelete(string categoryName)
            {
                return $"{categoryName} adlı kategori başarıyla veritabanından silinmiştir.";
            }
            public static string UndoDelete(string categoryName)
            {
                return $"{categoryName} adlı kategori başarıyla arşivden geri getirilmiştir.";
            }
        }

        public static class Article
        {
            public static string NotFound(bool isPlural)
            {
                if (isPlural) return "Makaleler bulunamadı.";
                return "Böyle bir makale bulunamadı.";
            }
            public static string NotFoundById(int articleId)
            {
                return $"{articleId} makale koduna ait bir makale bulunamadı.";
            }
            public static string Add(string articleTitle)
            {
                return $"{articleTitle} başlıklı makale başarıyla eklenmiştir.";
            }

            public static string Update(string articleTitle)
            {
                return $"{articleTitle} başlıklı makale başarıyla güncellenmiştir.";
            }
            public static string Delete(string articleTitle)
            {
                return $"{articleTitle} başlıklı makale başarıyla silinmiştir.";
            }
            public static string HardDelete(string articleTitle)
            {
                return $"{articleTitle} başlıklı makale başarıyla veritabanından silinmiştir.";
            }
            public static string UndoDelete(string articleTitle)
            {
                return $"{articleTitle} başlıklı makale başarıyla arşivden geri getirilmiştir.";
            }
            public static string IncreaseViewCount(string articleTitle)
            {
                return $"{articleTitle} başlıklı makale'nin okunma sayısı başarıyla arttırılmıştır.";
            }
        }
        public static class Place
        {
            public static string NotFound(bool isPlural)
            {
                if (isPlural) return "Adresler bulunamadı.";
                return "Böyle bir adres bulunamadı.";
            }
            public static string NotFoundById(int placeId)
            {
                return $"{placeId} adres koduna ait bir adres bulunamadı.";
            }
            public static string Add(string placeName)
            {
                return $"{placeName} başlıklı adres başarıyla eklenmiştir.";
            }

            public static string Update(string placeName)
            {
                return $"{placeName} başlıklı adres başarıyla güncellenmiştir.";
            }
            public static string Delete(string placeName)
            {
                return $"{placeName} başlıklı adres başarıyla silinmiştir.";
            }
            public static string HardDelete(string placeName)
            {
                return $"{placeName} başlıklı adres başarıyla veritabanından silinmiştir.";
            }
            public static string UndoDelete(string placeName)
            {
                return $"{placeName} başlıklı adres başarıyla arşivden geri getirilmiştir.";
            }
            public static string IncreaseViewCount(string placeName)
            {
                return $"{placeName} isimli adresin okunma sayısı başarıyla arttırılmıştır.";
            }
        }
        public static class Comment
        {
            public static string NotFound(bool isPlural)
            {
                if (isPlural) return "Hiç bir yorum bulunamadı.";
                return "Böyle bir yorum bulunamadı.";
            }

            public static string Approve(int commentId)
            {
                return $"{commentId} no'lu yorum başarıyla onaylanmıştır.";
            }
            public static string Add(string createdByName)
            {
                return $"Sayın {createdByName}, yorumunuz başarıyla eklenmiştir.";
            }

            public static string Update(string createdByName)
            {
                return $"{createdByName} tarafından eklenen yorum başarıyla güncellenmiştir.";
            }
            public static string Delete(string createdByName)
            {
                return $"{createdByName} tarafından eklenen yorum başarıyla silinmiştir.";
            }
            public static string HardDelete(string createdByName)
            {
                return $"{createdByName} tarafından eklenen yorum başarıyla veritabanından silinmiştir.";
            }
            public static string UndoDelete(string createdByName)
            {
                return $"{createdByName} tarafından eklenen yorum başarıyla arşivden geri getirilmiştir.";
            }
        }
        public static class User
        {
            //public static string NotFound(bool isPlural)
            //{
            //    if (isPlural) return "Hiç bir kategori bulunamadı.";
            //    return "Böyle bir kategori bulunamadı.";
            //}
            public static string NotFoundById(int userId)
            {
                return $"{userId} kullanıcı koduna ait bir kullanıcı bulunamadı.";
            }
            //public static string Add(string categoryName)
            //{
            //    return $"{categoryName} adlı kategori başarıyla eklenmiştir.";
            //}

            //public static string Update(string categoryName)
            //{
            //    return $"{categoryName} adlı kategori başarıyla güncellenmiştir.";
            //}
            //public static string Delete(string categoryName)
            //{
            //    return $"{categoryName} adlı kategori başarıyla silinmiştir.";
            //}
            //public static string HardDelete(string categoryName)
            //{
            //    return $"{categoryName} adlı kategori başarıyla veritabanından silinmiştir.";
            //}
            //public static string UndoDelete(string categoryName)
            //{
            //    return $"{categoryName} adlı kategori başarıyla arşivden geri getirilmiştir.";
            //}
        }
    }
}
