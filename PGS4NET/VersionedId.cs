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
///     A versioned identifier.
/// </summary>
public struct VersionedId<T>
{
    /// <summary>
    ///     The ID.
    /// </summary>
    public T Id;

    /// <summary>
    ///     The version.
    /// </summary>
    public byte Version;
}
