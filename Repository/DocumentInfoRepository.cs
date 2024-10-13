using Business;
using DataAccess;
using System.Collections.Generic;
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

        // Triển khai phương thức GetDocumentInfoByAccountId
        public async Task<DocumentInfo?> GetDocumentInfoByAccountId(int accountId)
        {
            return await DocumentInfoDAO.Instance.GetDocumentInfoByAccountId(accountId);
        }

        public async Task<IEnumerable<DocumentInfo>> GetDocumentInfosByArtworkId(int artworkId)
        {
            return await DocumentInfoDAO.Instance.GetDocumentInfosByArtworkId(artworkId);
        }

        public async Task<IEnumerable<DocumentInfo>> GetDocumentInfoByEventId(int eventId)
        {
            return await DocumentInfoDAO.Instance.GetDocumentInfoByEventId(eventId);
        }

        public async Task<IEnumerable<DocumentInfo>> GetDocumentInfoByProjectId(int projectId)
        {
            return await DocumentInfoDAO.Instance.GetDocumentInfoByProjectId(projectId);
        }
        public async Task<DocumentInfo?> GetDocumentInfoByIdAsNoTracking(int id)
        {
            return await DocumentInfoDAO.Instance.GetDocumentInfoByIdAsNoTracking(id);
        }

    }
}
