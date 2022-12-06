var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddScoped(typeof(ISplashTrackServicesWrapper<>),typeof(SplashTrackServicesWrapper<>));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

public interface IServiceWrapper
{
    string DevEndpoint { get; }
    string IntEndpoint { get;  }
    string ProdEndpoint { get;  }

    string Endpoint => $"{RunEnvironment.GetEnvironment() switch {"prod" => ProdEndpoint, "dev" => DevEndpoint, "int" => IntEndpoint}}";
}

public struct MyService : IServiceWrapper
{
    public string DevEndpoint => "https://dev.com";
    public string IntEndpoint => "https://int.com";
    public string ProdEndpoint => "https://prod.com";

    public string MyData(string clientId) => $"/api/v1/{clientId}";
}

public struct MySecondService : IServiceWrapper
{
    public string DevEndpoint => "https://dev.com";
    public string IntEndpoint => "https://int.com";
    public string ProdEndpoint => "https://prod.com";

    public string MyData(string employeeId) => $"/api/v1/{employeeId}";
    public string AllData => "/api/v1/";
}

public struct MyThirdService : IServiceWrapper
{
    public string DevEndpoint => "https://dev.com";
    public string IntEndpoint => "https://int.com";
    public string ProdEndpoint => "https://prod.com";

    public string SomeDataEndpoint => $"Random Data End Point {Random.Shared.Next()}";
}

public interface ISplashTrackServicesWrapper<T> where T: IServiceWrapper, new()
{
    string HttpGet(Func<T, string> getEndpoint, bool logException = true, bool useRun = true);

    Task<T1> HttpGet<T1>(Func<T, string> getEndpoint, bool logException = true, bool useRun = true);
}

public class SplashTrackServicesWrapper<T> : ISplashTrackServicesWrapper<T> where T: IServiceWrapper, new()
{
    protected string url;
    private T _serviceWrapper;
    private HttpClient _client = new();

    public SplashTrackServicesWrapper()
    {
        var serviceWrapper = new T();
        url = serviceWrapper.Endpoint;
        _serviceWrapper = serviceWrapper;

        _client.BaseAddress = new Uri(_serviceWrapper.Endpoint);
    }

    public string HttpGet(Func<T,string> getEndpoint, bool logException = true, bool useRun = true)
    {
        return getEndpoint(_serviceWrapper);
    }

    public Task<T1> HttpGet<T1>(Func<T, string> getEndpoint, bool logException = true, bool useRun = true)
    {
        var endpoint = getEndpoint(_serviceWrapper);
        return _client.GetFromJsonAsync<T1>(endpoint);
    }
}

public static class RunEnvironment
{
    public static string GetEnvironment() => "prod";
}
