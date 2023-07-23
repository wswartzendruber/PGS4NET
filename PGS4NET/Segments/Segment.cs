/*
 * Copyright 2023 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

namespace PGS4NET.Segments;

/// <summary>
///     Represents the common aspects shared by all types of segments.
/// </summary>
public abstract class Segment
{
    /// <summary>
    ///     The timestamp indicating when composition decoding should start. In practice, this
    ///     is the time at which the composition is displayed. All segments within a DS
    ///     typically have identical values here.
    /// </summary>
    public uint Pts;

    /// <summary>
    ///     The timestamp indicating when the composition should be displayed. In practice, this
    ///     value is always zero.
    /// </summary>
    public uint Dts;
}
