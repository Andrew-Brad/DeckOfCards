using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DeckOfCards.Test
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Add an Accept header to the HttClient with value "application/json"
        /// </summary>
        /// <param name="client"></param>
        public static void SetAcceptHeaderToJson(this HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Add an Accept header to the HttClient with value "application/json"
        /// </summary>
        /// <param name="client"></param>
        public static HttpClient WithJsonAcceptHeader(this HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(AB.Extensions.Common.StringConstants.HTTP.MimeTypes.ApplicationJson));
            return client;
        }

        /// <summary>
        /// Add an Accept header to the HttClient with value "application/xml"
        /// </summary>
        /// <param name="client"></param>
        public static void SetAcceptHeaderToXml(this HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(AB.Extensions.Common.StringConstants.HTTP.MimeTypes.ApplicationXml));
        }

        /// <summary>
        /// Add an Accept header to the HttClient with value "application/json"
        /// </summary>
        /// <param name="client"></param>
        public static HttpClient WithXmlAcceptHeader(this HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(AB.Extensions.Common.StringConstants.HTTP.MimeTypes.ApplicationXml));
            return client;
        }

        /// <summary>
        /// Add an Accept header to the HttClient with value "application/xml"
        /// </summary>
        /// <param name="client"></param>
        public static void SetAcceptHeaderToSomethingStrange(this HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(AB.Extensions.Common.StringConstants.HTTP.MimeTypes.ApplicationOctetStream));
        }
    }
}
