using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ViewModels.Addresses
{
    public class AddressAddModel
    {
        public string Street { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        [JsonIgnore]
        public string CustomerId { get; set; }
    }
    public class AddressUpdateModel
    {
        public string? Street { get; set; }
        public string? Ward { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
    }

}
