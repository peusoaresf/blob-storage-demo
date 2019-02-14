using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUI.Classes;

namespace WebUI.Ajax.File
{
    public partial class FileChunkMerge : System.Web.UI.Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();

            string token = Request.Params["token"];
            string fileName = Request.Params["fileName"];

            var fileManager = new FileManager();

            Response.Write(await fileManager.MergeChunks(fileName, token));            
            Response.End();
        }
    }
}