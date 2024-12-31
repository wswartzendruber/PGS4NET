/*
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

public class YcbcraPixelTests
{
    [Fact]
    public void Properties()
    {
        var a = new YcbcraPixel(0x01, 0x02, 0x03, 0x04);

        Assert.Equal(0x01, a.Y);
        Assert.Equal(0x02, a.Cb);
        Assert.Equal(0x03, a.Cr);
        Assert.Equal(0x04, a.Alpha);
    }

    [Fact]
    public void HashCodes()
    {
        var a = new YcbcraPixel(0x01, 0x02, 0x03, 0x04);
        var b = new YcbcraPixel(0x01, 0x02, 0x03, 0x04);

        Assert.True(a.GetHashCode() == a.GetHashCode());
        Assert.True(a.GetHashCode() == b.GetHashCode());
    }

    [Fact]
    public void Equality()
    {
        var a = new YcbcraPixel(0x01, 0x02, 0x03, 0x04);
        var b = new YcbcraPixel(0x01, 0x02, 0x03, 0x04);
        var c = new YcbcraPixel(0x01, 0x02, 0x03, 0x14);
        var d = new YcbcraPixel(0x01, 0x02, 0x13, 0x04);
        var e = new YcbcraPixel(0x01, 0x12, 0x03, 0x04);
        var f = new YcbcraPixel(0x11, 0x02, 0x03, 0x04);

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
