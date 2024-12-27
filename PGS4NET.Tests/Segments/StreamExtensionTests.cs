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

public class StreamExtensionTests
{
    [Fact]
    public void ReadWriteAllSegmentsEnumerable()
    {
        using var inputStream = new MemoryStream(SintelSubtitles.Buffer);
        using var outputStream = new MemoryStream();

        outputStream.WriteAllSegments(inputStream.Segments());

        Assert.True(inputStream.ToArray().SequenceEqual(outputStream.ToArray()));
    }

#if NETCOREAPP3_0_OR_GREATER
    [Fact]
    public async Task ReadWriteAllSegmentsAsyncEnumerable()
    {
        using var inputStream = new MemoryStream(SintelSubtitles.Buffer);
        using var outputStream = new MemoryStream();

        await outputStream.WriteAllSegmentsAsync(inputStream.SegmentsAsync());

        Assert.True(inputStream.ToArray().SequenceEqual(outputStream.ToArray()));
    }
#endif

    [Fact]
    public void ReadWriteAllSegments()
    {
        using var inputStream = new MemoryStream(SintelSubtitles.Buffer);
        using var outputStream = new MemoryStream();

        outputStream.WriteAllSegments(inputStream.ReadAllSegments());

        Assert.True(inputStream.ToArray().SequenceEqual(outputStream.ToArray()));
    }

    [Fact]
    public async Task ReadWriteAllSegmentsAsync()
    {
        using var inputStream = new MemoryStream(SintelSubtitles.Buffer);
        using var outputStream = new MemoryStream();

        await outputStream.WriteAllSegmentsAsync(await inputStream.ReadAllSegmentsAsync());

        Assert.True(inputStream.ToArray().SequenceEqual(outputStream.ToArray()));
    }

    [Fact]
    public void TrailingNullByteEnumerable()
    {
        using var stream = new MemoryStream();
        using var reader = new SegmentReader(stream);
        var buffer = SegmentBuffers.Buffers["pcs-es"];

        stream.Write(buffer, 0, buffer.Length);
        stream.WriteByte(0x00);
        stream.Position = 0;

        try
        {
            stream.Segments().ToList();

            throw new Exception("Successfully read a segment with a trailing null byte.");
        }
        catch (IOException ioe)
        {
            if (ioe.Message != "EOF reading segment header.")
                throw new Exception("Expected specific error message on header EOF.");
        }
    }

#if NETCOREAPP3_0_OR_GREATER
    [Fact]
    public async Task TrailingNullByteAsyncEnumerable()
    {
        using var stream = new MemoryStream();
        using var reader = new SegmentReader(stream);
        var buffer = SegmentBuffers.Buffers["pcs-es"];

        stream.Write(buffer, 0, buffer.Length);
        stream.WriteByte(0x00);
        stream.Position = 0;

        try
        {
            await stream.SegmentsAsync().ToListAsync();

            throw new Exception("Successfully read a segment with a trailing null byte.");
        }
        catch (IOException ioe)
        {
            if (ioe.Message != "EOF reading segment header.")
                throw new Exception("Expected specific error message on header EOF.");
        }
    }
#endif

    [Fact]
    public void TrailingNullByte()
    {
        using var stream = new MemoryStream();
        using var reader = new SegmentReader(stream);
        var buffer = SegmentBuffers.Buffers["pcs-es"];

        stream.Write(buffer, 0, buffer.Length);
        stream.WriteByte(0x00);
        stream.Position = 0;

        try
        {
            stream.ReadAllSegments();

            throw new Exception("Successfully read a segment with a trailing null byte.");
        }
        catch (IOException ioe)
        {
            if (ioe.Message != "EOF reading segment header.")
                throw new Exception("Expected specific error message on header EOF.");
        }
    }

    [Fact]
    public async Task TrailingNullByteAsync()
    {
        using var stream = new MemoryStream();
        using var reader = new SegmentReader(stream);
        var buffer = SegmentBuffers.Buffers["pcs-es"];

        stream.Write(buffer, 0, buffer.Length);
        stream.WriteByte(0x00);
        stream.Position = 0;

        try
        {
            await stream.ReadAllSegmentsAsync();

            throw new Exception("Successfully read a segment with a trailing null byte.");
        }
        catch (IOException ioe)
        {
            if (ioe.Message != "EOF reading segment header.")
                throw new Exception("Expected specific error message on header EOF.");
        }
    }
}
