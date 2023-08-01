﻿/*
 * Copyright 2023 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

/// <summary>
///     Defines a window within the screen.
/// </summary>
public struct Window
{
    /// <summary>
    ///     The horizontal offset of the window’s top-left corner relative to the top-left
    ///     corner of the screen.
    /// </summary>
    public ushort X;

    /// <summary>
    ///     The vertical offset of the window’s top-left corner relative to the top-left corner
    ///     of the screen.
    /// </summary>
    public ushort Y;

    /// <summary>
    ///     The width of the window in pixels.
    /// </summary>
    public ushort Width;

    /// <summary>
    ///     The height of the window in pixels.
    /// </summary>
    public ushort Height;
}
