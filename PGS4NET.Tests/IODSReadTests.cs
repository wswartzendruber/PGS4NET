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

namespace PGS4NET.Tests;

public class IODSReadTests
{
    [Fact]
    public void Empty()
    {
        using (var stream = new MemoryStream(IODS.Empty))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is InitialObjectDefinitionSegment iods)
            {
                Assert.True(iods.ID == 0xA0A1);
                Assert.True(iods.Version == 0xA2);
                Assert.True(iods.Length == 0xABCDEF);
                Assert.True(iods.Width == 0x2143);
                Assert.True(iods.Height == 0x6587);
                Assert.True(iods.Data.Length == 0);
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
        using (var stream = new MemoryStream(IODS.Empty))
        {
            var segment = stream.ReadSegmentAsync().Result;

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is InitialObjectDefinitionSegment iods)
            {
                Assert.True(iods.ID == 0xA0A1);
                Assert.True(iods.Version == 0xA2);
                Assert.True(iods.Length == 0xABCDEF);
                Assert.True(iods.Width == 0x2143);
                Assert.True(iods.Height == 0x6587);
                Assert.True(iods.Data.Length == 0);
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
        using (var stream = new MemoryStream(IODS.Small))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is InitialObjectDefinitionSegment iods)
            {
                Assert.True(iods.ID == 0xA0A1);
                Assert.True(iods.Version == 0xA2);
                Assert.True(iods.Length == 0xABCDEF);
                Assert.True(iods.Width == 0x2143);
                Assert.True(iods.Height == 0x6587);
                Assert.True(iods.Data.Length == 4);
                Assert.True(iods.Data[0] == 0xE0);
                Assert.True(iods.Data[1] == 0xE1);
                Assert.True(iods.Data[2] == 0xE2);
                Assert.True(iods.Data[3] == 0xE3);
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
        using (var stream = new MemoryStream(IODS.Small))
        {
            var segment = stream.ReadSegmentAsync().Result;

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is InitialObjectDefinitionSegment iods)
            {
                Assert.True(iods.ID == 0xA0A1);
                Assert.True(iods.Version == 0xA2);
                Assert.True(iods.Length == 0xABCDEF);
                Assert.True(iods.Width == 0x2143);
                Assert.True(iods.Height == 0x6587);
                Assert.True(iods.Data.Length == 4);
                Assert.True(iods.Data[0] == 0xE0);
                Assert.True(iods.Data[1] == 0xE1);
                Assert.True(iods.Data[2] == 0xE2);
                Assert.True(iods.Data[3] == 0xE3);
            }
            else
            {
                Assert.True(false);
            }
        }
    }
}
