using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusReservationSystem.Data;
using BusReservationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BusReservationSystem.Controllers
{   [Authorize]
    public class LinesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public LinesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Lines
        public async Task<IActionResult> Index()
        {
            var lines = await _context.Line.ToListAsync();
            for(int i = 0; i < lines.Count(); i++)
            { var bus = _context.Bus.Find(lines[i].BusFK);
                if (bus != null) 
                {
                    int reservedNum =_context.Bookings.Where(book => book.LineID == lines[i].Id).Count();
                    lines[i].ReservedSeats = reservedNum;
                    lines[i].AvailableSeats = bus.Capacity - reservedNum;
                } 
            }
            return View(lines);
        }

        // GET: Lines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var line = await _context.Line
                .FirstOrDefaultAsync(m => m.Id == id);
            if (line == null)
            {
                return NotFound();
            }
            int reservedNum = _context.Bookings.Where(book => book.LineID == line.Id).Count();
            line.ReservedSeats = reservedNum;
            var bus = await _context.Bus.FindAsync(line.BusFK);
            line.AvailableSeats = bus.Capacity - reservedNum;
            return View(line);
        }

        // GET: Lines/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BusFK,From,To,DepartureTime,ArrivalTime,ticketPrice")] Line line)
        {
            if (ModelState.IsValid)
            {
                var bus = await _context.Bus.FindAsync(line.BusFK);
                line.AvailableSeats = bus.Capacity;
                line.ReservedSeats = 0;
                _context.Add(line);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(line);
        }
        // GET: Lines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var line = await _context.Line.FindAsync(id);
            if (line == null)
            {
                return NotFound();
            }

            return View(line);
        }

        // POST: Lines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BusFK,From,To,DepartureTime,ArrivalTime,ticketPrice,AvailableSeats,ReservedSeats")] Line line)
        {
            if (id != line.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(line);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LineExists(line.Id))
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
            return View(line);
        }

        // GET: Lines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var line = await _context.Line
                .FirstOrDefaultAsync(m => m.Id == id);
            if (line == null)
            {
                return NotFound();
            }
            int reservedNum = _context.Bookings.Where(book => book.LineID == line.Id).Count();
            line.ReservedSeats = reservedNum;
            var bus = await _context.Bus.FindAsync(line.BusFK);
            line.AvailableSeats = bus.Capacity - reservedNum;
            return View(line);
        }

        // POST: Lines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var line = await _context.Line.FindAsync(id);
            _context.Line.Remove(line);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AvailableLines()
        {
            var lines = await _context.Line.ToListAsync();
            List<Line> available_lines = new List<Line>();
            for (int i = 0; i < lines.Count(); i++)
            {
                var bus = _context.Bus.Find(lines[i].BusFK);
                if (bus != null)
                {
                    int reservedNum = _context.Bookings.Where(book => book.LineID == lines[i].Id).Count();
                    lines[i].ReservedSeats = reservedNum;
                    lines[i].AvailableSeats = bus.Capacity - reservedNum;
                }
                if (lines[i].AvailableSeats > 0)
                    available_lines.Add(lines[i]);
            }
            return View(available_lines);
        }
        public async Task<IActionResult> Book(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var line = await _context.Line
                .FirstOrDefaultAsync(m => m.Id == id);
            if (line == null)
            {
                return NotFound();
            }
            int reservedNum = _context.Bookings.Where(book => book.LineID == line.Id).Count();
            line.ReservedSeats = reservedNum;
            var bus = await _context.Bus.FindAsync(line.BusFK);
            line.AvailableSeats = bus.Capacity - reservedNum;
            return View(line);
        }
        [HttpPost, ActionName("Book")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookingConfirmed(int id)
        {
            string user_id = _userManager.GetUserId(HttpContext.User);
            Bookings booking = new Bookings
            {
                LineID = (int)id,
                UserID = user_id,
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Bookings");
        }
        private bool LineExists(int id)
        {
            return _context.Line.Any(e => e.Id == id);
        }
    }
}
