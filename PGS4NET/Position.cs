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
///     Represents a location with X and Y offsets.
/// </summary>
public struct Position
{
    /// <summary>
    ///     The horizontal offset of the positions's top-left corner relative to the top-left
    ///     corner of the object itself.
    /// </summary>
    public ushort X { get; private set; }

    /// <summary>
    ///     The vertical offset of the positions's top-left corner relative to the top-left
    ///     corner of the object itself.
    /// </summary>
    public ushort Y { get; private set; }

    public Position(ushort x, ushort y)
    {
        X = x;
        Y = y;
    }
}
