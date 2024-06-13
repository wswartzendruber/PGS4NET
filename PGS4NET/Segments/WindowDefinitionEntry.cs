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
///     Defines a window within the screen.
/// </summary>
public struct WindowDefinitionEntry
{
    /// <summary>
    ///     The ID of this window within the epoch.
    /// </summary>
    public byte Id { get; private set; }

    /// <summary>
    ///     The horizontal offset of the window's top-left corner relative to the top-left
    ///     corner of the screen.
    /// </summary>
    public Area Area { get; private set; }

    public WindowDefinitionEntry(byte id, Area area)
    {
        Id = id;
        Area = area;
    }
}
