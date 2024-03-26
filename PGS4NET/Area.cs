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
///     Represents an area with X and Y offsets that has width and height.
/// </summary>
public struct Area : IEquatable<Area>
{
    /// <summary>
    ///     The horizontal offset of the area’s top-left corner relative to the top-left corner
    ///     of the object itself.
    /// </summary>
    public ushort X { get; private set; }

    /// <summary>
    ///     The vertical offset of the area’s top-left corner relative to the top-left corner of
    ///     the object itself.
    /// </summary>
    public ushort Y { get; private set; }

    /// <summary>
    ///     The width of the area.
    /// </summary>
    public ushort Width { get; private set; }

    /// <summary>
    ///     The height of the area.
    /// </summary>
    public ushort Height { get; private set; }

    public Area(ushort x, ushort y, ushort width, ushort height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    ///     Determines if the dimensions of another <see cref="Area" /> matches this one's.
    /// </summary>
    public bool Equals(Area other) =>
        other.X == this.X
            && other.Y == this.Y
            && other.Width == this.Width
            && other.Height == this.Height;

    /// <summary>
    ///     Checks if the <paramrem name="other" /> instance is of the same type as this one and
    ///     then returns the value of the implementation-specific function, otherwise returns
    ///     <see langword="false" />.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == typeof(Area) && Equals((Area)other);

    /// <summary>
    ///     Returns the hash code of this instance taking into account the values of all
    ///     readonly properties.
    /// </summary>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;

            hash = hash * 23 + ((int)X << 16) | (int)Y;
            hash = hash * 23 + ((int)Width << 16) | (int)Height;

            return hash;
        }
    }

    /// <summary>
    ///     Determines if the state of two <see cref="Area" />s match each other.
    /// </summary>
    public static bool operator ==(Area first, Area second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the state of two <see cref="Area" />s don't match each other.
    /// </summary>
    public static bool operator !=(Area first, Area second) =>
        !first.Equals(second);
}
