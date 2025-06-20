using HR_Operations_System.Data;
using HR_Operations_System.Models;

namespace HR_Operations_System.Business
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(AppUser user, Employee Emp);
    }
}
