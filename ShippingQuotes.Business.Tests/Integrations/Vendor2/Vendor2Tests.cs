using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShippingQuotes.Business.DTO;
using ShippingQuotes.Business.Integrations.Vendor2;
using ShippingQuotes.Business.Interfaces;
using Xunit;

namespace ShippingQuotes.Business.Tests.Integrations.Vendor2
{
    public class Vendor2Tests
    {
        private class Vendor2ServiceWrapper : Vendor2Service
        {
            public Vendor2ServiceWrapper(IApiService apiService, IVendorUrlProvider urlProvider, IVendorCredentialsProvider credentialsProvider) 
                : base(apiService, urlProvider, credentialsProvider, new Mocks.LoggerMock<Vendor2Service>())
            {
            }

            public new decimal? ParseResponse(string response)
            {
                return base.ParseResponse(response);
            }

            public new AuthenticationHeaderValue GetAuthorizationHeaderValue()
            {
                return base.GetAuthorizationHeaderValue();
            }
        }

        [Theory]
        [InlineData("abc", "123")]
        public void WhenCredentialsAreValid_CreatesAuthheader(string username, string password)
        {
            var vendorService = new Vendor2ServiceWrapper(null, null, new Mocks.CredentialsProviderMock(username, password));
            var header = vendorService.GetAuthorizationHeaderValue();
            var bytes = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", username, password));
            var base64AuthString = Convert.ToBase64String(bytes);
            var testHeader = new AuthenticationHeaderValue("Basic", base64AuthString);
            Assert.Equal(testHeader.Parameter, header.Parameter);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void WhenCredentialsAreNotFound_ReturnsNullAuthHeader(string username, string password)
        {
            var vendorService = new Vendor2ServiceWrapper(null, null, new Mocks.CredentialsProviderMock(username, password));
            var header = vendorService.GetAuthorizationHeaderValue();
            Assert.Null(header.Parameter);
        }
 
        [Fact]
        public void WhenSerialized_ObjectContainsProperKeys() 
        {
            var requestData = new RequestData 
            { 
                consignee = "consignee",
                consignor = "consignor",
                cartons = new List<decimal> { }
            };
            var json = JsonConvert.SerializeObject(requestData);
            var jObject = JObject.Parse(json);
            Assert.NotNull(jObject["consignee"]);
            Assert.NotNull(jObject["consignor"]);
            Assert.NotNull(jObject["cartons"]);
        }

        [Fact]
        public void WhenMapped_RequestDataIsCorrect()
        {
            string origin = "Calgary",
                   destination = "Vancouver";
            var dimensions = new [] { 10m, 10m, 10m };
  
            var shipment = new Shipment
            {
                Origin = origin,
                Destination = destination,
                Dimensions = dimensions
            };
            var requestData = shipment.ToRequestData();
            Assert.Equal(origin, requestData.consignee);
            Assert.Equal(destination, requestData.consignor);
            Assert.Equal(dimensions, requestData.cartons);
        }

        [Theory]
        [InlineData(4.45)]
        [InlineData(0)]
        [InlineData(-1)]
        public void WhenResponseContainsTotal_ReturnsValue(decimal val)
        {
            var vendorService = new Vendor2ServiceWrapper(null, null, null);
            var response = JsonConvert.SerializeObject(new{ amount = val });
            Assert.Equal(val, vendorService.ParseResponse(response));
        }

        [Fact]
        public void WhenResponseIsNull_ReturnsNull()
        {
            var vendorService = new Vendor2ServiceWrapper(null, null, null);
            var response = JsonConvert.SerializeObject(new{ amount = (decimal?)null });
            Assert.Null(vendorService.ParseResponse(response));
        }

        [Fact]
        public void WhenResponseIsInvalid_ReturnsNull()
        {
            var vendorService = new Vendor2ServiceWrapper(null, null, null);
            var response = JsonConvert.SerializeObject(new { charge = 4.45m });
            // don't want the app to crash if the vendor changes the api contract
            Assert.Null(vendorService.ParseResponse(response));
            // handle unexpected response
            Assert.Null(vendorService.ParseResponse("error"));
        }

        [Fact]
        public void WhenResponseContainsOtherProperties_ReturnsTotal()
        {
            var vendorService = new Vendor2ServiceWrapper(null, null, null);
            var response = JsonConvert.SerializeObject(
                new {
                    amount = 4.45m,
                    description = "new property"
                }
            );
            Assert.Equal(4.45m, vendorService.ParseResponse(response));
        }
    }
}