using System.ComponentModel.DataAnnotations;

namespace HR_Operations_System.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }
        public string DescA { get; set; }
        public string DescE { get; set; }

        public virtual IEnumerable<Node> Node { get; set; }
    }


}
