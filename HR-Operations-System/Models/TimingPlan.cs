namespace HR_Operations_System.Models
{
    public class TimingPlan
    {
        public int Id { get; set; }
        public string DescA { get; set; }
        public string DescE { get; set; }

        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public TimeSpan RmdFromTime { get; set; }
        public TimeSpan RmdToTime { get; set; }
        public bool IsRamadan { get; set; }
        public bool IsAllow { get; set; }
        public virtual IEnumerable<Employee>? Employees { get; set; }
        public virtual IEnumerable<EmployeeAllow>? EmployeeAllows { get; set; }
    }


}
