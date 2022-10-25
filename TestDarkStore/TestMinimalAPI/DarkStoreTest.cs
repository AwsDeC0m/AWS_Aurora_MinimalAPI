using System.Net;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Headers;

namespace TestDarkStore.TestMinimalAPI;

internal class DarkStoreApiApplication : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        return base.CreateHost(builder);
    }
}

public class DarkStoreTest
{
    [Fact]
    public async Task Get_OnSuccess_HelloWorld()
    {
        // Arrange
        await using var app = new DarkStoreApiApplication();

        //Act
        using var httpClient = app.CreateClient();
        using var response = await httpClient.GetAsync("/");

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Hello World!", await response.Content.ReadAsStringAsync());
    }
    [Fact]
    public async Task Post_OnSuccess_Add_Store()
    {
        // Arrange
        await using var app = new DarkStoreApiApplication();

        var nameStore = $"Магазин № {new Random().Next(1, 100)}";
        var addressStore = $"ул. Малиновая № {new Random().Next(50, 100)}";

        var jsonString = "{\"Name\":\"" + nameStore + "\",\"Address\":\"" + addressStore + "\"}";
        using var jsonContent = new StringContent(jsonString);
        jsonContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

        //Act
        using var httpClient = app.CreateClient();
        using var response = await httpClient.PostAsync("/v1/stores", jsonContent);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}