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
                
                string json = await HandleFileDownloadAsync();

                Response.Write(json);
                Response.End();
            }
        }

        private async Task<string> HandleFileDownloadAsync()
        {
            Arquivo arquivo = await PegarArquivoRequestAsync();

            IFileManager fileManager = FileManagerFactory.Create();

            return await fileManager.DownloadAsync(arquivo);
        }

        private async Task<Arquivo> PegarArquivoRequestAsync()
        {
            long idArquivo = Convert.ToInt64(Request.Params["idArquivo"]);

            return await _arquivoRepository.FindByIdAsync(idArquivo);
        }        
    }
}