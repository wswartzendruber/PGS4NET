﻿/*
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
using PGS4NET.Segments;

namespace PGS4NET.Tests.Segments;

public class PresentationCompositionSegmentReadTests
{
    [Fact]
    public void EpochStart_NoPaletteUpdateID_NoObjects()
    {
        using var reader = new SegmentReader(new MemoryStream(
            PresentationCompositionSegmentData.EpochStart_NoPaletteUpdateID_NoObjects));
        var segment = reader.Read() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is PresentationCompositionSegment pcs)
        {
            Assert.True(pcs.Width == 0x2143);
            Assert.True(pcs.Height == 0x6587);
            Assert.True(pcs.FrameRate == 0x10);
            Assert.True(pcs.Number == 0x6543);
            Assert.True(pcs.State == CompositionState.EpochStart);
            Assert.True(pcs.PaletteUpdateOnly == false);
            Assert.True(pcs.PaletteUpdateId == 0xAB);
            Assert.True(pcs.CompositionObjects.Count == 0);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public async Task EpochStart_NoPaletteUpdateID_NoObjects_Async()
    {
        using var reader = new SegmentReader(new MemoryStream(
            PresentationCompositionSegmentData.EpochStart_NoPaletteUpdateID_NoObjects));
        var segment = await reader.ReadAsync() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is PresentationCompositionSegment pcs)
        {
            Assert.True(pcs.Width == 0x2143);
            Assert.True(pcs.Height == 0x6587);
            Assert.True(pcs.FrameRate == 0x10);
            Assert.True(pcs.Number == 0x6543);
            Assert.True(pcs.State == CompositionState.EpochStart);
            Assert.True(pcs.PaletteUpdateOnly == false);
            Assert.True(pcs.PaletteUpdateId == 0xAB);
            Assert.True(pcs.CompositionObjects.Count == 0);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public void AcquisitionPoint_NoPaletteUpdateID_NoObjects()
    {
        using var reader = new SegmentReader(new MemoryStream(
            PresentationCompositionSegmentData.AcquisitionPoint_NoPaletteUpdateID_NoObjects));
        var segment = reader.Read() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is PresentationCompositionSegment pcs)
        {
            Assert.True(pcs.Width == 0x2143);
            Assert.True(pcs.Height == 0x6587);
            Assert.True(pcs.FrameRate == 0x10);
            Assert.True(pcs.Number == 0x6543);
            Assert.True(pcs.State == CompositionState.AcquisitionPoint);
            Assert.True(pcs.PaletteUpdateOnly == false);
            Assert.True(pcs.PaletteUpdateId == 0xAB);
            Assert.True(pcs.CompositionObjects.Count == 0);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public async Task AcquisitionPoint_NoPaletteUpdateID_NoObjects_Async()
    {
        using var reader = new SegmentReader(new MemoryStream(
            PresentationCompositionSegmentData.AcquisitionPoint_NoPaletteUpdateID_NoObjects));
        var segment = await reader.ReadAsync() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is PresentationCompositionSegment pcs)
        {
            Assert.True(pcs.Width == 0x2143);
            Assert.True(pcs.Height == 0x6587);
            Assert.True(pcs.FrameRate == 0x10);
            Assert.True(pcs.Number == 0x6543);
            Assert.True(pcs.State == CompositionState.AcquisitionPoint);
            Assert.True(pcs.PaletteUpdateOnly == false);
            Assert.True(pcs.PaletteUpdateId == 0xAB);
            Assert.True(pcs.CompositionObjects.Count == 0);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public void Normal_NoPaletteUpdateID_NoObjects()
    {
        using var reader = new SegmentReader(new MemoryStream(
            PresentationCompositionSegmentData.Normal_NoPaletteUpdateID_NoObjects));
        var segment = reader.Read() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is PresentationCompositionSegment pcs)
        {
            Assert.True(pcs.Width == 0x2143);
            Assert.True(pcs.Height == 0x6587);
            Assert.True(pcs.FrameRate == 0x10);
            Assert.True(pcs.Number == 0x6543);
            Assert.True(pcs.State == CompositionState.Normal);
            Assert.True(pcs.PaletteUpdateOnly == false);
            Assert.True(pcs.PaletteUpdateId == 0xAB);
            Assert.True(pcs.CompositionObjects.Count == 0);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public async Task Normal_NoPaletteUpdateID_NoObjects_Async()
    {
        using var reader = new SegmentReader(new MemoryStream(
            PresentationCompositionSegmentData.Normal_NoPaletteUpdateID_NoObjects));
        var segment = await reader.ReadAsync() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is PresentationCompositionSegment pcs)
        {
            Assert.True(pcs.Width == 0x2143);
            Assert.True(pcs.Height == 0x6587);
            Assert.True(pcs.FrameRate == 0x10);
            Assert.True(pcs.Number == 0x6543);
            Assert.True(pcs.State == CompositionState.Normal);
            Assert.True(pcs.PaletteUpdateOnly == false);
            Assert.True(pcs.PaletteUpdateId == 0xAB);
            Assert.True(pcs.CompositionObjects.Count == 0);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public void Normal_PaletteUpdateID_NoObjects()
    {
        using var reader = new SegmentReader(new MemoryStream(
            PresentationCompositionSegmentData.Normal_PaletteUpdateID_NoObjects));
        var segment = reader.Read() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is PresentationCompositionSegment pcs)
        {
            Assert.True(pcs.Width == 0x2143);
            Assert.True(pcs.Height == 0x6587);
            Assert.True(pcs.FrameRate == 0x10);
            Assert.True(pcs.Number == 0x6543);
            Assert.True(pcs.State == CompositionState.Normal);
            Assert.True(pcs.PaletteUpdateOnly == true);
            Assert.True(pcs.PaletteUpdateId == 0xAB);
            Assert.True(pcs.CompositionObjects.Count == 0);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public void Normal_NoPaletteUpdateID_OneObjectForced()
    {
        using var reader = new SegmentReader(new MemoryStream(
            PresentationCompositionSegmentData.Normal_NoPaletteUpdateID_OneObjectForced));
        var segment = reader.Read() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is PresentationCompositionSegment pcs)
        {
            Assert.True(pcs.Width == 0x2143);
            Assert.True(pcs.Height == 0x6587);
            Assert.True(pcs.FrameRate == 0x10);
            Assert.True(pcs.Number == 0x6543);
            Assert.True(pcs.State == CompositionState.Normal);
            Assert.True(pcs.PaletteUpdateOnly == false);
            Assert.True(pcs.PaletteUpdateId == 0xAB);
            Assert.True(pcs.CompositionObjects.Count == 1);
            Assert.True(pcs.CompositionObjects[0].ObjectId == 0xABCD);
            Assert.True(pcs.CompositionObjects[0].WindowId == 0xEF);
            Assert.True(pcs.CompositionObjects[0].X == 0x1A2B);
            Assert.True(pcs.CompositionObjects[0].Y == 0x3C4D);
            Assert.True(pcs.CompositionObjects[0].Forced == true);
            Assert.True(pcs.CompositionObjects[0].Crop == null);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public void Normal_NoPaletteUpdateID_OneObjectCropped()
    {
        using var reader = new SegmentReader(new MemoryStream(
            PresentationCompositionSegmentData.Normal_NoPaletteUpdateID_OneObjectCropped));
        var segment = reader.Read() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is PresentationCompositionSegment pcs)
        {
            Assert.True(pcs.Width == 0x2143);
            Assert.True(pcs.Height == 0x6587);
            Assert.True(pcs.FrameRate == 0x10);
            Assert.True(pcs.Number == 0x6543);
            Assert.True(pcs.State == CompositionState.Normal);
            Assert.True(pcs.PaletteUpdateOnly == false);
            Assert.True(pcs.PaletteUpdateId == 0xAB);
            Assert.True(pcs.CompositionObjects.Count == 1);
            Assert.True(pcs.CompositionObjects[0].ObjectId == 0xABCD);
            Assert.True(pcs.CompositionObjects[0].WindowId == 0xEF);
            Assert.True(pcs.CompositionObjects[0].X == 0x1A2B);
            Assert.True(pcs.CompositionObjects[0].Y == 0x3C4D);
            Assert.True(pcs.CompositionObjects[0].Forced == false);
            Assert.True(pcs.CompositionObjects[0].Crop?.X == 0xA1A2);
            Assert.True(pcs.CompositionObjects[0].Crop?.Y == 0xA3A4);
            Assert.True(pcs.CompositionObjects[0].Crop?.Width == 0xA5A6);
            Assert.True(pcs.CompositionObjects[0].Crop?.Height == 0xA7A8);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public void Normal_NoPaletteUpdateID_ThreeObjectsMixed()
    {
        using var reader = new SegmentReader(new MemoryStream(
            PresentationCompositionSegmentData.Normal_NoPaletteUpdateID_ThreeObjectsMixed));
        var segment = reader.Read() ?? throw new NullReferenceException();

        Assert.True(segment.Pts == 0x01234567);
        Assert.True(segment.Dts == 0x12345678);

        if (segment is PresentationCompositionSegment pcs)
        {
            Assert.True(pcs.Width == 0x2143);
            Assert.True(pcs.Height == 0x6587);
            Assert.True(pcs.FrameRate == 0x10);
            Assert.True(pcs.Number == 0x6543);
            Assert.True(pcs.State == CompositionState.Normal);
            Assert.True(pcs.PaletteUpdateOnly == false);
            Assert.True(pcs.PaletteUpdateId == 0xAB);
            Assert.True(pcs.CompositionObjects.Count == 3);
            Assert.True(pcs.CompositionObjects[0].ObjectId == 0xABCD);
            Assert.True(pcs.CompositionObjects[0].WindowId == 0xEF);
            Assert.True(pcs.CompositionObjects[0].X == 0x1A2B);
            Assert.True(pcs.CompositionObjects[0].Y == 0x3C4D);
            Assert.True(pcs.CompositionObjects[0].Forced == false);
            Assert.True(pcs.CompositionObjects[0].Crop?.X == 0xA1A2);
            Assert.True(pcs.CompositionObjects[0].Crop?.Y == 0xA3A4);
            Assert.True(pcs.CompositionObjects[0].Crop?.Width == 0xA5A6);
            Assert.True(pcs.CompositionObjects[0].Crop?.Height == 0xA7A8);
            Assert.True(pcs.CompositionObjects[1].ObjectId == 0xABCD);
            Assert.True(pcs.CompositionObjects[1].WindowId == 0xEF);
            Assert.True(pcs.CompositionObjects[1].X == 0x1A2B);
            Assert.True(pcs.CompositionObjects[1].Y == 0x3C4D);
            Assert.True(pcs.CompositionObjects[1].Forced == true);
            Assert.True(pcs.CompositionObjects[1].Crop == null);
            Assert.True(pcs.CompositionObjects[2].ObjectId == 0xABCD);
            Assert.True(pcs.CompositionObjects[2].WindowId == 0xEF);
            Assert.True(pcs.CompositionObjects[2].X == 0x1A2B);
            Assert.True(pcs.CompositionObjects[2].Y == 0x3C4D);
            Assert.True(pcs.CompositionObjects[2].Forced == true);
            Assert.True(pcs.CompositionObjects[2].Crop?.X == 0xA1A2);
            Assert.True(pcs.CompositionObjects[2].Crop?.Y == 0xA3A4);
            Assert.True(pcs.CompositionObjects[2].Crop?.Width == 0xA5A6);
            Assert.True(pcs.CompositionObjects[2].Crop?.Height == 0xA7A8);
        }
        else
        {
            Assert.True(false);
        }
    }
}