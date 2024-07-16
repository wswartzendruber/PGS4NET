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
public class WindowDefinitionEntry
{
    /// <summary>
    ///     The ID of this window within the epoch.
    /// </summary>
    public byte Id { get; set; }
    /// <summary>
    ///     The horizontal offset of the window's top-left corner relative to the top-left
    ///     corner of the object itself.
    /// </summary>
    public ushort X { get; set; }

    /// <summary>
    ///     The vertical offset of the window's top-left corner relative to the top-left corner
    ///     of the object itself.
    /// </summary>
    public ushort Y { get; set; }

    /// <summary>
    ///     The width of the window.
    /// </summary>
    public ushort Width { get; set; }

    /// <summary>
    ///     The height of the window.
    /// </summary>
    public ushort Height { get; set; }

    public WindowDefinitionEntry()
    {
    }

    public WindowDefinitionEntry(byte id, ushort x, ushort y, ushort width, ushort height)
    {
        Id = id;
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}
