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

namespace PGS4NET.Tests;

using System.Collections.Generic;
using System.IO;
using PGS4NET;

public class PDSWriteTests
{
    [Fact]
    public void NoEntries()
    {
        using (var stream = new MemoryStream())
        {
            var pds = new PaletteDefinitionSegment
            {
                PTS = 0x01234567,
                DTS = 0x12345678,
                ID = 0xA1,
                Version = 0xA2,
            };

            stream.WriteSegment(pds);

            Assert.True(Enumerable.SequenceEqual(
                PDS.NoEntries,
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
                PTS = 0x01234567,
                DTS = 0x12345678,
                ID = 0xA1,
                Version = 0xA2,
                Entries = new List<PaletteEntry>
                {
                    new PaletteEntry
                    {
                        ID = 0xB1,
                        Y = 0xB2,
                        Cr = 0xB3,
                        Cb = 0xB4,
                        Alpha = 0xB5,
                    },
                },
            };

            stream.WriteSegment(pds);

            Assert.True(Enumerable.SequenceEqual(
                PDS.OneEntry,
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
                PTS = 0x01234567,
                DTS = 0x12345678,
                ID = 0xA1,
                Version = 0xA2,
                Entries = new List<PaletteEntry>
                {
                    new PaletteEntry
                    {
                        ID = 0xB1,
                        Y = 0xB2,
                        Cr = 0xB3,
                        Cb = 0xB4,
                        Alpha = 0xB5,
                    },
                    new PaletteEntry
                    {
                        ID = 0xC1,
                        Y = 0xC2,
                        Cr = 0xC3,
                        Cb = 0xC4,
                        Alpha = 0xC5,
                    },
                },
            };

            stream.WriteSegment(pds);

            Assert.True(Enumerable.SequenceEqual(
                PDS.TwoEntries,
                stream.ToArray()
            ));
        }
    }
}
