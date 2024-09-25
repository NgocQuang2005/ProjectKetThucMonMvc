using Business;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public interface IDocumentInfoRepository
    {
        Task<IEnumerable<DocumentInfo>> GetDocumentInfoAll(); // Lấy danh sách tất cả DocumentInfo
        Task<DocumentInfo?> GetDocumentInfoById(int id); // Lấy DocumentInfo theo ID (nullable)
        Task Add(DocumentInfo documentInfo); // Thêm mới DocumentInfo
        Task Update(DocumentInfo documentInfo); // Cập nhật DocumentInfo
        Task Delete(int id); // Xóa DocumentInfo
        Task<bool> ChangeActive(int id); // Thay đổi trạng thái Active
        Task<DocumentInfo?> GetByAccountId(int accountId); // Lấy DocumentInfo theo IdAccount
        Task<IEnumerable<DocumentInfo>> GetDocumentInfosByArtworkId(int artworkId);
        Task<IEnumerable<DocumentInfo>> GetDocumentInfoByEventId(int eventId);

    }
}
