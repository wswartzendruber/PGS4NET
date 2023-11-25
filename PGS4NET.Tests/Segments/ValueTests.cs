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

#pragma warning disable 1718

using PGS4NET.Segments;

namespace PGS4NET.Tests.Segments;

public class ValueTests
{
    [Fact]
    public void CompositionObject()
    {
        var a = new CompositionObject
        {
            ObjectId = 1,
            WindowId = 2,
            X = 3,
            Y = 4,
            Forced = true,
            Crop = new CroppedArea
            {
                X = 5,
                Y = 6,
                Width = 7,
                Height = 8,
            },
        };
        var b = new CompositionObject
        {
            ObjectId = 1,
            WindowId = 2,
            X = 3,
            Y = 4,
            Forced = true,
            Crop = new CroppedArea
            {
                X = 5,
                Y = 6,
                Width = 7,
                Height = 8,
            },
        };
        var c = new CompositionObject
        {
            ObjectId = 1,
            WindowId = 2,
            X = 3,
            Y = 4,
            Forced = true,
            Crop = new CroppedArea
            {
                X = 5,
                Y = 6,
                Width = 7,
                Height = 9,
            },
        };
        var d = new CompositionObject
        {
            ObjectId = 1,
            WindowId = 2,
            X = 3,
            Y = 4,
            Forced = true,
            Crop = null,
        };
        var e = new CompositionObject
        {
            ObjectId = 1,
            WindowId = 2,
            X = 3,
            Y = 4,
            Forced = true,
            Crop = null,
        };
        var f = new CompositionObject
        {
            ObjectId = 1,
            WindowId = 2,
            X = 3,
            Y = 4,
            Forced = false,
            Crop = null,
        };

        Assert.True(a == a);
        Assert.True(a == b);
        Assert.True(d == e);
        Assert.True(a != c);
        Assert.True(d != f);
        Assert.True(a.GetHashCode() == a.GetHashCode());
        Assert.True(a.GetHashCode() == b.GetHashCode());
        Assert.True(d.GetHashCode() == e.GetHashCode());
        Assert.True(a.GetHashCode() != c.GetHashCode());
        Assert.True(d.GetHashCode() != f.GetHashCode());
    }

    [Fact]
    public void PaletteDefinitionEntry()
    {
        var a = new PaletteDefinitionEntry
        {
            Id = 1,
            Pixel = new PgsPixel
            {
                Y = 2,
                Cr = 3,
                Cb = 4,
                Alpha = 5,
            },
        };
        var b = new PaletteDefinitionEntry
        {
            Id = 1,
            Pixel = new PgsPixel
            {
                Y = 2,
                Cr = 3,
                Cb = 4,
                Alpha = 5,
            },
        };
        var c = new PaletteDefinitionEntry
        {
            Id = 1,
            Pixel = new PgsPixel
            {
                Y = 2,
                Cr = 3,
                Cb = 4,
                Alpha = 6,
            },
        };

        Assert.True(a == a);
        Assert.True(a == b);
        Assert.True(a != c);
        Assert.True(a.GetHashCode() == a.GetHashCode());
        Assert.True(a.GetHashCode() == b.GetHashCode());
        Assert.True(a.GetHashCode() != c.GetHashCode());
    }

    [Fact]
    public void WindowDefinitionEntry()
    {
        var a = new WindowDefinitionEntry
        {
            Id = 1,
            X = 2,
            Y = 3,
            Width = 4,
            Height = 5,
        };
        var b = new WindowDefinitionEntry
        {
            Id = 1,
            X = 2,
            Y = 3,
            Width = 4,
            Height = 5,
        };
        var c = new WindowDefinitionEntry
        {
            Id = 1,
            X = 2,
            Y = 3,
            Width = 4,
            Height = 6,
        };

        Assert.True(a == a);
        Assert.True(a == b);
        Assert.True(a != c);
        Assert.True(a.GetHashCode() == a.GetHashCode());
        Assert.True(a.GetHashCode() == b.GetHashCode());
        Assert.True(a.GetHashCode() != c.GetHashCode());
    }
}
