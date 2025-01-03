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
///     Represents a PGS pixel, which is typically limited range.
/// </summary>
public struct YcbcraPixel : IEquatable<YcbcraPixel>
{
    /// <summary>
    ///     The gamma-corrected luminosity value of the pixel. In the typical case of
    ///     limited-range encoding, black is represented by a value of <c>16</c> while white is
    ///     represented by a value of <c>235</c>. For standard Blu-ray discs, the BT.709
    ///     transfer function is typically used. However, 4K UltraHD discs seem to use the
    ///     ST.2084 transfer function instead.
    /// </summary>
    public readonly byte Y;

    /// <summary>
    ///     The horizontal position of the pixel on the YCbCr color plane, starting from the
    ///     left and going to the right.
    /// </summary>
    public readonly byte Cb;

    /// <summary>
    ///     The vertical position of the pixel on the YCbCr color plane, starting from the
    ///     bottom and going up.
    /// </summary>
    public readonly byte Cr;

    /// <summary>
    ///     The alpha value (transparency ratio) of the pixel.
    /// </summary>
    public readonly byte Alpha;

    /// <summary>
    ///     Initializes a new instance with the provided values.
    /// </summary>
    /// <param name="y">
    ///     The gamma-corrected luminosity value of the pixel. In the typical case of
    ///     limited-range encoding, black is represented by a value of <c>16</c> while white is
    ///     represented by a value of <c>235</c>. For standard Blu-ray discs, the BT.709
    ///     transfer function is typically used. However, 4K UltraHD discs seem to use the
    ///     ST.2084 transfer function instead.
    /// </param>
    /// <param name="cb">
    ///     The horizontal position of the pixel on the YCbCr color plane, starting from the
    ///     left and going to the right.
    /// </param>
    /// <param name="cr">
    ///     The vertical position of the pixel on the YCbCr color plane, starting from the
    ///     bottom and going up.
    /// </param>
    /// <param name="alpha">
    ///     The alpha value (transparency ratio) of the pixel.
    /// </param>
    public YcbcraPixel(byte y, byte cb, byte cr, byte alpha)
    {
        Y = y;
        Cb = cb;
        Cr = cr;
        Alpha = alpha;
    }

    /// <summary>
    ///     Determines if the state of the <paramref name="other"/> instance equals this one's.
    /// </summary>
    public bool Equals(YcbcraPixel other) =>
        other.Y == Y
            && other.Cb == Cb
            && other.Cr == Cr
            && other.Alpha == Alpha;

    /// <summary>
    ///     Determines if the type and state of the <paramref name="other"/> instance equals
    ///     this one's.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == typeof(YcbcraPixel) && Equals((YcbcraPixel)other);

    /// <summary>
    ///     Calculates and returns the hash code of this instance using its immutable state.
    /// </summary>
    public override int GetHashCode()
    {
        unchecked
        {
            return Y << 24 | Cb << 16 | Cr << 8 | Alpha;
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
    ///     Determines if the state of the <paramref name="first"/> instance equals the
    ///     state of the <paramref name="second"/> one.
    /// </summary>
    public static bool operator ==(YcbcraPixel first, YcbcraPixel second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the state of the <paramref name="first"/> instance doesn't equal the
    ///     state of the <paramref name="second"/> one.
    /// </summary>
    public static bool operator !=(YcbcraPixel first, YcbcraPixel second) =>
        !first.Equals(second);
}
