using GloboTicket.TicketManagement.Application.Contracts.Persistence;
using GloboTicket.TicketManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GloboTicket.TicketManagement.Persistence.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(GloboTicketDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<Category>> GetCategoriesWithEvents(bool includePassedEvents)
        {
            var allCategories = await _dbContext.Categories.Include(x => x.Events).ToListAsync();
            if(!includePassedEvents)
            {
                foreach(Category p in allCategories)
                {
                    // Events is only an ICollection<Event> but not a List<Events>. Therefore, you cannot p.Events.RemoveAll(c => c.Date < DateTime.Today);
                    var newList = p.Events.ToList();
                    newList.RemoveAll(c => c.Date < DateTime.Today);
                    p.Events = newList;
                }
            }
            return allCategories;
        }
    }
}
