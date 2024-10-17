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
        Task<bool> ChangeActive(int id);
        Task<int> GetTotalEvents();
        Task<IEnumerable<Event>> GetActiveEvents();  // Thêm phương thức lấy sự kiện đang hoạt động
        Task<IEnumerable<Event>> GetInactiveEvents();  // Thêm phương thức lấy sự kiện không hoạt động
    }
}
    