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

#pragma warning disable 1718

using PGS4NET.DisplaySets;

namespace PGS4NET.Tests.DisplaySets;

public class ValueTests
{
    [Fact]
    public void DisplayComposition()
    {
        var a = new DisplayComposition
        {
            X = 1,
            Y = 2,
            Forced = true,
            Crop = new CroppedArea
            {
                X = 3,
                Y = 4,
                Width = 5,
                Height = 6,
            },
        };
        var b = new DisplayComposition
        {
            X = 1,
            Y = 2,
            Forced = true,
            Crop = new CroppedArea
            {
                X = 3,
                Y = 4,
                Width = 5,
                Height = 6,
            },
        };
        var c = new DisplayComposition
        {
            X = 1,
            Y = 2,
            Forced = true,
            Crop = new CroppedArea
            {
                X = 3,
                Y = 4,
                Width = 5,
                Height = 7,
            },
        };
        var d = new DisplayComposition
        {
            X = 1,
            Y = 2,
            Forced = true,
            Crop = null,
        };
        var e = new DisplayComposition
        {
            X = 1,
            Y = 2,
            Forced = true,
            Crop = null,
        };
        var f = new DisplayComposition
        {
            X = 1,
            Y = 2,
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
    public void DisplayWindow()
    {
        var a = new DisplayWindow
        {
            X = 1,
            Y = 2,
            Width = 3,
            Height = 4,
        };
        var b = new DisplayWindow
        {
            X = 1,
            Y = 2,
            Width = 3,
            Height = 4,
        };
        var c = new DisplayWindow
        {
            X = 1,
            Y = 2,
            Width = 3,
            Height = 5,
        };

        Assert.True(a == a);
        Assert.True(a == b);
        Assert.True(a != c);
        Assert.True(a.GetHashCode() == a.GetHashCode());
        Assert.True(a.GetHashCode() == b.GetHashCode());
        Assert.True(a.GetHashCode() != c.GetHashCode());
    }
}