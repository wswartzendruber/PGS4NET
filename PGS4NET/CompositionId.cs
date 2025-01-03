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
///     Defines a composition ID, combining object and window IDs.
/// </summary>
public struct CompositionId : IEquatable<CompositionId>
{
    /// <summary>
    ///     The object ID.
    /// </summary>
    public int ObjectId { get; private set; }

    /// <summary>
    ///     The window ID.
    /// </summary>
    public byte WindowId { get; private set; }

    /// <summary>
    ///     Initializes a new instance with the provided values.
    /// </summary>
    /// <param name="objectId">
    ///     The object ID.
    /// </param>
    /// <param name="windowId">
    ///     The window ID.
    /// </param>
    public CompositionId(int objectId, byte windowId)
    {
        ObjectId = objectId;
        WindowId = windowId;
    }

    /// <summary>
    ///     Determines if the state of the <paramref name="other"/> instance equals this one's.
    /// </summary>
    public bool Equals(CompositionId other) =>
        other.ObjectId == ObjectId
            && other.WindowId == WindowId;

    /// <summary>
    ///     Determines if the type and state of the <paramref name="other"/> instance equals
    ///     this one's.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == typeof(CompositionId) && Equals((CompositionId)other);

    /// <summary>
    ///     Calculates and returns the hash code of this instance using its immutable state.
    /// </summary>
    public override int GetHashCode()
    {
        unchecked
        {
            return ObjectId << 16 | WindowId << 8;
        }
    }

    /// <summary>
    ///     Determines if the state of the <paramref name="first"/> instance equals the
    ///     state of the <paramref name="second"/> one.
    /// </summary>
    public static bool operator ==(CompositionId first, CompositionId second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the state of the <paramref name="first"/> instance doesn't equal the
    ///     state of the <paramref name="second"/> one.
    /// </summary>
    public static bool operator !=(CompositionId first, CompositionId second) =>
        !first.Equals(second);
}
