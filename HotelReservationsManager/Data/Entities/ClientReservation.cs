﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelReservationsManager.Data.Entities
{
    public class ClientReservation
    {
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public int ReservationId { get; set; }  
        public Reservation Reservation { get; set; }
    }
}
