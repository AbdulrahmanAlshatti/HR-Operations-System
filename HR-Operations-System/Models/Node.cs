using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Operations_System.Models
{
    public class Node
    {
        [Key]
        public string SerialNo { get; set; }
        public string DescA { get; set; }
        public string DescE { get; set; }

        [ForeignKey(nameof(Location))]
        public int LocCode { get; set; }
        public virtual Location? Location { get; set; }  

        public string Floor { get; set; }

        public virtual IEnumerable<Attendance>? Attendance { get; set; }
    }


}
