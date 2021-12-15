using ProgrammersBlog.Entities.ComplexTypes;
using ProgrammersBlog.Entities.Dtos;
using ProgrammersBlog.Shared.Utilities.Results.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Services.Abstract
{
    public interface IPlaceService
    {
        Task<IDataResult<PlaceDto>> GetAsync(int placeId);
        Task<IDataResult<PlaceDto>> GetByIdAsync(int placeId, bool includeCategory, bool includeComments, bool includeUser);
        Task<IDataResult<PlaceUpdateDto>> GetPlaceUpdateDtoAsync(int placeId);
        Task<IDataResult<PlaceListDto>> GetAllAsyncV2(int? categoryId, int? userId, bool? isActive, bool? isDeleted, int currentPage, int pageSize, OrderByGeneral orderBy, bool isAscending, bool includeCategory, bool includeComments, bool includeUser);
        Task<IDataResult<PlaceListDto>> GetAllAsync();
        Task<IDataResult<PlaceListDto>> GetAllByNonDeletedAsync();
        Task<IDataResult<PlaceListDto>> GetAllByNonDeletedAndActiveAsync();
        //Task<IDataResult<ArticleListDto>> GetAllByCity(int cityId);
        //Task<IDataResult<ArticleListDto>> GetAllByPlace(int placeId);
        Task<IDataResult<PlaceListDto>> GetAllByCategoryAsync(int categoryId);
        Task<IDataResult<PlaceListDto>> GetAllByDeletedAsync();
        Task<IDataResult<PlaceListDto>> GetAllByViewCountAsync(bool isAscending, int? takeSize);
        Task<IDataResult<PlaceListDto>> GetAllByPagingAsync(int? categoryId, int currentPage = 1, int pageSize = 5,
            bool isAscending = false);
        Task<IDataResult<PlaceListDto>> GetAllByUserIdOnFilter(int userId, FilterBy filterBy, OrderBy orderBy,
            bool isAscending, int takeSize, int categoryId, DateTime startAt, DateTime endAt, int minViewCount,
            int maxViewCount, int minCommentCount, int maxCommentCount);

        Task<IDataResult<PlaceListDto>> SearchAsync(string keyword, int currentPage = 1, int pageSize = 5,
            bool isAscending = false);

        Task<IResult> IncreaseViewCountAsync(int placeId);
        Task<IResult> AddAsync(PlaceAddDto placeAddDto, string createdByName, int userId);
        Task<IResult> UpdateAsync(PlaceUpdateDto placeUpdateDto, string modifiedByName);
        Task<IResult> DeleteAsync(int placeId, string modifiedByName);
        Task<IResult> HardDeleteAsync(int placeId);
        Task<IResult> UndoDeleteAsync(int placeId, string modifiedByName);
        Task<IDataResult<int>> CountAsync();
        Task<IDataResult<int>> CountByNonDeletedAsync();
    }
}
