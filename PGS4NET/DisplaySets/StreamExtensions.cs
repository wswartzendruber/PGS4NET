using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace PGS4NET.DisplaySets;

public static class StreamExtensions
{
    public static IEnumerable<DisplaySet> DisplaySets(this Stream stream)
    {
        using var reader = new DisplaySetReader(stream, true);

        while (reader.Read() is DisplaySet displaySet)
            yield return displaySet;
    }

#if NETSTANDARD2_1_OR_GREATER
    public static async IAsyncEnumerable<DisplaySet> DisplaySetsAsync(this Stream stream
        , [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var reader = new DisplaySetReader(stream, true);

        while (await reader.ReadAsync(cancellationToken) is DisplaySet displaySet)
            yield return displaySet;
    }
#endif

    public static IList<DisplaySet> ReadAllDisplaySets(this Stream stream)
    {
        var displaySets = new List<DisplaySet>();
        using var reader = new DisplaySetReader(stream, true);

        while (reader.Read() is DisplaySet displaySet)
            displaySets.Add(displaySet);

        return displaySets;
    }

    public static async Task<IList<DisplaySet>> ReadAllDisplaySetsAsync(this Stream stream
        , CancellationToken cancellationToken = default)
    {
        var displaySets = new List<DisplaySet>();
        using var reader = new DisplaySetReader(stream, true);

        while (await reader.ReadAsync(cancellationToken) is DisplaySet displaySet)
            displaySets.Add(displaySet);

        return displaySets;
    }

    public static void WriteAllDisplaySets(this Stream stream, IEnumerable<DisplaySet> displaySets)
    {
        using var writer = new DisplaySetWriter(stream, true);

        foreach (var displaySet in displaySets)
            writer.Write(displaySet);
    }

    public static async Task WriteAllDisplaySetsAsync(this Stream stream
        , IEnumerable<DisplaySet> displaySets,  CancellationToken cancellationToken = default)
    {
        using var writer = new DisplaySetWriter(stream, true);

        foreach (var displaySet in displaySets)
            await writer.WriteAsync(displaySet, cancellationToken);
    }

#if NETSTANDARD2_1_OR_GREATER
    public static async Task WriteAllDisplaySetsAsync(this Stream stream
        , IAsyncEnumerable<DisplaySet> displaySets, CancellationToken cancellationToken = default)
    {
        using var writer = new DisplaySetWriter(stream, true);

        await foreach (var displaySet in displaySets)
            await writer.WriteAsync(displaySet, cancellationToken);
    }
#endif
}
