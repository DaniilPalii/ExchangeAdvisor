using ExchangeAdvisor.Domain.Services.Implementation;
using ExchangeAdvisor.Domain.Values;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeAdvisor.Tests.UnitTests.DomainTests
{
    [TestFixture]
    public class RateFetcherTests
    {
        [SetUp]
        public void Setup()
        {
            httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            MockResponseMessage();

            httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(m => m.CreateClient("Exchange Rates API"))
                .Returns(new HttpClient(httpMessageHandlerMock.Object));

            rateFetcher = new RateWebFetcher(httpClientFactoryMock.Object);
        }

        [Test]
        public async Task WhenFetchHistoryAsync_ShouldSendProperRequest()
        {
            await rateFetcher
                .FetchAsync(
                    new DateRange(
                        new DateTime(2019, 1, 1),
                        new DateTime(2019, 1, 31)),
                    new CurrencyPair(Currency.USD, Currency.PLN))
                .ConfigureAwait(false);

            httpMessageHandlerMock.Protected()
                .Verify(
                    "SendAsync",
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(
                        req => req.Method == HttpMethod.Get
                               && req.RequestUri.ToString()
                                    == "https://api.exchangeratesapi.io/history" +
                                        "?start_at=2019-01-01" +
                                        "&end_at=2019-01-31" +
                                        "&base=USD" +
                                        "&symbols=PLN"),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task WhenFetchHistoryAsync_ShouldDeserializeResponseContentProperly()
        {
            MockResponseMessage(
                "{"
                    + "\"rates\":{"
                        + "\"2019-01-03\":{\"PLN\":1.111},"
                        + "\"2019-01-02\":{\"PLN\":2.222}"
                    + "},"
                    + "\"start_at\":\"2019-01-01\","
                    + "\"base\":\"USD\","
                    + "\"end_at\":\"2019-01-03\""
                + "}");

            var rateHistory = (await rateFetcher
                .FetchAsync(
                    new DateRange(
                        new DateTime(2019, 1, 1),
                        new DateTime(2019, 1, 2)),
                    new CurrencyPair(Currency.USD, Currency.PLN))
                .ConfigureAwait(false))
                    .ToArray();

            rateHistory.Should().BeEquivalentTo(new[]
            {
                new Rate(new DateTime(2019, 1, 2), 2.222f, new CurrencyPair(Currency.USD, Currency.PLN)),
                new Rate(new DateTime(2019, 1, 3), 1.111f, new CurrencyPair(Currency.USD, Currency.PLN))
            });
        }

        [Test]
        public async Task WhenFetchMultipleCurrenciesHistoryAsync_ShouldDeserializeResponseContentProperly()
        {
            MockResponseMessage(
                "{"
                    + "\"rates\":{"
                        + "\"2019-01-03\":{\"PLN\":1.111,\"CAD\":1.222},"
                        + "\"2019-01-02\":{\"PLN\":2.111,\"CAD\":2.222}"
                    + "},"
                    + "\"start_at\":\"2019-01-01\","
                    + "\"base\":\"EUR\","
                    + "\"end_at\":\"2019-01-03\""
                + "}");

            var rateHistory = (await rateFetcher
                .FetchAsync(
                    new DateRange(
                        new DateTime(2019, 1, 1),
                        new DateTime(2019, 1, 2)))
                .ConfigureAwait(false))
                    .ToArray();

            rateHistory.Should().BeEquivalentTo(new[]
            {
                new Rate(new DateTime(2019, 1, 2), 2.222f, new CurrencyPair(Currency.EUR, Currency.CAD)),
                new Rate(new DateTime(2019, 1, 2), 2.111f, new CurrencyPair(Currency.EUR, Currency.PLN)),
                new Rate(new DateTime(2019, 1, 3), 1.222f, new CurrencyPair(Currency.EUR, Currency.CAD)),
                new Rate(new DateTime(2019, 1, 3), 1.111f, new CurrencyPair(Currency.EUR, Currency.PLN))
            });
        }

        private void MockResponseMessage(string messageContent = "")
        {
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(messageContent)
                    })
                .Verifiable();
        }

        private RateWebFetcher rateFetcher;
        private Mock<IHttpClientFactory> httpClientFactoryMock;
        private Mock<HttpMessageHandler> httpMessageHandlerMock;
    }
}