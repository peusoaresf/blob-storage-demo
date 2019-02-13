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
        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Response.Clear();

                string result = await HandleFileUpload();               
                
                Response.Write(result);                
                Response.End();                
            }
        }

        private async Task<string> HandleFileUpload()
        {            
            HttpPostedFile httpPostedFile = Request.Files["file"];
            
            string urlArquivo = CriarReferenciaArquivo(httpPostedFile.FileName);

            var fileManager = new FileManager();

            return await fileManager.Upload(httpPostedFile.InputStream, urlArquivo);
        }

        private string CriarReferenciaArquivo(string nomeArquivo)
        {
            long idDiretorio = Convert.ToInt64(Request.Params["idDiretorio"]);

            Arquivo parent = ArquivoRepository.FindById(idDiretorio);

            var novoArquivo = new Arquivo()
            {
                IsDiretorio = false,
                Nome = nomeArquivo,
                Url = $"{parent.Url}{nomeArquivo}",
                Parent = parent
            };

            ArquivoRepository.Add(novoArquivo);

            return novoArquivo.Url;
        }
    }   
}