using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusReservationSystem.Data;
using BusReservationSystem.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BusReservationSystem.Controllers
{
   
    public class BusesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BusesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize(Roles = "Admin")]
        // GET: Buses
        public async Task<IActionResult> Index(string BusBrand,int Capacity)
        {
            IQueryable<string> brandQuery = from m in _context.Bus orderby m.Brand select m.Brand;
            var buses = from m in _context.Bus select m;
            if (!string.IsNullOrEmpty(BusBrand))
            {
                buses = buses.Where(b => b.Brand == BusBrand);
            }
            buses = buses.Where(b => b.Capacity >= Capacity);
            BrandListModel model = new BrandListModel
            {
                Buses = await buses.ToListAsync(),
                Brands = new SelectList(await brandQuery.Distinct().ToListAsync()),
            };
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        // GET: Buses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bus = await _context.Bus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bus == null)
            {
                return NotFound();
            }

            return View(bus);
        }
        [Authorize(Roles = "Admin")]
        // GET: Buses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Buses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Brand,Capacity,BusImage")] Bus bus, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                if (Image != null)
                {
                    if (Image.Length > 0)
                    {
                        byte[] p1 = null;
                        using (var fs1 = Image.OpenReadStream())
                        {
                            using (var ms1 = new MemoryStream())
                            {
                                fs1.CopyTo(ms1);
                                p1 = ms1.ToArray();
                            }
                        }
                        bus.BusImage = p1;
                    }
                }
                _context.Add(bus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bus);
        }
        [Authorize(Roles = "Admin")]
        // GET: Buses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bus = await _context.Bus.FindAsync(id);
            if (bus == null)
            {
                return NotFound();
            }
            return View(bus);
        }

        // POST: Buses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Brand,Capacity,BusImage")] Bus bus)
        {
            if (id != bus.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusExists(bus.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bus);
        }
        [Authorize(Roles = "Admin")]
        // GET: Buses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bus = await _context.Bus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bus == null)
            {
                return NotFound();
            }

            return View(bus);
        }

        // POST: Buses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bus = await _context.Bus.FindAsync(id);
            _context.Bus.Remove(bus);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BusExists(int id)
        {
            return _context.Bus.Any(e => e.Id == id);
        }
        public async Task<IActionResult> AvailableBus()
        {
            var buses = await _context.Bus.ToListAsync();
            List<Bus> available_bus = new List<Bus>();
            var bus_ids = await _context.BusBookings.Select(m => m.Id).ToListAsync();
            for (int i = 0; i < buses.Count(); i++)
            {
                if (!bus_ids.Contains(buses[i].Id))
                {
                    available_bus.Add(buses[i]);
                }
            }
            return View(available_bus);
        }
        public async Task<IActionResult> Book(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bus = await _context.Bus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bus == null)
            {
                return NotFound();
            }
            return View(bus);
        }
        [HttpPost, ActionName("Book")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookingConfirmed(int id)
        {
            string user_id = _userManager.GetUserId(HttpContext.User);
            BusBookingsModel model = new BusBookingsModel
            {
                UserID = user_id,
                BusID = id
            };
            _context.BusBookings.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Buses", "Bookings");
        }
    
}
}
