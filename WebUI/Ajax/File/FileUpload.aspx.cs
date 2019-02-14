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

                long result = await HandleFileUpload();               
                
                Response.Write(result);                
                Response.End();                
            }
        }

        private async Task<long> HandleFileUpload()
        {            
            HttpPostedFile httpPostedFile = Request.Files["file"];
            
            string urlArquivo = await CriarReferenciaArquivo(httpPostedFile);

            FileManager fileManager = FileManagerFactory.Create();

            return await fileManager.Upload(httpPostedFile.InputStream, urlArquivo);
        }

        private async Task<string> CriarReferenciaArquivo(HttpPostedFile arquivo)
        {
            long idDiretorio = Convert.ToInt64(Request.Params["idDiretorio"]);

            Arquivo parent = await _arquivoRepository.FindById(idDiretorio);

            Arquivo novoArquivo = ArquivoFactory.Create();
            novoArquivo.IsDiretorio = false;
            novoArquivo.Nome = arquivo.FileName;
            novoArquivo.Tamanho = arquivo.ContentLength;
            novoArquivo.Url = $"{parent.Url}{arquivo.FileName}";
            novoArquivo.Parent = parent;

            await _arquivoRepository.Add(novoArquivo);

            return novoArquivo.Url;
        }
    }   
}