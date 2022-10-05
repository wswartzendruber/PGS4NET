/*
 * Copyright 2022 William Swartzendruber
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

public class SODSWriteTests
{
    [Fact]
    public void Empty()
    {
        using (var stream = new MemoryStream())
        {
            var sods = new SingleObjectDefinitionSegment
            {
                PTS = 0x01234567,
                DTS = 0x12345678,
                ID = 0xA0A1,
                Version = 0xA2,
                Width = 0x2143,
                Height = 0x6587,
                Data = new byte[0],
            };

            stream.WriteSegment(sods);

            Assert.True(Enumerable.SequenceEqual(
                SODS.Empty,
                stream.ToArray()
            ));
        }
    }

    [Fact]
    public void Small()
    {
        using (var stream = new MemoryStream())
        {
            var sods = new SingleObjectDefinitionSegment
            {
                PTS = 0x01234567,
                DTS = 0x12345678,
                ID = 0xA0A1,
                Version = 0xA2,
                Width = 0x2143,
                Height = 0x6587,
                Data = new byte[] { 0xE0, 0xE1, 0xE2, 0xE3 },
            };

            stream.WriteSegment(sods);

            Assert.True(Enumerable.SequenceEqual(
                SODS.Small,
                stream.ToArray()
            ));
        }
    }
}
