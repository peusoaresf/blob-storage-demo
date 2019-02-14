<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="WebUI.Index" Async="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="https://fonts.googleapis.com/icon?family=Material+Icons">
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
        <p id="Blobs" runat="server" />

        <fieldset class="caixa">
            <legend>Criar Diretório</legend>
            <input type="text" id="InputNomeDiretorio" runat="server" />
            <button type="button" id="btnCriarDiretorio">Criar</button>
            <asp:Button OnClick="CriarDiretorio_Click" ID="btnCriarDiretorioSrv" style="display:none" runat="server" />
        </fieldset>

        <fieldset class="caixa">
            <legend>Adicionar Arquivo ao Diretório</legend>
            <input type="file" id="inputFile" multiple="multiple" />
            <button type="button" id="btnEnviarArquivo">Enviar</button>
        </fieldset>

        <br />
        <br />

        <p id="MensagemErro" runat="server"></p>
        <div id="containerProgresso"></div>

        <hr />

        <fieldset class="caixa">
            <legend runat="server" id="LegendDiretorioCorrente">Root</legend>
            <input type="hidden" id="InputIdDiretorioCorrente" runat="server" />
            <input type="hidden" id="InputIdArquivoSelecionado" runat="server" />

            <div runat="server" id="ContainerTabela"></div>
            <asp:Button OnClick="ExcluirDiretorio_Click" ID="btnExcluirDiretorioSrv" style="display:none" runat="server" />
        </fieldset>
    </form>
    
    <script
      src="https://code.jquery.com/jquery-3.3.1.min.js"
      integrity="sha256-FgpCb/KJQlLNfOu91ta32o/NMZxltwRo8QtmkMRdAu8="
      crossorigin="anonymous"></script>
    <script src="FileUploader.js"></script>
    <script>
        const abrirDiretorio = function (idArquivo) {
            $("#InputIdDiretorioCorrente").val(idArquivo);
            $("form")[0].submit();
        };

        const excluirArquivo = function (idArquivo) {
            $("#InputIdArquivoSelecionado").val(idArquivo);
            $("#btnExcluirDiretorioSrv").click();
        };

        const baixarArquivo = function (idArquivo) {
            $.ajax({
                type: "GET",
                url: "Ajax/File/FileDownload.aspx",
                data: {
                    idArquivo: idArquivo
                },
                dataType: "json",
                success: function (res) {
                    salvarArquivo(res.NomeArquivo, res.MimeType, res.Buffer);
                }
            });
        };

        const salvarArquivo = function (sNome, sTipo, aDataArray) {
            var a = document.createElement("a");
            document.body.appendChild(a);
            a.style = "display: none";
            var blob = new Blob([new Uint8Array(aDataArray)], {
                type: sTipo
            });
            var url = window.URL.createObjectURL(blob);
            a.href = url;
            a.download = sNome;
            a.click();
            window.URL.revokeObjectURL(url);
        };

        const isNomeDiretorioValido = function (sNome) {
            var bValido = true;

            $("td:first span", "table tbody tr").each(function (el) {
                if ($(this).text() === sNome) {
                    bValido = false;
                    return;
                }
            });

            return bValido;
        }

        $(document).ready(function () {
            $("#btnCriarDiretorio").click(function () {
                if (!$("#InputNomeDiretorio").val()) {
                    alert("insira o nome do diretório");
                }
                else if (!isNomeDiretorioValido($("#InputNomeDiretorio").val())) {
                    alert("O nome do diretório não pode ser repetido dentro desta pasta");
                }
                else {                    
                    $("#btnCriarDiretorioSrv").click();
                }
            });
            
            $("#btnEnviarArquivo").click(function () {
                if ($("#inputFile")[0].files.length) {
                    var fileUploader = new myJs.FileUploader($("#inputFile")[0].files, $("#InputIdDiretorioCorrente").val());

                    let contador = 0;

                    fileUploader.onUploadStarted = function (oFileHelper) {
                        oFileHelper.idProgresso = contador++;
                        $("#containerProgresso").append(
                            '<p>' + oFileHelper.file.name + ':<span style="left-margin: 10px" id="progressoUpload' + oFileHelper.idProgresso + '">0</span></p>'
                        );
                    };

                    fileUploader.onUploadProgress = function (oFileHelper, progress) {
                        var idProgresso = "#progressoUpload" + oFileHelper.idProgresso;
                        $(idProgresso).text((progress * 100).toFixed(2) + "%");
                    };

                    fileUploader.onUploadFinishing = function (oFileHelper) {
                        var idProgresso = "#progressoUpload" + oFileHelper.idProgresso;
                        $(idProgresso).text("Finalizando upload....");
                    };

                    fileUploader.onUploadFinished = function (oFileHelper) {
                        var idProgresso = "#progressoUpload" + oFileHelper.idProgresso;
                        $(idProgresso).text("Upload concluído com sucesso.");
                    };

                    fileUploader.onUploadError = function (oFileHelper) {
                        var idProgresso = "#progressoUpload" + oFileHelper.idProgresso;
                        $(idProgresso).text("Erro no upload. Tente novamente.");
                    };

                    fileUploader.onAllUploadFinished = function () {
                        alert("todos uploads concluídos");
                        $("form")[0].submit();
                    };

                    fileUploader.start();

                    /*var formData = new FormData();
                    formData.append("file", $("#inputFile")[0].files[0]);
                    formData.append("idDiretorio", $("#InputIdDiretorioCorrente").val());

                    try {
                        $.ajax({
                            type: "POST",
                            url: "Ajax/File/FileUpload.aspx",
                            data: formData,
                            processData: false,
                            contentType: false,
                            success: function (response) {
                                alert(response);
                                $("form")[0].submit();
                            },
                            error: function (err) {
                                alert(err.status + ' ' + err.statusText);
                                console.log(err);
                            }
                        });
                    }
                    catch (e) {
                        alert(JSON.stringify(e));
                    }*/
                }
                else {
                    alert("selecione um arquivo");
                }
            });
        });
    </script>
</body>
</html>
