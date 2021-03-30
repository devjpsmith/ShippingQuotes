using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShippingQuotes.Business.Interfaces;
using ShippingQuotes.Business.Services;
using Xunit;

namespace ShippingQuotes.Business.Tests.Services
{
    public class QuoteServiceTests
    {
        [Fact]
        public async void WhenVendorSupplied_ReturnsQuote()
        {
            var vendorServices = new List<IVendorService> 
            { 
                new Mocks.VendorServiceMock { TestQuote = 5m }
            }; 
            var quoteService = new QuoteService(vendorServices);
            var result = await quoteService.GetBestQuoteAsync(null, null, null);
            Assert.Equal(5m, result);
        }

        [Fact]
        public async void WhenMultipleVendorsSupplied_ReturnsLowestQuote()
        {
            var vendorServices = new List<IVendorService> 
            { 
                new Mocks.VendorServiceMock { TestQuote = 5m },
                new Mocks.VendorServiceMock { TestQuote = 2m }
            }; 
            var quoteService = new QuoteService(vendorServices);
            var result = await quoteService.GetBestQuoteAsync(null, null, null);
            Assert.Equal(2m, result);
        }

        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(3, 2, null)]
        [InlineData(null, 2, null)]
        public async void WhenVendorResponseIsNull_IgnoresNullResponse(int? d1, int? d2, int? d3)
        {
            var vendorServices = new List<IVendorService> 
            { 
                new Mocks.VendorServiceMock { TestQuote = (decimal?)d1 },
                new Mocks.VendorServiceMock { TestQuote = (decimal?)d2 },
                new Mocks.VendorServiceMock { TestQuote = (decimal?)d3 },
            }; 
            var quoteService = new QuoteService(vendorServices);
            var result = 0m;
            result = await quoteService.GetBestQuoteAsync(null, null, null);
            var expected = new []{ (decimal?)d1, (decimal?)d2, (decimal?)d3 }.Where(d => d.HasValue).Min();
            Assert.Equal(expected, result);
        }
        
    }
}