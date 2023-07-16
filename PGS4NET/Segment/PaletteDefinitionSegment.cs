/*
 * Copyright 2023 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using System.Collections.Generic;

namespace PGS4NET.Segment;

/// <summary>
///     Defines a set of palette entries within an epoch.
/// </summary>
/// <remarks>
///     Palette entries can be broken apart into sets so that they can be modified as a group
///     within an epoch. This can be used, for example, to provide a fade-out effect by
///     continuously updating the palette entries referenced by an object currently on the
///     screen.
/// </remarks>
public class PaletteDefinitionSegment : Segment
{
    /// <summary>
    ///     The ID of this pallete set within the epoch.
    /// </summary>
    public byte Id;

    /// <summary>
    ///     The version increment of this palette set.
    /// </summary>
    public byte Version;

    /// <summary>
    ///     Defines the individual palette entries in this set.
    /// </summary>
    public IList<PaletteEntry> Entries = new List<PaletteEntry>();
}
