using Business;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ProjectParticipantRepository : IProjectParticipantRepository
    {
        public async Task Add(ProjectParticipant projectParticipant)
        {
            await ProjectParticipantDAO.Instance.Add(projectParticipant);
        }

        public async Task Delete(int id)
        {
            await ProjectParticipantDAO.Instance.Delete(id);
        }

        public async Task<IEnumerable<ProjectParticipant>> GetProjectParticipantAll()
        {
            return await ProjectParticipantDAO.Instance.GetProjectParticipantAll();
        }

        public async Task<ProjectParticipant> GetProjectParticipantById(int id)
        {
            return await ProjectParticipantDAO.Instance.GetProjectParticipantById(id);
        }
        public async Task Update(ProjectParticipant projectParticipant)
        {
            await ProjectParticipantDAO.Instance.Update(projectParticipant);
        }
    }
}
