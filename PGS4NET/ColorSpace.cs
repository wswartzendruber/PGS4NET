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

namespace PGS4NET;

/// <summary>
///     Represents a RGB pixel.
/// </summary>
public class ColorSpace : IEquatable<ColorSpace>
{
    /// <summary>
    ///     Preconfigured RGB coefficients for BT.709.
    /// </summary>
    /// <remarks>
    ///     These should be used for the original incarnation of Blu-ray (in the blue cases) and
    ///     HDTV. These coefficients are also valid for sRGB. Most SDR (standard dynamic range)
    ///     content will use them.
    /// </remarks>
    public static readonly ColorSpace Bt709ColorSpace = new(0.2126, 0.7152, 0.0722);

    /// <summary>
    ///     Preconfigured RGB coefficients for BT.2020.
    /// </summary>
    /// <remarks>
    ///     These should be used for the later 4K-UHD incarnation of Blu-ray (in the black
    ///     cases). All HDR (high dynamic range) content should use them.
    /// </remarks>
    public static readonly ColorSpace Bt2020ColorSpace = new(0.2627, 0.6780, 0.0593);

    /// <summary>
    ///     The red coefficient.
    /// </summary>
    public readonly double Red;

    /// <summary>
    ///     The green coefficient.
    /// </summary>
    public readonly double Green;

    /// <summary>
    ///     The blue coefficient.
    /// </summary>
    public readonly double Blue;

    private readonly double Fr3;
    private readonly double Fg2;
    private readonly double Fg3;
    private readonly double Fb2;
    private readonly double Rb1;
    private readonly double Rb2;
    private readonly double Rr2;
    private readonly double Rr3;

    /// <summary>
    ///     Creates a new instance with the provided color coefficients.
    /// </summary>
    /// <param name="red">
    ///     The red coefficient.
    /// </param>
    /// <param name="green">
    ///     The green coefficient.
    /// </param>
    /// <param name="blue">
    ///     The blue coefficient.
    /// </param>
    public ColorSpace(double red, double green, double blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Fr3 = 2.0 - 2.0 * red;
        Fg2 = -(blue / green) * (2.0 - 2.0 * blue);
        Fg3 = -(red / green) * (2.0 - 2.0 * red);
        Fb2 = 2.0 - 2.0 * blue;
        Rb1 = -0.5 * (Red / (1.0 - Blue));
        Rb2 = -0.5 * (Green / (1.0 - Blue));
        Rr2 = -0.5 * (Green / (1.0 - Red));
        Rr3 = -0.5 * (Blue / (1.0 - Red));
    }

    /// <summary>
    ///     Determines if the values of this instance matches another one's.
    /// </summary>
    public bool Equals(ColorSpace other) =>
        other.Red == this.Red
            && other.Green == this.Green
            && other.Blue == this.Blue;

    /// <summary>
    ///     Determines if the type and values of this instance matches another one's.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == typeof(ColorSpace) && Equals((ColorSpace)other);

    /// <summary>
    ///     Calculates and returns the hash code of this instance using its immutable state.
    /// </summary>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;

            hash = hash * 23 + Red.GetHashCode();
            hash = hash * 23 + Green.GetHashCode();
            hash = hash * 23 + Blue.GetHashCode();

            return hash;
        }
    }

    public YcbcraPixel RgbaToYcbcra(RgbaPixel rgbaPixel)
    {
        var y1 = Red * rgbaPixel.Red + Green * rgbaPixel.Green + Blue * rgbaPixel.Blue;
        var y2 = (Compress(y1) * 255.0) - 0.25;
        var y3 = Math.Round(Math.Min(Math.Max(y2, 0.0), 255.0));
        var cb1 = Rb1 * rgbaPixel.Red + Rb2 * rgbaPixel.Green + 0.5 * rgbaPixel.Blue;
        var cb2 = cb1 * 128.0;
        var cb3 = Math.Round(Math.Min(Math.Max(cb2, 0.0), 255.0));
        var cr1 = 0.5 * rgbaPixel.Red + Rr2 * rgbaPixel.Green + Rr3 * rgbaPixel.Blue;
        var cr2 = cr1 * 128.0;
        var cr3 = Math.Round(Math.Min(Math.Max(cr2, 0.0), 255.0));
        var alpha1 = (rgbaPixel.Alpha) * 255.0;
        var alpha2 = Math.Round(Math.Min(Math.Max(alpha1, 0.0), 255.0));

        return new YcbcraPixel((byte)y3, (byte)cb3, (byte)cr3, (byte)alpha2);
    }

    public RgbaPixel YcbcraToRgba(YcbcraPixel ycbcraPixel)
    {
        var y = Expand((double)ycbcraPixel.Y / 255.0);
        var pb = ((double)ycbcraPixel.Cb - 128.0) / 128.0;
        var pr = ((double)ycbcraPixel.Cr - 128.0) / 128.0;
        var alpha = (double)ycbcraPixel.Alpha / 255.0;

        var red = 1.0 + Fr3 * pr;
        var green = 1.0 + Fg2 * pb + Fg3 * pr;
        var blue = 1.0 + Fb2 * pb;

        return new RgbaPixel(red, green, blue, alpha);
    }

    /// <summary>
    ///     Determines if the values of two <see cref="ColorSpace" />s match each other.
    /// </summary>
    public static bool operator ==(ColorSpace first, ColorSpace second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the values of two <see cref="ColorSpace" />s don't match each
    ///     other.
    /// </summary>
    public static bool operator !=(ColorSpace first, ColorSpace second) =>
        !first.Equals(second);

    private double Compress(double value)
    {
        return (value * 0.859375) + 0.06274509803;
    }

    private double Expand(double value)
    {
        if (value < 0.06274509803)
            return 0.0;
        else if (value > 0.92156862745)
            return 1.0;
        else
            return (value - 0.06274509803) / 0.859375;
    }
}
