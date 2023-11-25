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

namespace PGS4NET.Segments;

/// <summary>
///     Defines a mapping between an object (or an area of one) and a window within an epoch.
/// </summary>
public struct CompositionObject : IEquatable<CompositionObject>
{
    private static readonly Type ThisType = typeof(CompositionObject);

    /// <summary>
    ///     The ID of the object within the epoch.
    /// </summary>
    public ushort ObjectId;

    /// <summary>
    ///     The ID of the window within the epoch.
    /// </summary>
    public byte WindowId;

    /// <summary>
    ///     The horizontal offset of the object’s top-left corner relative to the top-left
    ///     corner of the screen. If the object is cropped, then this applies only to the
    ///     visible area.
    /// </summary>
    public ushort X;

    /// <summary>
    ///     The vertical offset of the object’s top-left corner relative to the top-left corner
    ///     of the screen. If the object is cropped, then this applies only to the visible area.
    /// </summary>
    public ushort Y;

    /// <summary>
    ///     Whether or not the composition object is forced. This is typically used to translate
    ///     foreign dialogue or text that appears.
    /// </summary>
    public bool Forced;

    /// <summary>
    ///     If set, defines the visible area of the object. Otherwise, the entire object is
    ///     shown.
    /// </summary>
    public CroppedArea? Crop;

    /// <summary>
    ///     Determines if the fields of another <see cref="CompositionObject" /> match this
    ///     one's.
    /// </summary>
    public bool Equals(CompositionObject other)
    {
        if (Object.ReferenceEquals(this, other))
            return true;

        return
            other.ObjectId == this.ObjectId
            && other.WindowId == this.WindowId
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
        other?.GetType() == ThisType && Equals((CompositionObject)other);

    /// <summary>
    ///     Returns the hash code of this instance taking into account the values of all fields.
    /// </summary>
    public override int GetHashCode() => (ObjectId, WindowId, X, Y, Forced, Crop).GetHashCode();

    /// <summary>
    ///     Determines if the fields of two <see cref="CompositionObject" />s match each other.
    /// </summary>
    public static bool operator ==(CompositionObject first, CompositionObject second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the fields of two <see cref="CompositionObject" />s don't match each
    ///     other.
    /// </summary>
    public static bool operator !=(CompositionObject first, CompositionObject second) =>
        !first.Equals(second);
}
