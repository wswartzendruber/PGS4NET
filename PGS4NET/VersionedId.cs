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
///     A versioned identifier.
/// </summary>
public struct VersionedId<T> : IEquatable<VersionedId<T>>
    where T : IEquatable<T>
{
    private static readonly Type ThisType = typeof(VersionedId<T>);

    /// <summary>
    ///     The ID.
    /// </summary>
    public T Id;

    /// <summary>
    ///     The version.
    /// </summary>
    public byte Version;

    /// <summary>
    ///     Determines if the fields of another <see cref="VersionedId{T}" /> match this
    ///     one's.
    /// </summary>
    public bool Equals(VersionedId<T> other)
    {
        if (Object.ReferenceEquals(this, other))
            return true;

        return
            other.Id.Equals(this.Id)
            && other.Version == this.Version;
    }

    /// <summary>
    ///     Checks if the <paramrem name="other" /> instance is of the same type as this one and
    ///     then returns the value of the implementation-specific function, otherwise returns
    ///     <see langword="false" />.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == ThisType && Equals((VersionedId<T>)other);

    /// <summary>
    ///     Returns the hash code of this instance taking into account the values of all fields.
    /// </summary>
    public override int GetHashCode() => (Id, Version).GetHashCode();

    /// <summary>
    ///     Determines if the fields of two <see cref="VersionedId{T}" />s match each other.
    /// </summary>
    public static bool operator ==(VersionedId<T> first, VersionedId<T> second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the fields of two <see cref="VersionedId{T}" />s don't match each
    ///     other.
    /// </summary>
    public static bool operator !=(VersionedId<T> first, VersionedId<T> second) =>
        !first.Equals(second);
}
