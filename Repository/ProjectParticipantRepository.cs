using Business;
using DataAccess;
using Microsoft.EntityFrameworkCore;
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
        public async Task<bool> ChangeActive(int id)
        {
            return await ProjectParticipantDAO.Instance.ChangeActive(id);
        }

        public async Task DeleteByProjectId(int projectId)
        {
            // Lấy danh sách tất cả người tham gia dựa trên IdProject
            var participants = await ProjectParticipantDAO.Instance.GetProjectParticipantsByProjectId(projectId);

            // Nếu tồn tại người tham gia, xóa từng người tham gia
            if (participants != null && participants.Any())
            {
                foreach (var participant in participants)
                {
                    await ProjectParticipantDAO.Instance.Delete(participant.IdProjectParticipant);
                }
            }
        }
        public async Task<IEnumerable<ProjectParticipant>> GetProjectParticipantsByProjectId(int projectId)
        {
            return await ProjectParticipantDAO.Instance.GetProjectParticipantsByProjectId(projectId);
        }
        public async Task DeleteByProjectAndAccount(int projectId, int accountId)
        {
            var participant = await ProjectParticipantDAO.Instance.GetProjectParticipantByProjectAndAccount(projectId, accountId);
            if (participant != null)
            {
                await ProjectParticipantDAO.Instance.Delete(participant.IdProjectParticipant);
            }
        }
        public async Task<int> GetParticipantCountByProjectId(int projectId)
        {
            return await ProjectParticipantDAO.Instance.GetParticipantCountByProjectId(projectId);
        }
    }
}
