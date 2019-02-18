using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WebUI.Classes
{
    public class SqlServerArquivoRepository : IArquivoRepository
    {
        public async Task AddAsync(Arquivo arquivo)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Arquivo>> FindAllAsync()
        {
            var arquivos = new List<Arquivo>();

            using (SqlConnection sqlConnection = new SqlConnection("Data Source=TENTIRJD0155;Initial Catalog=blobstoragedemodb;User ID=restuser;Password=123456"))
            {
                await sqlConnection.OpenAsync();

                SqlCommand sqlCommand = sqlConnection.CreateCommand();

                sqlCommand.CommandText = "select * from Arquivo";
                
                using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                {
                    var dtArquivo = new DataTable();
                    dtArquivo.Load(sqlDataReader);

                    foreach (DataRow dr in dtArquivo.Rows)
                    {
                        bool isDiretorio = (bool) dr["ind_diretorio"];
                        long id = Convert.ToInt64(dr["id_arquivo"]);
                        long tamanho = Convert.ToInt64(dr["tamanho"]);
                        string nome = dr["nome"].ToString();
                        string url = dr["url"].ToString();
                        DateTime dataCriacao = Convert.ToDateTime(dr["datahora_criacao"]);
                        Arquivo parent = null;

                        if (dr["fk_parent"] != DBNull.Value)
                        {
                            parent = await FindByIdAsync(Convert.ToInt64(dr["fk_parent"]));
                        }

                        Arquivo arquivo = ArquivoFactory.Create();
                        arquivo.IsDiretorio = isDiretorio;
                        arquivo.IdArquivo = id;
                        arquivo.Tamanho = tamanho;
                        arquivo.Nome = nome;
                        arquivo.Url = url;
                        arquivo.DataCriacao = dataCriacao;
                        arquivo.Parent = parent;

                        arquivos.Add(arquivo);
                    }
                }
            }

            return arquivos;
        }

        public async Task<Arquivo> FindByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Arquivo>> FindWhereParentAndNameEqualsAsync(long? id, string nome)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Arquivo>> FindWhereParentEqualsAsync(long? id)
        {
            throw new NotImplementedException();
        }

        public async Task<Arquivo> FindWhereParentIsNullAsync()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Arquivo arquivo)
        {
            throw new NotImplementedException();
        }
    }
}