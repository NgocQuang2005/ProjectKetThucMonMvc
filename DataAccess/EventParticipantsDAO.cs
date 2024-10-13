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
            var eventParticipants = await _context.EventParticipants
                .Include(ep => ep.Event)   // Nạp dữ liệu liên quan từ bảng Event
                .Include(ep => ep.Account) // Nạp dữ liệu liên quan từ bảng Account
                .ToListAsync();
            return eventParticipants;
        }

        public async Task<EventParticipants> GetEventParticipantsById(int id)
        {
            var eventParticipant = await _context.EventParticipants
                .Include(ep => ep.Event)
                .Include(ep => ep.Account)
                .FirstOrDefaultAsync(ep => ep.IdEventParticipant == id);
            return eventParticipant;
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
        public async Task<EventParticipants?> GetEventParticipantsByEventAndUser(int eventId, int userId)
        {
            return await _context.EventParticipants
                .Include(ep => ep.Event)  // Nạp dữ liệu liên quan từ bảng Event
                .Include(ep => ep.Account) // Nạp dữ liệu liên quan từ bảng Account
                .FirstOrDefaultAsync(ep => ep.IdEvent == eventId && ep.IdAc == userId);
        }

    }
}
