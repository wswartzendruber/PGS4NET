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

namespace PGS4NET.Segments;

/// <summary>
///     Defines a palette entry within a palette set.
/// </summary>
/// <remarks>
///     The role of a palette entry is to define or update exact pixel color, as later
///     referenced by any objects also defined within an epoch.
/// </remarks>
public struct PaletteDefinitionEntry : IEquatable<PaletteDefinitionEntry>
{
    /// <summary>
    ///     The ID of this palette entry, which should be unique within an epoch.
    /// </summary>
    public byte Id { get; private set; }

    /// <summary>
    ///     Defines the color properties of the palette entry.
    /// </summary>
    public PgsPixel Pixel { get; private set; }

    public PaletteDefinitionEntry(byte id, PgsPixel pixel)
    {
        Id = id;
        Pixel = pixel;
    }

    /// <summary>
    ///     Determines if the state of another <see cref="PaletteDefinitionEntry" /> match
    ///     this one's.
    /// </summary>
    public bool Equals(PaletteDefinitionEntry other) =>
        other.Id == this.Id
            && other.Pixel == this.Pixel;

    /// <summary>
    ///     Checks if the <paramrem name="other" /> instance is of the same type as this one and
    ///     then returns the value of the implementation-specific function, otherwise returns
    ///     <see langword="false" />.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == typeof(PaletteDefinitionEntry)
            && Equals((PaletteDefinitionEntry)other);

    /// <summary>
    ///     Returns the hash code of this instance taking into account the values of all
    ///     readonly properties.
    /// </summary>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;

            hash = hash * 23 + Id;
            hash = hash * 23 + Pixel.GetHashCode();

            return hash;
        }
    }

    /// <summary>
    ///     Determines if the state of two <see cref="PaletteDefinitionEntry" />s match each
    ///     other.
    /// </summary>
    public static bool operator ==(PaletteDefinitionEntry first
        , PaletteDefinitionEntry second) => first.Equals(second);

    /// <summary>
    ///     Determines if the state of two <see cref="PaletteDefinitionEntry" />s don't match
    ///     each other.
    /// </summary>
    public static bool operator !=(PaletteDefinitionEntry first
        , PaletteDefinitionEntry second) => !first.Equals(second);
}
