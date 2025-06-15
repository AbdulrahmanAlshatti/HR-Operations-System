using HR_Operations_System.Models;
using Microsoft.AspNetCore.Identity;

namespace HR_Operations_System.Data
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual Employee? Employee { get; set; }
    }

}
