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

public class SingleObjectDefinitionSegmentReadTests
{
    [Fact]
    public void Empty()
    {
        using (var stream = new MemoryStream(SingleObjectDefinitionSegmentData.Empty))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is SingleObjectDefinitionSegment sods)
            {
                Assert.True(sods.Id == 0xA0A1);
                Assert.True(sods.Version == 0xA2);
                Assert.True(sods.Width == 0x2143);
                Assert.True(sods.Height == 0x6587);
                Assert.True(sods.Data.Length == 0);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void EmptyASync()
    {
        using (var stream = new MemoryStream(SingleObjectDefinitionSegmentData.Empty))
        {
            var segment = stream.ReadSegmentAsync().Result;

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is SingleObjectDefinitionSegment sods)
            {
                Assert.True(sods.Id == 0xA0A1);
                Assert.True(sods.Version == 0xA2);
                Assert.True(sods.Width == 0x2143);
                Assert.True(sods.Height == 0x6587);
                Assert.True(sods.Data.Length == 0);
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
        using (var stream = new MemoryStream(SingleObjectDefinitionSegmentData.Small))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is SingleObjectDefinitionSegment sods)
            {
                Assert.True(sods.Id == 0xA0A1);
                Assert.True(sods.Version == 0xA2);
                Assert.True(sods.Width == 0x2143);
                Assert.True(sods.Height == 0x6587);
                Assert.True(sods.Data.Length == 4);
                Assert.True(sods.Data[0] == 0xE0);
                Assert.True(sods.Data[1] == 0xE1);
                Assert.True(sods.Data[2] == 0xE2);
                Assert.True(sods.Data[3] == 0xE3);
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
        using (var stream = new MemoryStream(SingleObjectDefinitionSegmentData.Small))
        {
            var segment = stream.ReadSegmentAsync().Result;

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is SingleObjectDefinitionSegment sods)
            {
                Assert.True(sods.Id == 0xA0A1);
                Assert.True(sods.Version == 0xA2);
                Assert.True(sods.Width == 0x2143);
                Assert.True(sods.Height == 0x6587);
                Assert.True(sods.Data.Length == 4);
                Assert.True(sods.Data[0] == 0xE0);
                Assert.True(sods.Data[1] == 0xE1);
                Assert.True(sods.Data[2] == 0xE2);
                Assert.True(sods.Data[3] == 0xE3);
            }
            else
            {
                Assert.True(false);
            }
        }
    }
}
