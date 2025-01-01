/*
 * Copyright 2025 William Swartzendruber
 *
 * To the extent possible under law, the person who associated CC0 with this file has waived all
 * copyright and related or neighboring rights to this file.
 *
 * You should have received a copy of the CC0 legalcode along with this work. If not, see
 * <http://creativecommons.org/publicdomain/zero/1.0/>.
 *
 * SPDX-License-Identifier: CC0-1.0
 */

using PGS4NET.DisplaySets;
using PGS4NET.Captions;

namespace PGS4NET.Tests;

public class CompositorTests
{
    private static readonly YcbcraPixel Pixel1 = new YcbcraPixel(0x01, 0x01, 0x01, 0x01);
    private static readonly YcbcraPixel Pixel2 = new YcbcraPixel(0x02, 0x02, 0x02, 0x02);
    private static readonly YcbcraPixel Pixel3 = new YcbcraPixel(0x03, 0x03, 0x03, 0x03);
    private static readonly YcbcraPixel Pixel4 = new YcbcraPixel(0x04, 0x04, 0x04, 0x04);
    private static readonly YcbcraPixel Pixel5 = new YcbcraPixel(0x05, 0x05, 0x05, 0x05);
    private static readonly PgsTimeStamp TimeStamp1 = new PgsTimeStamp(8);
    private static readonly PgsTimeStamp TimeStamp2 = new PgsTimeStamp(16);
    private static readonly PgsTimeStamp TimeStamp3 = new PgsTimeStamp(24);
    private static readonly PgsTimeStamp TimeStamp4 = new PgsTimeStamp(32);
    private static readonly PgsTimeStamp TimeStamp5 = new PgsTimeStamp(40);
    private static readonly PgsTimeStamp TimeStamp6 = new PgsTimeStamp(48);
    private static readonly PgsTimeStamp TimeStamp7 = new PgsTimeStamp(56);
    private static readonly PgsTimeStamp TimeStamp8 = new PgsTimeStamp(64);
    private static readonly PgsTimeStamp TimeStamp9 = new PgsTimeStamp(72);

    [Fact]
    public void Empty()
    {
        var window = new DisplayWindow(16, 16, 16, 16);
        var compositor = new Compositor(window);
        var captions = new List<Caption>();

        compositor.Ready += (sender, caption) =>
        {
            Assert.Equal(compositor, sender);

            captions.Add(caption);
        };

        compositor.Flush(TimeStamp1);
        Assert.Empty(captions);
    }

    [Fact]
    public void FullWindowNoCrop()
    {
        var entries = new Dictionary<byte, YcbcraPixel>
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
            0, 1, 4, 0,
            2, 0, 0, 1,
        };
        var palette = new DisplayPalette(entries);
        var object1 = new DisplayObject(4, 5, data1);
        var window = new DisplayWindow(8, 16, 4, 5);
        var composition = new DisplayComposition(8, 16, false, null);
        var compositor = new Compositor(window);
        var captions = new List<Caption>();
        var expectedPixels1 = new YcbcraPixel[]
        {
            Pixel4, default, default, Pixel3,
            default, Pixel1, Pixel2, default,
            default, Pixel3, Pixel4, default,
            default, Pixel1, Pixel4, default,
            Pixel2, default, default, Pixel1,
        };

        compositor.Ready += (sender, caption) =>
        {
            Assert.Equal(compositor, sender);

            captions.Add(caption);
        };

        compositor.Draw(TimeStamp1, new CompositorComposition(composition, object1, palette));
        Assert.True(compositor.Pending);
        compositor.Flush(TimeStamp2);
        Assert.False(compositor.Pending);

        Assert.Single(captions);
        Assert.Equal(TimeStamp1, captions[0].TimeStamp);
        Assert.Equal(TimeStamp2 - TimeStamp1, captions[0].Duration);
        Assert.Equal(8, captions[0].X);
        Assert.Equal(16, captions[0].Y);
        Assert.Equal(4, captions[0].Width);
        Assert.Equal(5, captions[0].Height);
        Assert.False(captions[0].Forced);
        Assert.Equal(expectedPixels1, captions[0].Data);
    }

    [Fact]
    public void FullWindowCrop()
    {
        var entries = new Dictionary<byte, YcbcraPixel>
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
            5, 1, 4, 5,
            5, 5, 5, 5,
        };
        var palette = new DisplayPalette(entries);
        var object1 = new DisplayObject(4, 5, data1);
        var window = new DisplayWindow(8, 16, 2, 3);
        var composition = new DisplayComposition(8, 16, false, new Crop(1, 1, 2, 3));
        var compositor = new Compositor(window);
        var captions = new List<Caption>();
        var expectedPixels1 = new YcbcraPixel[]
        {
            Pixel1, Pixel2,
            Pixel3, Pixel4,
            Pixel1, Pixel4,
        };

        compositor.Ready += (sender, caption) =>
        {
            Assert.Equal(compositor, sender);

            captions.Add(caption);
        };

        compositor.Draw(TimeStamp1, new CompositorComposition(composition, object1, palette));
        Assert.True(compositor.Pending);
        compositor.Flush(TimeStamp2);
        Assert.False(compositor.Pending);

        Assert.Single(captions);
        Assert.Equal(TimeStamp1, captions[0].TimeStamp);
        Assert.Equal(TimeStamp2 - TimeStamp1, captions[0].Duration);
        Assert.Equal(8, captions[0].X);
        Assert.Equal(16, captions[0].Y);
        Assert.Equal(2, captions[0].Width);
        Assert.Equal(3, captions[0].Height);
        Assert.False(captions[0].Forced);
        Assert.Equal(expectedPixels1, captions[0].Data);
    }

    [Fact]
    public void PartialWindowNoCrop()
    {
        var entries = new Dictionary<byte, YcbcraPixel>
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
            5, 1, 4, 5,
            5, 5, 5, 5,
        };
        var palette = new DisplayPalette(entries);
        var object1 = new DisplayObject(4, 5, data1);
        var window = new DisplayWindow(8, 16, 6, 7);
        var composition = new DisplayComposition(9, 17, false, null);
        var compositor = new Compositor(window);
        var captions = new List<Caption>();
        var expectedPixels1 = new YcbcraPixel[]
        {
            default, default, default, default, default, default,
            default, Pixel5, Pixel5, Pixel5, Pixel5, default,
            default, Pixel5, Pixel1, Pixel2, Pixel5, default,
            default, Pixel5, Pixel3, Pixel4, Pixel5, default,
            default, Pixel5, Pixel1, Pixel4, Pixel5, default,
            default, Pixel5, Pixel5, Pixel5, Pixel5, default,
            default, default, default, default, default, default,
        };

        compositor.Ready += (sender, caption) =>
        {
            Assert.Equal(compositor, sender);

            captions.Add(caption);
        };

        compositor.Draw(TimeStamp1, new CompositorComposition(composition, object1, palette));
        Assert.True(compositor.Pending);
        compositor.Flush(TimeStamp2);
        Assert.False(compositor.Pending);

        Assert.Single(captions);
        Assert.Equal(TimeStamp1, captions[0].TimeStamp);
        Assert.Equal(TimeStamp2 - TimeStamp1, captions[0].Duration);
        Assert.Equal(8, captions[0].X);
        Assert.Equal(16, captions[0].Y);
        Assert.Equal(6, captions[0].Width);
        Assert.Equal(7, captions[0].Height);
        Assert.False(captions[0].Forced);
        Assert.Equal(expectedPixels1, captions[0].Data);
    }

    [Fact]
    public void PartialWindowCrop()
    {
        var entries = new Dictionary<byte, YcbcraPixel>
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
            5, 1, 4, 5,
            5, 5, 5, 5,
        };
        var palette = new DisplayPalette(entries);
        var object1 = new DisplayObject(4, 5, data1);
        var window = new DisplayWindow(8, 16, 4, 5);
        var composition = new DisplayComposition(9, 17, false, new Crop(1, 1, 2, 3));
        var compositor = new Compositor(window);
        var captions = new List<Caption>();
        var expectedPixels1 = new YcbcraPixel[]
        {
            default, default, default, default,
            default, Pixel1, Pixel2, default,
            default, Pixel3, Pixel4, default,
            default, Pixel1, Pixel4, default,
            default, default, default, default,
        };

        compositor.Ready += (sender, caption) =>
        {
            Assert.Equal(compositor, sender);

            captions.Add(caption);
        };

        compositor.Draw(TimeStamp1, new CompositorComposition(composition, object1, palette));
        Assert.True(compositor.Pending);
        compositor.Flush(TimeStamp2);
        Assert.False(compositor.Pending);

        Assert.Single(captions);
        Assert.Equal(TimeStamp1, captions[0].TimeStamp);
        Assert.Equal(TimeStamp2 - TimeStamp1, captions[0].Duration);
        Assert.Equal(8, captions[0].X);
        Assert.Equal(16, captions[0].Y);
        Assert.Equal(4, captions[0].Width);
        Assert.Equal(5, captions[0].Height);
        Assert.False(captions[0].Forced);
        Assert.Equal(expectedPixels1, captions[0].Data);
    }

    [Fact]
    public void OutsideWindowNoCrop()
    {
        var entries = new Dictionary<byte, YcbcraPixel>
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
            5, 3, 0, 1, 2, 5,
            5, 5, 5, 5, 5, 5,
        };
        var palette = new DisplayPalette(entries);
        var object1 = new DisplayObject(6, 7, data1);
        var window = new DisplayWindow(8, 16, 4, 5);
        var composition = new DisplayComposition(7, 15, false, null);
        var compositor = new Compositor(window);
        var captions = new List<Caption>();
        var expectedPixels1 = new YcbcraPixel[]
        {
            Pixel1, Pixel2, Pixel3, Pixel4,
            Pixel2, Pixel3, Pixel4, Pixel1,
            Pixel3, Pixel4, Pixel1, Pixel2,
            Pixel4, Pixel1, Pixel2, Pixel3,
            Pixel3, default, Pixel1, Pixel2,
        };

        compositor.Ready += (sender, caption) =>
        {
            Assert.Equal(compositor, sender);

            captions.Add(caption);
        };

        compositor.Draw(TimeStamp1, new CompositorComposition(composition, object1, palette));
        Assert.True(compositor.Pending);
        compositor.Flush(TimeStamp2);
        Assert.False(compositor.Pending);

        Assert.Single(captions);
        Assert.Equal(TimeStamp1, captions[0].TimeStamp);
        Assert.Equal(TimeStamp2 - TimeStamp1, captions[0].Duration);
        Assert.Equal(8, captions[0].X);
        Assert.Equal(16, captions[0].Y);
        Assert.Equal(4, captions[0].Width);
        Assert.Equal(5, captions[0].Height);
        Assert.False(captions[0].Forced);
        Assert.Equal(expectedPixels1, captions[0].Data);
    }

    [Fact]
    public void OutsideWindowCrop()
    {
        var entries = new Dictionary<byte, YcbcraPixel>
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
            5, 3, 0, 1, 2, 5,
            5, 5, 5, 5, 5, 5,
        };
        var palette = new DisplayPalette(entries);
        var object1 = new DisplayObject(6, 7, data1);
        var window = new DisplayWindow(8, 16, 2, 3);
        var composition = new DisplayComposition(7, 15, false, new Crop(1, 1, 4, 5));
        var compositor = new Compositor(window);
        var captions = new List<Caption>();
        var expectedPixels1 = new YcbcraPixel[]
        {
            Pixel3, Pixel4,
            Pixel4, Pixel1,
            Pixel1, Pixel2,
        };

        compositor.Ready += (sender, caption) =>
        {
            Assert.Equal(compositor, sender);

            captions.Add(caption);
        };

        compositor.Draw(TimeStamp1, new CompositorComposition(composition, object1, palette));
        Assert.True(compositor.Pending);
        compositor.Flush(TimeStamp2);
        Assert.False(compositor.Pending);

        Assert.Single(captions);
        Assert.Equal(TimeStamp1, captions[0].TimeStamp);
        Assert.Equal(TimeStamp2 - TimeStamp1, captions[0].Duration);
        Assert.Equal(8, captions[0].X);
        Assert.Equal(16, captions[0].Y);
        Assert.Equal(2, captions[0].Width);
        Assert.Equal(3, captions[0].Height);
        Assert.False(captions[0].Forced);
        Assert.Equal(expectedPixels1, captions[0].Data);
    }

    [Fact]
    public void NoOverlapNoCrop()
    {
        var entries = new Dictionary<byte, YcbcraPixel>
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

        compositor.Ready += (sender, caption) =>
        {
            Assert.Equal(compositor, sender);

            captions.Add(caption);
        };

        compositor.Draw(TimeStamp1, new CompositorComposition(composition, object1, palette));
        Assert.False(compositor.Pending);
        compositor.Flush(TimeStamp2);
        Assert.False(compositor.Pending);

        Assert.Empty(captions);
    }

    [Fact]
    public void NoOverlapCrop()
    {
        var entries = new Dictionary<byte, YcbcraPixel>
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

        compositor.Ready += (sender, caption) =>
        {
            Assert.Equal(compositor, sender);

            captions.Add(caption);
        };

        compositor.Draw(TimeStamp1, new CompositorComposition(composition, object1, palette));
        Assert.False(compositor.Pending);
        compositor.Flush(TimeStamp2);
        Assert.False(compositor.Pending);

        Assert.Empty(captions);
    }

    [Fact]
    public void ChangeDetection()
    {
        var entries = new Dictionary<byte, YcbcraPixel>
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
        var expectedPixels1 = new YcbcraPixel[]
        {
            Pixel1, Pixel1,
            Pixel1, Pixel1,
        };
        var expectedPixels2 = new YcbcraPixel[]
        {
            Pixel2, Pixel2,
            Pixel2, Pixel2,
        };
        var expectedPixels4 = new YcbcraPixel[]
        {
            Pixel4, Pixel4,
            Pixel4, Pixel4,
        };

        compositor.Ready += (sender, caption) =>
        {
            Assert.Equal(compositor, sender);

            captions.Add(caption);
        };

        compositor.Draw(TimeStamp1, new CompositorComposition(composition, object1, palette));
        Assert.True(compositor.Pending);
        compositor.Draw(TimeStamp2, new CompositorComposition(composition, object2, palette));
        Assert.True(compositor.Pending);
        compositor.Flush(TimeStamp3);
        Assert.False(compositor.Pending);
        compositor.Draw(TimeStamp4, new CompositorComposition(composition, object3, palette));
        Assert.False(compositor.Pending);
        compositor.Draw(TimeStamp5, new CompositorComposition(composition, object4, palette));
        Assert.True(compositor.Pending);
        compositor.Flush(TimeStamp6);
        Assert.False(compositor.Pending);

        Assert.Equal(3, captions.Count);
        Assert.Equal(TimeStamp1, captions[0].TimeStamp);
        Assert.Equal(TimeStamp2 - TimeStamp1, captions[0].Duration);
        Assert.Equal(0, captions[0].X);
        Assert.Equal(0, captions[0].Y);
        Assert.Equal(2, captions[0].Width);
        Assert.Equal(2, captions[0].Height);
        Assert.False(captions[0].Forced);
        Assert.Equal(expectedPixels1, captions[0].Data);
        Assert.Equal(TimeStamp2, captions[1].TimeStamp);
        Assert.Equal(TimeStamp3 - TimeStamp2, captions[1].Duration);
        Assert.Equal(0, captions[1].X);
        Assert.Equal(0, captions[1].Y);
        Assert.Equal(2, captions[1].Width);
        Assert.Equal(2, captions[1].Height);
        Assert.False(captions[1].Forced);
        Assert.Equal(expectedPixels2, captions[1].Data);
        Assert.Equal(TimeStamp5, captions[2].TimeStamp);
        Assert.Equal(TimeStamp6 - TimeStamp5, captions[2].Duration);
        Assert.Equal(0, captions[2].X);
        Assert.Equal(0, captions[2].Y);
        Assert.Equal(2, captions[2].Width);
        Assert.Equal(2, captions[2].Height);
        Assert.False(captions[2].Forced);
        Assert.Equal(expectedPixels4, captions[2].Data);
    }

    [Fact]
    public void AlternatingForcedFlag()
    {
        var entries = new Dictionary<byte, YcbcraPixel>
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
            1, 1,
            1, 1,
        };
        var data3 = new byte[]
        {
            1, 1,
            1, 1,
        };
        var data4 = new byte[]
        {
            0, 0,
            0, 0,
        };
        var palette = new DisplayPalette(entries);
        var object1 = new DisplayObject(2, 2, data1);
        var object2 = new DisplayObject(2, 2, data2);
        var object3 = new DisplayObject(2, 2, data3);
        var object4 = new DisplayObject(2, 2, data4);
        var window = new DisplayWindow(0, 0, 2, 2);
        var composition1 = new DisplayComposition(0, 0, false, null);
        var composition2 = new DisplayComposition(0, 0, true, null);
        var compositor = new Compositor(window);
        var captions = new List<Caption>();
        var expectedPixels1 = new YcbcraPixel[]
        {
            Pixel1, Pixel1,
            Pixel1, Pixel1,
        };
        var expectedPixels2 = new YcbcraPixel[]
        {
            Pixel1, Pixel1,
            Pixel1, Pixel1,
        };
        var expectedPixels3 = new YcbcraPixel[]
        {
            Pixel1, Pixel1,
            Pixel1, Pixel1,
        };

        compositor.Ready += (sender, caption) =>
        {
            Assert.Equal(compositor, sender);

            captions.Add(caption);
        };

        compositor.Draw(TimeStamp1, new CompositorComposition(composition1, object1, palette));
        Assert.True(compositor.Pending);
        compositor.Draw(TimeStamp2, new CompositorComposition(composition2, object2, palette));
        Assert.True(compositor.Pending);
        compositor.Draw(TimeStamp3, new CompositorComposition(composition1, object3, palette));
        Assert.True(compositor.Pending);
        compositor.Flush(TimeStamp4);
        Assert.False(compositor.Pending);
        compositor.Draw(TimeStamp5, new CompositorComposition(composition2, object4, palette));
        Assert.False(compositor.Pending);
        compositor.Flush(TimeStamp6);
        Assert.False(compositor.Pending);
        compositor.Draw(TimeStamp7, new CompositorComposition(composition2, object4, palette));
        Assert.False(compositor.Pending);
        compositor.Draw(TimeStamp8, new CompositorComposition(composition1, object4, palette));
        Assert.False(compositor.Pending);
        compositor.Draw(TimeStamp9, new CompositorComposition(composition2, object4, palette));
        Assert.False(compositor.Pending);

        Assert.Equal(3, captions.Count);
        Assert.Equal(TimeStamp1, captions[0].TimeStamp);
        Assert.Equal(TimeStamp2 - TimeStamp1, captions[0].Duration);
        Assert.Equal(0, captions[0].X);
        Assert.Equal(0, captions[0].Y);
        Assert.Equal(2, captions[0].Width);
        Assert.Equal(2, captions[0].Height);
        Assert.False(captions[0].Forced);
        Assert.Equal(expectedPixels1, captions[0].Data);
        Assert.Equal(TimeStamp2, captions[1].TimeStamp);
        Assert.Equal(TimeStamp3 - TimeStamp2, captions[1].Duration);
        Assert.Equal(0, captions[1].X);
        Assert.Equal(0, captions[1].Y);
        Assert.Equal(2, captions[1].Width);
        Assert.Equal(2, captions[1].Height);
        Assert.True(captions[1].Forced);
        Assert.Equal(expectedPixels2, captions[1].Data);
        Assert.Equal(TimeStamp3, captions[2].TimeStamp);
        Assert.Equal(TimeStamp4 - TimeStamp3, captions[2].Duration);
        Assert.Equal(0, captions[2].X);
        Assert.Equal(0, captions[2].Y);
        Assert.Equal(2, captions[2].Width);
        Assert.Equal(2, captions[2].Height);
        Assert.False(captions[2].Forced);
        Assert.Equal(expectedPixels3, captions[2].Data);
    }

    [Fact]
    public void IllegalDrawTimeStamp()
    {
        var entries = new Dictionary<byte, YcbcraPixel>
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
        var palette = new DisplayPalette(entries);
        var object1 = new DisplayObject(2, 2, data1);
        var object2 = new DisplayObject(2, 2, data2);
        var window = new DisplayWindow(0, 0, 2, 2);
        var composition = new DisplayComposition(0, 0, false, null);
        var compositor = new Compositor(window);
        var captions = new List<Caption>();

        compositor.Draw(TimeStamp2, new CompositorComposition(composition, object1, palette));
        Assert.True(compositor.Pending);

        try
        {
            compositor.Draw(TimeStamp1, new CompositorComposition(composition, object2
                , palette));

            Assert.Fail("Was able to draw with illegal time stamp.");
        }
        catch (CompositorException ce)
        {
            Assert.Equal("Current time stamp is less than or equal to the previous one."
                , ce.Message);
        }
    }

    [Fact]
    public void IllegalFlushTimeStamp()
    {
        var entries = new Dictionary<byte, YcbcraPixel>
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
        var palette = new DisplayPalette(entries);
        var object1 = new DisplayObject(2, 2, data1);
        var object2 = new DisplayObject(2, 2, data2);
        var window = new DisplayWindow(0, 0, 2, 2);
        var composition = new DisplayComposition(0, 0, false, null);
        var compositor = new Compositor(window);
        var captions = new List<Caption>();

        compositor.Draw(TimeStamp2, new CompositorComposition(composition, object1, palette));
        Assert.True(compositor.Pending);

        try
        {
            compositor.Flush(TimeStamp1);

            Assert.Fail("Was able to flush with illegal time stamp.");
        }
        catch (CompositorException ce)
        {
            Assert.Equal("Current time stamp is less than or equal to the previous one."
                , ce.Message);
        }
    }
}
