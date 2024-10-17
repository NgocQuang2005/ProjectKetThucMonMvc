using Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess
{
    public class EventDAO : SingletonBase<EventDAO>
    {
        public async Task<IEnumerable<Event>> GetEventAll()
        {
            // Tránh lỗi trùng lặp AccountDetail bằng cách chỉ truy xuất khi thực sự cần thiết
            var events = await _context.Events
                                       .Include(e => e.Account)// Gộp Account trước
                                       .ThenInclude(a => a.AccountDetail)
                                       .AsNoTracking()
                                       .ToListAsync();
            return events;
        }

        public async Task<Event> GetEventById(int id)
        {
            // Chỉ truy xuất AccountDetail khi cần thiết
            var events = await _context.Events
                .Include(e => e.DocumentInfos)
                .Include(e => e.Account)
                .ThenInclude(a => a.AccountDetail)  // Tránh truy xuất lặp AccountDetail
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.IdEvent == id);

            return events;
        }
        public async Task<int> GetTotalEvents()
        {
            return await _context.Events.AsNoTracking().CountAsync();
        }
        public async Task Add(Event events)
        {
            _context.Events.Add(events);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Event events)
        {
            _context.Entry(events).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }


        public async Task Delete(int id)
        {
            var events = await GetEventById(id);
            if (events != null)
            {
                _context.Entry(events).State = EntityState.Detached;

                if (events.Account?.AccountDetail != null)
                {
                    _context.Entry(events.Account.AccountDetail).State = EntityState.Detached;
                }

                foreach (var doc in events.DocumentInfos)
                {
                    _context.Entry(doc).State = EntityState.Detached;
                }

                _context.Events.Remove(events);
                await _context.SaveChangesAsync();
            }
        }   

        public async Task<bool> ChangeActive(int id)
        {
            var events = await GetEventById(id);
            if (events != null)
            {
                events.Active = !events.Active;
                _context.Entry(events).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return events.Active;
            }
            return false;
        }
        public async Task<IEnumerable<Event>> GetActiveEvents()
        {
            return await _context.Events
                                 .Where(e => e.Active)  // Lọc sự kiện có Active = true
                                 .Include(e => e.Account)
                                 .ThenInclude(a => a.AccountDetail)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        // Lấy tất cả sự kiện không hoạt động (Active = false)
        public async Task<IEnumerable<Event>> GetInactiveEvents()
        {
            return await _context.Events
                                 .Where(e => !e.Active) // Lọc sự kiện có Active = false
                                 .Include(e => e.Account)
                                 .ThenInclude(a => a.AccountDetail)
                                 .AsNoTracking()
                                 .ToListAsync();
        }
    }
}
