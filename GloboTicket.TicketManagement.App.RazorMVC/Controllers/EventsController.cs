using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GloboTicket.TicketManagement.App.RazorMVC.Controllers
{
    public class EventsController : Controller
    {
        private readonly IGloboTicketClient _globoTicketClient;

        public EventsController(IGloboTicketClient globoTicketClient)
        {
            _globoTicketClient = globoTicketClient;
        }

        public async Task<IActionResult> Index()
        {
            var allEvents = await _globoTicketClient.GetAllEventsAsync();
            return View(allEvents);
        }

        public async Task<IActionResult> Details(Guid eventId)
        {
            var evnt = await _globoTicketClient.GetEventByIdAsync(eventId);
            return View(evnt);
        }
    }
}
