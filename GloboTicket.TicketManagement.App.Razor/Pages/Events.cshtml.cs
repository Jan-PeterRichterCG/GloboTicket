using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GloboTicket.TicketManagement.App.Razor.Pages
{
    public class EventsModel : PageModel
    {
        private readonly IGloboTicketClient globoTicketClient;

        public IEnumerable<EventListVm> AllEvents { get; set; }

        public EventsModel(IGloboTicketClient globoTicketClient)
        {
            this.globoTicketClient = globoTicketClient;
        }

        public async Task OnGet()
        {
            AllEvents = await globoTicketClient.GetAllEventsAsync(); 
        }
    }
}
