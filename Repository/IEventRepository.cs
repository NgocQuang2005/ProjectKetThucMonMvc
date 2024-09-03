using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetEventAll();
        Task<Event> GetEventById(int id);
        Task Add(Event events);
        Task Update(Event events);
        Task Delete(int id);
    }
}
