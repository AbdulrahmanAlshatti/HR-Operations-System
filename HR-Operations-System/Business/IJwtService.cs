using HR_Operations_System.Data;

namespace HR_Operations_System.Business
{
    public interface IJwtService
    {
        Task<string> GenerateToken(AppUser user);
    }
}
