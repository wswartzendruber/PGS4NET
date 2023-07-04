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
using PGS4NET;

namespace PGS4NET.Tests;

public class PCSReadTests
{
    [Fact]
    public void EpochStart_NoPaletteUpdateID_NoObjects()
    {
        using (var stream = new MemoryStream(PCS.EpochStart_NoPaletteUpdateID_NoObjects))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is PresentationCompositionSegment pcs)
            {
                Assert.True(pcs.Width == 0x2143);
                Assert.True(pcs.Height == 0x6587);
                Assert.True(pcs.FrameRate == 0x10);
                Assert.True(pcs.Number == 0x6543);
                Assert.True(pcs.State == CompositionState.EpochStart);
                Assert.True(pcs.PaletteUpdateOnly == false);
                Assert.True(pcs.PaletteUpdateID == 0xAB);
                Assert.True(pcs.Objects.Count == 0);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void EpochStart_NoPaletteUpdateID_NoObjects_Async()
    {
        using (var stream = new MemoryStream(PCS.EpochStart_NoPaletteUpdateID_NoObjects))
        {
            var segment = stream.ReadSegmentAsync().Result;

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is PresentationCompositionSegment pcs)
            {
                Assert.True(pcs.Width == 0x2143);
                Assert.True(pcs.Height == 0x6587);
                Assert.True(pcs.FrameRate == 0x10);
                Assert.True(pcs.Number == 0x6543);
                Assert.True(pcs.State == CompositionState.EpochStart);
                Assert.True(pcs.PaletteUpdateOnly == false);
                Assert.True(pcs.PaletteUpdateID == 0xAB);
                Assert.True(pcs.Objects.Count == 0);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void AcquisitionPoint_NoPaletteUpdateID_NoObjects()
    {
        using (var stream = new MemoryStream(PCS.AcquisitionPoint_NoPaletteUpdateID_NoObjects))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is PresentationCompositionSegment pcs)
            {
                Assert.True(pcs.Width == 0x2143);
                Assert.True(pcs.Height == 0x6587);
                Assert.True(pcs.FrameRate == 0x10);
                Assert.True(pcs.Number == 0x6543);
                Assert.True(pcs.State == CompositionState.AcquisitionPoint);
                Assert.True(pcs.PaletteUpdateOnly == false);
                Assert.True(pcs.PaletteUpdateID == 0xAB);
                Assert.True(pcs.Objects.Count == 0);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void AcquisitionPoint_NoPaletteUpdateID_NoObjects_Async()
    {
        using (var stream = new MemoryStream(PCS.AcquisitionPoint_NoPaletteUpdateID_NoObjects))
        {
            var segment = stream.ReadSegmentAsync().Result;

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is PresentationCompositionSegment pcs)
            {
                Assert.True(pcs.Width == 0x2143);
                Assert.True(pcs.Height == 0x6587);
                Assert.True(pcs.FrameRate == 0x10);
                Assert.True(pcs.Number == 0x6543);
                Assert.True(pcs.State == CompositionState.AcquisitionPoint);
                Assert.True(pcs.PaletteUpdateOnly == false);
                Assert.True(pcs.PaletteUpdateID == 0xAB);
                Assert.True(pcs.Objects.Count == 0);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void Normal_NoPaletteUpdateID_NoObjects()
    {
        using (var stream = new MemoryStream(PCS.Normal_NoPaletteUpdateID_NoObjects))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is PresentationCompositionSegment pcs)
            {
                Assert.True(pcs.Width == 0x2143);
                Assert.True(pcs.Height == 0x6587);
                Assert.True(pcs.FrameRate == 0x10);
                Assert.True(pcs.Number == 0x6543);
                Assert.True(pcs.State == CompositionState.Normal);
                Assert.True(pcs.PaletteUpdateOnly == false);
                Assert.True(pcs.PaletteUpdateID == 0xAB);
                Assert.True(pcs.Objects.Count == 0);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void Normal_NoPaletteUpdateID_NoObjects_Async()
    {
        using (var stream = new MemoryStream(PCS.Normal_NoPaletteUpdateID_NoObjects))
        {
            var segment = stream.ReadSegmentAsync().Result;

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is PresentationCompositionSegment pcs)
            {
                Assert.True(pcs.Width == 0x2143);
                Assert.True(pcs.Height == 0x6587);
                Assert.True(pcs.FrameRate == 0x10);
                Assert.True(pcs.Number == 0x6543);
                Assert.True(pcs.State == CompositionState.Normal);
                Assert.True(pcs.PaletteUpdateOnly == false);
                Assert.True(pcs.PaletteUpdateID == 0xAB);
                Assert.True(pcs.Objects.Count == 0);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void Normal_PaletteUpdateID_NoObjects()
    {
        using (var stream = new MemoryStream(PCS.Normal_PaletteUpdateID_NoObjects))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is PresentationCompositionSegment pcs)
            {
                Assert.True(pcs.Width == 0x2143);
                Assert.True(pcs.Height == 0x6587);
                Assert.True(pcs.FrameRate == 0x10);
                Assert.True(pcs.Number == 0x6543);
                Assert.True(pcs.State == CompositionState.Normal);
                Assert.True(pcs.PaletteUpdateOnly == true);
                Assert.True(pcs.PaletteUpdateID == 0xAB);
                Assert.True(pcs.Objects.Count == 0);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void Normal_NoPaletteUpdateID_OneObjectForced()
    {
        using (var stream = new MemoryStream(PCS.Normal_NoPaletteUpdateID_OneObjectForced))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is PresentationCompositionSegment pcs)
            {
                Assert.True(pcs.Width == 0x2143);
                Assert.True(pcs.Height == 0x6587);
                Assert.True(pcs.FrameRate == 0x10);
                Assert.True(pcs.Number == 0x6543);
                Assert.True(pcs.State == CompositionState.Normal);
                Assert.True(pcs.PaletteUpdateOnly == false);
                Assert.True(pcs.PaletteUpdateID == 0xAB);
                Assert.True(pcs.Objects.Count == 1);
                Assert.True(pcs.Objects[0].ObjectID == 0xABCD);
                Assert.True(pcs.Objects[0].WindowID == 0xEF);
                Assert.True(pcs.Objects[0].X == 0x1A2B);
                Assert.True(pcs.Objects[0].Y == 0x3C4D);
                Assert.True(pcs.Objects[0].Forced == true);
                Assert.True(pcs.Objects[0].Crop == null);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void Normal_NoPaletteUpdateID_OneObjectCropped()
    {
        using (var stream = new MemoryStream(PCS.Normal_NoPaletteUpdateID_OneObjectCropped))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is PresentationCompositionSegment pcs)
            {
                Assert.True(pcs.Width == 0x2143);
                Assert.True(pcs.Height == 0x6587);
                Assert.True(pcs.FrameRate == 0x10);
                Assert.True(pcs.Number == 0x6543);
                Assert.True(pcs.State == CompositionState.Normal);
                Assert.True(pcs.PaletteUpdateOnly == false);
                Assert.True(pcs.PaletteUpdateID == 0xAB);
                Assert.True(pcs.Objects.Count == 1);
                Assert.True(pcs.Objects[0].ObjectID == 0xABCD);
                Assert.True(pcs.Objects[0].WindowID == 0xEF);
                Assert.True(pcs.Objects[0].X == 0x1A2B);
                Assert.True(pcs.Objects[0].Y == 0x3C4D);
                Assert.True(pcs.Objects[0].Forced == false);
                Assert.True(pcs.Objects[0].Crop?.X == 0xA1A2);
                Assert.True(pcs.Objects[0].Crop?.Y == 0xA3A4);
                Assert.True(pcs.Objects[0].Crop?.Width == 0xA5A6);
                Assert.True(pcs.Objects[0].Crop?.Height == 0xA7A8);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public void Normal_NoPaletteUpdateID_ThreeObjectsMixed()
    {
        using (var stream = new MemoryStream(PCS.Normal_NoPaletteUpdateID_ThreeObjectsMixed))
        {
            var segment = stream.ReadSegment();

            Assert.True(segment.PTS == 0x01234567);
            Assert.True(segment.DTS == 0x12345678);

            if (segment is PresentationCompositionSegment pcs)
            {
                Assert.True(pcs.Width == 0x2143);
                Assert.True(pcs.Height == 0x6587);
                Assert.True(pcs.FrameRate == 0x10);
                Assert.True(pcs.Number == 0x6543);
                Assert.True(pcs.State == CompositionState.Normal);
                Assert.True(pcs.PaletteUpdateOnly == false);
                Assert.True(pcs.PaletteUpdateID == 0xAB);
                Assert.True(pcs.Objects.Count == 3);
                Assert.True(pcs.Objects[0].ObjectID == 0xABCD);
                Assert.True(pcs.Objects[0].WindowID == 0xEF);
                Assert.True(pcs.Objects[0].X == 0x1A2B);
                Assert.True(pcs.Objects[0].Y == 0x3C4D);
                Assert.True(pcs.Objects[0].Forced == false);
                Assert.True(pcs.Objects[0].Crop?.X == 0xA1A2);
                Assert.True(pcs.Objects[0].Crop?.Y == 0xA3A4);
                Assert.True(pcs.Objects[0].Crop?.Width == 0xA5A6);
                Assert.True(pcs.Objects[0].Crop?.Height == 0xA7A8);
                Assert.True(pcs.Objects[1].ObjectID == 0xABCD);
                Assert.True(pcs.Objects[1].WindowID == 0xEF);
                Assert.True(pcs.Objects[1].X == 0x1A2B);
                Assert.True(pcs.Objects[1].Y == 0x3C4D);
                Assert.True(pcs.Objects[1].Forced == true);
                Assert.True(pcs.Objects[1].Crop == null);
                Assert.True(pcs.Objects[2].ObjectID == 0xABCD);
                Assert.True(pcs.Objects[2].WindowID == 0xEF);
                Assert.True(pcs.Objects[2].X == 0x1A2B);
                Assert.True(pcs.Objects[2].Y == 0x3C4D);
                Assert.True(pcs.Objects[2].Forced == true);
                Assert.True(pcs.Objects[2].Crop?.X == 0xA1A2);
                Assert.True(pcs.Objects[2].Crop?.Y == 0xA3A4);
                Assert.True(pcs.Objects[2].Crop?.Width == 0xA5A6);
                Assert.True(pcs.Objects[2].Crop?.Height == 0xA7A8);
            }
            else
            {
                Assert.True(false);
            }
        }
    }
}
