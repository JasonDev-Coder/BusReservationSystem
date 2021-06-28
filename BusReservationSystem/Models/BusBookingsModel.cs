using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BusReservationSystem.Models
{
    public class BusBookingsModel
    {
        public int Id { get; set; }
        [ForeignKey("AspNetUsers")]
        public string UserID { get; set; }
        [ForeignKey("Bus")]
        public int BusID { get; set; }
    }
}
