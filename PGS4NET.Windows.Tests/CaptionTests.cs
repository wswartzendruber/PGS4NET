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

using System.Drawing;
using PGS4NET.Captions;

namespace PGS4NET.Windows.Tests;

public class CaptionTests
{
    private static readonly Random Rng = new();

    [Fact]
    public void RoundTrip()
    {
        var limitedRange = false;
        var colorSpace = ColorSpace.Bt2020ColorSpace;
        var timeStamp = new PgsTime(Rng.Next());
        var duration = new PgsTime(Rng.Next());
        int x = 17;
        int y = 23;
        int width = 1024;
        int height = 1024;
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
        var outCaption = bitmap.ToCaption(new Point(x, y), timeStamp, duration, forced, colorSpace, limitedRange);

        Assert.Equal(inCaption.TimeStamp, outCaption.TimeStamp);
        Assert.Equal(inCaption.Duration, outCaption.Duration);
        Assert.Equal(inCaption.X, outCaption.X);
        Assert.Equal(inCaption.Y, outCaption.Y);
        Assert.Equal(inCaption.Width, outCaption.Width);
        Assert.Equal(inCaption.Height, outCaption.Height);
        Assert.Equal(inCaption.Data.Length, outCaption.Data.Length);
        Assert.Equal(inCaption.Forced, outCaption.Forced);
        Assert.Equal(inCaption.Data, outCaption.Data, (expected, actual) =>
        {
            return Math.Abs(expected.Y - actual.Y) <= 6
                && Math.Abs(expected.Cb - actual.Cb) <= 6
                && Math.Abs(expected.Cr - actual.Cr) <= 6
                && Math.Abs(expected.Alpha - actual.Alpha) <= 6;
        });
    }
}
