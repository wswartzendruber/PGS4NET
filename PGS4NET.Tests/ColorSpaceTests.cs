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

public class ColorSpaceTests
{
    [Fact]
    public void Properties()
    {
        var a = new ColorSpace(0.01, 0.02, 0.03);

        Assert.True(a.Red == 0.01);
        Assert.True(a.Green == 0.02);
        Assert.True(a.Blue == 0.03);
    }

    [Fact]
    public void HashCodes()
    {
        var a = new ColorSpace(0.01, 0.02, 0.03);
        var b = new ColorSpace(0.01, 0.02, 0.03);

        Assert.True(a.GetHashCode() == a.GetHashCode());
        Assert.True(a.GetHashCode() == b.GetHashCode());
    }

    [Fact]
    public void Equality()
    {
        var a = new ColorSpace(0.01, 0.02, 0.03);
        var b = new ColorSpace(0.01, 0.02, 0.03);
        var c = new ColorSpace(0.01, 0.02, 0.13);
        var d = new ColorSpace(0.01, 0.12, 0.03);
        var e = new ColorSpace(0.11, 0.02, 0.03);

        Assert.True(a.Equals(a));
        Assert.True(a.Equals(b));
        Assert.False(a.Equals(c));
        Assert.False(a.Equals(d));
        Assert.False(a.Equals(e));
        Assert.True(a == a);
        Assert.True(a == b);
        Assert.True(a != c);
        Assert.True(a != d);
        Assert.True(a != e);
    }
    [Fact]
    public void RoundTripBt709LimitedRange()
    {
        var colorSpace = ColorSpace.Bt709ColorSpace;

        for (short y = 16; y <= 235; y += 16)
        {
            for (short cb = 0; cb <= 255; cb += 16)
            {
                for (short cr = 0; cr <= 255; cr += 16)
                {
                    for (short alpha = 0; alpha <= 255; alpha += 16)
                    {
                        var inPixel = new YcbcraPixel((byte)y, (byte)cb, (byte)cr, (byte)alpha);
                        var outPixel = colorSpace.RgbaToYcbcra(
                            colorSpace.YcbcraToRgba(inPixel, true), true);

                        Assert.True(inPixel == outPixel);
                    }
                }
            }
        }

        var finalInPixel = new YcbcraPixel(0xEB, 0xFF, 0xFF, 0xFF);
        var finalOutPixel = colorSpace.RgbaToYcbcra(
            colorSpace.YcbcraToRgba(finalInPixel, true), true);

        Assert.True(finalInPixel == finalOutPixel);
    }

    [Fact]
    public void RoundTripBt709FullRange()
    {
        var colorSpace = ColorSpace.Bt709ColorSpace;

        for (short y = 0; y <= 255; y += 16)
        {
            for (short cb = 0; cb <= 255; cb += 16)
            {
                for (short cr = 0; cr <= 255; cr += 16)
                {
                    for (short alpha = 0; alpha <= 255; alpha += 16)
                    {
                        var inPixel = new YcbcraPixel((byte)y, (byte)cb, (byte)cr, (byte)alpha);
                        var outPixel = colorSpace.RgbaToYcbcra(
                            colorSpace.YcbcraToRgba(inPixel, false), false);

                        Assert.True(inPixel == outPixel);
                    }
                }
            }
        }

        var finalInPixel = new YcbcraPixel(0xFF, 0xFF, 0xFF, 0xFF);
        var finalOutPixel = colorSpace.RgbaToYcbcra(
            colorSpace.YcbcraToRgba(finalInPixel, false), false);

        Assert.True(finalInPixel == finalOutPixel);
    }
}
