/*
 * Copyright 2025 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

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
    ///     The horizontal offset of the window's top-left corner within the screen.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    ///     The vertical offset of the window's top-left corner within the screen.
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    ///     The width of the window.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    ///     The height of the window.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    ///     Initializes a new instance with default values.
    /// </summary>
    public WindowDefinitionEntry()
    {
    }

    /// <summary>
    ///     Initializes a new instance with the provided values.
    /// </summary>
    /// <param name="id">
    ///     The ID of this window within the epoch.
    /// </param>
    /// <param name="x">
    ///     The horizontal offset of the window's top-left corner within the screen.
    /// </param>
    /// <param name="y">
    ///     The vertical offset of the window's top-left corner within the screen.
    /// </param>
    /// <param name="width">
    ///     The width of the window.
    /// </param>
    /// <param name="height">
    ///     The height of the window.
    /// </param>
    public WindowDefinitionEntry(byte id, int x, int y, int width, int height)
    {
        Id = id;
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}
