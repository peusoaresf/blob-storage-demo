using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebUI.Classes
{
    public interface IArquivoRepository
    {
        Task AddAsync(Arquivo arquivo);
        Task DeleteAsync(long id);
        Task<IEnumerable<Arquivo>> FindAllAsync();
        Task<Arquivo> FindByIdAsync(long id);
        Task<IEnumerable<Arquivo>> FindWhereParentEqualsAsync(long id);
        Task<Arquivo> FindWhereParentIsNullAsync();
        Task UpdateAsync(Arquivo arquivo);
    }
}