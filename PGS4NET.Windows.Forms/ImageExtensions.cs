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
using PGS4NET.Captions;

namespace PGS4NET.Windows.Forms;

/// <summary>
///     Extension methods against <see cref="System.Drawing.Image" /> for interoperating
///     with PGS4NET.
/// </summary>
public static class ImageExtensions
{
    /// <summary>
    ///     Converts a Windows Forms bitmap into a PGS4NET caption.
    /// </summary>
    /// <param name="bitmap">
    ///     The bitmap to convert.
    /// </param>
    /// <param name="location">
    ///     Gets or sets the coordinates of the upper-left corner of the caption relative to the
    ///     upper-left corner the screen.
    /// </param>
    /// <param name="timeStamp">
    ///     The time at which the caption appears.
    /// </param>
    /// <param name="duration">
    ///     The duration for which the caption is visible.
    /// </param>
    /// <param name="forced">
    ///     Whether or not the caption is forced. This is typically used to translate foreign
    ///     dialogue or text that appears.
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
    ///     A PGS4NET caption.
    /// </returns>
    public static Caption ToCaption(this Bitmap bitmap, Point location, PgsTimeStamp timeStamp
        , PgsTimeStamp duration, bool forced, ColorSpace colorSpace, bool limitedRange = true)
    {
        var x = Convert.ToUInt16(location.X);
        var y = Convert.ToUInt16(location.Y);
        var width = Convert.ToUInt16(bitmap.Width);
        var height = Convert.ToUInt16(bitmap.Height);
        var length = width * height;
        var data = new YcbcraPixel[length];
        var caption = new Caption(timeStamp, duration, x, y, width, height, data, forced);

        for (ushort j = 0; j < height; j++)
        {
            for (ushort i = 0; i < width; i++)
            {
                var index = (width * j) + i;
                var inColor = bitmap.GetPixel(i, j);
                var red = inColor.R / 255.0;
                var green = inColor.G / 255.0;
                var blue = inColor.B / 255.0;
                var alpha = inColor.A / 255.0;
                var outColor = new RgbaPixel(red, green, blue, alpha);

                data[index] = colorSpace.RgbaToYcbcra(outColor, limitedRange);
            }
        }

        return caption;
    }
}
