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
    ///     The time at which a composition is displayed, repeated, updated, or removed. This
    ///     value should be consistent across all segments in a display set.
    /// </summary>
    public PgsTime PresentationTime { get; set; }

    /// <summary>
    ///     The time by which the segment needs to be available to the decoder in order to be
    ///     presented in time.
    /// </summary>
    public PgsTime DecodeStartTime { get; set; }

    /// <summary>
    ///     Initializes a new instance with default values.
    /// </summary>
    protected Segment()
    {
    }

    /// <summary>
    ///     Initializes a new instance with the provided values.
    /// </summary>
    /// <param name="presentationTime">
    ///     The time at which a composition is displayed, repeated, updated, or removed. This
    ///     value should be consistent across all segments in a display set.
    /// </param>
    /// <param name="decodeStartTime">
    ///     The time by which the segment needs to be available to the decoder in order to be
    ///     presented in time.
    /// </param>
    protected Segment(PgsTime presentationTime, PgsTime decodeStartTime)
    {
        PresentationTime = presentationTime;
        DecodeStartTime = decodeStartTime;
    }
}
