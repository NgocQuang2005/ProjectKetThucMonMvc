using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IProjectParticipantRepository
    {
        Task<IEnumerable<ProjectParticipant>> GetProjectParticipantAll();
        Task<ProjectParticipant> GetProjectParticipantById(int id);
        Task Add(ProjectParticipant projectParticipant);
        Task Update(ProjectParticipant projectParticipant);
        Task Delete(int id);
        Task<bool> ChangeActive(int id);
        Task DeleteByProjectId(int projectId);
        Task<IEnumerable<ProjectParticipant>> GetProjectParticipantsByProjectId(int projectId);
        Task DeleteByProjectAndAccount(int projectId, int accountId);
        Task<int> GetParticipantCountByProjectId(int projectId);  // Số người tham gia theo Id dự án
    }
}
