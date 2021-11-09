using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace mysqlapi.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUser { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Username { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Password { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Role { get; set; }   

        [NotMapped]
        public string Token { get; set; }
    }
}
