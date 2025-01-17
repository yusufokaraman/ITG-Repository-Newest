﻿using AutoMapper;
using ProgrammersBlog.Data.Abstract;
using ProgrammersBlog.Data.Concrete;
using ProgrammersBlog.Entities.Concrete;
using ProgrammersBlog.Entities.Dtos;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Shared.Utilities.Results.Abstract;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using ProgrammersBlog.Shared.Utilities.Results.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Services.Concrete
{
    /// <summary>
    /// Add,Update,Delete ve Hard Delete bölümlerinde değişiklik yapılmıştır.
    /// </summary>
    /// 

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
    public class CityManager : ICityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CityManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IDataResult<CityDto>> AddAsync(CityAddDto cityAddDto, string createdByName)
        {
            var city = _mapper.Map<City>(cityAddDto);
            city.CreatedByName = createdByName;
            city.ModifiedByName = createdByName;
            var addedCity = await _unitOfWork.Cities.AddAsync(city);
            //.ContinueWith(t => _unitOfWork.SaveAsync());
            await _unitOfWork.SaveAsync();
            return new DataResult<CityDto>(ResultStatus.Success, $"{cityAddDto.Name} adlı şehir başarıyla eklenmiştir.", new CityDto
            {

                City = addedCity,
                ResultStatus = ResultStatus.Success,
                Message = $"{cityAddDto.Name} adlı şehir başarıyla eklenmiştir."



            });
        }

        public async Task<IResult> DeleteAsync(int cityId, string modifiedByName)
        {
            var city = await _unitOfWork.Cities.GetAsync(c => c.Id == cityId);
            if (city != null)
            {
                city.IsDeleted = true;
                city.ModifiedByName = modifiedByName;
                city.ModifiedDate = DateTime.Now;
                await _unitOfWork.Cities.UpdateAsync(city);
                //.ContinueWith(t => _unitOfWork.SaveAsync());
                await _unitOfWork.SaveAsync();
                return new Result(ResultStatus.Success, $"{city.Name} adlı şehir başarıyla silinmiştir.");
            }
            return new Result(ResultStatus.Error, "Böyle bir kategori bulunamadı.");
        }

        public async Task<IDataResult<CityDto>> GetAsync(int cityId)
        {
            var city = await _unitOfWork.Cities.GetAsync(c => c.Id == cityId);
            if (city != null)
            {
                return new DataResult<CityDto>(ResultStatus.Success, new CityDto
                {
                    City = city,
                    ResultStatus = ResultStatus.Success


                });
            }
            return new DataResult<CityDto>(ResultStatus.Error, "Böyle bir şehir bulunamadı.", new CityDto
            {

                City = null,
                ResultStatus = ResultStatus.Error,
                Message = "Böyle bir şehir bulunamadı."


            });
        }

        public async Task<IDataResult<CityListDto>> GetAllAsync(CityListDto cityListDto)
        {
            var cities = await _unitOfWork.Cities.GetAllAsync(null);
            if (cities.Count > -1)
            {
                return new DataResult<CityListDto>(ResultStatus.Success, new CityListDto
                {
                    Cities = cities,
                    ResultStatus = ResultStatus.Success

                });
            }
            return new DataResult<CityListDto>(ResultStatus.Error, "Hiçbir şehir bulunamadı.", new CityListDto
            {
                Cities = null,
                ResultStatus = ResultStatus.Error,
                Message = "Hiçbir şehir bulunamadı."

            });
        }


        public async Task<IDataResult<CityListDto>> GetAllByNonDeletedAsync()
        {
            var cities = await _unitOfWork.Cities.GetAllAsync(c => !c.IsDeleted);
            if (cities.Count > -1)
            {
                return new DataResult<CityListDto>(ResultStatus.Success, new CityListDto
                {
                    Cities = cities,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<CityListDto>(ResultStatus.Error, "Hiçbir Şehir bulunamadı.", null);
        }

        public async Task<IDataResult<CityListDto>> GetAllByNonDeletedAndActiveAsync()
        {
            var cities = await _unitOfWork.Cities.GetAllAsync(c => !c.IsDeleted && c.IsActive);
            if (cities.Count > -1)
            {
                return new DataResult<CityListDto>(ResultStatus.Success, new CityListDto
                {
                    Cities = cities,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<CityListDto>(ResultStatus.Error, "Hiçbir Şehir bulunamadı.", null);
        }

        public async Task<IResult> HardDeleteAsync(int cityId)
        {
            var city = await _unitOfWork.Cities.GetAsync(c => c.Id == cityId);
            if (city != null)
            {
                await _unitOfWork.Cities.DeleteAsync(city);
                //.ContinueWith(t => _unitOfWork.SaveAsync());
                await _unitOfWork.SaveAsync();
                return new Result(ResultStatus.Success, $"{city.Name} adlı şehir veritabanından başarıyla silinmiştir.");
            }
            return new Result(ResultStatus.Error, "Böyle bir şehir bulunamadı.");
        }

        public async Task<IDataResult<CityDto>> UpdateAsync(CityUpdateDto cityUpdateDto, string modifiedByName)
        {
            var city = _mapper.Map<City>(cityUpdateDto);
            city.ModifiedByName = modifiedByName;
            var updatedCity = await _unitOfWork.Cities.UpdateAsync(city);
            //.ContinueWith(t => _unitOfWork.SaveAsync());
            await _unitOfWork.SaveAsync();
            return new DataResult<CityDto>(ResultStatus.Success, $"{cityUpdateDto.Name} adlı şehir başarıyla güncellenmiştir.", new CityDto
            {
                City = updatedCity,
                ResultStatus = ResultStatus.Success,
                Message = $"{cityUpdateDto.Name} adlı şehir başarıyla güncellenmiştir."


            });
        }

        public async Task<IDataResult<int>> CountAsync()
        {
            var citiesCount = await _unitOfWork.Cities.CountAsync();
            if (citiesCount > -1)
            {
                return new DataResult<int>(ResultStatus.Success, citiesCount);
            }
            else
            {
                return new DataResult<int>(ResultStatus.Error, $"Beklenmeyen bir hata ile karşılaşıldı.", -1);
            }
        }

        public async Task<IDataResult<int>> CountByNonDeletedAsync()
        {
            var citiesCount = await _unitOfWork.Cities.CountAsync(c => !c.IsDeleted);
            if (citiesCount > -1)
            {
                return new DataResult<int>(ResultStatus.Success, citiesCount);
            }
            else
            {
                return new DataResult<int>(ResultStatus.Error, $"Beklenmeyen bir hata ile karşılaşıldı.", -1);
            }
        }

        public Task<IDataResult<CityUpdateDto>> GetCityUpdateDtoAsync(int cityId)
        {
            throw new NotImplementedException();
        }

        public Task<IDataResult<CityListDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(CityAddDto cityAddDto, object userName)
        {
            throw new NotImplementedException();
        }
    }
}
