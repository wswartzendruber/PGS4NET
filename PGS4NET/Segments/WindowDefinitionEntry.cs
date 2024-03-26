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

namespace PGS4NET.Segments;

/// <summary>
///     Defines a window within the screen.
/// </summary>
public class WindowDefinitionEntry : IEquatable<WindowDefinitionEntry>
{
    /// <summary>
    ///     The ID of this window within the epoch.
    /// </summary>
    public byte Id { get; private set; }

    /// <summary>
    ///     The horizontal offset of the window’s top-left corner relative to the top-left
    ///     corner of the screen.
    /// </summary>
    public Area Area { get; set; }

    public WindowDefinitionEntry(byte id, Area area)
    {
        Id = id;
        Area = area;
    }

    /// <summary>
    ///     Determines if the fields of another <see cref="WindowDefinitionEntry" /> match this
    ///     one's.
    /// </summary>
    public bool Equals(WindowDefinitionEntry other)
    {
        if (Object.ReferenceEquals(this, other))
            return true;

        return
            other.Id == this.Id
            && other.Area == this.Area;
    }

    /// <summary>
    ///     Checks if the <paramrem name="other" /> instance is of the same type as this one and
    ///     then returns the value of the implementation-specific function, otherwise returns
    ///     <see langword="false" />.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == typeof(WindowDefinitionEntry)
            && Equals((WindowDefinitionEntry)other);

    /// <summary>
    ///     Returns the hash code of this instance taking into account the values of all fields.
    /// </summary>
    public override int GetHashCode()
    {
        unchecked
        {
            return (int)((uint)Id << 24);
        }
    }

    /// <summary>
    ///     Determines if the fields of two <see cref="WindowDefinitionEntry" />s match each
    ///     other.
    /// </summary>
    public static bool operator ==(WindowDefinitionEntry first
        , WindowDefinitionEntry second) => first.Equals(second);

    /// <summary>
    ///     Determines if the fields of two <see cref="WindowDefinitionEntry" />s don't match
    ///     each other.
    /// </summary>
    public static bool operator !=(WindowDefinitionEntry first
        , WindowDefinitionEntry second) => !first.Equals(second);
}
