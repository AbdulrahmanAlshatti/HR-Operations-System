using System.ComponentModel.DataAnnotations;

namespace HR_Operations_System.Models.RequestModel
{
    public class RegisterRequestModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Gender { get; set; }
        public int TimingCode { get; set; }
        public int DeptCode {get;set; }
        public string Role { get; set; }
    }
}
