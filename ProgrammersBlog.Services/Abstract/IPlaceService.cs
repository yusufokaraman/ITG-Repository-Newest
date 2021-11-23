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
        Task<IDataResult<PlaceDto>> Get(int placeId);
        Task<IDataResult<PlaceListDto>> GetAll();
        Task<IDataResult<PlaceListDto>> GetAllByNonDeleted();
        Task<IDataResult<PlaceListDto>> GetAllByNonDeletedAndActive();
        Task<IDataResult<PlaceListDto>> GetAllByCategory(int categoryId);
        Task<IDataResult<PlaceListDto>> GetAllByCity(int cityId);
        Task<IResult> Add(PlaceAddDto placeAddDto, string createdByName);
        Task<IResult> Update(PlaceUpdateDto placeUpdateDto, string modifiedByName);
        Task<IResult> Delete(int placeId, string modifiedByName);
        Task<IResult> HardDelete(int placeId);
    }
}
