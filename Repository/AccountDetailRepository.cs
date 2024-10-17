using Business;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class AccountDetailRepository : IAccountDetailRepository
    {
        public async Task Add(AccountDetail accountDetail)
        {
            await AccountDetailDAO.Instance.Add(accountDetail);
        }

        public async Task Delete(int id)
        {
            await AccountDetailDAO.Instance.Delete(id);
        }

        public async Task<IEnumerable<AccountDetail>> GetAccountDetailAll()
        {
            return await AccountDetailDAO.Instance.GetAccountDetailAll();
        }

        public async Task<AccountDetail> GetAccountDetailById(int id)
        {
            return await AccountDetailDAO.Instance.GetAccountDetailById(id);
        }
        public async Task Update(AccountDetail accountDetail)
        {
            await AccountDetailDAO.Instance.Update(accountDetail);
        }
        public async Task<bool> ChangeActive(int id)
        {
            return await AccountDetailDAO.Instance.ChangeActive(id);
        }
        public async Task<IEnumerable<AccountDetail>> GetActiveAccounts()
        {
            return await AccountDetailDAO.Instance.GetActiveAccounts();
        }

        public async Task<IEnumerable<AccountDetail>> GetInactiveAccounts()
        {
            return await AccountDetailDAO.Instance.GetInactiveAccounts();
        }
    }
}
