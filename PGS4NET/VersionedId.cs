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
///     A versioned identifier.
/// </summary>
public struct VersionedId<T> : IEquatable<VersionedId<T>>
    where T : IEquatable<T>
{
    /// <summary>
    ///     The ID.
    /// </summary>
    public T Id { get; private set; }

    /// <summary>
    ///     The version.
    /// </summary>
    public byte Version { get; private set; }

    /// <summary>
    ///     Initializes a new instance with the provided ID and version.
    /// </summary>
    public VersionedId(T id, byte version)
    {
        Id = id;
        Version = version;
    }

    /// <summary>
    ///     Determines if the state of another <see cref="VersionedId{T}" /> matches this
    ///     one's.
    /// </summary>
    public bool Equals(VersionedId<T> other) =>
        other.Id.Equals(this.Id)
            && other.Version == this.Version;

    /// <summary>
    ///     Checks if the <paramrem name="other" /> instance is of the same type as this one and
    ///     then returns the value of the implementation-specific function, otherwise returns
    ///     <see langword="false" />.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == typeof(VersionedId<T>) && Equals((VersionedId<T>)other);

    /// <summary>
    ///     Returns the hash code of this instance taking into account the values of all
    ///     readonly properties.
    /// </summary>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;

            hash = hash * 23 + Id.GetHashCode();
            hash = hash * 23 + (int)Version;

            return hash;
        }
    }

    /// <summary>
    ///     Determines if the state of two <see cref="VersionedId{T}" />s match each other.
    /// </summary>
    public static bool operator ==(VersionedId<T> first, VersionedId<T> second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the state of two <see cref="VersionedId{T}" />s don't match each
    ///     other.
    /// </summary>
    public static bool operator !=(VersionedId<T> first, VersionedId<T> second) =>
        !first.Equals(second);
}
