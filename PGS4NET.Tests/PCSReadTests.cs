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

using System.IO;
using PGS4NET;

public class PCSReadTests
{
    [Fact]
    public void Normal_NoPaletteUpdateID_NoObjects()
    {
        using (var stream = new MemoryStream(PCS.Normal_NoPaletteUpdateID_NoObjects))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567, "Unexpected DTS.");
            Assert.True(segment.DTS == 0x12345678, "Unexpected DTS.");

            if (segment is PresentationCompositionSegment)
            {
                var pcs = (PresentationCompositionSegment)segment;

                Assert.True(pcs.Width == 0x2143, "Unexpected PCS width.");
                Assert.True(pcs.Height == 0x6587, "Unexpected PCS height.");
                Assert.True(pcs.FrameRate == 0x10, "Unexpected PCS frame rate.");
                Assert.True(pcs.Number == 0x6543, "Unexpected PCS number.");
                Assert.True(pcs.State == CompositionState.Normal, "Unexpected PCS state.");
                Assert.True(pcs.PaletteUpdateID == null, "Unexpected PCS palette update ID.");
                Assert.True(pcs.Objects.Count == 0, "Unexpected PCS object count.");
            }
            else
            {
                Assert.True(false, "Returned segment is not a PCS.");
            }
        }
    }
}
