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
using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

/// <summary>
///     Defines a window area within a display set.
/// </summary>
public class DisplayWindow
{
    /// <summary>
    ///     The horizontal offset of the window's top-left corner.
    /// </summary>
    public ushort X { get; set; }

    /// <summary>
    ///     The vertical offset of the window's top-left corner.
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

    public DisplayWindow()
    {
    }

    public DisplayWindow(ushort x, ushort y, ushort width, ushort height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}
