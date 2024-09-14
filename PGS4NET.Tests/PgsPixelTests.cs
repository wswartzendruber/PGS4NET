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

using PGS4NET;

namespace PGS4NET.Tests;

public class PgsPixelTests
{
    [Fact]
    public void Properties()
    {
        var a = new PgsPixel(0x01, 0x02, 0x03, 0x04);

        Assert.True(a.Y == 0x01);
        Assert.True(a.Cr == 0x02);
        Assert.True(a.Cb == 0x03);
        Assert.True(a.Alpha == 0x04);
    }

    [Fact]
    public void HashCodes()
    {
        var a = new PgsPixel(0x01, 0x02, 0x03, 0x04);
        var b = new PgsPixel(0x01, 0x02, 0x03, 0x04);

        Assert.True(a.GetHashCode() == a.GetHashCode());
        Assert.True(a.GetHashCode() == b.GetHashCode());
    }

    [Fact]
    public void Equality()
    {
        var a = new PgsPixel(0x01, 0x02, 0x03, 0x04);
        var b = new PgsPixel(0x01, 0x02, 0x03, 0x04);
        var c = new PgsPixel(0x01, 0x02, 0x03, 0x14);
        var d = new PgsPixel(0x01, 0x02, 0x13, 0x04);
        var e = new PgsPixel(0x01, 0x12, 0x03, 0x04);
        var f = new PgsPixel(0x11, 0x02, 0x03, 0x04);

        Assert.True(a.Equals(a));
        Assert.True(a.Equals(b));
        Assert.False(a.Equals(c));
        Assert.False(a.Equals(d));
        Assert.False(a.Equals(e));
        Assert.False(a.Equals(f));
        Assert.True(a == a);
        Assert.True(a == b);
        Assert.True(a != c);
        Assert.True(a != d);
        Assert.True(a != e);
        Assert.True(a != f);
    }
}
