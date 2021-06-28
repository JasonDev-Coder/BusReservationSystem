using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BusReservationSystem.Models
{
    public class BrandListModel
    {
        public List<Bus> Buses { get; set; }
        public SelectList Brands { get; set; }
        public string BusBrand { get; set; }
        public int Capacity { get; set; }
    }
}
