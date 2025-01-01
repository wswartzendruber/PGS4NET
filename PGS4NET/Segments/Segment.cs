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
///     Represents the common aspects shared by all types of segments.
/// </summary>
/// <remarks>
///     A segment is the most fundamental data structure within a PGS bitstream. Multiple
///     segments come together in a well-defined manner to form a display set. There are five
///     types of segments and within a display set, they typically appear in this order:
///     <list type="number">
///         <item>
///             <description>
///                 <see cref="PresentationCompositionSegment"/>
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="WindowDefinitionSegment"/>
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="PaletteDefinitionSegment"/>
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="ObjectDefinitionSegment"/>
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="EndSegment"/>
///             </description>
///         </item>
///     </list>
/// </remarks>
public abstract class Segment
{
    /// <summary>
    ///     The timestamp indicating when composition decoding should start. In practice, this
    ///     is the time at which the composition is displayed, repeated, modified, or removed.
    /// </summary>
    public PgsTimeStamp Pts { get; set; }

    /// <summary>
    ///     The timestamp indicating when the composition should be enacted. In practice, this
    ///     value is always zero.
    /// </summary>
    public PgsTimeStamp Dts { get; set; }

    /// <summary>
    ///     Initializes a new instance with default values.
    /// </summary>
    protected Segment()
    {
    }

    /// <summary>
    ///     Initializes a new instance with the provided values.
    /// </summary>
    /// <param name="pts">
    ///     The timestamp indicating when composition decoding should start. In practice, this
    ///     is the time at which the composition is displayed, repeated, modified, or removed.
    ///     All PTS values within a display set should match.
    /// </param>
    /// <param name="dts">
    ///     The timestamp indicating when the composition should be enacted. In practice, this
    ///     value is always zero.
    /// </param>
    protected Segment(PgsTimeStamp pts, PgsTimeStamp dts)
    {
        Pts = pts;
        Dts = dts;
    }
}
