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

public class WindowDefinitionSegmentReadTests
{
    [Fact]
    public void NoWindows()
    {
        using var reader
            = new SegmentReader(new MemoryStream(WindowDefinitionSegmentData.NoWindows));
        var segment = reader.Read() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is WindowDefinitionSegment wds)
        {
            Assert.True(wds.Definitions.Count == 0);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public async Task NoWindowsAsync()
    {
        using var reader
            = new SegmentReader(new MemoryStream(WindowDefinitionSegmentData.NoWindows));
        var segment = await reader.ReadAsync() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is WindowDefinitionSegment wds)
        {
            Assert.True(wds.Definitions.Count == 0);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public void OneWindow()
    {
        using var reader
            = new SegmentReader(new MemoryStream(WindowDefinitionSegmentData.OneWindow));
        var segment = reader.Read() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is WindowDefinitionSegment wds)
        {
            Assert.True(wds.Definitions.Count == 1);
            Assert.True(wds.Definitions[0].Id == 0xEF);
            Assert.True(wds.Definitions[0].X == 0xA1B2);
            Assert.True(wds.Definitions[0].Y == 0xC3D4);
            Assert.True(wds.Definitions[0].Width == 0x2143);
            Assert.True(wds.Definitions[0].Height == 0x6587);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public async Task OneWindowAsync()
    {
        using var reader
            = new SegmentReader(new MemoryStream(WindowDefinitionSegmentData.OneWindow));
        var segment = await reader.ReadAsync() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is WindowDefinitionSegment wds)
        {
            Assert.True(wds.Definitions.Count == 1);
            Assert.True(wds.Definitions[0].Id == 0xEF);
            Assert.True(wds.Definitions[0].X == 0xA1B2);
            Assert.True(wds.Definitions[0].Y == 0xC3D4);
            Assert.True(wds.Definitions[0].Width == 0x2143);
            Assert.True(wds.Definitions[0].Height == 0x6587);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public void TwoWindows()
    {
        using var reader
            = new SegmentReader(new MemoryStream(WindowDefinitionSegmentData.TwoWindows));
        var segment = reader.Read() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is WindowDefinitionSegment wds)
        {
            Assert.True(wds.Definitions.Count == 2);
            Assert.True(wds.Definitions[0].Id == 0xEF);
            Assert.True(wds.Definitions[0].X == 0xA1B2);
            Assert.True(wds.Definitions[0].Y == 0xC3D4);
            Assert.True(wds.Definitions[0].Width == 0x2143);
            Assert.True(wds.Definitions[0].Height == 0x6587);
            Assert.True(wds.Definitions[1].Id == 0xFE);
            Assert.True(wds.Definitions[1].X == 0x1A2B);
            Assert.True(wds.Definitions[1].Y == 0x3C4D);
            Assert.True(wds.Definitions[1].Width == 0x1234);
            Assert.True(wds.Definitions[1].Height == 0x5678);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public async Task TwoWindowsAsync()
    {
        using var reader
            = new SegmentReader(new MemoryStream(WindowDefinitionSegmentData.TwoWindows));
        var segment = await reader.ReadAsync() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is WindowDefinitionSegment wds)
        {
            Assert.True(wds.Definitions.Count == 2);
            Assert.True(wds.Definitions[0].Id == 0xEF);
            Assert.True(wds.Definitions[0].X == 0xA1B2);
            Assert.True(wds.Definitions[0].Y == 0xC3D4);
            Assert.True(wds.Definitions[0].Width == 0x2143);
            Assert.True(wds.Definitions[0].Height == 0x6587);
            Assert.True(wds.Definitions[1].Id == 0xFE);
            Assert.True(wds.Definitions[1].X == 0x1A2B);
            Assert.True(wds.Definitions[1].Y == 0x3C4D);
            Assert.True(wds.Definitions[1].Width == 0x1234);
            Assert.True(wds.Definitions[1].Height == 0x5678);
        }
        else
        {
            Assert.True(false);
        }
    }
}
