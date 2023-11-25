﻿/*
 * Copyright 2023 William Swartzendruber
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
    private static readonly Type ThisType = typeof(PgsPixel);

    /// <summary>
    ///     The range-limited, gamma-corrected luminosity value of the pixel. Black is
    ///     represented by a value of <c>16</c> while white is represented by a value of
    ///     <c>235</c>. For standard Blu-ray discs, the BT.709 transfer function is typically
    ///     used. However, 4K UltraHD discs seem to use the ST.2084 transfer function instead.
    /// </summary>
    public byte Y;

    /// <summary>
    ///     The vertical position of the pixel on the YCbCr color plane, starting from the
    ///     bottom and going up.
    /// </summary>
    public byte Cr;

    /// <summary>
    ///     The horizontal position of the pixel on the YCbCr color plane, starting from the
    ///     left and going to the right.
    /// </summary>
    public byte Cb;

    /// <summary>
    ///     The alpha value (transparency ratio) of the pixel.
    /// </summary>
    public byte Alpha;

    /// <summary>
    ///     Determines if the fields of another <see cref="PgsPixel" /> match this one's.
    /// </summary>
    public bool Equals(PgsPixel other)
    {
        if (Object.ReferenceEquals(this, other))
            return true;

        return
            other.Y == this.Y
            && other.Cr == this.Cr
            && other.Cb == this.Cb
            && other.Alpha == this.Alpha;
    }

    /// <summary>
    ///     Checks if the <paramrem name="other" /> instance is of the same type as this one and
    ///     then returns the value of the implementation-specific function, otherwise returns
    ///     <see langword="false" />.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == ThisType && Equals((PgsPixel)other);

    /// <summary>
    ///     Returns the hash code of this instance taking into account the values of all fields.
    /// </summary>
    public override int GetHashCode() => (Y, Cr, Cb, Alpha).GetHashCode();

    /// <summary>
    ///     Determines if the fields of two <see cref="PgsPixel" />s match each other.
    /// </summary>
    public static bool operator ==(PgsPixel first, PgsPixel second) => first.Equals(second);

    /// <summary>
    ///     Determines if the fields of two <see cref="PgsPixel" />s don't match each other.
    /// </summary>
    public static bool operator !=(PgsPixel first, PgsPixel second) => !first.Equals(second);
}
