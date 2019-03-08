using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WebUI.Classes
{
    public class ArquivoRules
    {
        public async static Task DeleteAsync(long id)
        {
            IArquivoRepository arquivoRepository = ArquivoRepositoryFactory.Create();

            if (!(await arquivoRepository.FindWhereParentEqualsAsync(id)).Any())
            {
                IFileManager fileManager = FileManagerFactory.Create();

                await fileManager.DeleteAsync(await arquivoRepository.FindByIdAsync(id));
                await arquivoRepository.DeleteAsync(id);
            }
            else
            {
                throw new ArgumentException("Diretório não pode ser excluído pois possui dependentes");
            }
        }

        public async static Task<Arquivo> AddAsync(string nome, bool isDiretorio, long fkParent)
        {
            IArquivoRepository arquivoRepository = ArquivoRepositoryFactory.Create();

            if (isDiretorio)
            {
                if ((await arquivoRepository.FindWhereParentAndNameEqualsAsync(fkParent, nome)).Any())
                {
                    throw new ArgumentException("Diretório não pode ser inserido com nome repetido dentro desta pasta");
                }
            }         

            // Adicionar verificação de versão para criar a referência com o versionamento correto

            Arquivo parent = await arquivoRepository.FindByIdAsync(fkParent);

            Arquivo arquivo = ArquivoFactory.Create(nome, isDiretorio, parent);

            await arquivoRepository.AddAsync(arquivo);

            return arquivo;
        }
    }
}