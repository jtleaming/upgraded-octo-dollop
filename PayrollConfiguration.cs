namespace GenericInjectionPattern;

public struct PayrollConfiguration: IServiceWrapper
{
    public string DevEndpoint => "https://configurator.com"; 
    public string IntEndpoint => "https://configurator.com";
    public string ProdEndpoint => "https://configurator.com";

    private const string _eeConfigApi = @"/api/ConfiguratorEmployees";

    public string GetEmployeeConfig(string eeId) => $"{_eeConfigApi}/{eeId}";
}

public record ConfiguratorEmployee();