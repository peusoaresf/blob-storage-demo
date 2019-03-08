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

            long idDiretorio = Convert.ToInt64(Request.Params["parentFolderId"]);
            string token = Request.Params["token"];
            string fileName = Request.Params["fileName"];
            string mimeType = Request.Params["mimeType"];

            Arquivo arquivoReferencia = await ArquivoRules.AddAsync(fileName, false, idDiretorio);

            IFileManager fileManager = FileManagerFactory.Create();

            long fileSize = await fileManager.MergeChunksAsync(arquivoReferencia.Url, token);

            arquivoReferencia.Tamanho = fileSize;
            arquivoReferencia.MimeType = mimeType;

            await _arquivoRepository.UpdateAsync(arquivoReferencia);

            Response.Write(fileSize);
            Response.End();
        }
    }
}