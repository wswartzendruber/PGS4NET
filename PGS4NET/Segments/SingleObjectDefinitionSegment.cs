﻿/*
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
///     Defines a complete object within an epoch.
/// </summary>
public class SingleObjectDefinitionSegment : ObjectDefinitionSegment
{
    /// <summary>
    ///     The width of this object in pixels.
    /// </summary>
    public ushort Width;

    /// <summary>
    ///     The height of this object in pixels.
    /// </summary>
    public ushort Height;
}
