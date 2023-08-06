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

using System.Collections.Generic;
using System.IO;
using PGS4NET.Segments;

namespace PGS4NET.Tests.Segments;

public class InitialObjectDefinitionSegmentWriteTests
{
    [Fact]
    public void Empty()
    {
        using var stream = new MemoryStream();
        using var writer = new SegmentWriter(stream);
        var iods = new InitialObjectDefinitionSegment
        {
            Pts = 0x01234567,
            Dts = 0x12345678,
            Id = 0xA0A1,
            Version = 0xA2,
            Length = 0xABCDEF,
            Width = 0x2143,
            Height = 0x6587,
            Data = new byte[0],
        };

        writer.Write(iods);

        Assert.True(Enumerable.SequenceEqual(
            InitialObjectDefinitionSegmentData.Empty,
            stream.ToArray()
        ));
    }

    [Fact]
    public async Task EmptyAsync()
    {
        using var stream = new MemoryStream();
        using var writer = new SegmentWriter(stream);
        var iods = new InitialObjectDefinitionSegment
        {
            Pts = 0x01234567,
            Dts = 0x12345678,
            Id = 0xA0A1,
            Version = 0xA2,
            Length = 0xABCDEF,
            Width = 0x2143,
            Height = 0x6587,
            Data = new byte[0],
        };

        await writer.WriteAsync(iods);

        Assert.True(Enumerable.SequenceEqual(
            InitialObjectDefinitionSegmentData.Empty,
            stream.ToArray()
        ));
    }

    [Fact]
    public void Small()
    {
        using var stream = new MemoryStream();
        using var writer = new SegmentWriter(stream);
        var iods = new InitialObjectDefinitionSegment
        {
            Pts = 0x01234567,
            Dts = 0x12345678,
            Id = 0xA0A1,
            Version = 0xA2,
            Length = 0xABCDEF,
            Width = 0x2143,
            Height = 0x6587,
            Data = new byte[] { 0xE0, 0xE1, 0xE2, 0xE3 },
        };

        writer.Write(iods);

        Assert.True(Enumerable.SequenceEqual(
            InitialObjectDefinitionSegmentData.Small,
            stream.ToArray()
        ));
    }

    [Fact]
    public async Task SmallAsync()
    {
        using var stream = new MemoryStream();
        using var writer = new SegmentWriter(stream);
        var iods = new InitialObjectDefinitionSegment
        {
            Pts = 0x01234567,
            Dts = 0x12345678,
            Id = 0xA0A1,
            Version = 0xA2,
            Length = 0xABCDEF,
            Width = 0x2143,
            Height = 0x6587,
            Data = new byte[] { 0xE0, 0xE1, 0xE2, 0xE3 },
        };

        await writer.WriteAsync(iods);

        Assert.True(Enumerable.SequenceEqual(
            InitialObjectDefinitionSegmentData.Small,
            stream.ToArray()
        ));
    }
}
