using Ecommerce.Data.EF;
using Ecommerce.Data.Entities;
using Ecommerce.Services.Core.Common;
using Ecommerce.Services.Core.IService;
using Ecommerce.ViewModels.Addresses;
using Ecommerce.ViewModels.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services.Core.Catalog
{
    public class AddressService : EntityBaseService<Address>, IAddressService
    {
        public AddressService(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Address> AddNewAddressAsync(string userId, AddressAddModel data)
        {
            data.CustomerId = userId;

            var item = _mapper.Map<AddressAddModel, Address>(data);

            await _context.Addresses.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Address> GetAddressDetail(string userId, Guid id)
        {
            var address = await _context.Addresses.FirstOrDefaultAsync(n => n.Id.Equals(id) && n.CustomerId.Equals(userId));
            return address!;
        }

        public async Task<List<Address>> GetAddressesByUserId(string userId)
        {
            var addresses = await _context.Addresses.Where(n => n.CustomerId.Equals(userId)).ToListAsync();
            return addresses;
        }

        public async Task<Address> UpdateAddressAsync(AddressUpdateModel data, string userId, Guid id)
        {
            var address = await _context.Addresses.FirstOrDefaultAsync(n => n.Id.Equals(id) && n.CustomerId.Equals(userId)) ?? throw new KeyNotFoundException($"Address with ID {id} not found.");
            
            address = _mapper.Map(data, address);
            _context.Update(address);
            await _context.SaveChangesAsync();
            return address;
        }
    }
}