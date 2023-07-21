/*
 * Copyright 2023 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using System.Collections.Generic;
using PGS4NET.Segment;

namespace PGS4NET.DisplaySet;

/// <summary>
///     Represents a composition of objects into windows.
/// </summary>
public class Composition
{
    /// <summary>
    ///     Starting at zero, this increments each time graphics are updated within an epoch.
    /// </summary>
    public ushort Number;

    /// <summary>
    ///     Defines the role of this DS within the larger epoch.
    /// </summary>
    public CompositionState State;

    /// <summary>
    ///     A collection of composition objects, each mapped according to its compound ID
    ///     (object ID + window ID).
    /// </summary>
    public IDictionary<CompoundId, CompositionObject> Objects
        = new Dictionary<CompoundId, CompositionObject>();
}
