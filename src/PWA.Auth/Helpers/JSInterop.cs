// Helpers/JSInterop.cs
using Microsoft.JSInterop;

namespace PWA.Auth.Helpers;

/// <summary>
/// Helper for JavaScript interop operations
/// </summary>
public static class JSInterop
{
    private static IJSRuntime? _jsRuntime;

    public static void Initialize(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Shows a browser confirmation dialog
    /// </summary>
    public static async Task<bool> ConfirmAsync(string message)
    {
        if (_jsRuntime == null)
            throw new InvalidOperationException("JSInterop not initialized");

        return await _jsRuntime.InvokeAsync<bool>("confirm", message);
    }

    /// <summary>
    /// Shows a browser alert dialog
    /// </summary>
    public static async Task AlertAsync(string message)
    {
        if (_jsRuntime == null)
            throw new InvalidOperationException("JSInterop not initialized");

        await _jsRuntime.InvokeVoidAsync("alert", message);
    }
}