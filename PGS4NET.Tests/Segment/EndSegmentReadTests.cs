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

public class EndSegmentReadTests
{
    [Fact]
    public void Single()
    {
        using (var stream = new MemoryStream(EndSegmentData.Single))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is not EndSegment)
                Assert.True(false);
        }
    }

    [Fact]
    public void SingleAsync()
    {
        using (var stream = new MemoryStream(EndSegmentData.Single))
        {
            var segment = stream.ReadSegmentAsync().Result;

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is not EndSegment)
                Assert.True(false);
        }
    }
}
