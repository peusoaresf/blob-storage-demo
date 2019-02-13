using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WebUI.Classes;

namespace WebUI
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LimparMensagemErro();
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            Arquivo diretorio; 

            if (!IsPostBack)
            {
                diretorio = ArquivoRepository.FindWhereParentIsNull();                
            }
            else
            {
                diretorio = ArquivoRepository.FindById(PegarIdDiretorioCorrente());
            }

            ListarArquivos(diretorio);
        }
        
        private long PegarIdDiretorioCorrente()
        {
            return Convert.ToInt64(this.InputIdDiretorioCorrente.Value);
        }

        private long PegarIdArquivoSelecionado()
        {
            return Convert.ToInt64(this.InputIdArquivoSelecionado.Value);
        }

        private void ListarArquivos(Arquivo diretorio)
        {            

            SinalizarDiretorioCorrente(diretorio);

            IEnumerable<Arquivo> arquivosNoDiretorio = ArquivoRepository.FindWhereParentEquals(diretorio.IdArquivo);

            MontarTabela(arquivosNoDiretorio);
        }

        private void SinalizarDiretorioCorrente(Arquivo arquivo)
        {
            this.InputIdDiretorioCorrente.Value = arquivo.IdArquivo.ToString();
            this.LegendDiretorioCorrente.InnerHtml = MontarNavegacaoDiretorio(arquivo);
        }

        private string MontarNavegacaoDiretorio(Arquivo arquivo)
        {
            Arquivo aux = arquivo;

            var stringBuilder = new StringBuilder();

            stringBuilder.Insert(0, $"\\{aux.Nome}");
            aux = aux.Parent;

            while (aux != null)
            {
                stringBuilder.Insert(0, $"\\<a href='#' onclick='abrirDiretorio(\"{aux.IdArquivo}\")'>{aux.Nome}</a>");
                aux = aux.Parent;
            }

            return stringBuilder.ToString();
        }

        private void MontarTabela(IEnumerable<Arquivo> arquivos)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("<table>");
            stringBuilder.Append(MontarHeaderTabela());

            if (!arquivos.Any())
            {
                stringBuilder.Append(MontarFooterDiretorioVazio());
            }
            else
            {
                stringBuilder.Append(MontarCorpoTabela(arquivos));
            }

            stringBuilder.Append("</table>");

            this.ContainerTabela.InnerHtml = stringBuilder.ToString();
        }

        private string MontarHeaderTabela()
        {
            return
                "<thead>"
                + "<tr>"
                + "<th class='cabecalho' style='width: 250px'>Nome</th>"
                + "<th class='cabecalho' style='width: 150px'>Data Criação</th>"
                + "<th class='cabecalho' style='width: 50px'></th>"
                + "</tr>"
                + "</thead>";
        }
        
        private string MontarFooterDiretorioVazio()
        {
            return
                "<tfoot>"
                + "<tr>"
                + "<td>Diretório vazio</td>"
                + "</tr>"
                + "</tfoot>";
        }

        private string MontarCorpoTabela(IEnumerable<Arquivo> arquivos)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("<tbody>");

            foreach (var arquivo in arquivos)
            {
                string titleArquivo = arquivo.IsDiretorio 
                                        ? "Navegar para o diretório" 
                                        : "Baixar arquivo";

                string clickArquivo = arquivo.IsDiretorio 
                                        ? $"onclick = 'abrirDiretorio(\"{arquivo.IdArquivo}\")'" 
                                        : $"onclick = 'baixarArquivo(\"{arquivo.IdArquivo}\")'";

                string iconeArquivo = arquivo.IsDiretorio
                                        ? "folder"
                                        : "insert_drive_file";

                stringBuilder.Append(
                    "<tr>"
                    + "<td>"
                    + $"<i class='material-icons'>{iconeArquivo}</i><a href='#' title='{titleArquivo}' {clickArquivo}><span>{arquivo.Nome}</span></a>"
                    + "</td>"
                    + $"<td>{arquivo.DataCriacao.ToString()}</td>"
                    + "<td>"
                    + $"<button type='button' btn='btnExcluirArquivo' onclick='excluirArquivo(\"{arquivo.IdArquivo}\")'>Excluir</button>"
                    + "</td>"
                    + "</tr>"
                );
            }

            stringBuilder.Append("</tbody>");

            return stringBuilder.ToString();
        }
         
        private void ExibirMensagemErro(string msg)
        {
            this.MensagemErro.InnerText = msg;
        }

        private void LimparMensagemErro()
        {
            this.MensagemErro.InnerText = String.Empty;
        }

        protected void CriarDiretorio_Click(object sender, EventArgs e)
        {
            string nomeDiretorio = this.InputNomeDiretorio.Value;
            Arquivo parent = ArquivoRepository.FindById(PegarIdDiretorioCorrente());

            var novoDiretorio = new Arquivo()
            {
                IsDiretorio = true,
                Nome = nomeDiretorio,
                Url = $"{parent.Url}{nomeDiretorio}/",
                Parent = parent
            };

            ArquivoRepository.Add(novoDiretorio);
        }

        protected async void ExcluirDiretorio_Click(object sender, EventArgs e)
        {
            long idArquivo = PegarIdArquivoSelecionado();

            if (ArquivoRepository.FindWhereParentEquals(idArquivo).Any())
            {
                ExibirMensagemErro("Diretório não pode ser excluído pois possui dependentes");                
            }
            else
            {
                var fileManager = new FileManager();
                await fileManager.Delete(ArquivoRepository.FindById(idArquivo));

                ArquivoRepository.Delete(idArquivo);
            }
        }        
    }
}