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

using PGS4NET.Captions;

namespace PGS4NET.Windows.Forms.Tests;

public class CaptionTests
{
    private static readonly Random Rng = new();

    [Fact]
    public void RoundTrip()
    {
        var limitedRange = false;
        var colorSpace = ColorSpace.Bt709ColorSpace;
        var timeStamp = new PgsTimeStamp((uint)Rng.Next());
        var duration = new PgsTimeStamp((uint)Rng.Next());
        ushort x = 16;
        ushort y = 24;
        ushort width = 384;
        ushort height = 128;
        int length = width * height;
        var data = new YcbcraPixel[length];
        bool forced = true;

        for (int index = 0; index < length; index++)
        {
            var red = Rng.NextDouble();
            var green = Rng.NextDouble();
            var blue = Rng.NextDouble();
            var alpha = Rng.NextDouble();
            var pixel = colorSpace.RgbaToYcbcra(new RgbaPixel(red, green, blue, alpha));

            data[index] = pixel;
        }

        var inCaption = new Caption(timeStamp, duration, x, y, width, height, data, forced);
        var bitmap = inCaption.ToBitmap(colorSpace, limitedRange);
        var outCaption = bitmap.ToCaption(new System.Drawing.Point(x, y), timeStamp, duration, forced, colorSpace, limitedRange);

        AssertCaptionsEqual(inCaption, outCaption);
    }

    private void AssertCaptionsEqual(Caption first, Caption second)
    {
        Assert.True(first.TimeStamp == second.TimeStamp);
        Assert.True(first.Duration == second.Duration);
        Assert.True(first.X == second.X);
        Assert.True(first.Y == second.Y);
        Assert.True(first.Width == second.Width);
        Assert.True(first.Height == second.Height);
        Assert.True(first.Data.Length == second.Data.Length);
        Assert.True(PixelSequenceNearlyEqual(first.Data, second.Data));
        Assert.True(first.Forced == second.Forced);
    }

    private bool PixelSequenceNearlyEqual(YcbcraPixel[] first, YcbcraPixel[] second)
    {
        if (first.Length != second.Length)
            return false;

        var length = first.Length;

        for (int index = 0; index < length; index++)
        {
            if (!PixelsNearlyEqual(first[index], second[index]))
                return false;
        }

        return true;
    }

    private bool PixelsNearlyEqual(YcbcraPixel first, YcbcraPixel second)
    {
        return Math.Abs(first.Y - second.Y) <= 8
            && Math.Abs(first.Cb - second.Cb) <= 8
            && Math.Abs(first.Cr - second.Cr) <= 8
            && Math.Abs(first.Alpha - second.Alpha) <= 4;
    }
}