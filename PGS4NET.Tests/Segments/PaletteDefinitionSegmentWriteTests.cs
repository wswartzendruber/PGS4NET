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

using System.Collections.Generic;
using System.IO;
using PGS4NET.Segments;

namespace PGS4NET.Tests.Segments;

public class PaletteDefinitionSegmentWriteTests
{
    [Fact]
    public void NoEntries()
    {
        using (var stream = new MemoryStream())
        {
            var pds = new PaletteDefinitionSegment
            {
                Pts = 0x01234567,
                Dts = 0x12345678,
                Id = 0xA1,
                Version = 0xA2,
            };

            stream.WriteSegment(pds);

            Assert.True(Enumerable.SequenceEqual(
                PaletteDefinitionSegmentData.NoEntries,
                stream.ToArray()
            ));
        }
    }

    [Fact]
    public void NoEntriesAsync()
    {
        using (var stream = new MemoryStream())
        {
            var pds = new PaletteDefinitionSegment
            {
                Pts = 0x01234567,
                Dts = 0x12345678,
                Id = 0xA1,
                Version = 0xA2,
            };

            stream.WriteSegmentAsync(pds).Wait();

            Assert.True(Enumerable.SequenceEqual(
                PaletteDefinitionSegmentData.NoEntries,
                stream.ToArray()
            ));
        }
    }

    [Fact]
    public void OneEntry()
    {
        using (var stream = new MemoryStream())
        {
            var pds = new PaletteDefinitionSegment
            {
                Pts = 0x01234567,
                Dts = 0x12345678,
                Id = 0xA1,
                Version = 0xA2,
                Entries = new List<PaletteDefinitionEntry>
                {
                    new PaletteDefinitionEntry
                    {
                        Id = 0xB1,
                        Y = 0xB2,
                        Cr = 0xB3,
                        Cb = 0xB4,
                        Alpha = 0xB5,
                    },
                },
            };

            stream.WriteSegment(pds);

            Assert.True(Enumerable.SequenceEqual(
                PaletteDefinitionSegmentData.OneEntry,
                stream.ToArray()
            ));
        }
    }

    [Fact]
    public void OneEntryAsync()
    {
        using (var stream = new MemoryStream())
        {
            var pds = new PaletteDefinitionSegment
            {
                Pts = 0x01234567,
                Dts = 0x12345678,
                Id = 0xA1,
                Version = 0xA2,
                Entries = new List<PaletteDefinitionEntry>
                {
                    new PaletteDefinitionEntry
                    {
                        Id = 0xB1,
                        Y = 0xB2,
                        Cr = 0xB3,
                        Cb = 0xB4,
                        Alpha = 0xB5,
                    },
                },
            };

            stream.WriteSegmentAsync(pds).Wait();

            Assert.True(Enumerable.SequenceEqual(
                PaletteDefinitionSegmentData.OneEntry,
                stream.ToArray()
            ));
        }
    }

    [Fact]
    public void TwoEntries()
    {
        using (var stream = new MemoryStream())
        {
            var pds = new PaletteDefinitionSegment
            {
                Pts = 0x01234567,
                Dts = 0x12345678,
                Id = 0xA1,
                Version = 0xA2,
                Entries = new List<PaletteDefinitionEntry>
                {
                    new PaletteDefinitionEntry
                    {
                        Id = 0xB1,
                        Y = 0xB2,
                        Cr = 0xB3,
                        Cb = 0xB4,
                        Alpha = 0xB5,
                    },
                    new PaletteDefinitionEntry
                    {
                        Id = 0xC1,
                        Y = 0xC2,
                        Cr = 0xC3,
                        Cb = 0xC4,
                        Alpha = 0xC5,
                    },
                },
            };

            stream.WriteSegment(pds);

            Assert.True(Enumerable.SequenceEqual(
                PaletteDefinitionSegmentData.TwoEntries,
                stream.ToArray()
            ));
        }
    }

    [Fact]
    public void TwoEntriesAsync()
    {
        using (var stream = new MemoryStream())
        {
            var pds = new PaletteDefinitionSegment
            {
                Pts = 0x01234567,
                Dts = 0x12345678,
                Id = 0xA1,
                Version = 0xA2,
                Entries = new List<PaletteDefinitionEntry>
                {
                    new PaletteDefinitionEntry
                    {
                        Id = 0xB1,
                        Y = 0xB2,
                        Cr = 0xB3,
                        Cb = 0xB4,
                        Alpha = 0xB5,
                    },
                    new PaletteDefinitionEntry
                    {
                        Id = 0xC1,
                        Y = 0xC2,
                        Cr = 0xC3,
                        Cb = 0xC4,
                        Alpha = 0xC5,
                    },
                },
            };

            stream.WriteSegmentAsync(pds).Wait();

            Assert.True(Enumerable.SequenceEqual(
                PaletteDefinitionSegmentData.TwoEntries,
                stream.ToArray()
            ));
        }
    }
}
