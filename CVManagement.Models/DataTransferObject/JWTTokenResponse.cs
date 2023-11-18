namespace CVManagement.Models.DataTransferObject
{
    public class JWTTokenResponse
    {
        public string? Token
        {
            get;
            set;
        }
        public string Email { get; set; }
        public string Role { get; set; }
        public int Status { get; set; }
        public long Id { get;  set; }
        public string FullName { get; set; }
    }
}
