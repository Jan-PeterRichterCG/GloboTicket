
// see https://stackoverflow.com/questions/51642671/adding-handler-to-default-http-client-in-asp-net-core answer #8

using Microsoft.Extensions.Logging;

namespace GloboTicket.TicketManagement.App.Razor
{
    using System = global::System;

    public class CustomHttpMessageHandlerBuilder : Microsoft.Extensions.Http.HttpMessageHandlerBuilder
    {
        public override string Name { get; set; }
        public override System.Net.Http.HttpMessageHandler PrimaryHandler { get; set; }
        public override System.Collections.Generic.IList<System.Net.Http.DelegatingHandler> AdditionalHandlers => new System.Collections.Generic.List<System.Net.Http.DelegatingHandler>();
        // Our custom builder doesn't care about any of the above.
        private ILoggerFactory _loggerFactory;
        private ILogger<CustomHttpMessageHandlerBuilder> _logger;
        public CustomHttpMessageHandlerBuilder(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<CustomHttpMessageHandlerBuilder>();
        }

        public override System.Net.Http.HttpMessageHandler Build()
        {
            _logger.LogDebug("Build the HttpMessageHandler by creating a XLocalhostCertificateWithChainErrorAcceptingHttpCLientHandler");
            return new LocalhostCertificateWithChainErrorAcceptingHttpCLientHandler(_loggerFactory);
        }

        // see https://www.conradakunga.com/blog/disable-ssl-certificate-validation-in-net/
        private class LocalhostCertificateWithChainErrorAcceptingHttpCLientHandler : System.Net.Http.HttpClientHandler
        {
            private ILogger _logger;
            public LocalhostCertificateWithChainErrorAcceptingHttpCLientHandler(ILoggerFactory loggerFactory)
            {
                _logger = loggerFactory.CreateLogger<LocalhostCertificateWithChainErrorAcceptingHttpCLientHandler>();

                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    // if the server is on localhost we ignore RemoteCertificateChainErrors allowing for self-signed certificates without a proper certification chain
                    if (message.RequestUri.AbsoluteUri.StartsWith("https://localhost") && sslPolicyErrors == System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors)
                    {
                        _logger.LogWarning("RemoteCertificateChainErrors ignored for https://localhost");
                        return true;
                    }
                    return sslPolicyErrors == System.Net.Security.SslPolicyErrors.None;
                };
            }
        }
    }
}
