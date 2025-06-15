using HR_Operations_System.Data;

namespace HR_Operations_System.Models
{
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Employee
    {
        public int Id { get; set; }


        public string? UserId { get; set; }
        public virtual AppUser? ApplicationUser { get; set; }
    }

}
