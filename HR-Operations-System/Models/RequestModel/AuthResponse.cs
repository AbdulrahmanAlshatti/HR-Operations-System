namespace HR_Operations_System.Models.RequestModel
{
    public class AuthResponse
    {
            public string Token { get; set; }
            public string Id { get; set; }
            public string Username { get; set; }
            public bool IsFirstLogin { get; set; }
    }
}
