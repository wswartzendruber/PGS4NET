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
    private static readonly PgsPixel Pixel = new PgsPixel(255, 63, 31, 15);
    private static readonly PgsTimeStamp TimeStamp1 = new PgsTimeStamp(8);
    private static readonly PgsTimeStamp TimeStamp2 = new PgsTimeStamp(16);

    [Fact]
    public void Emptry()
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
    public void Single()
    {
        var entries = new Dictionary<byte, PgsPixel>
        {
            { 0, default },
            { 1, Pixel },
        };
        var data1 = new byte[]
        {
            1, 0, 0, 1,
            0, 1, 1, 0,
            0, 1, 1, 0,
            1, 0, 0, 1,
        };
        var palette = new DisplayPalette(entries);
        var object1 = new DisplayObject(4, 4, data1);
        var window = new DisplayWindow(8, 16, 24, 32);
        var composition = new DisplayComposition(16, 16, false, null);
        var compositor = new Compositor(window);
        var captions = new List<Caption>();
        var expectedPixels1 = new PgsPixel[]
        {
            Pixel, default, default, Pixel,
            default, Pixel, Pixel, default,
            default, Pixel, Pixel, default,
            Pixel, default, default, Pixel,
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
        Assert.True(captions[0].Width == 24);
        Assert.True(captions[0].Height == 32);
        Assert.True(captions[0].Forced == false);
        Assert.True(captions[0].Data.SequenceEqual(expectedPixels1));
    }
}
