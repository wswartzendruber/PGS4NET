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
///     Defines an end segment (ES), the end of a display set.
/// </summary>
public class EndSegment : Segment
{
    /// <summary>
    ///     Initializes a new instance with default values.
    /// </summary>
    public EndSegment()
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
    public EndSegment(PgsTime presentationTime, PgsTime decodeStartTime) :
        base(presentationTime, decodeStartTime)
    {
    }
}
