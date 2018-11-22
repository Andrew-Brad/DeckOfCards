using System;
using System.Net.Http;
using System.Text;
using DeckOfCards.Domain;
using Newtonsoft.Json;

namespace DeckOfCards.Test
{
    public static class TestExtensions
    {
        public static StringContent ToJsonStringContent(this object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, AB.Extensions.Common.StringConstants.HTTP.MimeTypes.ApplicationJson);
        }

        public static StringContent GetStringContent(object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, AB.Extensions.Common.StringConstants.HTTP.MimeTypes.ApplicationJson);
        }

        public static object ToAnonymousDto(this CardTemplate inputObject)
        {
            return new
            {
                WidgetName = inputObject.CardName,
                Suit = inputObject.Suit.Value,
                
            };
        }
    }
}
