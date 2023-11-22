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

namespace PGS4NET;

/// <summary>
///     Represents a PGS time stamp accurate to 90 kHz, with 90,000 ticks composing one second.
/// </summary>
public struct PgsTimeStamp
{
    /// <summary>
    ///     The number of ticks. 90,000 ticks makes one second.
    /// </summary>
    public uint Ticks;

    /// <summary>
    ///     Initializes a new instance with the given tick count.
    /// </summary>
    /// <param name="ticks">
    ///     The tick count of the time stamp.
    /// </param>
    public PgsTimeStamp(uint ticks)
    {
        Ticks = ticks;
    }

    /// <summary>
    ///     Returns the number of milliseconds represented by the time stamp. The return value
    ///     is rounded down to the nearest millisecond.
    /// </summary>
    public uint ToMilliseconds() => Ticks / 90;

    /// <summary>
    ///     Initializes a new instance using the specified millisecond value.
    /// </summary>
    /// <param name="milliseconds">
    ///     The millisecond count; must not exceed 47,721,858.
    /// </param>
    public static PgsTimeStamp FromMilliseconds(uint milliseconds)
    {
        if (milliseconds > 47_721_858)
        {
            throw new ArgumentOutOfRangeException("milliseconds", milliseconds
                , "The milliseconds value cannot exceed 47,721,858.");
        }

        return new PgsTimeStamp(milliseconds * 90);
    }

    /// <summary>
    ///     Implicitly returns the <see cref="Ticks" /> value.
    /// </summary>
    public static implicit operator uint(PgsTimeStamp pts) => pts.Ticks;

    /// <summary>
    ///     Implicitly creates a new instance using the provided <paramref name="ticks" />
    ///     value.
    /// </summary>
    /// <param name="ticks">
    ///     The tick count.
    /// </param>
    public static implicit operator PgsTimeStamp(uint ticks) => new PgsTimeStamp(ticks);
}
