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
using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

/// <summary>
///     Defines a window within the screen.
/// </summary>
public struct DisplayWindow : IEquatable<DisplayWindow>
{
    /// <summary>
    ///     The horizontal offset of the window’s top-left corner relative to the top-left
    ///     corner of the screen.
    /// </summary>
    public ushort X;

    /// <summary>
    ///     The vertical offset of the window’s top-left corner relative to the top-left corner
    ///     of the screen.
    /// </summary>
    public ushort Y;

    /// <summary>
    ///     The width of the window in pixels.
    /// </summary>
    public ushort Width;

    /// <summary>
    ///     The height of the window in pixels.
    /// </summary>
    public ushort Height;

    /// <summary>
    ///     Determines if the fields of another <see cref="DisplayWindow" /> match this one's.
    /// </summary>
    public bool Equals(DisplayWindow other)
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
        other?.GetType() == typeof(DisplayWindow) && Equals((DisplayWindow)other);

    /// <summary>
    ///     Returns the hash code of this instance taking into account the values of all fields.
    /// </summary>
    public override int GetHashCode() => (X, Y, Width, Height).GetHashCode();

    /// <summary>
    ///     Determines if the fields of two <see cref="DisplayWindow" />s match each other.
    /// </summary>
    public static bool operator ==(DisplayWindow first, DisplayWindow second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the fields of two <see cref="DisplayWindow" />s don't match each
    ///     other.
    /// </summary>
    public static bool operator !=(DisplayWindow first, DisplayWindow second) =>
        !first.Equals(second);
}
