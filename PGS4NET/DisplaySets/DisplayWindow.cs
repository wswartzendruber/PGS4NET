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
    ///     The horizontal offset of the window's top-left corner relative to the top-left
    ///     corner of the object itself.
    /// </summary>
    public ushort X { get; private set; }

    /// <summary>
    ///     The vertical offset of the window's top-left corner relative to the top-left corner
    ///     of the object itself.
    /// </summary>
    public ushort Y { get; private set; }

    /// <summary>
    ///     The width of the window.
    /// </summary>
    public ushort Width { get; private set; }

    /// <summary>
    ///     The height of the window.
    /// </summary>
    public ushort Height { get; private set; }

    public DisplayWindow(ushort x, ushort y, ushort width, ushort height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}
