
// see https://stackoverflow.com/questions/51642671/adding-handler-to-default-http-client-in-asp-net-core answer #8

namespace GloboTicket.TicketManagement.App.Razor
{
    using System = global::System;

    public class CustomHttpMessageHandlerBuilder : Microsoft.Extensions.Http.HttpMessageHandlerBuilder
    {
        public override string Name { get; set; }
        public override System.Net.Http.HttpMessageHandler PrimaryHandler { get; set; }
        public override System.Collections.Generic.IList<System.Net.Http.DelegatingHandler> AdditionalHandlers => new System.Collections.Generic.List<System.Net.Http.DelegatingHandler>();
        // Our custom builder doesn't care about any of the above.
        public override System.Net.Http.HttpMessageHandler Build()
        {
            return new LocalhostCertificateWithChainErrorAcceptingHttpCLientHandler();
        }
    }
}
