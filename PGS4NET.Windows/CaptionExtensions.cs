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
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PGS4NET.Captions;

namespace PGS4NET.Windows;

/// <summary>
///     Extension methods against <see cref="PGS4NET.Captions.Caption" /> for interoperating
///     with Windows Forms.
/// </summary>
public static class CaptionExtensions
{
    private static readonly System.Drawing.Imaging.PixelFormat WinFormsPixelFormat
        = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
    private static readonly System.Windows.Media.PixelFormat WpfPixelFormat
        = System.Windows.Media.PixelFormats.Rgba64;

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
        var bitmap = new Bitmap(width, height, WinFormsPixelFormat);
        var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
        var pointer = data.Scan0;
        var stride = Math.Abs(data.Stride);
        var length = stride * bitmap.Height;
        var values = new byte[length];
        var readIndex = 0;

        for (var j = 0; j < height; j++)
        {
            var writeIndex = stride * j;

            for (var i = 0; i < width; i++)
            {
                var inColor = colorSpace.YcbcraToRgba(caption.Data[readIndex++], limitedRange);

                values[writeIndex++] = ClampToByte((int)(inColor.Blue * 255.0));
                values[writeIndex++] = ClampToByte((int)(inColor.Green * 255.0));
                values[writeIndex++] = ClampToByte((int)(inColor.Red * 255.0));
                values[writeIndex++] = ClampToByte((int)(inColor.Alpha * 255.0));
            }
        }

        Marshal.Copy(values, 0, pointer, values.Length);
        bitmap.UnlockBits(data);

        return bitmap;
    }

    //public static BitmapSource ToBitmapSource(this Caption caption)
    //{
    //    var width = caption.Width;
    //    var height = caption.Height;
    //    var values = new byte[width * height * 8];
    //    var bitmapSource = BitmapSource.Create(width, height, 96.0, 96.0, WpfPixelFormat, null,  )
    //}

    private static byte ClampToByte(int value)
    {
        if ((value & ~0xFF) != 0)
            value = ((~value) >> 31) & 0xFF;

        return (byte)value;
    }
}
