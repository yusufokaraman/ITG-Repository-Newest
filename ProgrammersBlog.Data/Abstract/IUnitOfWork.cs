using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Data.Abstract
{
    public interface IUnitOfWork:IAsyncDisposable
    {
        /// <summary>
        /// her seferinde repositoryleri ayrı ayrı çağırmamak  adına bu çalışma yapılmıştır.
        /// EntityBase veya EfEntityRepositoryBase de save metodu oluşturmadım. Veritabanına işleyebilmek adına burada implemente ediyorum.
        /// </summary>
        IArticleRepository Articles { get; } // unitofwork.Articles
        ICategoryRepository Categories { get; }
        ICommentRepository Comments { get; }
        // _unitOfWork.Categories.AddAsync();
        // _unitOfWork.Categories.AddAsync(category);
        //_unitOfWork.Users.AddAsync(user);
        //_unitOfWork.SaveAsync();
        ICityRepository Cities { get; }
        IPlaceRepository Places { get; }
        Task<int> SaveAsync();
    }
}
