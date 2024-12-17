/*
 * Copyright 2024 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using System.Collections.Generic;

namespace PGS4NET.DisplaySets;

/// <summary>
///     Defines a palette within a display set.
/// </summary>
public class DisplayPalette
{
    /// <summary>
    ///     The entries within this palette, each mapped according to its ID.
    /// </summary>
    public IDictionary<byte, YcbcraPixel> Entries { get; set; }

    /// <summary>
    ///     Initializes a new instance with default values and an empty list of entries.
    /// </summary>
    public DisplayPalette()
    {
        Entries = new Dictionary<byte, YcbcraPixel>();
    }

    /// <summary>
    ///     Initializes a new instance with the provided values.
    /// </summary>
    /// <param name="entries">
    ///     The entries within this palette, each mapped according to its ID.
    /// </param>
    public DisplayPalette(IDictionary<byte, YcbcraPixel> entries)
    {
        Entries = entries;
    }
}
