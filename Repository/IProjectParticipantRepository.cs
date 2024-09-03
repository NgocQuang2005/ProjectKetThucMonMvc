﻿using Business;
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
    }
}