using Business;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public class ArtworkRepository : IArtworkRepository
    {
        public async Task Add(Artwork artwork)
        {
            await ArtworkDAO.Instance.Add(artwork);
        }

        public async Task Delete(int id)
        {
            await ArtworkDAO.Instance.Delete(id);
        }

        public async Task<IEnumerable<Artwork>> GetArtworkAll()
        {
            return await ArtworkDAO.Instance.GetArtworkAll();
        }

        public async Task<Artwork> GetArtworkById(int id)
        {
            return await ArtworkDAO.Instance.GetArtworkById(id);
        }

        public async Task Update(Artwork artwork)
        {
            await ArtworkDAO.Instance.Update(artwork);
        }
        public async Task<bool> ChangeActive(int id)
        {
            return await ArtworkDAO.Instance.ChangeActive(id);
        }
        public async Task<Artwork> GetArtworkByIdAsNoTracking(int id)
        {
            return await ArtworkDAO.Instance.GetArtworkByIdAsNoTracking(id);
        }
        public async Task<int> GetTotalArtwork()
        {
            return await ArtworkDAO.Instance.GetTotalArtwork();
        }
    }
}
