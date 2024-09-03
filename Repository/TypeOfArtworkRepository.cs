using Business;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class TypeOfArtworkRepository : ITypeOfArtworkRepository
    {
        public async Task Add(TypeOfArtwork typeOfArtwork)
        {
            await TypeOfArtworkDAO.Instance.Add(typeOfArtwork);
        }

        public async Task Delete(int id)
        {
            await TypeOfArtworkDAO.Instance.Delete(id);
        }

        public async Task<IEnumerable<TypeOfArtwork>> GetTypeOfArtworkAll()
        {
            return await TypeOfArtworkDAO.Instance.GetTypeOfArtworkAll();
        }

        public async Task<TypeOfArtwork> GetTypeOfArtworkById(int id)
        {
            return await TypeOfArtworkDAO.Instance.GetTypeOfArtworkById(id);
        }
        public async Task Update(TypeOfArtwork typeOfArtwork)
        {
            await TypeOfArtworkDAO.Instance.Update(typeOfArtwork);
        }
        public async Task<bool> ChangeActive(int id)
        {
            return await TypeOfArtworkDAO.Instance.ChangeActive(id);
        }
    }
}
