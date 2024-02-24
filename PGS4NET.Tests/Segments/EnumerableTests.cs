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

public class EnumerableTests
{
    [Fact]
    public void ReadSegmentBuffers()
    {
        using var inputStream = new MemoryStream();
        using var outputStream1 = new MemoryStream();
        using var outputStream2 = new MemoryStream();

        foreach (var buffer in SegmentBuffers.Buffers.Values)
            inputStream.Write(buffer);

        inputStream.Seek(0, SeekOrigin.Begin);
        var bulk = inputStream.ReadAllSegments();

        inputStream.Seek(0, SeekOrigin.Begin);
        var enumerable = inputStream.Segments();

        outputStream1.WriteAllSegments(bulk);
        outputStream2.WriteAllSegments(enumerable);

        Assert.True(outputStream1.ToArray().SequenceEqual(outputStream2.ToArray()));
    }

#if TEST_NETSTANDARD2_1
    [Fact]
    public async void ReadSegmentBuffersAsyncAsync()
    {
        using var inputStream = new MemoryStream();
        using var outputStream1 = new MemoryStream();
        using var outputStream2 = new MemoryStream();

        foreach (var buffer in SegmentBuffers.Buffers.Values)
            await inputStream.WriteAsync(buffer);

        inputStream.Seek(0, SeekOrigin.Begin);
        var bulk = await inputStream.ReadAllSegmentsAsync();

        inputStream.Seek(0, SeekOrigin.Begin);
        var asyncEnumerable = inputStream.SegmentsAsync();

        await outputStream1.WriteAllSegmentsAsync(bulk);
        await outputStream2.WriteAllSegmentsAsync(asyncEnumerable);

        Assert.True(outputStream1.ToArray().SequenceEqual(outputStream2.ToArray()));
    }
#endif
}
