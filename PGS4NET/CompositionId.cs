﻿/*
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
///     Defines a composition ID, combining an object and window identifier.
/// </summary>
public struct CompositionId
{
    /// <summary>
    ///     The object ID.
    /// </summary>
    public ushort ObjectId { get; private set; }

    /// <summary>
    ///     The window ID.
    /// </summary>
    public byte WindowId { get; private set; }

    /// <summary>
    ///     Initializes a new instance with the provided object and window IDs.
    /// </summary>
    public CompositionId(ushort objectId, byte windowId)
    {
        ObjectId = objectId;
        WindowId = windowId;
    }

    /// <summary>
    ///     Determines if the values of this instance matches another one's.
    /// </summary>
    public bool Equals(CompositionId other) =>
        other.ObjectId == this.ObjectId
            && other.WindowId == this.WindowId;

    /// <summary>
    ///     Determines if the type and values of this instance matches another one's.
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
            return ((int)ObjectId << 16) | ((int)WindowId << 8);
        }
    }

    /// <summary>
    ///     Determines if the values of two <see cref="CompositionId" />s match each other.
    /// </summary>
    public static bool operator ==(CompositionId first, CompositionId second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the values of two <see cref="CompositionId" />s don't match each
    ///     other.
    /// </summary>
    public static bool operator !=(CompositionId first, CompositionId second) =>
        !first.Equals(second);
}
