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
        private static string CONN_STRING = "Data Source=TENTIRJD0155;Initial Catalog=blobstoragedemodb;User ID=restuser;Password=123456";

        public async Task AddAsync(Arquivo arquivo)
        {
            arquivo.DataCriacao = DateTime.Now;

            using (var sqlConnection = new SqlConnection(CONN_STRING))
            {
                await sqlConnection.OpenAsync();

                SqlCommand sqlCommand = sqlConnection.CreateCommand();

                sqlCommand.CommandText = 
                    "insert into Arquivo( "
                    + "nome, "
                    + "url, "
                    + "ind_diretorio, "
                    + "fk_parent, "
                    + "datahora_criacao, "
                    + "tamanho) "
                    + "output INSERTED.id_arquivo "
                    + "values(@param0, @param1, @param2, @param3, @param4, @param5)";
                
                sqlCommand.Parameters.AddWithValue("@param0", arquivo.Nome);
                sqlCommand.Parameters.AddWithValue("@param1", arquivo.Url);
                sqlCommand.Parameters.AddWithValue("@param2", arquivo.IsDiretorio);
                sqlCommand.Parameters.AddWithValue("@param3", arquivo.FkParent);
                sqlCommand.Parameters.AddWithValue("@param4", arquivo.DataCriacao);
                sqlCommand.Parameters.AddWithValue("@param5", arquivo.Tamanho);

                long generatedId = (long) await sqlCommand.ExecuteScalarAsync();

                arquivo.IdArquivo = generatedId;
            }
        }

        public async Task DeleteAsync(long id)
        {
            using (var sqlConnection = new SqlConnection(CONN_STRING))
            {
                await sqlConnection.OpenAsync();

                SqlCommand sqlCommand = sqlConnection.CreateCommand();

                sqlCommand.CommandText = "delete from Arquivo where id_arquivo = @param0";

                sqlCommand.Parameters.AddWithValue("@param0", id);

                await sqlCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<Arquivo>> FindAllAsync()
        {
            var arquivos = new List<Arquivo>();

            using (var sqlConnection = new SqlConnection(CONN_STRING))
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
                        Arquivo arquivo = await ExtrairArquivoDataRow(dr);
                        arquivos.Add(arquivo);
                    }
                }
            }

            return arquivos;
        }

        public async Task<Arquivo> FindByIdAsync(long id)
        {
            Arquivo arquivo = null;

            using (var sqlConnection = new SqlConnection(CONN_STRING))
            {
                await sqlConnection.OpenAsync();

                SqlCommand sqlCommand = sqlConnection.CreateCommand();

                sqlCommand.CommandText = "select * from Arquivo where id_arquivo = @param0";

                sqlCommand.Parameters.AddWithValue("@param0", id);

                using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                {
                    var dtArquivo = new DataTable();
                    dtArquivo.Load(sqlDataReader);

                    if (dtArquivo.Rows.Count > 0)
                    {
                        arquivo = await ExtrairArquivoDataRow(dtArquivo.Rows[0]);
                    }
                }
            }

            return arquivo;
        }

        public async Task<IEnumerable<Arquivo>> FindWhereParentAndNameEqualsAsync(long? idParent, string nome)
        {
            var arquivos = new List<Arquivo>();

            using (var sqlConnection = new SqlConnection(CONN_STRING))
            {
                await sqlConnection.OpenAsync();

                SqlCommand sqlCommand = sqlConnection.CreateCommand();

                sqlCommand.CommandText = "select * from Arquivo where fk_parent = @param0 and nome = @param1";

                sqlCommand.Parameters.AddWithValue("@param0", idParent);
                sqlCommand.Parameters.AddWithValue("@param1", nome);

                using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                {
                    var dtArquivo = new DataTable();
                    dtArquivo.Load(sqlDataReader);

                    foreach (DataRow dr in dtArquivo.Rows)
                    {
                        Arquivo arquivo = await ExtrairArquivoDataRow(dr);
                        arquivos.Add(arquivo);
                    }
                }
            }

            return arquivos;
        }

        public async Task<IEnumerable<Arquivo>> FindWhereParentEqualsAsync(long? id)
        {
            var arquivos = new List<Arquivo>();

            if (id == null)
            {
                arquivos.Add(await FindWhereParentIsNullAsync());
            }
            else
            {
                using (var sqlConnection = new SqlConnection(CONN_STRING))
                {
                    await sqlConnection.OpenAsync();

                    SqlCommand sqlCommand = sqlConnection.CreateCommand();

                    sqlCommand.CommandText = "select * from Arquivo where fk_parent = @param0";

                    sqlCommand.Parameters.AddWithValue("@param0", id);

                    using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        var dtArquivo = new DataTable();
                        dtArquivo.Load(sqlDataReader);

                        foreach (DataRow dr in dtArquivo.Rows)
                        {
                            Arquivo arquivo = await ExtrairArquivoDataRow(dr);
                            arquivos.Add(arquivo);
                        }
                    }
                }
            }

            return arquivos;
        }

        public async Task<Arquivo> FindWhereParentIsNullAsync()
        {
            Arquivo arquivo = null;

            using (var sqlConnection = new SqlConnection(CONN_STRING))
            {
                await sqlConnection.OpenAsync();

                SqlCommand sqlCommand = sqlConnection.CreateCommand();

                sqlCommand.CommandText = "select * from Arquivo where fk_parent is null";
                
                using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                {
                    var dtArquivo = new DataTable();
                    dtArquivo.Load(sqlDataReader);

                    if (dtArquivo.Rows.Count > 0)
                    {
                        arquivo = await ExtrairArquivoDataRow(dtArquivo.Rows[0]);                        
                    }
                }
            }

            return arquivo;
        }

        public async Task UpdateAsync(Arquivo arquivo)
        {
            using (var sqlConnection = new SqlConnection(CONN_STRING))
            {
                await sqlConnection.OpenAsync();

                SqlCommand sqlCommand = sqlConnection.CreateCommand();

                sqlCommand.CommandText =
                    "update Arquivo set "
                    + "nome = @param0, "
                    + "url = @param1, "
                    + "ind_diretorio = @param2, "
                    + "fk_parent = @param3, "
                    + "datahora_criacao = @param4, "
                    + "tamanho = @param5 "
                    + "where "
                    + "id_arquivo = @param6";

                sqlCommand.Parameters.AddWithValue("@param0", arquivo.Nome);
                sqlCommand.Parameters.AddWithValue("@param1", arquivo.Url);
                sqlCommand.Parameters.AddWithValue("@param2", arquivo.IsDiretorio);
                sqlCommand.Parameters.AddWithValue("@param3", arquivo.FkParent);
                sqlCommand.Parameters.AddWithValue("@param4", arquivo.DataCriacao);
                sqlCommand.Parameters.AddWithValue("@param5", arquivo.Tamanho);
                sqlCommand.Parameters.AddWithValue("@param6", arquivo.IdArquivo);

                await sqlCommand.ExecuteNonQueryAsync();
            }
        }

        private async Task<Arquivo> ExtrairArquivoDataRow(DataRow dr)
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

            return arquivo;
        } 
    }
}