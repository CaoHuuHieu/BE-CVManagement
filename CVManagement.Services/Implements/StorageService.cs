using CVManagement.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CVManagement.Services.Implements
{
    public class StorageService : IStorageService
    {
        private string _uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        public async Task<bool> DeleteFile(string fileName)
        {
            string filePath = Path.Combine(_uploadsPath, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            else throw new FileNotFoundException("Tệp không tồn tại.");
        }

        public async Task<bool> RenameFile(string oldName, string newName)
        {
            if (string.IsNullOrWhiteSpace(oldName) || string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentException("Tên tệp không hợp lệ.");
            }

            String oldPath = Path.Combine(_uploadsPath, oldName);
            String newPath = Path.Combine(_uploadsPath, newName);

            if (File.Exists(oldPath))
            {
                if (!File.Exists(newPath))
                {
                        File.Move(oldPath, newPath);
                        return true;
                }
                else throw new IOException("File đã tồn tại. Không thể đổi tên!");

            }
            else throw new FileNotFoundException("Tệp không tồn tại.");
        }

        public Task<bool> RenameFile(string oldName, string newName, string url)
        {
            throw new NotImplementedException();
        }

        public async Task<string> StoreFile(IFormFile file, string fileName)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Tệp trống hoặc không tồn tại.");
            }
            var filePath = Path.Combine(_uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return fileName;
        }

        public async Task<byte[]> ViewFileByName(string fileName)
        {
            string filePath = Path.Combine(_uploadsPath, fileName);

            if (File.Exists(filePath))
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                    return fileBytes;
            }
            else
            {
                throw new FileNotFoundException("Tệp không tồn tại.");
            }
        }

    }
}
