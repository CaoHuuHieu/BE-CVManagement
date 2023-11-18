using Microsoft.AspNetCore.Http;

namespace CVManagement.Services.Interfaces
{
    public interface IStorageService
    {
        public Task<string> StoreFile(IFormFile file, string fileName);
        public Task<byte[]> ViewFileByName(string fileName);
        public Task<bool> DeleteFile(string fileName);
        public Task<bool> RenameFile(string oldName, string newName);
        public Task<bool> RenameFile(string oldName, string newName, string url);
    }
}
