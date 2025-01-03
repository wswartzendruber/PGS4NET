/*
 * Copyright 2025 William Swartzendruber
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
    ///     Initializes a new instance with the provided values.
    /// </summary>
    /// <param name="id">
    ///     The ID.
    /// </param>
    /// <param name="version">
    ///     The version.
    /// </param>
    public VersionedId(T id, byte version)
    {
        Id = id;
        Version = version;
    }

    /// <summary>
    ///     Determines if the state of the <paramref name="other"/> instance equals this one's.
    /// </summary>
    public bool Equals(VersionedId<T> other) =>
        other.Id.Equals(Id)
            && other.Version == Version;

    /// <summary>
    ///     Determines if the type and state of the <paramref name="other"/> instance equals
    ///     this one's.
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
    ///     Determines if the state of the <paramref name="first"/> instance equals the
    ///     state of the <paramref name="second"/> one.
    /// </summary>
    public static bool operator ==(VersionedId<T> first, VersionedId<T> second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the state of the <paramref name="first"/> instance doesn't equal the
    ///     state of the <paramref name="second"/> one.
    /// </summary>
    public static bool operator !=(VersionedId<T> first, VersionedId<T> second) =>
        !first.Equals(second);
}
