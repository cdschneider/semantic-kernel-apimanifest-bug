using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.OpenApi;
using Microsoft.SemanticKernel.Plugins.OpenApi.Extensions;

namespace SemanticKernelBug;

public class UnitTest1
{
    [Fact]
    public async Task WeatherGovAsApiManifest()
    {
        var kernel = new Kernel();
        
        bool httpResponseContentReaderCalled = false;
        bool restApiOperationResponseFactoryCalled = false;
        
        var functionExecutionParameters = new Dictionary<string, OpenApiFunctionExecutionParameters>
        {
            ["api.weather.gov"] = new()
            {
                HttpClient = new HttpClient(),
                IgnoreNonCompliantErrors =  true,
                HttpResponseContentReader = async (context, cancellationToken) =>
                {
                    httpResponseContentReaderCalled = true;
                    return await context.Response.Content.ReadAsStringAsync(cancellationToken);
                },
                RestApiOperationResponseFactory = (context, cancellationToken) =>
                {
                    restApiOperationResponseFactoryCalled = true;
                    return context.InternalFactory.Invoke(context, cancellationToken);
                }
            }
        };

        var pluginParameters = new ApiManifestPluginParameters { FunctionExecutionParameters = functionExecutionParameters };
        
        var plugin = await kernel.ImportPluginFromApiManifestAsync(
            "MyWeatherPlugin",
            Path.Combine(AppContext.BaseDirectory, "Resources", "manifest.json"),
            pluginParameters);
        
        _ = await kernel.InvokeAsync(plugin["point"], new KernelArguments { ["latitude"] = 40.71, ["longitude"] = -74.01 });
        
        Assert.True(httpResponseContentReaderCalled, "HttpResponseContentReader was not called during invocation.");
        Assert.True(restApiOperationResponseFactoryCalled, "RestApiOperationResponseFactory was not called during invocation.");
    }
    
    [Fact]
    public async Task WeatherGovAsOpenApi()
    {
        var kernel = new Kernel();
        
        bool httpResponseContentReaderCalled = false;
        bool restApiOperationResponseFactoryCalled = false;

        var functionExecutionParameters = new OpenApiFunctionExecutionParameters
        {
            HttpClient = new HttpClient(),
            IgnoreNonCompliantErrors =  true,
            HttpResponseContentReader = async (context, cancellationToken) =>
            {
                httpResponseContentReaderCalled = true;
                return await context.Response.Content.ReadAsStringAsync(cancellationToken);
            },
            RestApiOperationResponseFactory = (context, cancellationToken) =>
            {
                restApiOperationResponseFactoryCalled = true;
                return context.InternalFactory.Invoke(context, cancellationToken);
            }
        };
        
        var plugin = await kernel.ImportPluginFromOpenApiAsync(
            "MyWeatherPlugin",
            Path.Combine(AppContext.BaseDirectory, "Resources", "openapi.json"),
            functionExecutionParameters);
        
        _ = await kernel.InvokeAsync(plugin["point"], new KernelArguments { ["latitude"] = 40.71, ["longitude"] = -74.01 });
        
        Assert.True(httpResponseContentReaderCalled, "HttpResponseContentReader was not called during invocation.");
        Assert.True(restApiOperationResponseFactoryCalled, "RestApiOperationResponseFactory was not called during invocation.");
    }
}
