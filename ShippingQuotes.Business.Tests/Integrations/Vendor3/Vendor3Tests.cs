using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using ShippingQuotes.Business.Helpers;
using ShippingQuotes.Business.Integrations.Vendor3;
using ShippingQuotes.Business.Interfaces;
using Xunit;

namespace ShippingQuotes.Business.Tests.Integrations.Vendor3
{
    public class Vendor3Tests
    {
        private class Vendor3ServiceWrapper : Vendor3Service
        {
            public Vendor3ServiceWrapper(IApiService apiService, IVendorUrlProvider urlProvider, IVendorCredentialsProvider credentialsProvider) 
                : base(apiService, urlProvider, credentialsProvider, new Mocks.LoggerMock<Vendor3Service>())
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
            var vendorService = new Vendor3ServiceWrapper(null, null, new Mocks.CredentialsProviderMock(username, password));
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
            var vendorService = new Vendor3ServiceWrapper(null, null, new Mocks.CredentialsProviderMock(username, password));
            var header = vendorService.GetAuthorizationHeaderValue();
            Assert.Null(header.Parameter);
        }

        /// <summary>
        /// Ensures the serialzed XML contains elements with the expected names
        /// </summary>
        [Fact]
        public void WhenSerialized_ObjectContainsProperElements()
        {
            var requestData = new RequestData
            {
                source = "s",
                destination = "d",
                packages = new List<decimal> { 1 }
            };
            var xml = XmlSerializationHelper.GetXml(requestData);
            Assert.Contains("<source>", xml);
            Assert.Contains("<destination>", xml);
            Assert.Contains("<packages>", xml);
            Assert.Contains("<package>", xml);
        }

        [Theory]
        [InlineData(4.45)]
        [InlineData(0)]
        [InlineData(-1)]
        public void WhenResponseContainsQuote_ReturnsValue(decimal val)
        {
            var vendor3Service = new Vendor3ServiceWrapper(null, null, null);
            var response = $"<quote>{val}</quote>";
            Assert.Equal(val, vendor3Service.ParseResponse(response));
        }
        [Fact]
        public void WhenResponseIsNull_ReturnsNull()
        {
            var vendor3Service = new Vendor3ServiceWrapper(null, null, null);
            var response = $"<quote />";
            Assert.Null(vendor3Service.ParseResponse(response));
        }

        [Fact]
        public void WhenResponseIsInvalid_ReturnsNull()
        {
            var vendor3Service = new Vendor3ServiceWrapper(null, null, null);
            var response = "<amount />";
            // don't want the app to crash if the vendor changes the api contract
            Assert.Null(vendor3Service.ParseResponse(response));
            // handle unexpected response
            Assert.Null(vendor3Service.ParseResponse("error"));
        }

        [Fact]
        public void WhenResponseContainsOtherProperties_ReturnsTotal()
        {
            var vendor3Service = new Vendor3ServiceWrapper(null, null, null);
            var response = "<xml><quote>4.45</quote><desc>Description</desc></xml>";
            Assert.Equal(4.45m, vendor3Service.ParseResponse(response));
        }

    }
}