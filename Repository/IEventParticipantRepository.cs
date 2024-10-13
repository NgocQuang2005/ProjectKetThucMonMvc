using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IEventParticipantRepository
    {
        Task<IEnumerable<EventParticipants>> GetEventParticipantsAll();
        Task<EventParticipants> GetEventParticipantsById(int id);
        Task Add(EventParticipants eventParticipants);
        Task Update(EventParticipants eventParticipants);
        Task Delete(int id);
        Task<bool> ChangeActive(int id);
        Task<EventParticipants?> GetEventParticipantsByEventAndUser(int eventId, int userId);
    }
}
