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
public struct ColorSpace : IEquatable<ColorSpace>
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
}
