﻿/*
 * Copyright 2024 William Swartzendruber
 *
 * To the extent possible under law, the person who associated CC0 with this file has waived all
 * copyright and related or neighboring rights to this file.
 *
 * You should have received a copy of the CC0 legalcode along with this work. If not, see
 * <http://creativecommons.org/publicdomain/zero/1.0/>.
 *
 * SPDX-License-Identifier: CC0-1.0
 */

using PGS4NET;
using PGS4NET.Captions;
using PGS4NET.DisplaySets;

namespace PGS4NET.Tests;

public class CompositorTests
{
    private static readonly PgsPixel Pixel1 = new PgsPixel(0x01, 0x01, 0x01, 0x01);
    private static readonly PgsPixel Pixel2 = new PgsPixel(0x02, 0x02, 0x02, 0x02);
    private static readonly PgsPixel Pixel3 = new PgsPixel(0x03, 0x03, 0x03, 0x03);
    private static readonly PgsPixel Pixel4 = new PgsPixel(0x04, 0x04, 0x04, 0x04);
    private static readonly PgsPixel Pixel5 = new PgsPixel(0x05, 0x05, 0x05, 0x05);
    private static readonly PgsTimeStamp TimeStamp1 = new PgsTimeStamp(8);
    private static readonly PgsTimeStamp TimeStamp2 = new PgsTimeStamp(16);
    private static readonly PgsTimeStamp TimeStamp3 = new PgsTimeStamp(24);
    private static readonly PgsTimeStamp TimeStamp4 = new PgsTimeStamp(32);
    private static readonly PgsTimeStamp TimeStamp5 = new PgsTimeStamp(40);
    private static readonly PgsTimeStamp TimeStamp6 = new PgsTimeStamp(48);

    [Fact]
    public void Empty()
    {
        var window = new DisplayWindow(16, 16, 16, 16);
        var compositor = new Compositor(window);
        var captions = new List<Caption>();

        compositor.NewCaptionReady += (sender, caption) =>
        {
            Assert.True(sender == compositor);

            captions.Add(caption);
        };

        compositor.Clear(default);

        Assert.True(captions.Count == 0);
    }

    [Fact]
    public void FullWindowNoCrop()
    {
        var entries = new Dictionary<byte, PgsPixel>
        {
            { 0, default },
            { 1, Pixel1 },
            { 2, Pixel2 },
            { 3, Pixel3 },
            { 4, Pixel4 },
        };
        var data1 = new byte[]
        {
            4, 0, 0, 3,
            0, 1, 2, 0,
            0, 3, 4, 0,
            2, 0, 0, 1,
        };
        var palette = new DisplayPalette(entries);
        var object1 = new DisplayObject(4, 4, data1);
        var window = new DisplayWindow(8, 16, 4, 4);
        var composition = new DisplayComposition(8, 16, false, null);
        var compositor = new Compositor(window);
        var captions = new List<Caption>();
        var expectedPixels1 = new PgsPixel[]
        {
            Pixel4, default, default, Pixel3,
            default, Pixel1, Pixel2, default,
            default, Pixel3, Pixel4, default,
            Pixel2, default, default, Pixel1,
        };

        compositor.NewCaptionReady += (sender, caption) =>
        {
            Assert.True(sender == compositor);

            captions.Add(caption);
        };

        compositor.Draw(TimeStamp1, object1, composition, palette);
        compositor.Clear(TimeStamp2);

        Assert.True(captions.Count == 1);
        Assert.True(captions[0].TimeStamp == TimeStamp1);
        Assert.True(captions[0].Duration == TimeStamp2 - TimeStamp1);
        Assert.True(captions[0].X == 8);
        Assert.True(captions[0].Y == 16);
        Assert.True(captions[0].Width == 4);
        Assert.True(captions[0].Height == 4);
        Assert.True(captions[0].Forced == false);
        Assert.True(captions[0].Data.SequenceEqual(expectedPixels1));
    }

    [Fact]
    public void FullWindowCrop()
    {
        var entries = new Dictionary<byte, PgsPixel>
        {
            { 0, default },
            { 1, Pixel1 },
            { 2, Pixel2 },
            { 3, Pixel3 },
            { 4, Pixel4 },
            { 5, Pixel5 },
        };
        var data1 = new byte[]
        {
            5, 5, 5, 5,
            5, 1, 2, 5,
            5, 3, 4, 5,
            5, 5, 5, 5,
        };
        var palette = new DisplayPalette(entries);
        var object1 = new DisplayObject(4, 4, data1);
        var window = new DisplayWindow(8, 16, 2, 2);
        var composition = new DisplayComposition(8, 16, false, new Crop(1, 1, 2, 2));
        var compositor = new Compositor(window);
        var captions = new List<Caption>();
        var expectedPixels1 = new PgsPixel[]
        {
            Pixel1, Pixel2,
            Pixel3, Pixel4,
        };

        compositor.NewCaptionReady += (sender, caption) =>
        {
            Assert.True(sender == compositor);

            captions.Add(caption);
        };

        compositor.Draw(TimeStamp1, object1, composition, palette);
        compositor.Clear(TimeStamp2);

        Assert.True(captions.Count == 1);
        Assert.True(captions[0].TimeStamp == TimeStamp1);
        Assert.True(captions[0].Duration == TimeStamp2 - TimeStamp1);
        Assert.True(captions[0].X == 8);
        Assert.True(captions[0].Y == 16);
        Assert.True(captions[0].Width == 2);
        Assert.True(captions[0].Height == 2);
        Assert.True(captions[0].Forced == false);
        Assert.True(captions[0].Data.SequenceEqual(expectedPixels1));
    }

    [Fact]
    public void PartialWindowNoCrop()
    {
        var entries = new Dictionary<byte, PgsPixel>
        {
            { 0, default },
            { 1, Pixel1 },
            { 2, Pixel2 },
            { 3, Pixel3 },
            { 4, Pixel4 },
            { 5, Pixel5 },
        };
        var data1 = new byte[]
        {
            5, 5, 5, 5,
            5, 1, 2, 5,
            5, 3, 4, 5,
            5, 5, 5, 5,
        };
        var palette = new DisplayPalette(entries);
        var object1 = new DisplayObject(4, 4, data1);
        var window = new DisplayWindow(8, 16, 6, 6);
        var composition = new DisplayComposition(9, 17, false, null);
        var compositor = new Compositor(window);
        var captions = new List<Caption>();
        var expectedPixels1 = new PgsPixel[]
        {
            default, default, default, default, default, default,
            default, Pixel5, Pixel5, Pixel5, Pixel5, default,
            default, Pixel5, Pixel1, Pixel2, Pixel5, default,
            default, Pixel5, Pixel3, Pixel4, Pixel5, default,
            default, Pixel5, Pixel5, Pixel5, Pixel5, default,
            default, default, default, default, default, default,
        };

        compositor.NewCaptionReady += (sender, caption) =>
        {
            Assert.True(sender == compositor);

            captions.Add(caption);
        };

        compositor.Draw(TimeStamp1, object1, composition, palette);
        compositor.Clear(TimeStamp2);

        Assert.True(captions.Count == 1);
        Assert.True(captions[0].TimeStamp == TimeStamp1);
        Assert.True(captions[0].Duration == TimeStamp2 - TimeStamp1);
        Assert.True(captions[0].X == 8);
        Assert.True(captions[0].Y == 16);
        Assert.True(captions[0].Width == 6);
        Assert.True(captions[0].Height == 6);
        Assert.True(captions[0].Forced == false);
        Assert.True(captions[0].Data.SequenceEqual(expectedPixels1));
    }

    [Fact]
    public void PartialWindowCrop()
    {
        var entries = new Dictionary<byte, PgsPixel>
        {
            { 0, default },
            { 1, Pixel1 },
            { 2, Pixel2 },
            { 3, Pixel3 },
            { 4, Pixel4 },
            { 5, Pixel5 },
        };
        var data1 = new byte[]
        {
            5, 5, 5, 5,
            5, 1, 2, 5,
            5, 3, 4, 5,
            5, 5, 5, 5,
        };
        var palette = new DisplayPalette(entries);
        var object1 = new DisplayObject(4, 4, data1);
        var window = new DisplayWindow(8, 16, 4, 4);
        var composition = new DisplayComposition(9, 17, false, new Crop(1, 1, 2, 2));
        var compositor = new Compositor(window);
        var captions = new List<Caption>();
        var expectedPixels1 = new PgsPixel[]
        {
            default, default, default, default,
            default, Pixel1, Pixel2, default,
            default, Pixel3, Pixel4, default,
            default, default, default, default,
        };

        compositor.NewCaptionReady += (sender, caption) =>
        {
            Assert.True(sender == compositor);

            captions.Add(caption);
        };

        compositor.Draw(TimeStamp1, object1, composition, palette);
        compositor.Clear(TimeStamp2);

        Assert.True(captions.Count == 1);
        Assert.True(captions[0].TimeStamp == TimeStamp1);
        Assert.True(captions[0].Duration == TimeStamp2 - TimeStamp1);
        Assert.True(captions[0].X == 8);
        Assert.True(captions[0].Y == 16);
        Assert.True(captions[0].Width == 4);
        Assert.True(captions[0].Height == 4);
        Assert.True(captions[0].Forced == false);
        Assert.True(captions[0].Data.SequenceEqual(expectedPixels1));
    }

    [Fact]
    public void OutsideWindowNoCrop()
    {
        var entries = new Dictionary<byte, PgsPixel>
        {
            { 0, default },
            { 1, Pixel1 },
            { 2, Pixel2 },
            { 3, Pixel3 },
            { 4, Pixel4 },
            { 5, Pixel5 },
        };
        var data1 = new byte[]
        {
            5, 5, 5, 5, 5, 5,
            5, 1, 2, 3, 4, 5,
            5, 2, 3, 4, 1, 5,
            5, 3, 4, 1, 2, 5,
            5, 4, 1, 2, 3, 5,
            5, 5, 5, 5, 5, 5,
        };
        var palette = new DisplayPalette(entries);
        var object1 = new DisplayObject(6, 6, data1);
        var window = new DisplayWindow(8, 16, 4, 4);
        var composition = new DisplayComposition(7, 15, false, null);
        var compositor = new Compositor(window);
        var captions = new List<Caption>();
        var expectedPixels1 = new PgsPixel[]
        {
            Pixel1, Pixel2, Pixel3, Pixel4,
            Pixel2, Pixel3, Pixel4, Pixel1,
            Pixel3, Pixel4, Pixel1, Pixel2,
            Pixel4, Pixel1, Pixel2, Pixel3,
        };

        compositor.NewCaptionReady += (sender, caption) =>
        {
            Assert.True(sender == compositor);

            captions.Add(caption);
        };

        compositor.Draw(TimeStamp1, object1, composition, palette);
        compositor.Clear(TimeStamp2);

        Assert.True(captions.Count == 1);
        Assert.True(captions[0].TimeStamp == TimeStamp1);
        Assert.True(captions[0].Duration == TimeStamp2 - TimeStamp1);
        Assert.True(captions[0].X == 8);
        Assert.True(captions[0].Y == 16);
        Assert.True(captions[0].Width == 4);
        Assert.True(captions[0].Height == 4);
        Assert.True(captions[0].Forced == false);
        Assert.True(captions[0].Data.SequenceEqual(expectedPixels1));
    }

    [Fact]
    public void OutsideWindowCrop()
    {
        var entries = new Dictionary<byte, PgsPixel>
        {
            { 0, default },
            { 1, Pixel1 },
            { 2, Pixel2 },
            { 3, Pixel3 },
            { 4, Pixel4 },
            { 5, Pixel5 },
        };
        var data1 = new byte[]
        {
            5, 5, 5, 5, 5, 5,
            5, 1, 2, 3, 4, 5,
            5, 2, 3, 4, 1, 5,
            5, 3, 4, 1, 2, 5,
            5, 4, 1, 2, 3, 5,
            5, 5, 5, 5, 5, 5,
        };
        var palette = new DisplayPalette(entries);
        var object1 = new DisplayObject(6, 6, data1);
        var window = new DisplayWindow(8, 16, 2, 2);
        var composition = new DisplayComposition(7, 15, false, new Crop(1, 1, 4, 4));
        var compositor = new Compositor(window);
        var captions = new List<Caption>();
        var expectedPixels1 = new PgsPixel[]
        {
            Pixel3, Pixel4,
            Pixel4, Pixel1,
        };

        compositor.NewCaptionReady += (sender, caption) =>
        {
            Assert.True(sender == compositor);

            captions.Add(caption);
        };

        compositor.Draw(TimeStamp1, object1, composition, palette);
        compositor.Clear(TimeStamp2);

        Assert.True(captions.Count == 1);
        Assert.True(captions[0].TimeStamp == TimeStamp1);
        Assert.True(captions[0].Duration == TimeStamp2 - TimeStamp1);
        Assert.True(captions[0].X == 8);
        Assert.True(captions[0].Y == 16);
        Assert.True(captions[0].Width == 2);
        Assert.True(captions[0].Height == 2);
        Assert.True(captions[0].Forced == false);
        Assert.True(captions[0].Data.SequenceEqual(expectedPixels1));
    }

    [Fact]
    public void NoOverlapNoCrop()
    {
        var entries = new Dictionary<byte, PgsPixel>
        {
            { 0, default },
            { 1, Pixel1 },
            { 2, Pixel2 },
            { 3, Pixel3 },
            { 4, Pixel4 },
            { 5, Pixel5 },
        };
        var data1 = new byte[]
        {
            5, 5, 5, 5, 5, 5,
            5, 1, 2, 3, 4, 5,
            5, 2, 3, 4, 1, 5,
            5, 3, 4, 1, 2, 5,
            5, 4, 1, 2, 3, 5,
            5, 5, 5, 5, 5, 5,
        };
        var palette = new DisplayPalette(entries);
        var object1 = new DisplayObject(6, 6, data1);
        var window = new DisplayWindow(8, 16, 4, 4);
        var composition = new DisplayComposition(12, 15, false, null);
        var compositor = new Compositor(window);
        var captions = new List<Caption>();

        compositor.NewCaptionReady += (sender, caption) =>
        {
            Assert.True(sender == compositor);

            captions.Add(caption);
        };

        compositor.Draw(TimeStamp1, object1, composition, palette);
        compositor.Clear(TimeStamp2);

        Assert.True(captions.Count == 0);
    }

    [Fact]
    public void NoOverlapCrop()
    {
        var entries = new Dictionary<byte, PgsPixel>
        {
            { 0, default },
            { 1, Pixel1 },
            { 2, Pixel2 },
            { 3, Pixel3 },
            { 4, Pixel4 },
            { 5, Pixel5 },
        };
        var data1 = new byte[]
        {
            5, 5, 5, 5, 5, 5,
            5, 1, 2, 3, 4, 5,
            5, 2, 3, 4, 1, 5,
            5, 3, 4, 1, 2, 5,
            5, 4, 1, 2, 3, 5,
            5, 5, 5, 5, 5, 5,
        };
        var palette = new DisplayPalette(entries);
        var object1 = new DisplayObject(6, 6, data1);
        var window = new DisplayWindow(8, 16, 2, 2);
        var composition = new DisplayComposition(10, 15, false, new Crop(1, 1, 4, 4));
        var compositor = new Compositor(window);
        var captions = new List<Caption>();

        compositor.NewCaptionReady += (sender, caption) =>
        {
            Assert.True(sender == compositor);

            captions.Add(caption);
        };

        compositor.Draw(TimeStamp1, object1, composition, palette);
        compositor.Clear(TimeStamp2);

        Assert.True(captions.Count == 0);
    }

    [Fact]
    public void Sequence1()
    {
        var entries = new Dictionary<byte, PgsPixel>
        {
            { 0, default },
            { 1, Pixel1 },
            { 2, Pixel2 },
            { 3, Pixel3 },
            { 4, Pixel4 },
        };
        var data1 = new byte[]
        {
            1, 1,
            1, 1,
        };
        var data2 = new byte[]
        {
            2, 2,
            2, 2,
        };
        var data3 = new byte[]
        {
            0, 0,
            0, 0,
        };
        var data4 = new byte[]
        {
            4, 4,
            4, 4,
        };
        var palette = new DisplayPalette(entries);
        var object1 = new DisplayObject(2, 2, data1);
        var object2 = new DisplayObject(2, 2, data2);
        var object3 = new DisplayObject(2, 2, data3);
        var object4 = new DisplayObject(2, 2, data4);
        var window = new DisplayWindow(0, 0, 2, 2);
        var composition = new DisplayComposition(0, 0, false, null);
        var compositor = new Compositor(window);
        var captions = new List<Caption>();
        var expectedPixels1 = new PgsPixel[]
        {
            Pixel1, Pixel1,
            Pixel1, Pixel1,
        };
        var expectedPixels2 = new PgsPixel[]
        {
            Pixel2, Pixel2,
            Pixel2, Pixel2,
        };
        var expectedPixels4 = new PgsPixel[]
        {
            Pixel4, Pixel4,
            Pixel4, Pixel4,
        };

        compositor.NewCaptionReady += (sender, caption) =>
        {
            Assert.True(sender == compositor);

            captions.Add(caption);
        };

        compositor.Draw(TimeStamp1, object1, composition, palette);
        compositor.Draw(TimeStamp2, object2, composition, palette);
        compositor.Clear(TimeStamp3);
        compositor.Draw(TimeStamp4, object3, composition, palette);
        compositor.Draw(TimeStamp5, object4, composition, palette);
        compositor.Clear(TimeStamp6);

        Assert.True(captions.Count == 3);
        Assert.True(captions[0].TimeStamp == TimeStamp1);
        Assert.True(captions[0].Duration == TimeStamp2 - TimeStamp1);
        Assert.True(captions[0].X == 0);
        Assert.True(captions[0].Y == 0);
        Assert.True(captions[0].Width == 2);
        Assert.True(captions[0].Height == 2);
        Assert.True(captions[0].Forced == false);
        Assert.True(captions[0].Data.SequenceEqual(expectedPixels1));
        Assert.True(captions[1].TimeStamp == TimeStamp2);
        Assert.True(captions[1].Duration == TimeStamp3 - TimeStamp2);
        Assert.True(captions[1].X == 0);
        Assert.True(captions[1].Y == 0);
        Assert.True(captions[1].Width == 2);
        Assert.True(captions[1].Height == 2);
        Assert.True(captions[1].Forced == false);
        Assert.True(captions[1].Data.SequenceEqual(expectedPixels2));
        Assert.True(captions[2].TimeStamp == TimeStamp5);
        Assert.True(captions[2].Duration == TimeStamp6 - TimeStamp5);
        Assert.True(captions[2].X == 0);
        Assert.True(captions[2].Y == 0);
        Assert.True(captions[2].Width == 2);
        Assert.True(captions[2].Height == 2);
        Assert.True(captions[2].Forced == false);
        Assert.True(captions[2].Data.SequenceEqual(expectedPixels4));
    }
}
