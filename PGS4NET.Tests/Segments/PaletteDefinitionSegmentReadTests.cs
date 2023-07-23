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

public class PaletteDefinitionSegmentReadTests
{
    [Fact]
    public void NoEntries()
    {
        using (var stream = new MemoryStream(PaletteDefinitionSegmentData.NoEntries))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.Pts == 0x01234567);
            Assert.True(segment.Dts == 0x12345678);

            if (segment is PaletteDefinitionSegment pds)
            {
                Assert.True(pds.Id == 0xA1);
                Assert.True(pds.Version == 0xA2);
                Assert.True(pds.Entries.Count == 0);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void NoEntriesAsync()
    {
        using (var stream = new MemoryStream(PaletteDefinitionSegmentData.NoEntries))
        {
            var segment = stream.ReadSegmentAsync().Result;

            Assert.True(segment.Pts == 0x01234567);
            Assert.True(segment.Dts == 0x12345678);

            if (segment is PaletteDefinitionSegment pds)
            {
                Assert.True(pds.Id == 0xA1);
                Assert.True(pds.Version == 0xA2);
                Assert.True(pds.Entries.Count == 0);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void OneEntry()
    {
        using (var stream = new MemoryStream(PaletteDefinitionSegmentData.OneEntry))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.Pts == 0x01234567);
            Assert.True(segment.Dts == 0x12345678);

            if (segment is PaletteDefinitionSegment pds)
            {
                Assert.True(pds.Id == 0xA1);
                Assert.True(pds.Version == 0xA2);
                Assert.True(pds.Entries.Count == 1);
                Assert.True(pds.Entries[0].Id == 0xB1);
                Assert.True(pds.Entries[0].Y == 0xB2);
                Assert.True(pds.Entries[0].Cr == 0xB3);
                Assert.True(pds.Entries[0].Cb == 0xB4);
                Assert.True(pds.Entries[0].Alpha == 0xB5);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void OneEntryAsync()
    {
        using (var stream = new MemoryStream(PaletteDefinitionSegmentData.OneEntry))
        {
            var segment = stream.ReadSegmentAsync().Result;

            Assert.True(segment.Pts == 0x01234567);
            Assert.True(segment.Dts == 0x12345678);

            if (segment is PaletteDefinitionSegment pds)
            {
                Assert.True(pds.Id == 0xA1);
                Assert.True(pds.Version == 0xA2);
                Assert.True(pds.Entries.Count == 1);
                Assert.True(pds.Entries[0].Id == 0xB1);
                Assert.True(pds.Entries[0].Y == 0xB2);
                Assert.True(pds.Entries[0].Cr == 0xB3);
                Assert.True(pds.Entries[0].Cb == 0xB4);
                Assert.True(pds.Entries[0].Alpha == 0xB5);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void TwoEntries()
    {
        using (var stream = new MemoryStream(PaletteDefinitionSegmentData.TwoEntries))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.Pts == 0x01234567);
            Assert.True(segment.Dts == 0x12345678);

            if (segment is PaletteDefinitionSegment pds)
            {
                Assert.True(pds.Id == 0xA1);
                Assert.True(pds.Version == 0xA2);
                Assert.True(pds.Entries.Count == 2);
                Assert.True(pds.Entries[0].Id == 0xB1);
                Assert.True(pds.Entries[0].Y == 0xB2);
                Assert.True(pds.Entries[0].Cr == 0xB3);
                Assert.True(pds.Entries[0].Cb == 0xB4);
                Assert.True(pds.Entries[0].Alpha == 0xB5);
                Assert.True(pds.Entries[1].Id == 0xC1);
                Assert.True(pds.Entries[1].Y == 0xC2);
                Assert.True(pds.Entries[1].Cr == 0xC3);
                Assert.True(pds.Entries[1].Cb == 0xC4);
                Assert.True(pds.Entries[1].Alpha == 0xC5);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void TwoEntriesAsync()
    {
        using (var stream = new MemoryStream(PaletteDefinitionSegmentData.TwoEntries))
        {
            var segment = stream.ReadSegmentAsync().Result;

            Assert.True(segment.Pts == 0x01234567);
            Assert.True(segment.Dts == 0x12345678);

            if (segment is PaletteDefinitionSegment pds)
            {
                Assert.True(pds.Id == 0xA1);
                Assert.True(pds.Version == 0xA2);
                Assert.True(pds.Entries.Count == 2);
                Assert.True(pds.Entries[0].Id == 0xB1);
                Assert.True(pds.Entries[0].Y == 0xB2);
                Assert.True(pds.Entries[0].Cr == 0xB3);
                Assert.True(pds.Entries[0].Cb == 0xB4);
                Assert.True(pds.Entries[0].Alpha == 0xB5);
                Assert.True(pds.Entries[1].Id == 0xC1);
                Assert.True(pds.Entries[1].Y == 0xC2);
                Assert.True(pds.Entries[1].Cr == 0xC3);
                Assert.True(pds.Entries[1].Cb == 0xC4);
                Assert.True(pds.Entries[1].Alpha == 0xC5);
            }
            else
            {
                Assert.True(false);
            }
        }
    }
}
