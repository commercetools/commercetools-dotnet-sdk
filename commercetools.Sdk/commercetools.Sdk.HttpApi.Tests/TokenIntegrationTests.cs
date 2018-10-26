namespace commercetools.Sdk.HttpApi.Tests
{
    using commercetools.Sdk.HttpApi;
    using commercetools.Sdk.HttpApi.Domain;
    using commercetools.Sdk.Serialization;
    using System.Net.Http;
    using Xunit;

    // TODO Thing of better names for tests
    // These are not real unit tests, but something like "integration tests" and a way to test code in a simple way
    public class TokenIntegrationTests
    {
        [Fact]
        public void GetClientCredentialsToken()
        {
            ISerializerService serializerService = TestUtils.GetSerializerService();
            IClientConfiguration clientConfiguration = TestUtils.GetClientConfiguration("Client");
            // Resetting scope to an empty string for testing purposes
            clientConfiguration.Scope = "";
            IHttpClientFactory httpClientFactory = new MockHttpClientFactory(null, null, null);
            ITokenStoreManager tokenStoreManager = new InMemoryTokenStoreManager();
            ITokenProvider tokenProvider = new ClientCredentialsTokenProvider(httpClientFactory, clientConfiguration, tokenStoreManager, serializerService);
            Token token = tokenProvider.Token;
            Assert.NotNull(token.AccessToken);
        }

        [Fact]
        public void GetClientCredentialsTokenWithScope()
        {
            ISerializerService serializerService = TestUtils.GetSerializerService();
            IClientConfiguration clientConfiguration = TestUtils.GetClientConfiguration("ClientWithSmallerScope");
            IHttpClientFactory httpClientFactory = new MockHttpClientFactory(null, null, null);
            ITokenStoreManager tokenStoreManager = new InMemoryTokenStoreManager();
            ITokenProvider tokenProvider = new ClientCredentialsTokenProvider(httpClientFactory, clientConfiguration, tokenStoreManager, serializerService);
            Token token = tokenProvider.Token;
            Assert.NotNull(token.AccessToken);
            Assert.Equal(clientConfiguration.Scope, token.Scope);
        }

        [Fact]
        public void GetPasswordToken()
        {
            ISerializerService serializerService = TestUtils.GetSerializerService();
            IClientConfiguration clientConfiguration = TestUtils.GetClientConfiguration("ClientWithSmallerScope");
            IHttpClientFactory httpClientFactory = new MockHttpClientFactory(null, null, null);
            IUserCredentialsStoreManager userCredentialsStoreManager = new InMemoryUserCredentialsStoreManager();
            userCredentialsStoreManager.Username = "mick.jagger@commercetools.com";
            userCredentialsStoreManager.Password = "st54e9m4";
            ITokenProvider tokenProvider = new PasswordTokenProvider(httpClientFactory, clientConfiguration, userCredentialsStoreManager, serializerService);
            Token token = tokenProvider.Token;
            Assert.NotNull(token.AccessToken);
        }

        [Fact]
        public void GetAnonymousTokenNoIdProvided()
        {
            ISerializerService serializerService = TestUtils.GetSerializerService();
            IClientConfiguration clientConfiguration = TestUtils.GetClientConfiguration("ClientWithAnonymousScope");
            IHttpClientFactory httpClientFactory = new MockHttpClientFactory(null, null, null);
            IAnonymousCredentialsStoreManager anonymousStoreManager = new InMemoryAnonymousCredentialsStoreManager();
            ITokenProvider tokenProvider = new AnonymousSessionTokenProvider(httpClientFactory, clientConfiguration, anonymousStoreManager, serializerService);
            Token token = tokenProvider.Token;
            Assert.NotNull(token.AccessToken);
        }

        [Fact]
        public void GetAnonymousTokenIdProvided()
        {
            ISerializerService serializerService = TestUtils.GetSerializerService();
            IClientConfiguration clientConfiguration = TestUtils.GetClientConfiguration("ClientWithAnonymousScope");
            IHttpClientFactory httpClientFactory = new MockHttpClientFactory(null, null, null);
            IAnonymousCredentialsStoreManager anonymousStoreManager = new InMemoryAnonymousCredentialsStoreManager();
            anonymousStoreManager.AnonymousId = TestUtils.RandomString(10);
            ITokenProvider tokenProvider = new AnonymousSessionTokenProvider(httpClientFactory, clientConfiguration, anonymousStoreManager, serializerService);
            Token token = tokenProvider.Token;
            Assert.NotNull(token.AccessToken);
        }

        [Fact]
        public void RefreshTokenPasswordFlow()
        {
            ISerializerService serializerService = TestUtils.GetSerializerService();
            IClientConfiguration clientConfiguration = TestUtils.GetClientConfiguration("ClientWithSmallerScope");
            IHttpClientFactory httpClientFactory = new MockHttpClientFactory(null, null, null);
            IUserCredentialsStoreManager userCredentialsStoreManager = new InMemoryUserCredentialsStoreManager();
            userCredentialsStoreManager.Username = "mick.jagger@commercetools.com";
            userCredentialsStoreManager.Password = "st54e9m4";
            ITokenProvider tokenProvider = new PasswordTokenProvider(httpClientFactory, clientConfiguration, userCredentialsStoreManager, serializerService);
            Token token = tokenProvider.Token;
            string initialAccessToken = token.AccessToken;
            // TODO Find a better way to test this (with mock objects perhaps)
            token.ExpiresIn = 0;
            token = tokenProvider.Token;
            Assert.NotEqual(token.AccessToken, initialAccessToken);
        }
    }
}