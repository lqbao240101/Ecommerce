namespace Ecommerce.Services.Core.IService
{
    public interface IDriveService
    {
        public string AddFile(string filePath, string ex, string folderUrl);
        public Task<bool> Remove(string id);
        public Task<bool> RenamingFolder(string name, string id);
        public string AddFolder(string folderName, string folderUrl);
    }
}