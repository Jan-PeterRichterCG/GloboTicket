using System.ComponentModel.DataAnnotations;

namespace GloboTicket.TicketManagement.Application.Models.Authentication
{
    public class UnregistrationRequest
    {
        [Required]
        public string UserName { get; set; }
    }
}
