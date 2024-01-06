/*
 * Copyright 2024 William Swartzendruber
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
/// <remarks>
///     A segment is the most fundamental data structure within a PGS bitstream. Multiple
///     segments come together in a well-defined manner to form a display set. There are five
///     types of segments and within a display set, they typically appear in this order:
///     <list type="number">
///         <item><see cref="PresentationCompositionSegment" /></item>
///         <item><see cref="WindowDefinitionSegment" /></item>
///         <item><see cref="PaletteDefinitionSegment" /></item>
///         <item><see cref="ObjectDefinitionSegment" /></item>
///         <item><see cref="EndSegment" /></item>
///     </list>
/// </remarks>
public abstract class Segment
{
    /// <summary>
    ///     The timestamp indicating when composition decoding should start. In practice, this
    ///     is the time at which the composition is displayed, repeated, modified, or removed.
    /// </summary>
    public PgsTimeStamp Pts;

    /// <summary>
    ///     The timestamp indicating when the composition should be enacted. In practice, this
    ///     value is always zero.
    /// </summary>
    public PgsTimeStamp Dts;
}
