/*
 * Copyright 2024 William Swartzendruber
 *
 * To the extent possible under law, the person who associated CC0 with this file has waived all
 * copyright and related or neighboring rights to this file.
 *
 * You should have received a copy of the CC0 legalcode along with this work. If not, see
 * <http://creativecommons.org/publicdomain/zero/1.0/>.
 *
 * SPDX-License-Identifier: CC0-1.0
 */

using PGS4NET.Segments;

namespace PGS4NET.Tests.Segments;

public class StreamTests
{
    [Fact]
    public void EnumerateWriteAllSegments()
    {
        using var inputStream = new MemoryStream();
        using var outputStream = new MemoryStream();

        foreach (var segmentBuffer in SegmentBuffers.Buffers.Values)
            inputStream.Write(segmentBuffer);
        inputStream.Seek(0, SeekOrigin.Begin);

        outputStream.WriteAllSegments(inputStream.Segments());

        Assert.True(inputStream.ToArray().SequenceEqual(outputStream.ToArray()));
    }

#if TEST_NETSTANDARD2_1
    [Fact]
    public async Task EnumerateWriteAllSegmentsAsync()
    {
        using var inputStream = new MemoryStream();
        using var outputStream = new MemoryStream();

        foreach (var segmentBuffer in SegmentBuffers.Buffers.Values)
            await inputStream.WriteAsync(segmentBuffer);
        inputStream.Seek(0, SeekOrigin.Begin);

        await outputStream.WriteAllSegmentsAsync(inputStream.SegmentsAsync());

        Assert.True(inputStream.ToArray().SequenceEqual(outputStream.ToArray()));
    }
#endif

    [Fact]
    public void ReadWriteAllSegments()
    {
        using var inputStream = new MemoryStream();

        foreach (var segmentBuffer in SegmentBuffers.Buffers.Values)
            inputStream.Write(segmentBuffer);
        inputStream.Seek(0, SeekOrigin.Begin);

        var segments = inputStream.ReadAllSegments();
        using var outputStream = new MemoryStream();

        outputStream.WriteAllSegments(segments);

        Assert.True(inputStream.ToArray().SequenceEqual(outputStream.ToArray()));
    }

    [Fact]
    public async Task ReadWriteAllSegmentsAsync()
    {
        using var inputStream = new MemoryStream();

        foreach (var segmentBuffer in SegmentBuffers.Buffers.Values)
            await inputStream.WriteAsync(segmentBuffer);
        inputStream.Seek(0, SeekOrigin.Begin);

        var segments = await inputStream.ReadAllSegmentsAsync();
        using var outputStream = new MemoryStream();

        await outputStream.WriteAllSegmentsAsync(segments);

        Assert.True(inputStream.ToArray().SequenceEqual(outputStream.ToArray()));
    }
}
