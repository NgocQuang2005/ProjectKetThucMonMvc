using Business;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class DocumentInfoRepository : IDocumentInfoRepository
    {
        public async Task Add(DocumentInfo documentInfo)
        {
            await DocumentInfoDAO.Instance.Add(documentInfo);
        }

        public async Task Delete(int id)
        {
            await DocumentInfoDAO.Instance.Delete(id);
        }

        public async Task<IEnumerable<DocumentInfo>> GetDocumentInfoAll()
        {
            return await DocumentInfoDAO.Instance.GetDocumentInfoAll();
        }

        public async Task<DocumentInfo> GetDocumentInfoById(int id)
        {
            return await DocumentInfoDAO.Instance.GetDocumentInfoById(id);
        }
        public async Task Update(DocumentInfo documentInfo)
        {
            await DocumentInfoDAO.Instance.Update(documentInfo);
        }
        public async Task<bool> ChangeActive(int id)
        {
            return await DocumentInfoDAO.Instance.ChangeActive(id);
        }
    }
}
