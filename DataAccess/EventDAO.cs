using Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class EventDAO : SingletonBase<EventDAO>
    {
        public async Task<IEnumerable<Event>> GetEventAll()
        {
            var events = await _context.Events
                                       .Include(e => e.Account) // Include Account
                                       .ThenInclude(a => a.AccountDetail) // Include AccountDetail
                                       .ToListAsync();
            return events;
        }

        public async Task<Event> GetEventById(int id)
        {
            // Use Include to fetch related DocumentInfos and Account details
            var events = await _context.Events
                .Include(e => e.DocumentInfos)
                .Include(e => e.Account) // Include Account
                .ThenInclude(a => a.AccountDetail) // Include AccountDetail
                .FirstOrDefaultAsync(e => e.IdEvent == id);

            return events;
        }
        public async Task Add(Event events)
        {
            _context.Events.Add(events);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Event events)
        {

            var existingItem = await GetEventById(events.IdEvent);
            if (existingItem != null)
            {
                // Cập nhật các thuộc tính cần thiết
                _context.Entry(existingItem).CurrentValues.SetValues(events);
                await _context.SaveChangesAsync();
            }

        }
        public async Task Delete(int id)
        {
            var events = await GetEventById(id);
            if (events != null)
            {
                _context.Events.Remove(events);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ChangeActive(int id)
        {
            var events = await GetEventById(id);
            events.Active = !events.Active;
            await _context.SaveChangesAsync();
            return events.Active;
        }
    }
}
