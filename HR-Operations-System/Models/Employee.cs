using HR_Operations_System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Operations_System.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(AppUser))]
        public string? UserId { get; set; }
        public virtual AppUser? ApplicationUser { get; set; }

        public int FingerCode { get; set; }
        public virtual IEnumerable<Attendance>? Attendance { get; set; }

        public int DeptCode { get; set; }
        public string NameA { get; set; }
        public string NameE { get; set; }

        [ForeignKey(nameof(TimingPlan))]
        public int TimingCode { get; set; }
        public virtual TimingPlan? TimingPlan { get; set; }

        public int JobType { get; set; }
        public int Sex { get; set; }
        public bool CheckLate { get; set; }
        public bool HasAllow { get; set; }
        public bool IsActive { get; set; }


    }



}
