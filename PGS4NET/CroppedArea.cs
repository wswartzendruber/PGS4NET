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
///     Defines the specific area within an object to be shown.
/// </summary>
public struct CroppedArea : IEquatable<CroppedArea>
{
    private static readonly Type ThisType = typeof(CroppedArea);

    /// <summary>
    ///     The horizontal offset of the area’s top-left corner relative to the top-left corner
    ///     of the object itself.
    /// </summary>
    public ushort X;

    /// <summary>
    ///     The vertical offset of the area’s top-left corner relative to the top-left corner of
    ///     the object itself.
    /// </summary>
    public ushort Y;

    /// <summary>
    ///     The width of the area.
    /// </summary>
    public ushort Width;

    /// <summary>
    ///     The height of the area.
    /// </summary>
    public ushort Height;

    /// <summary>
    ///     Determines if the fields of another <see cref="CroppedArea" /> match this one's.
    /// </summary>
    public bool Equals(CroppedArea other)
    {
        if (Object.ReferenceEquals(this, other))
            return true;

        return
            other.X == this.X
            && other.Y == this.Y
            && other.Width == this.Width
            && other.Height == this.Height;
    }

    /// <summary>
    ///     Checks if the <paramrem name="other" /> instance is of the same type as this one and
    ///     then returns the value of the implementation-specific function, otherwise returns
    ///     <see langword="false" />.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == ThisType && Equals((CroppedArea)other);

    /// <summary>
    ///     Returns the hash code of this instance taking into account the values of all fields.
    /// </summary>
    public override int GetHashCode() => (X, Y, Width, Height).GetHashCode();

    /// <summary>
    ///     Determines if the fields of two <see cref="CroppedArea" />s match each other.
    /// </summary>
    public static bool operator ==(CroppedArea first, CroppedArea second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the fields of two <see cref="CroppedArea" />s don't match each other.
    /// </summary>
    public static bool operator !=(CroppedArea first, CroppedArea second) =>
        !first.Equals(second);
}
