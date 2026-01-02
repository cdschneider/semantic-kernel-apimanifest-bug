This repo demonstrates a bug where `OpenApiFunctionExecutionParameters` callbacks (`HttpResponseContentReader` and `RestApiOperationResponseFactory`) are not invoked when loading plugins when importing an OpenAPI ApiManifest (via `ImportPluginFromApiManifestAsync`).

## The Bug

When using `ApiManifestPluginParameters.FunctionExecutionParameters` to pass custom callbacks, they are never called during function invocation.

**Test:** `WeatherGovAsApiManifest` - Fails ⛔

```csharp
var functionExecutionParameters = new Dictionary<string, OpenApiFunctionExecutionParameters>
{
    ["api.weather.gov"] = new()
    {
        HttpResponseContentReader = async (context, ct) => { /* never called */ },
        RestApiOperationResponseFactory = (context, ct) => { /* never called */ }
    }
};

var pluginParameters = new ApiManifestPluginParameters
{
    FunctionExecutionParameters = functionExecutionParameters
};

await kernel.ImportPluginFromApiManifestAsync("MyWeatherPlugin", "manifest.json", pluginParameters);
```

## Expected Behavior

When using `ImportPluginFromOpenApiAsync` directly with `OpenApiFunctionExecutionParameters`, the callbacks are invoked as expected.

**Test:** `WeatherGovAsOpenApi` - Passes :white_check_mark:

```csharp
var functionExecutionParameters = new OpenApiFunctionExecutionParameters
{
    HttpResponseContentReader = async (context, ct) => { /* called */ },
    RestApiOperationResponseFactory = (context, ct) => { /* called */ }
};

await kernel.ImportPluginFromOpenApiAsync("MyWeatherPlugin", "openapi.json", functionExecutionParameters);
```

## Running the Tests

```bash
dotnet test
```

- `WeatherGovAsOpenApi` - Passes :white_check_mark:
- `WeatherGovAsApiManifest` - Fails ⛔
