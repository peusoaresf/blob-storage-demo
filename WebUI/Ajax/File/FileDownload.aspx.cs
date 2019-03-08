using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUI.Classes;

namespace WebUI.Ajax.File
{
    public partial class FileDownload : System.Web.UI.Page
    {
        private IArquivoRepository _arquivoRepository;

        protected async void Page_Load(object sender, EventArgs e)
        {
            _arquivoRepository = ArquivoRepositoryFactory.Create();

            if (!IsPostBack)
            {
                Response.Clear();

                Arquivo arquivo = await PegarArquivoRequestAsync();

                IFileManager fileManager = FileManagerFactory.Create();

                using (Stream fileStream = await fileManager.GetStream(arquivo))
                {
                    Response.BufferOutput = false;
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + arquivo.Nome);
                    Response.AddHeader("Content-Length", fileStream.Length.ToString());
                    Response.ContentType = String.IsNullOrWhiteSpace(arquivo.MimeType) ? "application/octet-stream" : arquivo.MimeType;
                    fileStream.CopyTo(Response.OutputStream);
                    Response.End();
                }   
            }
        }
        
        private async Task<Arquivo> PegarArquivoRequestAsync()
        {
            long idArquivo = Convert.ToInt64(Request.Params["idArquivo"]);

            return await _arquivoRepository.FindByIdAsync(idArquivo);
        }        
    }
}