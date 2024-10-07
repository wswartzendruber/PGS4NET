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
///     Represents a RGBA pixel, which is always full-range.
/// </summary>
public struct RgbaPixel : IEquatable<RgbaPixel>
{
    /// <summary>
    ///     The mangitude of the red channel, ranging from <c>0</c> to <c>1</c>.
    /// </summary>
    public readonly double Red;

    /// <summary>
    ///     The mangitude of the green channel, ranging from <c>0</c> to <c>1</c>.
    /// </summary>
    public readonly double Green;

    /// <summary>
    ///     The mangitude of the blue channel, ranging from <c>0</c> to <c>1</c>.
    /// </summary>
    public readonly double Blue;

    /// <summary>
    ///     The alpha value (transparency ratio), ranging from <c>0</c> to <c>1</c>.
    /// </summary>
    public readonly double Alpha;

    /// <summary>
    ///     Creates a new instance with the provided color values.
    /// </summary>
    /// <param name="red">
    ///     The mangitude of the red channel, ranging from <c>0</c> to <c>1</c>.
    /// </param>
    /// <param name="green">
    ///     The mangitude of the green channel, ranging from <c>0</c> to <c>1</c>.
    /// </param>
    /// <param name="blue">
    ///     The mangitude of the blue channel, ranging from <c>0</c> to <c>1</c>.
    /// </param>
    /// <param name="alpha">
    ///     The alpha value (transparency ratio), ranging from <c>0</c> to <c>1</c>.
    /// </param>
    public RgbaPixel(double red, double green, double blue, double alpha)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
    }

    /// <summary>
    ///     Determines if the values of this instance matches another one's.
    /// </summary>
    public bool Equals(RgbaPixel other) =>
        other.Red == this.Red
            && other.Green == this.Green
            && other.Blue == this.Blue
            && other.Alpha == this.Alpha;

    /// <summary>
    ///     Determines if the type and values of this instance matches another one's.
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
    ///     Returns a string that represents this pixel in the form of <c>#RRGGBBAA</c> where
    ///     each component is hexadecimal-encoded.
    /// </summary>
    public override string ToString()
    {
        return $"R[{Red}]-G[{Green}]-B[{Blue}]-A[{Alpha}]";
    }

    /// <summary>
    ///     Determines if the values of two <see cref="RgbaPixel" />s match each other.
    /// </summary>
    public static bool operator ==(RgbaPixel first, RgbaPixel second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the values of two <see cref="RgbaPixel" />s don't match each other.
    /// </summary>
    public static bool operator !=(RgbaPixel first, RgbaPixel second) =>
        !first.Equals(second);
}
