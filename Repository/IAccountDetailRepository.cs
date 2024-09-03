using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IAccountDetailRepository
    {
        Task<IEnumerable<AccountDetail>> GetAccountDetailAll();
        Task<AccountDetail> GetAccountDetailById(int id);
        Task Add(AccountDetail accountDetail);
        Task Update(AccountDetail accountDetail);
        Task Delete(int id);
        Task<bool> ChangeActive(int id);
    }
}
