/*
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
///     Defines a composition ID, combining an object and window identifier.
/// </summary>
public struct CompositionId : IEquatable<CompositionId>
{
    private static readonly Type ThisType = typeof(CompositionId);

    /// <summary>
    ///     The object ID.
    /// </summary>
    public ushort ObjectId;

    /// <summary>
    ///     The window ID.
    /// </summary>
    public byte WindowId;

    /// <summary>
    ///     Determines if the fields of another <see cref="CompositionId" /> match this one's.
    /// </summary>
    public bool Equals(CompositionId other)
    {
        if (Object.ReferenceEquals(this, other))
            return true;

        return
            other.ObjectId == this.ObjectId
            && other.WindowId == this.WindowId;
    }

    /// <summary>
    ///     Checks if the <paramrem name="other" /> instance is of the same type as this one and
    ///     then returns the value of the implementation-specific function, otherwise returns
    ///     <see langword="false" />.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == ThisType && Equals((CompositionId)other);

    /// <summary>
    ///     Returns the hash code of this instance taking into account the values of all fields.
    /// </summary>
    public override int GetHashCode() => (ObjectId, WindowId).GetHashCode();

    /// <summary>
    ///     Determines if the fields of two <see cref="CompositionId" />s match each other.
    /// </summary>
    public static bool operator ==(CompositionId first, CompositionId second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the fields of two <see cref="CompositionId" />s don't match each
    ///     other.
    /// </summary>
    public static bool operator !=(CompositionId first, CompositionId second) =>
        !first.Equals(second);
}
