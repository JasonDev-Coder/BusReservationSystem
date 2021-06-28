using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace BusReservationSystem.Models
{   
    public class Bookings
    {   
        public int Id { get; set; }
        [ForeignKey("AspNetUsers")]
        public string UserID { get; set; }
        [ForeignKey("Line")]
        public int LineID { get; set; }
    }
}
