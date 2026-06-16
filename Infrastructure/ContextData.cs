using System;
using System.Collections.Generic;
using System.Linq;
using Constructs;
using Infrastructure.Constants;

namespace Infrastructure;

public class ContextDataHelper
{
    public static string GetAppEnvironment(Construct scope)
    {
        string path = $"@feltboard/{ContextKeys.AppEnvironment}";
        var value = scope.Node.TryGetContext(path);

        if (value == null)
        {
            throw new Exception($"CDK Context Value {ContextKeys.AppEnvironment} not found.");
        }

        return value.ToString()!;
    }

    public static string GetString(Construct scope, string key)
    {
        string appEnvironment = GetAppEnvironment(scope);
        var path = $"@feltboard/{appEnvironment}/{key}";
        var value = scope.Node.TryGetContext(path);

        if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
            return value.ToString()!;

        string defaultPath = $"feltboard/default/{key}";
        var defaultValue = scope.Node.TryGetContext(defaultPath);

        if (defaultValue != null && !string.IsNullOrWhiteSpace(defaultValue.ToString()))
            return defaultValue.ToString()!;

        throw new InvalidOperationException($"CDK context key '{path}' is not set and no default was provided.");
    }

    public static bool GetBool(Construct scope, string key)
    {
        string appEnvironment = GetAppEnvironment(scope);
        var path = $"@feltboard/{appEnvironment}/{key}";
        var value = scope.Node.TryGetContext(path);

        if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
            return bool.Parse(value.ToString()!);

        string defaultPath = $"feltboard/default/{key}";
        var defaultValue = scope.Node.TryGetContext(defaultPath);

        if (defaultValue != null && bool.TryParse(defaultValue.ToString(), out bool parsed))
            return parsed;

        throw new InvalidOperationException($"CDK context key '{path}' is not set and no default was provided.");
    }

    public static List<string> GetList(Construct scope, string key)
    {
        string appEnv = GetAppEnvironment(scope);
        string path = $"@feltboard/{appEnv}/{key}";

        var value = scope.Node.TryGetContext(path);

        if (value is object[] arr)
            return arr.Select(x => x.ToString()!).ToList();

        string defaultPath = $"@feltboard/default/{key}";
        var defaultValue = scope.Node.TryGetContext(defaultPath);

        if (defaultValue is object[] defArr)
            return defArr.Select(x => x.ToString()!).ToList();

        throw new InvalidOperationException($"CDK context key '{key}' is not set and no default was provided.");
    }
}
