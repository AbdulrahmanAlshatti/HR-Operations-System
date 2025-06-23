using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Operations_System.Models
{
    public class EmployeeAllow
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Employee))]
        public int EmpId { get; set; }
        public virtual Employee? Employee { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [ForeignKey(nameof(TimingPlan))]
        public int TimingCode { get; set; }
        public virtual TimingPlan? TimingPlan { get; set; }
        public bool Status { get; set; } = true;

    }
}
