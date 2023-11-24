/*
 * Copyright 2023 William Swartzendruber
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
///     Represents a PGS pixel.
/// </summary>
public struct PgsPixel
{
    /// <summary>
    ///     The range-limited, gamma-corrected luminosity value of the pixel. Black is
    ///     represented by a value of <c>16</c> while white is represented by a value of
    ///     <c>235</c>. For standard Blu-ray discs, the BT.709 transfer function is typically
    ///     used. However, 4K UltraHD discs seem to use the ST.2084 transfer function instead.
    /// </summary>
    public byte Y;

    /// <summary>
    ///     The vertical position of the pixel on the YCbCr color plane, starting from the
    ///     bottom and going up.
    /// </summary>
    public byte Cr;

    /// <summary>
    ///     The horizontal position of the pixel on the YCbCr color plane, starting from the
    ///     left and going to the right.
    /// </summary>
    public byte Cb;

    /// <summary>
    ///     The alpha value (transparency ratio) of the pixel.
    /// </summary>
    public byte Alpha;
}
