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
///     Represents a location with X and Y offsets.
/// </summary>
public struct Position : IEquatable<Position>
{
    /// <summary>
    ///     The horizontal offset of the positions's top-left corner relative to the top-left
    ///     corner of the object itself.
    /// </summary>
    public ushort X { get; private set; }

    /// <summary>
    ///     The vertical offset of the positions's top-left corner relative to the top-left
    ///     corner of the object itself.
    /// </summary>
    public ushort Y { get; private set; }

    public Position(ushort x, ushort y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    ///     Determines if the coordinates of another <see cref="Position" /> matches this one's.
    /// </summary>
    public bool Equals(Position other) =>
        other.X == this.X
            && other.Y == this.Y;

    /// <summary>
    ///     Checks if the <paramrem name="other" /> instance is of the same type as this one and
    ///     then returns the value of the implementation-specific function, otherwise returns
    ///     <see langword="false" />.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == typeof(Position) && Equals((Position)other);

    /// <summary>
    ///     Returns the hash code of this instance taking into account the values of all
    ///     readonly properties.
    /// </summary>
    public override int GetHashCode()
    {
        unchecked
        {
            return ((int)X << 16) | (int)Y;
        }
    }

    /// <summary>
    ///     Determines if the state of two <see cref="Position" />s match each other.
    /// </summary>
    public static bool operator ==(Position first, Position second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the state of two <see cref="Position" />s don't match each other.
    /// </summary>
    public static bool operator !=(Position first, Position second) =>
        !first.Equals(second);
}
