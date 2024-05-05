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
///     Represents a size that has width and height.
/// </summary>
public struct Size : IEquatable<Size>
{
    /// <summary>
    ///     The width of the size.
    /// </summary>
    public ushort Width { get; private set; }

    /// <summary>
    ///     The height of the size.
    /// </summary>
    public ushort Height { get; private set; }

    public Size(ushort width, ushort height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    ///     Determines if the dimensions of another <see cref="Size" /> matches this one's.
    /// </summary>
    public bool Equals(Size other) =>
        other.Width == this.Width
            && other.Height == this.Height;

    /// <summary>
    ///     Checks if the <paramrem name="other" /> instance is of the same type as this one and
    ///     then returns the value of the implementation-specific function, otherwise returns
    ///     <see langword="false" />.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == typeof(Size) && Equals((Size)other);

    /// <summary>
    ///     Returns the hash code of this instance taking into account the values of all
    ///     readonly properties.
    /// </summary>
    public override int GetHashCode()
    {
        unchecked
        {
            return ((int)Width << 16) | (int)Height;
        }
    }

    /// <summary>
    ///     Determines if the state of two <see cref="Size" />s match each other.
    /// </summary>
    public static bool operator ==(Size first, Size second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the state of two <see cref="Size" />s don't match each other.
    /// </summary>
    public static bool operator !=(Size first, Size second) =>
        !first.Equals(second);
}
