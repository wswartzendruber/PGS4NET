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

#pragma warning disable 1718

using PGS4NET;

namespace PGS4NET.Tests;

public class RgbaPixelTests
{
    [Fact]
    public void Properties()
    {
        var a = new RgbaPixel(0.01, 0.02, 0.03, 0.04);

        Assert.Equal(0.01, a.Red);
        Assert.Equal(0.02, a.Green);
        Assert.Equal(0.03, a.Blue);
        Assert.Equal(0.04, a.Alpha);
    }

    [Fact]
    public void HashCodes()
    {
        var a = new RgbaPixel(0.01, 0.02, 0.03, 0.04);
        var b = new RgbaPixel(0.01, 0.02, 0.03, 0.04);

        Assert.True(a.GetHashCode() == a.GetHashCode());
        Assert.True(a.GetHashCode() == b.GetHashCode());
    }

    [Fact]
    public void Equality()
    {
        var a = new RgbaPixel(0.01, 0.02, 0.03, 0.04);
        var b = new RgbaPixel(0.01, 0.02, 0.03, 0.04);
        var c = new RgbaPixel(0.01, 0.02, 0.03, 0.14);
        var d = new RgbaPixel(0.01, 0.02, 0.13, 0.04);
        var e = new RgbaPixel(0.01, 0.12, 0.03, 0.04);
        var f = new RgbaPixel(0.11, 0.02, 0.03, 0.04);

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
