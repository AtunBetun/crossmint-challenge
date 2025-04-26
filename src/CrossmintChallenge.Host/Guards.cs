using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CrossmintChallenge.Host;

public static class Guards
{
    public static T NotNull<T>(
        [NotNull] this T? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null
    )
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);
        return argument;
    }
}
