using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HotelReservationsManager.Data.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int? ReservedRoomId { get; set; }
        public Room ReservedRoom { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime DateSet { get; set; }
        public DateTime DateSetOff { get; set; }
        public bool HaveBreakfast { get; set; }
        public bool AllInclusive { get; set; }
        public decimal TotalPrice { get; set; }
        public virtual ICollection<ClientReservation> ClientReservations { get; set; } = new HashSet<ClientReservation>();
       
    }
}
