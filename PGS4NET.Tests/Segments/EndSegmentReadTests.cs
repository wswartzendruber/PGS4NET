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

using System.IO;
using PGS4NET.Segments;

namespace PGS4NET.Tests.Segments;

public class EndSegmentReadTests
{
    [Fact]
    public void Single()
    {
        using var reader = new SegmentReader(new MemoryStream(EndSegmentData.Single));
        var segment = reader.Read() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is not EndSegment)
            Assert.True(false);
    }

    [Fact]
    public async Task SingleAsync()
    {
        await using var reader = new SegmentReader(new MemoryStream(EndSegmentData.Single));
        var segment = await reader.ReadAsync() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is not EndSegment)
            Assert.True(false);
    }
}
