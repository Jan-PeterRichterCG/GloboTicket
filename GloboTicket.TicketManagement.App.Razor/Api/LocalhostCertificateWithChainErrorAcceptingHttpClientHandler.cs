
// see https://www.conradakunga.com/blog/disable-ssl-certificate-validation-in-net/

namespace GloboTicket.TicketManagement.App.Razor
{
    using System = global::System;

    public class LocalhostCertificateWithChainErrorAcceptingHttpCLientHandler : System.Net.Http.HttpClientHandler
    {
        public LocalhostCertificateWithChainErrorAcceptingHttpCLientHandler() 
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
            {
               // if the server is on localhost we ignore RemoteCertificateChainErrors allowing for self-signed certificates without a proper certification chain
                if(message.RequestUri.AbsoluteUri.StartsWith("https://localhost") && sslPolicyErrors == System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) {
                    System.Console.WriteLine("RemoteCertificateChainErrors ignored for https://localhost"); //ToDo: use proper logging
                    return true;
                }
                return sslPolicyErrors == System.Net.Security.SslPolicyErrors.None;
            };
        }


    }

}