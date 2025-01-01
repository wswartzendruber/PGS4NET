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
    ///     Determines if the values of this instance match another one's.
    /// </summary>
    public bool Equals(VersionedId<T> other) =>
        other.Id.Equals(Id)
            && other.Version == Version;

    /// <summary>
    ///     Determines if the type and values of this instance match another one's.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == typeof(VersionedId<T>) && Equals((VersionedId<T>)other);

    /// <summary>
    ///     Calculates and returns the hash code of this instance using its immutable state.
    /// </summary>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;

            hash = hash * 23 + Id.GetHashCode();
            hash = hash * 23 + Version;

            return hash;
        }
    }

    /// <summary>
    ///     Determines if the values of two <see cref="VersionedId{T}"/>s match each other.
    /// </summary>
    public static bool operator ==(VersionedId<T> first, VersionedId<T> second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the values of two <see cref="VersionedId{T}"/>s don't match each
    ///     other.
    /// </summary>
    public static bool operator !=(VersionedId<T> first, VersionedId<T> second) =>
        !first.Equals(second);
}
