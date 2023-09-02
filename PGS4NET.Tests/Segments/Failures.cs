/*
 * Copyright 2023 William Swartzendruber
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

public class Failures
{
    [Fact]
    public void EmptyStream()
    {
        using var stream = new MemoryStream();

        if (stream.ReadSegment() is not null)
            throw new Exception("Returned segment is not null.");
    }

    [Fact]
    public async Task EmptyStreamAsync()
    {
        using var stream = new MemoryStream();

        if (await stream.ReadSegmentAsync() is not null)
            throw new Exception("Returned segment is not null.");
    }

    [Fact]
    public void NullByte()
    {
        using var stream = new MemoryStream();

        stream.WriteByte(0x00);
        stream.Position = 0;

        try
        {
            stream.ReadSegment();

            throw new Exception("Successfully read a stream with a trailing null byte.");
        }
        catch (IOException ioe)
        {
            if (ioe.Message != "EOF reading segment header.")
                throw new Exception("Expected specific error message on header EOF.");
        }
    }

    [Fact]
    public async Task NullByteAsync()
    {
        using var stream = new MemoryStream();

        stream.WriteByte(0x00);
        stream.Position = 0;

        try
        {
            await stream.ReadSegmentAsync();

            throw new Exception("Successfully read a stream with a trailing null byte.");
        }
        catch (IOException ioe)
        {
            if (ioe.Message != "EOF reading segment header.")
                throw new Exception("Expected specific error message on header EOF.");
        }
    }

    [Fact]
    public void TrailingNullByte()
    {
        using var stream = new MemoryStream();
        var buffer = SegmentBuffers.Buffers["pcs-es"];

        stream.Write(buffer);
        stream.WriteByte(0x00);
        stream.Position = 0;

        try
        {
            stream.ReadAllSegments();

            throw new Exception("Successfully read a stream with a trailing null byte.");
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
        var buffer = SegmentBuffers.Buffers["pcs-es"];

        stream.Write(buffer);
        stream.WriteByte(0x00);
        stream.Position = 0;

        try
        {
            await stream.ReadAllSegmentsAsync();

            throw new Exception("Successfully read a stream with a trailing null byte.");
        }
        catch (IOException ioe)
        {
            if (ioe.Message != "EOF reading segment header.")
                throw new Exception("Expected specific error message on header EOF.");
        }
    }
}
