using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Operations_System.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Employee))]
        public int FingerCode { get; set; }
        public virtual Employee? Employee { get; set; }

        public DateTime IODateTIme { get; set; }

        [ForeignKey(nameof(Node))]
        public string NodeSerialNo { get; set; }
        public virtual Node? Node { get; set; }

        public bool IsActive { get; set; }
        public string? Photo { get; set; }
        public int TrType { get; set; }

    }

}
