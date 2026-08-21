using Microsoft.AspNetCore.Mvc.Testing;
using Music.DataAccess.Data;

namespace Music_portal.Tests;

public class AuthorizationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthorizationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClientNoRedirect() =>
        _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Fact]
    public async Task AnonymousSongCreate_RedirectsToLogin()
    {
        var client = CreateClientNoRedirect();

        var response = await client.GetAsync("/Song/Create");

        Assert.Equal(System.Net.HttpStatusCode.Found, response.StatusCode);
        Assert.Contains("/Auth/Login", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task AnonymousMySongs_RedirectsToLogin()
    {
        var client = CreateClientNoRedirect();

        var response = await client.GetAsync("/Song/MySongs");

        Assert.Equal(System.Net.HttpStatusCode.Found, response.StatusCode);
        Assert.Contains("/Auth/Login", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task AnonymousAdminPanel_RedirectsToLogin()
    {
        var client = CreateClientNoRedirect();

        var response = await client.GetAsync("/Admin");

        Assert.Equal(System.Net.HttpStatusCode.Found, response.StatusCode);
        Assert.Contains("/Auth/Login", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task AnonymousDownload_RedirectsToLogin()
    {
        var client = CreateClientNoRedirect();

        var response = await client.GetAsync("/Song/Download/1");

        Assert.Equal(System.Net.HttpStatusCode.Found, response.StatusCode);
        Assert.Contains("/Auth/Login", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task PublicPages_AreAccessible()
    {
        var client = CreateClientNoRedirect();

        var home = await client.GetAsync("/");
        var details = await client.GetAsync("/Song/Details/1");
        var register = await client.GetAsync("/Auth/Register");
        var login = await client.GetAsync("/Auth/Login");

        var homeBody = await home.Content.ReadAsStringAsync();
        var registerBody = await register.Content.ReadAsStringAsync();
        var loginBody = await login.Content.ReadAsStringAsync();

        Assert.True(home.StatusCode == System.Net.HttpStatusCode.OK, $"Home: {home.StatusCode}\n{homeBody}");
        Assert.Equal(System.Net.HttpStatusCode.OK, register.StatusCode);
        Assert.Equal(System.Net.HttpStatusCode.OK, login.StatusCode);
    }
}