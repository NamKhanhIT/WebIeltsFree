using System;
using System.Text.RegularExpressions;

namespace WebIeltsFree.Models;

/// <summary>
/// Helper class for resolving environment variables in configuration strings
/// using the ${ENV_VAR:-default} pattern.
/// </summary>
public static class EnvHelper
{
    public static string? ResolveEnvVars(string? template)
    {
        if (string.IsNullOrWhiteSpace(template)) return template;
        const string pattern = @"\$\{([^:}]+)(?::-(.*?))?\}";
        return Regex.Replace(template, pattern, match =>
        {
            var varName = match.Groups[1].Value;
            var defaultValue = match.Groups[2].Value;
            return Environment.GetEnvironmentVariable(varName) ?? defaultValue ?? string.Empty;
        });
    }
}
