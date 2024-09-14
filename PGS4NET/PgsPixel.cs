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
///     Represents a PGS pixel.
/// </summary>
public struct PgsPixel : IEquatable<PgsPixel>
{
    /// <summary>
    ///     The range-limited, gamma-corrected luminosity value of the pixel. Black is
    ///     represented by a value of <c>16</c> while white is represented by a value of
    ///     <c>235</c>. For standard Blu-ray discs, the BT.709 transfer function is typically
    ///     used. However, 4K UltraHD discs seem to use the ST.2084 transfer function instead.
    /// </summary>
    public byte Y { get; private set; }

    /// <summary>
    ///     The vertical position of the pixel on the YCbCr color plane, starting from the
    ///     bottom and going up.
    /// </summary>
    public byte Cr { get; private set; }

    /// <summary>
    ///     The horizontal position of the pixel on the YCbCr color plane, starting from the
    ///     left and going to the right.
    /// </summary>
    public byte Cb { get; private set; }

    /// <summary>
    ///     The alpha value (transparency ratio) of the pixel.
    /// </summary>
    public byte Alpha { get; private set; }

    /// <summary>
    ///     Creates a new instance with the provided color values.
    /// </summary>
    /// <param name="y">
    ///     The range-limited, gamma-corrected luminosity value of the pixel. Black is
    ///     represented by a value of <c>16</c> while white is represented by a value of
    ///     <c>235</c>. For standard Blu-ray discs, the BT.709 transfer function is typically
    ///     used. However, 4K UltraHD discs seem to use the ST.2084 transfer function instead.
    /// </param>
    /// <param name="cr">
    ///     The vertical position of the pixel on the YCbCr color plane, starting from the
    ///     bottom and going up.
    /// </param>
    /// <param name="cb">
    ///     The horizontal position of the pixel on the YCbCr color plane, starting from the
    ///     left and going to the right.
    /// </param>
    /// <param name="alpha">
    ///     The alpha value (transparency ratio) of the pixel.
    /// </param>
    public PgsPixel(byte y, byte cr, byte cb, byte alpha)
    {
        Y = y;
        Cr = cr;
        Cb = cb;
        Alpha = alpha;
    }

    /// <summary>
    ///     Determines if the values of this instance matches another one's.
    /// </summary>
    public bool Equals(PgsPixel other) =>
        other.Y == this.Y
            && other.Cr == this.Cr
            && other.Cb == this.Cb
            && other.Alpha == this.Alpha;

    /// <summary>
    ///     Determines if the type and values of this instance matches another one's.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == typeof(PgsPixel) && Equals((PgsPixel)other);

    /// <summary>
    ///     Calculates and returns the hash code of this instance using its immutable state.
    /// </summary>
    public override int GetHashCode()
    {
        unchecked
        {
            return (int)Y << 24 | (int)Cr << 16 | (int)Cb << 8 | (int)Alpha;
        }
    }

    /// <summary>
    ///     Returns a string that represents this pixel in the form of <c>Y-Cb-Cr-A</c> where
    ///     each component is hexadecimal-encoded.
    /// </summary>
    public override string ToString()
    {
        return $"{Y.ToString("X2")}-{Cb.ToString("X2")}-{Cr.ToString("X2")}-"
            + $"{Alpha.ToString("X2")}";
    }

    /// <summary>
    ///     Determines if the values of two <see cref="PgsPixel" />s match each other.
    /// </summary>
    public static bool operator ==(PgsPixel first, PgsPixel second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the values of two <see cref="PgsPixel" />s don't match each other.
    /// </summary>
    public static bool operator !=(PgsPixel first, PgsPixel second) =>
        !first.Equals(second);
}
