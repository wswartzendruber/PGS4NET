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
    /// <param name="presentationTime">
    ///     The time at which a composition is displayed, repeated, updated, or removed. This
    ///     value should be consistent across all segments in a display set.
    /// </param>
    /// <param name="decodeStartTime">
    ///     The time by which the segment needs to be available to the decoder in order to be
    ///     presented in time.
    /// </param>
    /// <param name="definitions">
    ///     Defines the window regions within the screen for this epoch. All definitions within
    ///     an epoch should match and be defined for every display set of that epoch.
    /// </param>
    public WindowDefinitionSegment(PgsTime presentationTime, PgsTime decodeStartTime,
        IList<WindowDefinitionEntry> definitions) : base(presentationTime, decodeStartTime)
    {
        Definitions = definitions;
    }
}
