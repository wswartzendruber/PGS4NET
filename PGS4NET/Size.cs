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
///     Represents a size that has width and height.
/// </summary>
public struct Size
{
    /// <summary>
    ///     The width of the size.
    /// </summary>
    public ushort Width { get; private set; }

    /// <summary>
    ///     The height of the size.
    /// </summary>
    public ushort Height { get; private set; }

    public Size(ushort width, ushort height)
    {
        Width = width;
        Height = height;
    }
}
