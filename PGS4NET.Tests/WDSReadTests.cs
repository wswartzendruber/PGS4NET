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

public class WDSReadTests
{
    [Fact]
    public void NoWindows()
    {
        using (var stream = new MemoryStream(WDS.NoWindows))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is WindowDefinitionSegment wds)
            {
                Assert.True(wds.Definitions.Count == 0);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void NoWindowsAsync()
    {
        using (var stream = new MemoryStream(WDS.NoWindows))
        {
            var segment = stream.ReadSegmentAsync().Result;

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is WindowDefinitionSegment wds)
            {
                Assert.True(wds.Definitions.Count == 0);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void OneWindow()
    {
        using (var stream = new MemoryStream(WDS.OneWindow))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is WindowDefinitionSegment wds)
            {
                Assert.True(wds.Definitions.Count == 1);
                Assert.True(wds.Definitions[0].ID == 0xEF);
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
    }

    [Fact]
    public void OneWindowAsync()
    {
        using (var stream = new MemoryStream(WDS.OneWindow))
        {
            var segment = stream.ReadSegmentAsync().Result;

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is WindowDefinitionSegment wds)
            {
                Assert.True(wds.Definitions.Count == 1);
                Assert.True(wds.Definitions[0].ID == 0xEF);
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
    }

    [Fact]
    public void TwoWindows()
    {
        using (var stream = new MemoryStream(WDS.TwoWindows))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is WindowDefinitionSegment wds)
            {
                Assert.True(wds.Definitions.Count == 2);
                Assert.True(wds.Definitions[0].ID == 0xEF);
                Assert.True(wds.Definitions[0].X == 0xA1B2);
                Assert.True(wds.Definitions[0].Y == 0xC3D4);
                Assert.True(wds.Definitions[0].Width == 0x2143);
                Assert.True(wds.Definitions[0].Height == 0x6587);
                Assert.True(wds.Definitions[1].ID == 0xFE);
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

    [Fact]
    public void TwoWindowsAsync()
    {
        using (var stream = new MemoryStream(WDS.TwoWindows))
        {
            var segment = stream.ReadSegmentAsync().Result;

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is WindowDefinitionSegment wds)
            {
                Assert.True(wds.Definitions.Count == 2);
                Assert.True(wds.Definitions[0].ID == 0xEF);
                Assert.True(wds.Definitions[0].X == 0xA1B2);
                Assert.True(wds.Definitions[0].Y == 0xC3D4);
                Assert.True(wds.Definitions[0].Width == 0x2143);
                Assert.True(wds.Definitions[0].Height == 0x6587);
                Assert.True(wds.Definitions[1].ID == 0xFE);
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
}
