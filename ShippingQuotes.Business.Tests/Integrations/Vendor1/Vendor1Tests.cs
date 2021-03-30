using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShippingQuotes.Business.DTO;
using ShippingQuotes.Business.Integrations.Vendor1;
using ShippingQuotes.Business.Interfaces;
using Xunit;

namespace ShippingQuotes.Business.Tests.Integrations.Vendor1
{
    public class Vendor1Tests
    {
        private class Vendor1ServiceWrapper : Vendor1Service
        {
            public Vendor1ServiceWrapper(IApiService apiService, IVendorUrlProvider urlProvider, IVendorCredentialsProvider credentialProvider) 
                : base(apiService, urlProvider, credentialProvider, new Mocks.LoggerMock<Vendor1Service>())
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
            var vendorService = new Vendor1ServiceWrapper(null, null, new Mocks.CredentialsProviderMock(username, password));
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
            var vendorService = new Vendor1ServiceWrapper(null, null, new Mocks.CredentialsProviderMock(username, password));
            var header = vendorService.GetAuthorizationHeaderValue();
            Assert.Null(header.Parameter);
        }

        [Fact]
        public void WhenSerialized_ObjectContainsProperKeys()
        {
            var requestData = new RequestData() 
            { 
                contactAddress = "ca",
                warehouseAddress = "wa",
                packageDimensions = new List<decimal> { }
            };
            var json = JsonConvert.SerializeObject(requestData);

            var jObject= JObject.Parse(json);
            Assert.NotNull(jObject["contact address"]);
            Assert.NotNull(jObject["warehouse address"]);
            Assert.NotNull(jObject["package dimensions"]);
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
            Assert.Equal(origin, requestData.contactAddress);
            Assert.Equal(destination, requestData.warehouseAddress);
            Assert.Equal(dimensions, requestData.packageDimensions);
        }

        [Theory]
        [InlineData(4.45)]
        [InlineData(0)]
        [InlineData(-1)]
        public void WhenResponseContainsTotal_ReturnsValue(decimal val)
        {
            var vendorService = new Vendor1ServiceWrapper(null, null, null);
            var response = JsonConvert.SerializeObject(new{ total = val });
            Assert.Equal(val, vendorService.ParseResponse(response));
        }

        [Fact]
        public void WhenResponseIsNull_ReturnsNull()
        {
            var vendorService = new Vendor1ServiceWrapper(null, null, null);
            var response = JsonConvert.SerializeObject(new{ total = (decimal?)null });
            Assert.Null(vendorService.ParseResponse(response));
        }

        [Fact]
        public void WhenResponseIsInvalid_ReturnsNull()
        {
            var vendorService = new Vendor1ServiceWrapper(null, null, null);
            var response = JsonConvert.SerializeObject(new { charge = 4.45m });
            // don't want the app to crash if the vendor changes the api contract
            Assert.Null(vendorService.ParseResponse(response));
            // handle unexpected response
            Assert.Null(vendorService.ParseResponse("error"));
        }

        [Fact]
        public void WhenResponseContainsOtherProperties_ReturnsTotal()
        {
            var vendorService = new Vendor1ServiceWrapper(null, null, null);
            var response = JsonConvert.SerializeObject(
                new {
                    total = 4.45m,
                    description = "new property"
                }
            );
            Assert.Equal(4.45m, vendorService.ParseResponse(response));
        }
    }
}