using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProgrammersBlog.Data.Abstract;
using ProgrammersBlog.Entities.ComplexTypes;
using ProgrammersBlog.Entities.Concrete;
using ProgrammersBlog.Entities.Dtos;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Services.Utilities;
using ProgrammersBlog.Shared.Entities.Concrete;
using ProgrammersBlog.Shared.Utilities.Results.Abstract;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using ProgrammersBlog.Shared.Utilities.Results.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Services.Concrete
{
    /// <summary>
    /// Add,Update,Delete ve Hard Delete bölümlerinde değişiklik yapılmıştır.
    /// </summary>


    /// Update,Delete,Add ve Hard Delete kısımlarında DbContext'in Thread-Safe olmamasından kaynaklı ".ContinueWith(t => _unitOfWork.SaveAsync());" "await _unitOfWork.SaveAsync();"
    /// arasında değişikliğe gidilmiştir. Thread-Safe farklı thread'ler üzerinden yani farklı iş parçacıkları üzerinden DbContext'i çağırmak ve kullanmaktır.
    /// Daha açık olmak gerekirse AddAsync metodu çağırıldığında SaveAsync metodunu bir task olarak veriyoruz.Await keywordü sayesinde AddAsync'in bitmesini bekliyoruz.
    /// Metot bittiği gibi arkaplanda SaveAsync metodunu çalıştırıyoruz.Dolayısıyla bu işlem arkplanda çalıştığı için farklı bir iş parçacığında çalışıyor.Diğer thread bir alt satırdan devam edip
    /// controller içine geri döndüğünde arkplanda da bir tane SaveAsync metodu çalışıyor. Bu bize büyük oranda hız kazandırıyor.
    /// Sorun işe şurda oluşmakta; controller a geri döndüğümüzde -özellikle AJAX işlemlerinde başımıza gelmek- bizler hala istek yapmaya devam edip farklı bir istekte bulunduğumuzda Örnk:GetAll();
    /// Arkaplanda DbContext SaveAsync metodu ile kayıt edilirken bizler tekrar DbContext'i ikinci bir iş parçacığından kullanmaya çalıştığımızda bizlere bir hata fırlatmakta. Bize thread üzerinde
    /// halihazırda bri DbContex'in çalıştığını, ikinci bir thread'in bunu beklemesini gerektiğini söylemekte.Kısaca hızlı işliyor olmamızdan bir hata ile karşılaşmaktayız. 
    /// Bu sorunu yine await ile çözmekteyiz. "await _unitOfWork.SaveAsync();" satırındaki await işlemi ilgili kod parçaçığı bittikten sonra return işlemini  yapıp controller a geri dönmekte. 
    /// Bu sayede her bir thread kendi DbContext'i ile çalıştığı için bizler bu Scope içerisinde hiçbir sorun yaşamadan işlemlerimiz gerçekleştiriyoruz.
    public class PlaceManager : ManagerBase,IPlaceService
    {
        private readonly UserManager<User> _userManager;

        public PlaceManager(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager) : base(unitOfWork, mapper)
        {
            _userManager = userManager;
        }

        public async Task<IResult> AddAsync(PlaceAddDto placeAddDto, string createdByName, int userId)
        {
            var place = Mapper.Map<Place>(placeAddDto);
            place.CreatedByName = createdByName;
            place.ModifiedByName = createdByName;
            place.UserId = userId;
            await UnitOfWork.Places.AddAsync(place);
            await UnitOfWork.SaveAsync();
            return new Result(ResultStatus.Success, Messages.Place.Add(place.Name));
        }

        public async Task<IDataResult<int>> CountAsync()
        {
            var placesCount = await UnitOfWork.Places.CountAsync();
            if (placesCount > -1)
            {
                return new DataResult<int>(ResultStatus.Success, placesCount);
            }
            else
            {
                return new DataResult<int>(ResultStatus.Error, $"Beklenmeyen bir hata ile karşılaşıldı.", -1);
            }
        }

        public async Task<IDataResult<int>> CountByNonDeletedAsync()
        {
            var placesCount = await UnitOfWork.Places.CountAsync(a => !a.IsDeleted);
            if (placesCount > -1)
            {
                return new DataResult<int>(ResultStatus.Success, placesCount);
            }
            else
            {
                return new DataResult<int>(ResultStatus.Error, $"Beklenmeyen bir hata ile karşılaşıldı.", -1);
            }
        }

        public async Task<IResult> DeleteAsync(int placeId, string modifiedByName)
        {
            var result = await UnitOfWork.Places.AnyAsync(a => a.Id == placeId);
            if (result)
            {
                var place = await UnitOfWork.Places.GetAsync(a => a.Id == placeId);
                place.IsDeleted = true;
                place.IsActive = false;
                place.ModifiedByName = modifiedByName;
                place.ModifiedDate = DateTime.Now;
                await UnitOfWork.Places.UpdateAsync(place);
                await UnitOfWork.SaveAsync();
                return new Result(ResultStatus.Success, Messages.Place.Delete(place.Name));
            }
            return new Result(ResultStatus.Error, Messages.Article.NotFound(isPlural: false));
        }

        public async Task<IDataResult<PlaceListDto>> GetAllAsync()
        {
            var places = await UnitOfWork.Places.GetAllAsync(null, a => a.User, a => a.Category);
            if (places.Count > -1)
            {
                return new DataResult<PlaceListDto>(ResultStatus.Success, new PlaceListDto
                {
                    Places = places,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<PlaceListDto>(ResultStatus.Error, Messages.Place.NotFound(isPlural: true), null);
        }

        public async Task<IDataResult<PlaceListDto>> GetAllAsyncV2(int? categoryId, int? userId, bool? isActive, bool? isDeleted, int currentPage, int pageSize, OrderByGeneral orderBy, bool isAscending, bool includeCategory, bool includeComments, bool includeUser)
        {
            List<Expression<Func<Place, bool>>> predicates = new List<Expression<Func<Place, bool>>>();
            List<Expression<Func<Place, object>>> includes = new List<Expression<Func<Place, object>>>();

            
            //Predicates
            if (categoryId.HasValue)
            {
                if (!await UnitOfWork.Categories.AnyAsync(c => c.Id == categoryId.Value))
                {
                    return new DataResult<PlaceListDto>(ResultStatus.Warning, Messages.General.ValidationError(), null,
                        new List<ValidationError>
                        {
                            new ValidationError
                            {
                                PropertyName = "categoryId",
                                Message = Messages.Category.NotFoundById(categoryId.Value)
                            }
                        });
                }
                predicates.Add(a => a.CategoryId == categoryId.Value);
            }
            if (userId.HasValue)
            {
                if (!await _userManager.Users.AnyAsync(u => u.Id == userId.Value))
                {
                    return new DataResult<PlaceListDto>(ResultStatus.Warning, Messages.General.ValidationError(), null,
                        new List<ValidationError>
                        {
                            new ValidationError
                            {
                                PropertyName = "userId",
                                Message = Messages.User.NotFoundById(userId.Value)
                            }
                        });
                }
                predicates.Add(a => a.UserId == userId.Value);
            }
            if (isActive.HasValue) predicates.Add(a => a.IsActive == isActive.Value);
            if (isDeleted.HasValue) predicates.Add(a => a.IsDeleted == isDeleted.Value);
            //Includes
            if (includeCategory) includes.Add(a => a.Category);
            if (includeComments) includes.Add(a => a.Comments);
            if (includeUser) includes.Add(a => a.User);
            var places = await UnitOfWork.Places.GetAllAsyncV2(predicates, includes);

            IOrderedEnumerable<Place> sortedPlaces;

            switch (orderBy)
            {
                case OrderByGeneral.Id:
                    sortedPlaces = isAscending ? places.OrderBy(a => a.Id) : places.OrderByDescending(a => a.Id);
                    break;
                case OrderByGeneral.Az:
                    sortedPlaces = isAscending ? places.OrderBy(a => a.Name) : places.OrderByDescending(a => a.Name);
                    break;
                // Default CreatedDate
                default:
                    sortedPlaces = isAscending ? places.OrderBy(a => a.CreatedDate) : places.OrderByDescending(a => a.CreatedDate);
                    break;
            }

            return new DataResult<PlaceListDto>(ResultStatus.Success, new PlaceListDto
            {
                Places = sortedPlaces.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList(),
                CategoryId = categoryId.HasValue ? categoryId.Value : null,
                CurrentPage = currentPage,
                PageSize = pageSize,
                IsAscending = isAscending,
                TotalCount = places.Count,
                ResultStatus = ResultStatus.Success
            });
        }

        public async Task<IDataResult<PlaceListDto>> GetAllByCategoryAsync(int categoryId)
        {
            var result = await UnitOfWork.Categories.AnyAsync(c => c.Id == categoryId);
            if (result)
            {
                var places = await UnitOfWork.Places.GetAllAsync(
                    a => a.CategoryId == categoryId && !a.IsDeleted && a.IsActive, ar => ar.User, ar => ar.Category);
                if (places.Count > -1)
                {
                    return new DataResult<PlaceListDto>(ResultStatus.Success, new PlaceListDto
                    {
                        Places = places,
                        ResultStatus = ResultStatus.Success
                    });
                }
                return new DataResult<PlaceListDto>(ResultStatus.Error, Messages.Place.NotFound(isPlural: true), null);
            }
            return new DataResult<PlaceListDto>(ResultStatus.Error, Messages.Category.NotFound(isPlural: false), null);
        }

        public async Task<IDataResult<PlaceListDto>> GetAllByDeletedAsync()
        {
            var places =
                await UnitOfWork.Places.GetAllAsync(a => a.IsDeleted, ar => ar.User,
                    ar => ar.Category);
            if (places.Count > -1)
            {
                return new DataResult<PlaceListDto>(ResultStatus.Success, new PlaceListDto
                {
                    Places = places,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<PlaceListDto>(ResultStatus.Error, Messages.Place.NotFound(isPlural: true), null);
        }

        public async Task<IDataResult<PlaceListDto>> GetAllByNonDeletedAndActiveAsync()
        {
            var places =
                await UnitOfWork.Places.GetAllAsync(a => !a.IsDeleted && a.IsActive, ar => ar.User,
                    ar => ar.Category);
            if (places.Count > -1)
            {
                return new DataResult<PlaceListDto>(ResultStatus.Success, new PlaceListDto
                {
                    Places = places,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<PlaceListDto>(ResultStatus.Error, Messages.Place.NotFound(isPlural: true), null);
        }

        public async Task<IDataResult<PlaceListDto>> GetAllByNonDeletedAsync()
        {
            var places = await UnitOfWork.Places.GetAllAsync(a => !a.IsDeleted, ar => ar.User, ar => ar.Category);
            if (places.Count > -1)
            {
                return new DataResult<PlaceListDto>(ResultStatus.Success, new PlaceListDto
                {
                    Places = places,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<PlaceListDto>(ResultStatus.Error, Messages.Place.NotFound(isPlural: true), null);
        }

        public async Task<IDataResult<PlaceListDto>> GetAllByPagingAsync(int? categoryId, int currentPage = 1, int pageSize = 5, bool isAscending = false)
        {
            pageSize = pageSize > 20 ? 20 : pageSize;
            var places = categoryId == null
                ? await UnitOfWork.Places.GetAllAsync(a => a.IsActive && !a.IsDeleted, a => a.Category, a => a.User)
                : await UnitOfWork.Places.GetAllAsync(a => a.CategoryId == categoryId && a.IsActive && !a.IsDeleted,
                    a => a.Category, a => a.User);
            var sortedPlaces = isAscending
                ? places.OrderBy(a => a.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
                : places.OrderByDescending(a => a.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            return new DataResult<PlaceListDto>(ResultStatus.Success, new PlaceListDto
            {
                Places = sortedPlaces,
                CategoryId = categoryId == null ? null : categoryId.Value,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalCount = places.Count,
                IsAscending = isAscending
            });
        }

        public async Task<IDataResult<PlaceListDto>> GetAllByUserIdOnFilter(int userId, FilterBy filterBy, OrderBy orderBy, bool isAscending, int takeSize, int categoryId, DateTime startAt, DateTime endAt, int minViewCount, int maxViewCount, int minCommentCount, int maxCommentCount)
        {
            var anyUser = await _userManager.Users.AnyAsync(u => u.Id == userId);
            if (!anyUser)
            {
                return new DataResult<PlaceListDto>(ResultStatus.Error, $"{userId} numaralı kullanıcı bulunamadı.",
                    null);
            }

            var userPlaces =
                await UnitOfWork.Places.GetAllAsync(a => a.IsActive && !a.IsDeleted && a.UserId == userId);
            List<Place> sortedPlaces = new List<Place>();
            switch (filterBy)
            {
                case FilterBy.Category:
                    switch (orderBy)
                    {
                        case OrderBy.Date:
                            sortedPlaces = isAscending
                                ? userPlaces.Where(a => a.CategoryId == categoryId).Take(takeSize)
                                    .OrderBy(a => a.Date).ToList()
                                : userPlaces.Where(a => a.CategoryId == categoryId).Take(takeSize)
                                    .OrderByDescending(a => a.Date).ToList();
                            break;
                        case OrderBy.ViewCount:
                            sortedPlaces = isAscending
                                ? userPlaces.Where(a => a.CategoryId == categoryId).Take(takeSize)
                                    .OrderBy(a => a.ViewCount).ToList()
                                : userPlaces.Where(a => a.CategoryId == categoryId).Take(takeSize)
                                    .OrderByDescending(a => a.ViewCount).ToList();
                            break;
                        case OrderBy.CommentCount:
                            sortedPlaces = isAscending
                                ? userPlaces.Where(a => a.CategoryId == categoryId).Take(takeSize)
                                    .OrderBy(a => a.CommentCount).ToList()
                                : userPlaces.Where(a => a.CategoryId == categoryId).Take(takeSize)
                                    .OrderByDescending(a => a.CommentCount).ToList();
                            break;
                    }
                    break;
                case FilterBy.Date:
                    switch (orderBy)
                    {
                        case OrderBy.Date:
                            sortedPlaces = isAscending
                                ? userPlaces.Where(a => a.Date >= startAt && a.Date <= endAt).Take(takeSize)
                                    .OrderBy(a => a.Date).ToList()
                                : userPlaces.Where(a => a.Date >= startAt && a.Date <= endAt).Take(takeSize)
                                    .OrderByDescending(a => a.Date).ToList();
                            break;
                        case OrderBy.ViewCount:
                            sortedPlaces = isAscending
                                ? userPlaces.Where(a => a.Date >= startAt && a.Date <= endAt).Take(takeSize)
                                    .OrderBy(a => a.ViewCount).ToList()
                                : userPlaces.Where(a => a.Date >= startAt && a.Date <= endAt).Take(takeSize)
                                    .OrderByDescending(a => a.ViewCount).ToList();
                            break;
                        case OrderBy.CommentCount:
                            sortedPlaces = isAscending
                                ? userPlaces.Where(a => a.Date >= startAt && a.Date <= endAt).Take(takeSize)
                                    .OrderBy(a => a.CommentCount).ToList()
                                : userPlaces.Where(a => a.Date >= startAt && a.Date <= endAt).Take(takeSize)
                                    .OrderByDescending(a => a.CommentCount).ToList();
                            break;
                    }
                    break;
                case FilterBy.ViewCount:
                    switch (orderBy)
                    {
                        case OrderBy.Date:
                            sortedPlaces = isAscending
                                ? userPlaces.Where(a => a.ViewCount >= minViewCount && a.ViewCount <= maxViewCount).Take(takeSize)
                                    .OrderBy(a => a.Date).ToList()
                                : userPlaces.Where(a => a.ViewCount >= minViewCount && a.ViewCount <= maxViewCount).Take(takeSize)
                                    .OrderByDescending(a => a.Date).ToList();
                            break;
                        case OrderBy.ViewCount:
                            sortedPlaces = isAscending
                                ? userPlaces.Where(a => a.ViewCount >= minViewCount && a.ViewCount <= maxViewCount).Take(takeSize)
                                    .OrderBy(a => a.ViewCount).ToList()
                                : userPlaces.Where(a => a.ViewCount >= minViewCount && a.ViewCount <= maxViewCount).Take(takeSize)
                                    .OrderByDescending(a => a.ViewCount).ToList();
                            break;
                        case OrderBy.CommentCount:
                            sortedPlaces = isAscending
                                ? userPlaces.Where(a => a.ViewCount >= minViewCount && a.ViewCount <= maxViewCount).Take(takeSize)
                                    .OrderBy(a => a.CommentCount).ToList()
                                : userPlaces.Where(a => a.ViewCount >= minViewCount && a.ViewCount <= maxViewCount).Take(takeSize)
                                    .OrderByDescending(a => a.CommentCount).ToList();
                            break;
                    }
                    break;
                case FilterBy.CommentCount:
                    switch (orderBy)
                    {
                        case OrderBy.Date:
                            sortedPlaces = isAscending
                                ? userPlaces.Where(a =>
                                        a.CommentCount >= minCommentCount && a.CommentCount <= maxCommentCount)
                                    .Take(takeSize)
                                    .OrderBy(a => a.Date).ToList()
                                : userPlaces.Where(a =>
                                        a.CommentCount >= minCommentCount && a.CommentCount <= maxCommentCount)
                                    .Take(takeSize)
                                    .OrderByDescending(a => a.Date).ToList();
                            break;
                        case OrderBy.ViewCount:
                            sortedPlaces = isAscending
                                ? userPlaces.Where(a => a.CommentCount >= minCommentCount && a.CommentCount <= maxCommentCount)
                                    .Take(takeSize)
                                    .OrderBy(a => a.ViewCount).ToList()
                                : userPlaces.Where(a => a.CommentCount >= minCommentCount && a.CommentCount <= maxCommentCount)
                                    .Take(takeSize)
                                    .OrderByDescending(a => a.ViewCount).ToList();
                            break;
                        case OrderBy.CommentCount:
                            sortedPlaces = isAscending
                                ? userPlaces.Where(a => a.CommentCount >= minCommentCount && a.CommentCount <= maxCommentCount)
                                    .Take(takeSize)
                                    .OrderBy(a => a.CommentCount).ToList()
                                : userPlaces.Where(a => a.CommentCount >= minCommentCount && a.CommentCount <= maxCommentCount)
                                    .Take(takeSize)
                                    .OrderByDescending(a => a.CommentCount).ToList();
                            break;
                    }

                    break;
            }
            return new DataResult<PlaceListDto>(ResultStatus.Success, new PlaceListDto
            {
                Places = sortedPlaces
            });
        }

        public async Task<IDataResult<PlaceListDto>> GetAllByViewCountAsync(bool isAscending, int? takeSize)
        {
            var places =
                 await UnitOfWork.Places.GetAllAsync(a => a.IsActive && !a.IsDeleted, a => a.Category, a => a.User);
            var sortedPlaces = isAscending
                ? places.OrderBy(a => a.ViewCount)
                : places.OrderByDescending(a => a.ViewCount);
            return new DataResult<PlaceListDto>(ResultStatus.Success, new PlaceListDto
            {
                Places = takeSize == null ? sortedPlaces.ToList() : sortedPlaces.Take(takeSize.Value).ToList()
            });
        }

        public async Task<IDataResult<PlaceDto>> GetAsync(int placeId)
        {
            var place = await UnitOfWork.Places.GetAsync(a => a.Id == placeId, a => a.User, a => a.Category);
            if (place != null)
            {
                place.Comments = await UnitOfWork.Comments.GetAllAsync(c => c.PlaceId == placeId && !c.IsDeleted && c.IsActive);
                return new DataResult<PlaceDto>(ResultStatus.Success, new PlaceDto
                {
                    Place = place,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<PlaceDto>(ResultStatus.Error, Messages.Place.NotFound(isPlural: false), null);
        }

        public async Task<IDataResult<PlaceDto>> GetByIdAsync(int placeId, bool includeCategory, bool includeComments, bool includeUser)
        {
            List<Expression<Func<Place, bool>>> predicates = new List<Expression<Func<Place, bool>>>();
            List<Expression<Func<Place, object>>> includes = new List<Expression<Func<Place, object>>>();
            if (includeCategory) includes.Add(a => a.Category);
            if (includeComments) includes.Add(a => a.Comments);
            if (includeUser) includes.Add(a => a.User);
            predicates.Add(a => a.Id == placeId);
            var place = await UnitOfWork.Places.GetAsyncV2(predicates, includes);
            if (place == null)
            {
                return new DataResult<PlaceDto>(ResultStatus.Warning, Messages.General.ValidationError(), null,
                    new List<ValidationError>
                    {
                        new ValidationError
                        {
                            PropertyName = "placeId",
                            Message = Messages.Article.NotFoundById(placeId)
                        }
                    });
            }

            return new DataResult<PlaceDto>(ResultStatus.Success, new PlaceDto
            {
                Place = place
            });
        }

        public async Task<IDataResult<PlaceUpdateDto>> GetPlaceUpdateDtoAsync(int placeId)
        {
            var result = await UnitOfWork.Places.AnyAsync(a => a.Id == placeId);
            if (result)
            {
                var place = await UnitOfWork.Places.GetAsync(a => a.Id == placeId);
                var placeUpdateDto = Mapper.Map<PlaceUpdateDto>(place);
                return new DataResult<PlaceUpdateDto>(ResultStatus.Success, placeUpdateDto);
            }
            else
            {
                return new DataResult<PlaceUpdateDto>(ResultStatus.Error, Messages.Place.NotFound(isPlural: false), null);
            }
        }

        public async Task<IResult> HardDeleteAsync(int placeId)
        {
            var result = await UnitOfWork.Places.AnyAsync(a => a.Id == placeId);
            if (result)
            {
                var place = await UnitOfWork.Places.GetAsync(a => a.Id == placeId);
                await UnitOfWork.Places.DeleteAsync(place);
                await UnitOfWork.SaveAsync();
                return new Result(ResultStatus.Success, Messages.Place.HardDelete(place.Name));
            }
            return new Result(ResultStatus.Error, Messages.Place.NotFound(isPlural: false));
        }

        public async Task<IResult> IncreaseViewCountAsync(int placeId)
        {
            var place = await UnitOfWork.Places.GetAsync(a => a.Id == placeId);
            if (place == null)
            {
                return new Result(ResultStatus.Error, Messages.Place.NotFound(isPlural: false));
            }

            place.ViewCount += 1;
            await UnitOfWork.Places.UpdateAsync(place);
            await UnitOfWork.SaveAsync();
            return new Result(ResultStatus.Success, Messages.Place.IncreaseViewCount(place.Name));
        }

        public async Task<IDataResult<PlaceListDto>> SearchAsync(string keyword, int currentPage = 1, int pageSize = 5, bool isAscending = false)
        {
            pageSize = pageSize > 20 ? 20 : pageSize;
            if (string.IsNullOrWhiteSpace(keyword))
            {
                var places =
                    await UnitOfWork.Places.GetAllAsync(a => a.IsActive && !a.IsDeleted, a => a.Category,
                        a => a.User);
                var sortedPlaces = isAscending
                    ? places.OrderBy(a => a.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
                    : places.OrderByDescending(a => a.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                return new DataResult<PlaceListDto>(ResultStatus.Success, new PlaceListDto
                {
                    Places = sortedPlaces,
                    CurrentPage = currentPage,
                    PageSize = pageSize,
                    TotalCount = places.Count,
                    IsAscending = isAscending
                });
            }

            var searchedPlaces = await UnitOfWork.Places.SearchAsync(new List<Expression<Func<Place, bool>>>
            {
                (a) => a.Name.Contains(keyword),
                (a) => a.Category.Name.Contains(keyword),
                (a) => a.SeoDescription.Contains(keyword),
                (a) => a.SeoTags.Contains(keyword)
            },
            a => a.Category, a => a.User);
            var searchedAndSortedPlaces = isAscending
                ? searchedPlaces.OrderBy(a => a.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
                : searchedPlaces.OrderByDescending(a => a.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            return new DataResult<PlaceListDto>(ResultStatus.Success, new PlaceListDto
            {
                Places = searchedAndSortedPlaces,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalCount = searchedPlaces.Count,
                IsAscending = isAscending
            });
        }

        public async Task<IResult> UndoDeleteAsync(int placeId, string modifiedByName)
        {
            var result = await UnitOfWork.Places.AnyAsync(a => a.Id == placeId);
            if (result)
            {
                var place = await UnitOfWork.Places.GetAsync(a => a.Id == placeId);
                place.IsDeleted = false;
                place.IsActive = true;
                place.ModifiedByName = modifiedByName;
                place.ModifiedDate = DateTime.Now;
                await UnitOfWork.Places.UpdateAsync(place);
                await UnitOfWork.SaveAsync();
                return new Result(ResultStatus.Success, Messages.Place.UndoDelete(place.Name));
            }
            return new Result(ResultStatus.Error, Messages.Article.NotFound(isPlural: false));
        }

        public async Task<IResult> UpdateAsync(PlaceUpdateDto placeUpdateDto, string modifiedByName)
        {
            var oldPlace = await UnitOfWork.Places.GetAsync(a => a.Id == placeUpdateDto.Id);
            var place = Mapper.Map<PlaceUpdateDto, Place>(placeUpdateDto, oldPlace);
            place.ModifiedByName = modifiedByName;
            await UnitOfWork.Places.UpdateAsync(place);
            await UnitOfWork.SaveAsync();
            return new Result(ResultStatus.Success, Messages.Place.Update(place.Name));
        }
    }
}
