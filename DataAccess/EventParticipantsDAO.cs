using Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class EventParticipantsDAO : SingletonBase<EventParticipantsDAO>
    {
        public async Task<IEnumerable<EventParticipants>> GetEventParticipantsAll()
        {
            var eventParticipants = await _context.EventParticipants.ToListAsync();
            return eventParticipants;
        }
        public async Task<EventParticipants> GetEventParticipantsById(int id)
        {
            var eventParticipants = await _context.EventParticipants.FirstOrDefaultAsync(ep => ep.IdEventParticipant == id);
            if (eventParticipants == null) return null;

            return eventParticipants;
        }
        public async Task Add(EventParticipants eventParticipants)
        {
            _context.EventParticipants.Add(eventParticipants);
            await _context.SaveChangesAsync();
        }
        public async Task Update(EventParticipants eventParticipants)
        {

            var existingItem = await GetEventParticipantsById(eventParticipants.IdEventParticipant);
            if (existingItem != null)
            {
                // Cập nhật các thuộc tính cần thiết
                _context.Entry(existingItem).CurrentValues.SetValues(eventParticipants);
                await _context.SaveChangesAsync();
            }

        }
        public async Task Delete(int id)
        {
            var eventParticipants = await GetEventParticipantsById(id);
            if (eventParticipants != null)
            {
                _context.EventParticipants.Remove(eventParticipants);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ChangeActive(int id)
        {
            var eventParticipants = await GetEventParticipantsById(id);
            eventParticipants.Active = !eventParticipants.Active;
            await _context.SaveChangesAsync();
            return eventParticipants.Active;
        }
    }
}
