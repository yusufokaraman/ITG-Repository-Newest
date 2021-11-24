using ProgrammersBlog.Entities.Dtos;
using ProgrammersBlog.Shared.Utilities.Results.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Services.Abstract
{
    public interface ICityService
    {
        Task<IDataResult<CityDto>> GetAsync(int cityId);
        Task<IDataResult<CityListDto>> GetAllAsync();
        Task<IDataResult<CityUpdateDto>> GetCityUpdateDtoAsync(int cityId);
        Task<IDataResult<CityListDto>> GetAllByNonDeletedAsync();
        Task<IDataResult<CityListDto>> GetAllByNonDeletedAndActiveAsync();
        Task<IDataResult<CityDto>> AddAsync(CityAddDto cityAddDto, string createdByName);
        Task<IDataResult<CityDto>> UpdateAsync(CityUpdateDto cityUpdateDto, string modifiedByName);
        Task<IResult> DeleteAsync(int cityId, string modifiedByName);
        Task<IResult> HardDeleteAsync(int cityId);
        Task<IDataResult<int>> CountAsync();
        Task<IDataResult<int>> CountByNonDeletedAsync();
    }
}
