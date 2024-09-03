using Business;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EventRepository : IEventRepository
    {
        public async Task Add(Event events)
        {
            await EventDAO.Instance.Add(events);
        }

        public async Task Delete(int id)
        {
            await EventDAO.Instance.Delete(id);
        }

        public async Task<IEnumerable<Event>> GetEventAll()
        {
            return await EventDAO.Instance.GetEventAll();
        }

        public async Task<Event> GetEventById(int id)
        {
            return await EventDAO.Instance.GetEventById(id);
        }
        public async Task Update(Event events)
        {
            await EventDAO.Instance.Update(events);
        }
    }
}
