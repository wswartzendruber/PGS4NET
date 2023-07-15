/*
 * Copyright 2023 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

namespace PGS4NET.Segment;

using System.Collections.Generic;

/// <summary>
///     Defines a Window Definition Segment (WDS).
/// </summary>
public class WindowDefinitionSegment : Segment
{
    /// <summary>
    ///     Defines the window regions within the screen for this epoch.
    /// </summary>
    public IList<WindowDefinition> Definitions = new List<WindowDefinition>();
}
