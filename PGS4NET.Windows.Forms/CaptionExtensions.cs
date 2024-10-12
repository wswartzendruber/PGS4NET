/*
 * Copyright 2024 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using PGS4NET.Captions;

namespace PGS4NET.Windows.Forms;

/// <summary>
///     Extension methods against <see cref="PGS4NET.Captions.Caption" /> for interoperating
///     with Windows Forms.
/// </summary>
public static class CaptionExtensions
{
    /// <summary>
    ///     Converts a PGS4NET caption to a standard Windows Forms bitmap.
    /// </summary>
    /// <param name="caption">
    ///     The caption to convert.
    /// </param>
    /// <param name="colorSpace">
    ///     The color space to use for conversion, with
    ///     <see cref="ColorSpace.Bt709ColorSpace" /> being typical for legacy Blu-ray releases
    ///     and <see cref="ColorSpace.Bt2020ColorSpace" /> being typical for 4K UltraHD Blu-ray
    ///     releases.
    /// </param>
    /// <param name="limitedRange">
    ///     Whether the caption uses limited-range values, which is commonly
    ///     <see langword="true" />.
    /// </param>
    /// <returns>
    ///     A standard Windows Forms bitmap.
    /// </returns>
    public static Bitmap ToBitmap(this Caption caption, ColorSpace colorSpace
        , bool limitedRange = true)
    {
        var width = caption.Width;
        var height = caption.Height;
        var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var index = (width * y) + x;
                var inColor = colorSpace.YcbcraToRgba(caption.Data[index], limitedRange);
                var red = (int)Clamp(inColor.Red * 255.0, 0.0, 255.0);
                var green = (int)Clamp(inColor.Green * 255.0, 0.0, 255.0);
                var blue = (int)Clamp(inColor.Blue * 255.0, 0.0, 255.0);
                var alpha = (int)Clamp(inColor.Alpha * 255.0, 0.0, 255.0);
                var outColor = Color.FromArgb(alpha, red, green, blue);

                bitmap.SetPixel(x, y, outColor);
            }
        }

        return bitmap;
    }

    private static double Clamp(double value, double min, double max)
    {
        if (min <= value && value <= max)
            return value;
        else if (value < min)
            return min;
        else if (max < value)
            return max;
        else
            throw new InvalidOperationException();
    }
}
