﻿/*
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

public class FinalObjectDefinitionSegmentReadTests
{
    [Fact]
    public void Empty()
    {
        using var reader
            = new SegmentReader(new MemoryStream(FinalObjectDefinitionSegmentData.Empty));
        var segment = reader.Read() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is FinalObjectDefinitionSegment fods)
        {
            Assert.True(fods.Id == 0xA0A1);
            Assert.True(fods.Version == 0xA2);
            Assert.True(fods.Data.Length == 0);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public async Task EmptyAsync()
    {
        using var reader
            = new SegmentReader(new MemoryStream(FinalObjectDefinitionSegmentData.Empty));
        var segment = await reader.ReadAsync() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is FinalObjectDefinitionSegment fods)
        {
            Assert.True(fods.Id == 0xA0A1);
            Assert.True(fods.Version == 0xA2);
            Assert.True(fods.Data.Length == 0);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public void Small()
    {
        using var reader
            = new SegmentReader(new MemoryStream(FinalObjectDefinitionSegmentData.Small));
        var segment = reader.Read() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is FinalObjectDefinitionSegment fods)
        {
            Assert.True(fods.Id == 0xA0A1);
            Assert.True(fods.Version == 0xA2);
            Assert.True(fods.Data.Length == 4);
            Assert.True(fods.Data[0] == 0xE0);
            Assert.True(fods.Data[1] == 0xE1);
            Assert.True(fods.Data[2] == 0xE2);
            Assert.True(fods.Data[3] == 0xE3);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public async Task SmallAsync()
    {
        using var reader
            = new SegmentReader(new MemoryStream(FinalObjectDefinitionSegmentData.Small));
        var segment = await reader.ReadAsync() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is FinalObjectDefinitionSegment fods)
        {
            Assert.True(fods.Id == 0xA0A1);
            Assert.True(fods.Version == 0xA2);
            Assert.True(fods.Data.Length == 4);
            Assert.True(fods.Data[0] == 0xE0);
            Assert.True(fods.Data[1] == 0xE1);
            Assert.True(fods.Data[2] == 0xE2);
            Assert.True(fods.Data[3] == 0xE3);
        }
        else
        {
            Assert.True(false);
        }
    }
}