/*
 * Copyright 2023 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

namespace PGS4NET;

/// <summary>
///     Defines the specific area within an object to be shown.
/// </summary>
public struct CroppedArea
{
    /// <summary>
    ///     The horizontal offset of the area’s top-left corner relative to the top-left corner
    ///     of the object itself.
    /// </summary>
    public ushort X;

    /// <summary>
    ///     The vertical offset of the area’s top-left corner relative to the top-left corner of
    ///     the object itself.
    /// </summary>
    public ushort Y;

    /// <summary>
    ///     The width of the area.
    /// </summary>
    public ushort Width;

    /// <summary>
    ///     The height of the area.
    /// </summary>
    public ushort Height;
}
