using Business;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
