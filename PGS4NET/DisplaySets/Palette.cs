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
using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

/// <summary>
///     Defines a palette within a display set.
/// </summary>
public class Palette
{
    /// <summary>
    ///     The entries within this palette, each mapped according to its ID.
    /// </summary>
    public IDictionary<byte, PaletteEntry> Entries = new Dictionary<byte, PaletteEntry>();
}
