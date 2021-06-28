using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace BusReservationSystem.Models
{   public class Line
    {   [Key]
        public int Id { get; set; }
        [ForeignKey("Bus")]
        public int BusFK { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int ticketPrice { get; set; }
        public int AvailableSeats { get; set; }
        public int ReservedSeats { get; set; }
    }
}
