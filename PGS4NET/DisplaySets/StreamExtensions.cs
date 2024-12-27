using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

public static class StreamExtensions
{
    private static readonly DisplaySetException PendingSegments
        = new("Successfully read an incomplete display set.");

    public static IEnumerable<DisplaySet> DisplaySets(this Stream stream)
    {
        using var reader = new SegmentReader(stream, true);
        var displaySetComposer = new DisplaySetComposer();
        var queue = new Queue<DisplaySet>();

        displaySetComposer.Ready += (_, displaySet) =>
        {
            queue.Enqueue(displaySet);
        };

        while (reader.Read() is Segment segment)
        {
            displaySetComposer.Input(segment);

            while (queue.Count > 0)
                yield return queue.Dequeue();
        }

        if (displaySetComposer.Pending)
            throw PendingSegments;
    }

#if NETSTANDARD2_1_OR_GREATER
    public static async IAsyncEnumerable<DisplaySet> DisplaySetsAsync(this Stream stream
        , CancellationToken cancellationToken = default)
    {
        using var reader = new SegmentReader(stream, true);
        var displaySetComposer = new DisplaySetComposer();
        var queue = new Queue<DisplaySet>();

        displaySetComposer.Ready += (_, displaySet) =>
        {
            queue.Enqueue(displaySet);
        };

        while (await reader.ReadAsync(cancellationToken) is Segment segment)
        {
            displaySetComposer.Input(segment);

            while (queue.Count > 0)
                yield return queue.Dequeue();
        }

        if (displaySetComposer.Pending)
            throw PendingSegments;
    }
#endif

    public static IList<DisplaySet> ReadAllDisplaySets(this Stream stream)
    {
        using var reader = new SegmentReader(stream, true);
        var displaySetComposer = new DisplaySetComposer();
        var displaySets = new List<DisplaySet>();

        displaySetComposer.Ready += (_, displaySet) =>
        {
            displaySets.Add(displaySet);
        };

        while (reader.Read() is Segment segment)
            displaySetComposer.Input(segment);

        if (displaySetComposer.Pending)
            throw PendingSegments;

        return displaySets;
    }

    public static async Task<IList<DisplaySet>> ReadAllDisplaySetsAsync(this Stream stream
        , CancellationToken cancellationToken = default)
    {
        using var reader = new SegmentReader(stream, true);
        var displaySetComposer = new DisplaySetComposer();
        var displaySets = new List<DisplaySet>();

        displaySetComposer.Ready += (_, displaySet) =>
        {
            displaySets.Add(displaySet);
        };

        while (await reader.ReadAsync(cancellationToken) is Segment segment)
            displaySetComposer.Input(segment);

        if (displaySetComposer.Pending)
            throw PendingSegments;

        return displaySets;
    }

    public static void WriteAllDisplaySets(this Stream stream
        , IEnumerable<DisplaySet> displaySets)
    {
        using var writer = new SegmentWriter(stream, true);
        var decomposer = new DisplaySetDecomposer();
        var queue = new Queue<Segment>();

        decomposer.Ready += (_, segment) =>
        {
            queue.Enqueue(segment);
        };

        foreach (var displaySet in displaySets)
        {
            decomposer.Input(displaySet);

            while (queue.Count > 0)
                writer.Write(queue.Dequeue());
        }
    }

    public static async Task WriteAllDisplaySetsAsync(this Stream stream
        , IEnumerable<DisplaySet> displaySets,  CancellationToken cancellationToken = default)
    {
        using var writer = new SegmentWriter(stream, true);
        var decomposer = new DisplaySetDecomposer();
        var queue = new Queue<Segment>();

        decomposer.Ready += (_, segment) =>
        {
            queue.Enqueue(segment);
        };

        foreach (var displaySet in displaySets)
        {
            decomposer.Input(displaySet);

            while (queue.Count > 0)
                await writer.WriteAsync(queue.Dequeue(), cancellationToken);
        }
    }

#if NETSTANDARD2_1_OR_GREATER
    public static async Task WriteAllDisplaySetsAsync(this Stream stream
        , IAsyncEnumerable<DisplaySet> displaySets
        , CancellationToken cancellationToken = default)
    {
        using var writer = new SegmentWriter(stream, true);
        var decomposer = new DisplaySetDecomposer();
        var queue = new Queue<Segment>();

        decomposer.Ready += (_, segment) =>
        {
            queue.Enqueue(segment);
        };

        await foreach (var displaySet in displaySets)
        {
            decomposer.Input(displaySet);

            while (queue.Count > 0)
                await writer.WriteAsync(queue.Dequeue(), cancellationToken);
        }
    }
#endif
}
