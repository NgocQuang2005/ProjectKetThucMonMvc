using Business;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public interface IArtworkRepository
    {
        Task<IEnumerable<Artwork>> GetArtworkAll(); // Danh sách tất cả Artwork
        Task<Artwork?> GetArtworkById(int id); // Artwork theo ID (nullable)
        Task Add(Artwork artwork); // Thêm mới Artwork
        Task Update(Artwork artwork); // Cập nhật Artwork
        Task Delete(int id); // Xóa Artwork
        Task<bool> ChangeActive(int id);
        Task<Artwork> GetArtworkByIdAsNoTracking(int id);
        Task<int> GetTotalArtwork();

    }
}
