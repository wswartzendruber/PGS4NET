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

public class PixelConversionTests
{
    [Fact]
    public void RoundTripBt709LimitedRange()
    {
        var colorSpace = ColorSpace.Bt709ColorSpace;

        for (byte y = 16; y < 235; y++)
        {
            for (short cb = 0; cb <= 255; cb++)
            {
                for (short cr = 0; cr <= 255; cr++)
                {
                    for (short alpha = 0; alpha <= 255; alpha += 32)
                    {
                        var inPixel = new YcbcraPixel(y, (byte)cb, (byte)cr, (byte)alpha);
                        var outPixel = colorSpace.RgbaToYcbcra(
                            colorSpace.YcbcraToRgba(inPixel, true), true);

                        Assert.True(inPixel == outPixel);
                    }
                }
            }
        }
    }

    [Fact]
    public void RoundTripBt709FullRange()
    {
        var colorSpace = ColorSpace.Bt709ColorSpace;

        for (short y = 0; y <= 255; y++)
        {
            for (short cb = 0; cb <= 255; cb++)
            {
                for (short cr = 0; cr <= 255; cr++)
                {
                    for (short alpha = 0; alpha <= 255; alpha += 32)
                    {
                        var inPixel = new YcbcraPixel((byte)y, (byte)cb, (byte)cr, (byte)alpha);
                        var outPixel = colorSpace.RgbaToYcbcra(
                            colorSpace.YcbcraToRgba(inPixel, false), false);

                        Assert.True(inPixel == outPixel);
                    }
                }
            }
        }
    }
}
