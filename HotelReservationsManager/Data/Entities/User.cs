using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelReservationsManager.Data.Entities
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(20)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20)]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(20)]
        public string LastName { get; set; }

        [StringLength(10)]
        public string TIN { get; set; }

        [Required]
        public DateTime HiringDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime FiringDate { get; set; }
        public bool IsAdmin { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
    }
}
