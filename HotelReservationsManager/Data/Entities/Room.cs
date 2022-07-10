using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelReservationsManager.Data.Entities
{
    public class Room
    {
        public int Id { get; set; }
        [Required]
        public int Capacity { get; set; }

        [Required]
        public string Type { get; set; }
        public bool InUse { get; set; }
        
        [Required]
        public decimal AdultBedPrice { get; set; }
        [Required]
        public decimal ChildBedPrice { get; set; }
        [Required]
        public int RoomNumber { get; set; }
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
    }
}
