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

            var fileManager = new FileManager();
            return await fileManager.Upload(httpPostedFile.InputStream, httpPostedFile.FileName);
        }
    }   
}