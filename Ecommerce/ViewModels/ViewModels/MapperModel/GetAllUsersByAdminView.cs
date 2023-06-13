namespace Ecommerce.ViewModels.ViewModels.MapperModel
{
    public class GetAllUsersByAdminView
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public string Avatar { get; set; }
    }
}