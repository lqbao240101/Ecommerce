using Ecommerce.Data.Entities;
using Ecommerce.Services.Core.Common;
using Ecommerce.ViewModels.Addresses;
using Ecommerce.ViewModels.ViewModels;

namespace Ecommerce.Services.Core.IService
{
    public interface IAddressService : IEntityBaseService<Address>
    {
        Task<List<Address>> GetAddressesByUserId(string userId);

        Task<Address> GetAddressDetail(string userId, Guid id);

        Task<Address> AddNewAddressAsync(string userId, AddressAddModel data);
        Task<Address> UpdateAddressAsync(AddressUpdateModel data, string userId, Guid id);
    }
}