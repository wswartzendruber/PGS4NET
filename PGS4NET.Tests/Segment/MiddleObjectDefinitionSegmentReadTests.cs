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
using PGS4NET.Segment;

namespace PGS4NET.Tests.Segment;

public class MiddleObjectDefinitionSegmentReadTests
{
    [Fact]
    public void Empty()
    {
        using (var stream = new MemoryStream(MiddleObjectDefinitionSegmentData.Empty))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is MiddleObjectDefinitionSegment mods)
            {
                Assert.True(mods.ID == 0xA0A1);
                Assert.True(mods.Version == 0xA2);
                Assert.True(mods.Data.Length == 0);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void EmptyAsync()
    {
        using (var stream = new MemoryStream(MiddleObjectDefinitionSegmentData.Empty))
        {
            var segment = stream.ReadSegmentAsync().Result;

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is MiddleObjectDefinitionSegment mods)
            {
                Assert.True(mods.ID == 0xA0A1);
                Assert.True(mods.Version == 0xA2);
                Assert.True(mods.Data.Length == 0);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void Small()
    {
        using (var stream = new MemoryStream(MiddleObjectDefinitionSegmentData.Small))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is MiddleObjectDefinitionSegment mods)
            {
                Assert.True(mods.ID == 0xA0A1);
                Assert.True(mods.Version == 0xA2);
                Assert.True(mods.Data.Length == 4);
                Assert.True(mods.Data[0] == 0xE0);
                Assert.True(mods.Data[1] == 0xE1);
                Assert.True(mods.Data[2] == 0xE2);
                Assert.True(mods.Data[3] == 0xE3);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void SmallAsync()
    {
        using (var stream = new MemoryStream(MiddleObjectDefinitionSegmentData.Small))
        {
            var segment = stream.ReadSegmentAsync().Result;

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is MiddleObjectDefinitionSegment mods)
            {
                Assert.True(mods.ID == 0xA0A1);
                Assert.True(mods.Version == 0xA2);
                Assert.True(mods.Data.Length == 4);
                Assert.True(mods.Data[0] == 0xE0);
                Assert.True(mods.Data[1] == 0xE1);
                Assert.True(mods.Data[2] == 0xE2);
                Assert.True(mods.Data[3] == 0xE3);
            }
            else
            {
                Assert.True(false);
            }
        }
    }
}
