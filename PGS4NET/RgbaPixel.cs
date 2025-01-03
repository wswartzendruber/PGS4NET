/*
 * Copyright 2025 William Swartzendruber
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
///     Represents an RGBA pixel, which is always full-range.
/// </summary>
public struct RgbaPixel : IEquatable<RgbaPixel>
{
    /// <summary>
    ///     The mangitude of the red channel, ranging from <c>0.0</c> to <c>1.0</c>.
    /// </summary>
    public readonly double Red;

    /// <summary>
    ///     The mangitude of the green channel, ranging from <c>0.0</c> to <c>1.0</c>.
    /// </summary>
    public readonly double Green;

    /// <summary>
    ///     The mangitude of the blue channel, ranging from <c>0.0</c> to <c>1.0</c>.
    /// </summary>
    public readonly double Blue;

    /// <summary>
    ///     The alpha value (transparency ratio), ranging from <c>0.0</c> to <c>1.0</c>.
    /// </summary>
    public readonly double Alpha;

    /// <summary>
    ///     Initializes a new instance with the provided values.
    /// </summary>
    /// <param name="red">
    ///     The mangitude of the red channel, ranging from <c>0.0</c> to <c>1.0</c>.
    /// </param>
    /// <param name="green">
    ///     The mangitude of the green channel, ranging from <c>0.0</c> to <c>1.0</c>.
    /// </param>
    /// <param name="blue">
    ///     The mangitude of the blue channel, ranging from <c>0.0</c> to <c>1.0</c>.
    /// </param>
    /// <param name="alpha">
    ///     The alpha value (transparency ratio), ranging from <c>0.0</c> to <c>1.0</c>.
    /// </param>
    public RgbaPixel(double red, double green, double blue, double alpha)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
    }

    /// <summary>
    ///     Determines if the state of the <paramref name="other"/> instance equals this one's.
    /// </summary>
    public bool Equals(RgbaPixel other) =>
        other.Red == Red
            && other.Green == Green
            && other.Blue == Blue
            && other.Alpha == Alpha;

    /// <summary>
    ///     Determines if the type and state of the <paramref name="other"/> instance equals
    ///     this one's.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == typeof(RgbaPixel) && Equals((RgbaPixel)other);

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
            hash = hash * 23 + Alpha.GetHashCode();

            return hash;
        }
    }

    /// <summary>
    ///     Returns a string that represents this RGBA pixel in the form of
    ///     <c>R[#.###]-G[#.###]-B[#.###]-A[#.###]</c> where each <c>#.###</c> respresnts a
    ///     floating point value rounded to three decimal places.
    /// </summary>
    public override string ToString() => ToString(3);

    /// <summary>
    ///     Returns a string that represents this RGBA pixel in the form of
    ///     <c>R[#.###]-G[#.###]-B[#.###]-A[#.###]</c> where each <c>#.###</c> respresnts a
    ///     floating point value rounded to a configurable number of decimal places.
    /// </summary>
    /// <param name="digits">
    ///     The number of fractional digits to round to.
    /// </param>
    /// <param name="mode">
    ///     The rounding strategy to use.
    /// </param>
    public string ToString(int digits, MidpointRounding mode = MidpointRounding.AwayFromZero)
    {
        var red = Math.Round(Red, digits, mode);
        var green = Math.Round(Green, digits, mode);
        var blue = Math.Round(Blue, digits, mode);
        var alpha = Math.Round(Alpha, digits, mode);

        return $"R[{red}]-G[{green}]-B[{blue}]-A[{alpha}]";
    }

    /// <summary>
    ///     Determines if the state of the <paramref name="first"/> instance equals the
    ///     state of the <paramref name="second"/> one.
    /// </summary>
    public static bool operator ==(RgbaPixel first, RgbaPixel second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the state of the <paramref name="first"/> instance doesn't equal the
    ///     state of the <paramref name="second"/> one.
    /// </summary>
    public static bool operator !=(RgbaPixel first, RgbaPixel second) =>
        !first.Equals(second);
}
