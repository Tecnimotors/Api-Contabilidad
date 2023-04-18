using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Renci.SshNet;
using System.Collections;
using System.IO;


namespace ApiContabilidad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SSHController : ControllerBase
    {
        [HttpGet("DescargarCDR")]
        public IActionResult DescargarCDR(string cdr, string url)
        {
            var cdrclear = cdr.Trim();

            ///Summary R-20100990998-01-F912-00004227.zip
            var host = "190.81.59.142";
            var userName = "contador";
            var passPhrase = "tecni20100";
            var priv = @"C:\Users\Provisional04\Desktop\ConsoleApp1\keys\contadortecni";
            string path = Directory.GetCurrentDirectory()+"\\keys";


            var authMethod = new PrivateKeyAuthenticationMethod(userName, new PrivateKeyFile(priv, passPhrase));

            var connectionInfo = new Renci.SshNet.ConnectionInfo(host, 9972, userName, authMethod);

            var client = new SftpClient(connectionInfo);

            client.Connect();
            var paths = client.ListDirectory(".");

            var remoteFolderPath = "/home/contador/documents/bizlinks/CDR/";
            var files = client.ListDirectory(remoteFolderPath);
            var pruebanombre = "";
            var fil = client.ListDirectory(remoteFolderPath).Where(f => !f.IsDirectory);
           
            var ruta = url.ToString() + "\\";
            foreach (var file in fil)
            {

                //  R-20100990998-01-F912-00004227.zip
                if (file.Name == cdrclear)
                {
                    pruebanombre = file.Name;
                    if (cdrclear == pruebanombre)
                    {
                        using (Stream file1 = System.IO.File.Create(ruta + pruebanombre))
                        {
                            client.DownloadFile(remoteFolderPath + pruebanombre, file1);
                            return Ok(new { code = 200, estado = "Success", msg = "Se descargo Con exito" });
                        }
                    }
                    /*
                    else
                    {
                        return Ok(new { code = 400, estado = "warning", msg = "Ya tiene descargado Este Archivo" });
                    }
                    */
                }
                //return Ok(new {  code = 400, estado = "Estado Inactivo", msg = "No se Encontro Archivo CDR" });

            }
            Console.WriteLine(pruebanombre);
            return Ok(remoteFolderPath + pruebanombre);
        }


        [HttpGet("ListarCDR")]
        public IActionResult ListarCDR()
        {
            var host = "190.81.59.142";
            var userName = "contador";
            var passPhrase = "tecni20100";
            var priv = @"C:\Users\Provisional04\Desktop\ConsoleApp1\keys\contadortecni";



            var authMethod = new PrivateKeyAuthenticationMethod(userName, new PrivateKeyFile(priv, passPhrase));

            var connectionInfo = new Renci.SshNet.ConnectionInfo(host, 9972, userName, authMethod);

            var client = new SftpClient(connectionInfo);
            client.Connect();
            List<string> values = new();
            var remoteFolderPath = "/home/contador/documents/bizlinks/CDR/";
            var fil = client.ListDirectory(remoteFolderPath);
            foreach (var file in fil)
            {
                values.Add(file.FullName);
            }
            return Ok(fil);
        }
    }
}
