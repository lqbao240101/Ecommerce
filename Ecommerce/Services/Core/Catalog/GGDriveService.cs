using Ecommerce.Services.Core.IService;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Ecommerce.Services.Core.Catalog
{
    public class GGDriveService : IDriveService
    {
        private readonly DriveService _driveService;

        public GGDriveService()
        {
            string[] Scopes = { DriveService.Scope.Drive };
            string ApplicationName = "testapp";

            UserCredential credential;
            using (var stream = new FileStream("./Application/credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,  // Quyền truy xuất dữ liệu của người dùng
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Tạo ra 1 dịch vụ Drive API - Create Drive API service với thông tin xác thực và ApplicationName
            _driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }
        public string AddFile(string filePath, string ex, string folderId)
        {
            // ID thư mục file, các bạn thay bằng ID của các bạn khi chạy

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                // Tên file sẽ lưu trên Google Drive
                Name = Guid.NewGuid().ToString() + ex,

                // Thư mục chưa file
                Parents = new List<string> { folderId }
            };

            // Đường dẫn file trong thiết bị của bạn, dùng để upload lên Goolge Drive
            FilesResource.CreateMediaUpload request;
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                request = _driveService.Files.Create(fileMetadata, stream, "image/jpeg");

                // Cấu hình thông tin lấy về là ID
                request.Fields = "id";

                // thực hiện Upload
                request.Upload();
            }

            // Trả về thông tin file đã được upload lên Google Drive
            var file = request.ResponseBody;
            return "https://drive.google.com/uc?export=view&id=" + file.Id;
        }

        public string AddFolder(string folderName, string folderParent)
        {
            // File metadata
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder",
                // Thư mục chưa file
                Parents = new List<string> { folderParent }
            };

            // Create a new folder on drive.
            var request = _driveService.Files.Create(fileMetadata);
            request.Fields = "id";
            var file = request.Execute();
            return file.Id;
        }

        public async Task<bool> Remove(string id)
        {
            try
            {
                await _driveService.Files.Delete(id).ExecuteAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RenamingFolder(string name, string id)
        {
            try
            {
                // Lấy thông tin của folder cần đổi tên
                var file = await _driveService.Files.Get(id).ExecuteAsync();

                // Cập nhật tên mới cho folder
                file.Id = null;
                file.Name = name;

                // Update lại thông tin của folder
                var updateRequest = _driveService.Files.Update(file, id);
                await updateRequest.ExecuteAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}