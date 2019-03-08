using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUI.Classes;

namespace WebUI.Ajax.File
{
    public partial class FileUpload : System.Web.UI.Page
    {
        private IArquivoRepository _arquivoRepository;

        protected async void Page_Load(object sender, EventArgs e)
        {
            _arquivoRepository = ArquivoRepositoryFactory.Create();

            if (!IsPostBack)
            {
                Response.Clear();

                long result = await HandleFileUploadAsync();               
                
                Response.Write(result);                
                Response.End();                
            }
        }

        private async Task<long> HandleFileUploadAsync()
        {            
            HttpPostedFile httpPostedFile = Request.Files["file"];
            
            string urlArquivo = await CriarReferenciaArquivoAsync(httpPostedFile);

            IFileManager fileManager = FileManagerFactory.Create();

            return await fileManager.UploadAsync(httpPostedFile.InputStream, urlArquivo);
        }

        private async Task<string> CriarReferenciaArquivoAsync(HttpPostedFile arquivo)
        {
            long idDiretorio = Convert.ToInt64(Request.Params["idDiretorio"]);

            Arquivo novoArquivo = await ArquivoRules.AddAsync(arquivo.FileName, false, idDiretorio);

            novoArquivo.Tamanho = arquivo.ContentLength;
            novoArquivo.MimeType = arquivo.ContentType;

            await _arquivoRepository.UpdateAsync(novoArquivo);

            return novoArquivo.Url;
        }
    }   
}