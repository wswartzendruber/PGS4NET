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

        Assert.Equal(inputStream.ToArray(), outputStream.ToArray());
    }

#if NETCOREAPP3_0_OR_GREATER
    [Fact]
    public async Task ReadWriteAllSegmentsAsyncEnumerable()
    {
        using var inputStream = new MemoryStream(SintelSubtitles.Buffer);
        using var outputStream = new MemoryStream();

        await outputStream.WriteAllSegmentsAsync(inputStream.SegmentsAsync());

        Assert.Equal(inputStream.ToArray(), outputStream.ToArray());
    }
#endif

    [Fact]
    public void ReadWriteAllSegments()
    {
        using var inputStream = new MemoryStream(SintelSubtitles.Buffer);
        using var outputStream = new MemoryStream();

        outputStream.WriteAllSegments(inputStream.ReadAllSegments());

        Assert.Equal(inputStream.ToArray(), outputStream.ToArray());
    }

    [Fact]
    public async Task ReadWriteAllSegmentsAsync()
    {
        using var inputStream = new MemoryStream(SintelSubtitles.Buffer);
        using var outputStream = new MemoryStream();

        await outputStream.WriteAllSegmentsAsync(await inputStream.ReadAllSegmentsAsync());

        Assert.Equal(inputStream.ToArray(), outputStream.ToArray());
    }

    [Fact]
    public void TrailingNullByteEnumerable()
    {
        using var stream = new MemoryStream();
        var buffer = SegmentBuffers.Buffers["pcs-es"];

        stream.Write(buffer, 0, buffer.Length);
        stream.WriteByte(0x00);
        stream.Position = 0;

        try
        {
            stream.Segments().ToList();

            Assert.Fail("Was able to read a segment with a trailing null byte.");
        }
        catch (IOException ioe)
        {
            Assert.Equal("EOF reading segment header.", ioe.Message);
        }
    }

#if NETCOREAPP3_0_OR_GREATER
    [Fact]
    public async Task TrailingNullByteAsyncEnumerable()
    {
        using var stream = new MemoryStream();
        var buffer = SegmentBuffers.Buffers["pcs-es"];

        stream.Write(buffer, 0, buffer.Length);
        stream.WriteByte(0x00);
        stream.Position = 0;

        try
        {
            await stream.SegmentsAsync().ToListAsync();

            Assert.Fail("Was able to read a segment with a trailing null byte.");
        }
        catch (IOException ioe)
        {
            Assert.Equal("EOF reading segment header.", ioe.Message);
        }
    }
#endif

    [Fact]
    public void TrailingNullByte()
    {
        using var stream = new MemoryStream();
        var buffer = SegmentBuffers.Buffers["pcs-es"];

        stream.Write(buffer, 0, buffer.Length);
        stream.WriteByte(0x00);
        stream.Position = 0;

        try
        {
            stream.ReadAllSegments();

            Assert.Fail("Was able to read a segment with a trailing null byte.");
        }
        catch (IOException ioe)
        {
            Assert.Equal("EOF reading segment header.", ioe.Message);
        }
    }

    [Fact]
    public async Task TrailingNullByteAsync()
    {
        using var stream = new MemoryStream();
        var buffer = SegmentBuffers.Buffers["pcs-es"];

        stream.Write(buffer, 0, buffer.Length);
        stream.WriteByte(0x00);
        stream.Position = 0;

        try
        {
            await stream.ReadAllSegmentsAsync();

            Assert.Fail("Was able to read a segment with a trailing null byte.");
        }
        catch (IOException ioe)
        {
            Assert.Equal("EOF reading segment header.", ioe.Message);
        }
    }
}
