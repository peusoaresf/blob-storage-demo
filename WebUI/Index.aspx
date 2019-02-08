<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="WebUI.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        .caixa {
            width: fit-content;
            display: inline-block;
        }

        .cabecalho {
            text-align: left;
        }
    </style>
</head>
<body>
    <form runat="server">
        <fieldset class="caixa">
            <legend>Criar Diretório</legend>
            <input type="text" id="InputNomeDiretorio" runat="server" />
            <button type="button" id="btnCriarDiretorio">Criar</button>
            <asp:Button OnClick="CriarDiretorio_Click" ID="btnCriarDiretorioSrv" style="display:none" runat="server" />
        </fieldset>

        <fieldset class="caixa">
            <legend>Adicionar Arquivo ao Diretório</legend>
            <input type="file" id="inputFile" />
            <button type="button" id="btnEnviarArquivo">Enviar</button>
        </fieldset>

        <br />
        <br />
        <hr />

        <fieldset class="caixa">
            <legend runat="server" id="LegendDiretorioCorrente">Root</legend>
            <input type="hidden" id="InputIdDiretorioCorrente" runat="server" />

            <div runat="server" id="ContainerTabela"></div>
        </fieldset>
    </form>

    <!--<thead>
        <tr>
            <th class="cabecalho" style="width: 200px">Nome</th>
            <th class="cabecalho" style="width: 150px">Data Upload</th>
            <th class="cabecalho" style="width: 50px"></th>
        </tr>
    </thead>-->

    <!--<tbody>
        <tr>
        <td>
            <a href="#">Arquivo1.txt</a>
        </td>
        <td>
            10/11/2018
        </td>
        <td>
            <button type="button" btn="btnExcluirArquivo">Excluir</button>
        </td>
        </tr>
    </tbody-->
    
    <!--<tfoot>
        <tr>
            <td>Sum</td>
            <td>$180</td>
        </tr>
    </tfoot>-->

    <script
      src="https://code.jquery.com/jquery-3.3.1.min.js"
      integrity="sha256-FgpCb/KJQlLNfOu91ta32o/NMZxltwRo8QtmkMRdAu8="
      crossorigin="anonymous"></script>
    <script>
        $(document).ready(function () {

            $("#btnCriarDiretorio").click(function () {
                if (!$("#InputNomeDiretorio").val()) {
                    alert("insira o nome do diretório");
                }
                else {                    
                    $("#btnCriarDiretorioSrv").click();
                }
            });
            
            $("#btnEnviarArquivo").click(function () {
                if ($("#inputFile")[0].files.length) {
                    var formData = new FormData();
                    formData.append("file", $("#inputFile")[0].files[0]);

                    $.ajax({
                        type: "POST",
                        url: "Ajax/File/FileUpload.aspx",
                        data: formData,
                        processData: false,
                        contentType: false,
                        success: function (response) {
                            alert(response);
                            $("#inputFile").val("")
                        }
                    });
                }
                else {
                    alert("selecione um arquivo");
                }
            });


        });
    </script>
</body>
</html>
