/*
 * Copyright 2023 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using System;
using System.Collections.Generic;
using System.IO;
using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

/// <summary>
///     Contains extensions against <see cref="Stream" /> for reading PGS display sets.
/// </summary>
public static partial class StreamExtensions
{
    /// <summary>
    ///     Reads the next PGS display set from a <see cref="Stream" />.
    /// </summary>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a sequence of segments cannot form a display set.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to read a display set.
    /// </exception>
    public static DisplaySet ReadDisplaySet(this Stream stream)
    {
        throw new NotImplementedException();
    }
}
