using Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entites
{
    [Table("Sessions")]
    public class Session : BaseEntity
    {
        [Required]
        public int MovieId { get; set; }

        [Required]
        [StringLength(255)]
        public string RoomName { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        public virtual Movie Movie { get; set; }
    }
}
