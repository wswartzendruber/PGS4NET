using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PGS4NET.Segments;

public static class StreamExtensions
{
    public static IEnumerable<Segment> Segments(this Stream stream)
    {
        using var reader = new SegmentReader(stream, true);

        while (reader.Read() is Segment segment)
            yield return segment;
    }
    
#if NETSTANDARD2_1_OR_GREATER
    public static async IAsyncEnumerable<Segment> SegmentsAsync(this Stream stream
        , CancellationToken cancellationToken = default)
    {
        using var reader = new SegmentReader(stream, true);

        while (await reader.ReadAsync(cancellationToken) is Segment segment)
            yield return segment;
    }
#endif

    public static IList<Segment> ReadAllSegments(this Stream stream)
    {
        var segments = new List<Segment>();
        using var reader = new SegmentReader(stream, true);

        while (reader.Read() is Segment segment)
            segments.Add(segment);
        
        return segments;
    }

    public static async Task<IList<Segment>> ReadAllSegmentsAsync(this Stream stream
        , CancellationToken cancellationToken = default)
    {
        var segments = new List<Segment>();
        using var reader = new SegmentReader(stream, true);

        while (await reader.ReadAsync(cancellationToken) is Segment segment)
            segments.Add(segment);

        return segments;
    }

    public static void WriteAllSegments(this Stream stream, IEnumerable<Segment> segments)
    {
        using var writer = new SegmentWriter(stream, true);

        foreach (var segment in segments)
            writer.Write(segment);
    }

    public static async Task WriteAllSegmentsAsync(this Stream stream
        , IEnumerable<Segment> segments,  CancellationToken cancellationToken = default)
    {
        using var writer = new SegmentWriter(stream, true);

        foreach (var segment in segments)
            await writer.WriteAsync(segment, cancellationToken);
    }

#if NETSTANDARD2_1_OR_GREATER
    public static async Task WriteAllSegmentsAsync(this Stream stream
        , IAsyncEnumerable<Segment> segments, CancellationToken cancellationToken = default)
    {
        using var writer = new SegmentWriter(stream, true);

        await foreach (var segment in segments)
            await writer.WriteAsync(segment, cancellationToken);
    }
#endif
}
