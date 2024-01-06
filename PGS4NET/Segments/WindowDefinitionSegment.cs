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
    ///     Defines the window regions within the screen for this epoch.
    /// </summary>
    public IList<WindowDefinitionEntry> Definitions = new List<WindowDefinitionEntry>();
}
