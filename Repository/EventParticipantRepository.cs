using Business;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EventParticipantRepository : IEventParticipantRepository
    {
        public async Task Add(EventParticipants eventParticipants)
        {
            await EventParticipantsDAO.Instance.Add(eventParticipants);
        }

        public async Task Delete(int id)
        {
            await EventParticipantsDAO.Instance.Delete(id);
        }

        public async Task<IEnumerable<EventParticipants>> GetEventParticipantsAll()
        {
            return await EventParticipantsDAO.Instance.GetEventParticipantsAll();
        }

        public async Task<EventParticipants> GetEventParticipantsById(int id)
        {
            return await EventParticipantsDAO.Instance.GetEventParticipantsById(id);
        }
        public async Task Update(EventParticipants eventParticipants)
        {
            await EventParticipantsDAO.Instance.Update(eventParticipants);
        }
        public async Task<bool> ChangeActive(int id)
        {
            return await EventParticipantsDAO.Instance.ChangeActive(id);
        }
    }
}
