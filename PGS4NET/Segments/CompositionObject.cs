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
///     Defines a mapping between an object (or an area of one) and a window within an epoch.
/// </summary>
public class CompositionObject : IEquatable<CompositionObject>
{
    /// <summary>
    ///     The ID of the composition object.
    /// </summary>
    public CompositionId Id { get; private set; }

    /// <summary>
    ///     The horizontal offset of the object’s top-left corner relative to the top-left
    ///     corner of the screen. If the object is cropped, then this applies only to the
    ///     visible area.
    /// </summary>
    public ushort X { get; set; }

    /// <summary>
    ///     The vertical offset of the object’s top-left corner relative to the top-left corner
    ///     of the screen. If the object is cropped, then this applies only to the visible area.
    /// </summary>
    public ushort Y { get; set; }

    /// <summary>
    ///     Whether or not the composition object is forced. This is typically used to translate
    ///     foreign dialogue or text that appears.
    /// </summary>
    public bool Forced { get; set; }

    /// <summary>
    ///     If set, defines the visible area of the object. Otherwise, the entire object is
    ///     shown.
    /// </summary>
    public Area? Crop { get; set; }

    public CompositionObject(CompositionId id, ushort x, ushort y, bool forced, Area? crop)
        : this(id)
    {
        X = x;
        Y = y;
        Forced = forced;
        Crop = crop;
    }

    public CompositionObject(CompositionId id)
    {
        Id = id;
    }

    /// <summary>
    ///     Determines if the state of another <see cref="CompositionObject" /> matches this
    ///     one's.
    /// </summary>
    public bool Equals(CompositionObject other)
    {
        if (Object.ReferenceEquals(this, other))
            return true;

        return
            other.Id == this.Id
            && other.X == this.X
            && other.Y == this.Y
            && other.Forced == this.Forced
            && other.Crop == this.Crop;
    }

    /// <summary>
    ///     Checks if the <paramrem name="other" /> instance is of the same type as this one and
    ///     then returns the value of the implementation-specific function, otherwise returns
    ///     <see langword="false" />.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == typeof(CompositionObject) && Equals((CompositionObject)other);

    /// <summary>
    ///     Returns the hash code of this instance taking into account the values of all fields.
    /// </summary>
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    ///     Determines if the state of two <see cref="CompositionObject" />s match each other.
    /// </summary>
    public static bool operator ==(CompositionObject first, CompositionObject second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the state of two <see cref="CompositionObject" />s don't match each
    ///     other.
    /// </summary>
    public static bool operator !=(CompositionObject first, CompositionObject second) =>
        !first.Equals(second);
}
