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

public class SegmentReaderTests
{
    [Fact]
    public void EmptyStream()
    {
        using var stream = new MemoryStream();
        using var reader = new SegmentReader(stream);

        Assert.Null(reader.Read());
    }

    [Fact]
    public async Task EmptyStreamAsync()
    {
        using var stream = new MemoryStream();
        using var reader = new SegmentReader(stream);

        Assert.Null(await reader.ReadAsync());
    }

    [Fact]
    public void NullByte()
    {
        using var stream = new MemoryStream();
        using var reader = new SegmentReader(stream);

        stream.WriteByte(0x00);
        stream.Position = 0;

        try
        {
            reader.Read();

            Assert.Fail("Was able to read a segment with a trailing null byte.");
        }
        catch (IOException ioe)
        {
            Assert.Equal("EOF reading segment header.", ioe.Message);
        }
    }

    [Fact]
    public async Task NullByteAsync()
    {
        using var stream = new MemoryStream();
        using var reader = new SegmentReader(stream);

        stream.WriteByte(0x00);
        stream.Position = 0;

        try
        {
            await reader.ReadAsync();

            Assert.Fail("Was able to read a segment with a trailing null byte.");
        }
        catch (IOException ioe)
        {
            Assert.Equal("EOF reading segment header.", ioe.Message);
        }
    }
}
