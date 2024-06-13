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

namespace PGS4NET;

/// <summary>
///     Represents an area with X and Y offsets that has width and height.
/// </summary>
public struct Area
{
    /// <summary>
    ///     The horizontal offset of the area's top-left corner relative to the top-left corner
    ///     of the object itself.
    /// </summary>
    public ushort X { get; private set; }

    /// <summary>
    ///     The vertical offset of the area's top-left corner relative to the top-left corner of
    ///     the object itself.
    /// </summary>
    public ushort Y { get; private set; }

    /// <summary>
    ///     The width of the area.
    /// </summary>
    public ushort Width { get; private set; }

    /// <summary>
    ///     The height of the area.
    /// </summary>
    public ushort Height { get; private set; }

    public Area(ushort x, ushort y, ushort width, ushort height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public Area(Position position, Size size)
    {
        X = position.X;
        Y = position.Y;
        Width = size.Width;
        Height = size.Height;
    }

    public Position ToPosition() => new Position(X, Y);

    public Size ToSize() => new Size(Width, Height);
}
