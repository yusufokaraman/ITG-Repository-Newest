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
        Task<IDataResult<CityDto>> Get(int cityId);
        Task<IDataResult<CityListDto>> GetAll();
        Task<IDataResult<CityListDto>> GetAllByNonDeleted();
        Task<IDataResult<CityListDto>> GetAllByNonDeletedAndActive();
        Task<IDataResult<CityDto>> Add(CityAddDto cityAddDto, string createdByName);
        Task<IDataResult<CityDto>> Update(CityUpdateDto cityUpdateDto, string modifiedByName);
        Task<IResult> Delete(int cityId, string modifiedByName);
        Task<IResult> HardDelete(int cityId);
    }
}
