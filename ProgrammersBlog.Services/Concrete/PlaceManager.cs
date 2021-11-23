using AutoMapper;
using ProgrammersBlog.Data.Abstract;
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
    public class PlaceManager : IPlaceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PlaceManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IResult> Add(PlaceAddDto placeAddDto, string createdByName)
        {
            var place = _mapper.Map<Place>(placeAddDto);
            place.CreatedByName = createdByName;
            place.ModifiedByName = createdByName;
            await _unitOfWork.Places.AddAsync(place);
            //.ContinueWith(t => _unitOfWork.SaveAsync());
            await _unitOfWork.SaveAsync();
            return new Result(ResultStatus.Success, $"{placeAddDto.Name} başlıklı mekan başarıyla eklenmiştir.");
        }

        public async Task<IResult> Delete(int placeId, string modifiedByName)
        {
            var result = await _unitOfWork.Places.AnyAsync(a => a.Id == placeId);
            if (result)
            {
                var place = await _unitOfWork.Places.GetAsync(a => a.Id == placeId);
                place.IsDeleted = true;
                place.ModifiedByName = modifiedByName;
                place.ModifiedDate = DateTime.Now;
                await _unitOfWork.Places.UpdateAsync(place);
                //.ContinueWith(t => _unitOfWork.SaveAsync());
                await _unitOfWork.SaveAsync();
                return new Result(ResultStatus.Success, $"{place.Name} başlıklı mekan başarıyla silinmiştir.");
            }
            return new Result(ResultStatus.Error, "Böyle bir mekan bulunamadı.");
        }

        public async Task<IDataResult<PlaceDto>> Get(int placeId)
        {
            var place = await _unitOfWork.Places.GetAsync(a => a.Id == placeId, a => a.Category, a => a.City);
            if (place != null)
            {
                return new DataResult<PlaceDto>(ResultStatus.Success, new PlaceDto
                {
                    Place = place,
                    ResultStatus = ResultStatus.Success

                });
            }
            return new DataResult<PlaceDto>(ResultStatus.Error, "Aradığınız mekan bulunamadı.", null);
        }

        public async Task<IDataResult<PlaceListDto>> GetAll()
        {
            var places = await _unitOfWork.Places.GetAllAsync(null, a => a.Category, a => a.City);
            if (places.Count > -1)
            {
                return new DataResult<PlaceListDto>(ResultStatus.Success, new PlaceListDto
                {
                    Places = places,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<PlaceListDto>(ResultStatus.Error, "Mekanlar bulunamadı.", null);
        }

        public async Task<IDataResult<PlaceListDto>> GetAllByCategory(int categoryId)
        {
            var result = await _unitOfWork.Categories.AnyAsync(c => c.Id == categoryId);
            if (result)
            {
                var places = await _unitOfWork.Places.GetAllAsync(a => a.CategoryId == categoryId && !a.IsDeleted && a.IsActive, ar => ar.Category);
                if (places.Count > -1)
                {
                    return new DataResult<PlaceListDto>(ResultStatus.Success, new PlaceListDto
                    {
                        Places = places,
                        ResultStatus = ResultStatus.Success
                    });
                }
                return new DataResult<PlaceListDto>(ResultStatus.Error, "Mekanlar bulunamadı.", null);
            }
            return new DataResult<PlaceListDto>(ResultStatus.Error, "Aradığınız kategori bulunamadı.", null);
        }

        public async Task<IDataResult<PlaceListDto>> GetAllByCity(int cityId)
        {
            var result = await _unitOfWork.Cities.AnyAsync(c => c.Id == cityId);
            if (result)
            {
                var places = await _unitOfWork.Places.GetAllAsync(a => a.CityId == cityId && !a.IsDeleted && a.IsActive, ar => ar.City);
                if (places.Count > -1)
                {
                    return new DataResult<PlaceListDto>(ResultStatus.Success, new PlaceListDto
                    {
                        Places = places,
                        ResultStatus = ResultStatus.Success
                    });
                }
                return new DataResult<PlaceListDto>(ResultStatus.Error, "Mekanlar bulunamadı.", null);
            }
            return new DataResult<PlaceListDto>(ResultStatus.Error, "Aradığınız şehir bulunamadı.", null);
        }

        public async Task<IDataResult<PlaceListDto>> GetAllByNonDeleted()
        {
            var places = await _unitOfWork.Places.GetAllAsync(a => !a.IsDeleted, ar => ar.Category, ar => ar.City);
            if (places.Count > -1)
            {
                return new DataResult<PlaceListDto>(ResultStatus.Success, new PlaceListDto
                {
                    Places = places,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<PlaceListDto>(ResultStatus.Error, "Mekanlar bulunamadı.", null);
        }

        public async Task<IDataResult<PlaceListDto>> GetAllByNonDeletedAndActive()
        {
            var places = await _unitOfWork.Places.GetAllAsync(a => !a.IsDeleted && a.IsActive, ar => ar.Category, ar => ar.City);
            if (places.Count > -1)
            {
                return new DataResult<PlaceListDto>(ResultStatus.Success, new PlaceListDto
                {
                    Places = places,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<PlaceListDto>(ResultStatus.Error, "Mekanlar bulunamadı.", null);
        }

        public async Task<IResult> HardDelete(int placeId)
        {
            var result = await _unitOfWork.Places.AnyAsync(a => a.Id == placeId);
            if (result)
            {
                var place = await _unitOfWork.Places.GetAsync(a => a.Id == placeId);
                await _unitOfWork.Places.DeleteAsync(place);
                //.ContinueWith(t => _unitOfWork.SaveAsync());
                await _unitOfWork.SaveAsync();
                return new Result(ResultStatus.Success, $"{place.Name} isimli mekan başarıyla veritabanından silinmiştir.");
            }
            return new Result(ResultStatus.Error, "Böyle bir mekan bulunamadı.");
        }

        public async Task<IResult> Update(PlaceUpdateDto placeUpdateDto, string modifiedByName)
        {
            var place = _mapper.Map<Place>(placeUpdateDto);
            place.ModifiedByName = modifiedByName;
            await _unitOfWork.Places.UpdateAsync(place);
            //.ContinueWith(t => _unitOfWork.SaveAsync());
            await _unitOfWork.SaveAsync();
            return new Result(ResultStatus.Success, $"{placeUpdateDto.Name} isimli mekan başarıyla güncellenmiştir.");
        }
    }
}
