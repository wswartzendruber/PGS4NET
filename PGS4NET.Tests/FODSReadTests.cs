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

using System.IO;
using PGS4NET;

public class FODSReadTests
{
    [Fact]
    public void Empty()
    {
        using (var stream = new MemoryStream(FODS.Empty))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is FinalObjectDefinitionSegment fods)
            {
                Assert.True(fods.ID == 0xA0A1);
                Assert.True(fods.Version == 0xA2);
                Assert.True(fods.Data.Length == 0);
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
        using (var stream = new MemoryStream(FODS.Small))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is FinalObjectDefinitionSegment fods)
            {
                Assert.True(fods.ID == 0xA0A1);
                Assert.True(fods.Version == 0xA2);
                Assert.True(fods.Data.Length == 4);
                Assert.True(fods.Data[0] == 0xE0);
                Assert.True(fods.Data[1] == 0xE1);
                Assert.True(fods.Data[2] == 0xE2);
                Assert.True(fods.Data[3] == 0xE3);
            }
            else
            {
                Assert.True(false);
            }
        }
    }
}
