using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUI.Classes;

namespace WebUI.Ajax.File
{
    public partial class FileChunkMerge : System.Web.UI.Page
    {
        private IArquivoRepository _arquivoRepository;

        protected async void Page_Load(object sender, EventArgs e)
        {
            _arquivoRepository = ArquivoRepositoryFactory.Create();

            Response.Clear();

            string token = Request.Params["token"];
            string fileName = Request.Params["fileName"];

            Arquivo arquivoReferencia = await CriarReferenciaArquivoAsync(fileName);

            IFileManager fileManager = FileManagerFactory.Create();

            long fileSize = await fileManager.MergeChunksAsync(arquivoReferencia.Url, token);

            arquivoReferencia.Tamanho = fileSize;
            await _arquivoRepository.UpdateAsync(arquivoReferencia);

            Response.Write(fileSize);
            Response.End();
        }

        private async Task<Arquivo> CriarReferenciaArquivoAsync(string nomeArquivo)
        {
            long idDiretorio = Convert.ToInt64(Request.Params["parentFolderId"]);

            Arquivo parent = await _arquivoRepository.FindByIdAsync(idDiretorio);

            Arquivo novoArquivo = ArquivoFactory.Create(nomeArquivo, false, parent);

            await _arquivoRepository.AddAsync(novoArquivo);

            return novoArquivo;
        }
    }
}