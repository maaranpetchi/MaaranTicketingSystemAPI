using System.ComponentModel.DataAnnotations;

namespace MaaranTicketingSystemAPI.Models
{
    public class User
    {

        [Key]
        public int Id { get; set; }
        public string? firstname { get; set; }
        public string? lastname { get; set; }
        public string username { get; set; }
        public string emailId { get; set; }
        

        public int Departmentid { get; set; }
        public string password { get; set; }
        public string? Role { get; set; }

        public string? Token { get; set; }
        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }

    }
}