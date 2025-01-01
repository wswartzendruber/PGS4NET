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

using System.Collections.Generic;

/// <summary>
///     Defines a window definition segment (WDS), consisting of multiple window entries.
/// <remarks>
/// </remarks>
///     A WDS lists window regions that are to be used within an epoch. Each display set that
///     has a WDS should only have one, where the single WDS has multiple windows defined.
/// </summary>
public class WindowDefinitionSegment : Segment
{
    /// <summary>
    ///     Defines the window regions within the screen for this epoch. All definitions within
    ///     an epoch should match and be defined for every display set of that epoch.
    /// </summary>
    public IList<WindowDefinitionEntry> Definitions { get; set; }

    /// <summary>
    ///     Initializes a new instance with default values.
    /// </summary>
    public WindowDefinitionSegment()
    {
        Definitions = new List<WindowDefinitionEntry>();
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
    /// <param name="definitions">
    ///     Defines the window regions within the screen for this epoch. All definitions within
    ///     an epoch should match and be defined for every display set of that epoch.
    /// </param>
    public WindowDefinitionSegment(PgsTimeStamp pts, PgsTimeStamp dts
        , IList<WindowDefinitionEntry> definitions) : base(pts, dts)
    {
        Definitions = definitions;
    }
}
