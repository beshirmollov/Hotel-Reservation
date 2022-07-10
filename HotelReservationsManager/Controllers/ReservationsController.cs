using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelReservationsManager.Data;
using HotelReservationsManager.Data.Entities;

namespace HotelReservationsManager.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index(string creatorFirstName)
        {
            var applicationDbContext = _context.Reservations.Include(r => r.ReservedRoom).Include(r => r.User);
            ViewData["ClientsReservations"] = _context.ClientReservations;
            //ViewBag.ReservationsId = _context.ClientReservations.Select(x => x.ReservationId).ToList();

            var reservations = await applicationDbContext.ToListAsync();

            if (!String.IsNullOrEmpty(creatorFirstName))
            {
                reservations = reservations.Where(c => c.User.FirstName.Contains(creatorFirstName)).ToList();
            }

            return View(reservations);
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.ReservedRoom)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["ReservedRoomId"] = new SelectList(_context.Rooms.Where(r => !r.InUse).ToList(), "Id", "Type");
            ViewData["Users"] = new SelectList(_context.Users, "Id", "FirstName");
            ViewBag.Clients = new MultiSelectList(_context.Clients, "Id", "FirstName");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReservedRoomId,UserId,DateSet,DateSetOff,HaveBreakfast,AllInclusive,TotalPrice")] Reservation reservation, int[] clients)
        {

            if (ModelState.IsValid)
            {
                if (clients != null)
                {
                    

                    foreach (var clientId in clients)
                    {
                        var client = _context.Clients.Find(clientId);

                        var clientReservation = new ClientReservation()
                        {
                            Client = client,
                            ClientId = clientId,
                            Reservation = reservation,
                            ReservationId = reservation.Id
                        };
                        _context.ClientReservations.Add(clientReservation);
                    }

                    reservation.ReservedRoom = _context.Rooms.Find(reservation.ReservedRoomId);

                    reservation.ReservedRoom.InUse = true;

                    var allClients = reservation.ClientReservations.Where(x => x.ReservationId == reservation.Id).Select(x => x.Client).ToList();

                    var days = (int)reservation.DateSetOff.Day - (int)reservation.DateSet.Day;

                    var adultClientsPrice = allClients.Select(x => x.IsAdult).Count() * reservation.ReservedRoom.AdultBedPrice;
                    var childClientsPrice = allClients.Select(x => x.IsAdult == false).Count() * reservation.ReservedRoom.ChildBedPrice;

                    var totalPrice = adultClientsPrice + childClientsPrice;

                    if (reservation.HaveBreakfast)
                    {
                        totalPrice += 30;
                    }
                    if (reservation.AllInclusive)
                    {
                        totalPrice += 50;
                    }

                    reservation.TotalPrice = totalPrice;
                }

                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            

            ViewData["ReservedRoomId"] = new SelectList(_context.Rooms, "Id", "Type", reservation.ReservedRoomId);
            ViewData["Users"] = new SelectList(_context.Users, "Id", "FirstName", reservation.UserId);
            ViewBag.Clients = new MultiSelectList(_context.Clients, "Id", "FirstName");
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["ReservedRoomId"] = new SelectList(_context.Rooms, "Id", "Type", reservation.ReservedRoomId);
            ViewData["Users"] = new SelectList(_context.Users, "Id", "FirstName", reservation.UserId);
            ViewBag.Clients = new MultiSelectList(_context.Clients, "Id", "FirstName");
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReservedRoomId,UserId,DateSet,DateSetOff,HaveBreakfast,AllInclusive,TotalPrice")] Reservation reservation, int[] clients)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (clients != null)
                    {
                        foreach (var clientId in clients)
                        {
                            var client = _context.Clients.Find(clientId);

                            var clientReservation = new ClientReservation()
                            {
                                Client = client,
                                Reservation = reservation
                            };
                            _context.ClientReservations.Add(clientReservation);
                        }
                    }
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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
            ViewData["ReservedRoomId"] = new SelectList(_context.Rooms, "Id", "Type", reservation.ReservedRoomId);
            ViewData["Users"] = new SelectList(_context.Users, "Id", "FirstName", reservation.UserId);
            ViewBag.Clients = new MultiSelectList(_context.Clients, "Id", "FirstName");
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.ReservedRoom)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clientReservation = _context.ClientReservations.Where(cr => cr.ReservationId == id).ToList();

            foreach (var item in clientReservation)
            {
                _context.ClientReservations.Remove(item);
            }

            var reservation = await _context.Reservations.FindAsync(id);
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
