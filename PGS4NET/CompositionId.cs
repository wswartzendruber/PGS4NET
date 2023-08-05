﻿/*
 * Copyright 2023 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

namespace PGS4NET;

/// <summary>
///     Defines a composition ID, combining an object and window identifier.
/// </summary>
public struct CompositionId
{
    /// <summary>
    ///     The object ID.
    /// </summary>
    public ushort ObjectId;

    /// <summary>
    ///     The window ID.
    /// </summary>
    public byte WindowId;
}
