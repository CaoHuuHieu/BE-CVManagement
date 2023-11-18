using CVManagement.Services.Interfaces;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CVManagement.Services.Implements
{
    public class FirebaseStorageService : IStorageService
    {

        private readonly string _AuthEmail, _AuthPassword, _Bucket, _ApiKey;
        private readonly IConfiguration _config;
        public FirebaseStorageService(IConfiguration config)
        {
            _config = config;
            _AuthEmail = _config["Firebase:Email"];
            _AuthPassword = _config["Firebase:Password"];
            _Bucket = _config["Firebase:Bucket"];
            _ApiKey = _config["Firebase:Key"];

        }

   

        public async Task<bool> DeleteFile(string fileName)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(_ApiKey));
            var auth = await authProvider.SignInWithEmailAndPasswordAsync(_AuthEmail, _AuthPassword);

            var cancellation = new CancellationTokenSource();
            var task = new FirebaseStorage(_Bucket, new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken),
                ThrowOnCancel = true
            })
            .Child(fileName).DeleteAsync();
            await task;
            return true;
        }

        public async Task<bool> RenameFile( string oldName, string newName, string fileUrl)
        {
            byte[] fileBytes = await ViewFileByName(fileUrl);
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(_ApiKey));
            var auth = await authProvider.SignInWithEmailAndPasswordAsync(_AuthEmail, _AuthPassword);

            var cancellation = new CancellationTokenSource();
            var task = new FirebaseStorage(_Bucket, new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken),
                ThrowOnCancel = true
            })
            .Child(newName).PutAsync(new MemoryStream(fileBytes), cancellation.Token);
            await task;
            await DeleteFile(oldName);
            return true;
        }

        public Task<bool> RenameFile(string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        public async Task<string> StoreFile(IFormFile file, string fileName)
        {

            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(_ApiKey));
            var auth = await authProvider.SignInWithEmailAndPasswordAsync(_AuthEmail, _AuthPassword);

            var cancellation = new CancellationTokenSource();
            var task = new FirebaseStorage(_Bucket, new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken),
                ThrowOnCancel = true
            })
            .Child(fileName)
            .PutAsync(new MemoryStream(fileBytes), cancellation.Token);

            return await task;
        }
        public async Task<byte[]> ViewFileByName(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                throw new Exception("Failed to download the file.");
            }
        }
    }
}
