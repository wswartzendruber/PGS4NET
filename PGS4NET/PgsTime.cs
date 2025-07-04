﻿/*
 * Copyright 2025 William Swartzendruber
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
///     Represents either a single point in time or a span of time within PGS.
/// </summary>
/// <remarks>
///     Time is accurate to 90 kHz with 90,000 ticks composing one second.
/// </remarks>
public struct PgsTime : IEquatable<PgsTime>, IComparable<PgsTime>
{
    /// <summary>
    ///     The highest millisecond value that can be represented by this class.
    /// </summary>
    public const long MaxMilliseconds = 47_721_858;

    /// <summary>
    ///     The number of ticks as used by the PTS and DTS fields of PGS segments. 90,000 ticks
    ///     make one second. This is fundamentally the value of the class.
    /// </summary>
    public long Ticks { get; private set; }

    /// <summary>
    ///     Initializes a new instance with the provided tick count.
    /// </summary>
    /// <param name="ticks">
    ///     The number of ticks as used by the PTS and DTS fields of PGS segments. 90,000 ticks
    ///     make one second.
    /// </param>
    public PgsTime(long ticks)
    {
        Ticks = ticks;
    }

    /// <summary>
    ///     Returns the number of milliseconds represented by the <see cref="Ticks"/> property
    ///     rounded down to the nearest millisecond.
    /// </summary>
    public long ToMilliseconds() =>
        Ticks / 90;

    /// <summary>
    ///     Initializes a new instance using the specified millisecond value.
    /// </summary>
    /// <param name="milliseconds">
    ///     The millisecond value which must not exceed <see cref="MaxMilliseconds"/>.
    /// </param>
    public static PgsTime FromMilliseconds(long milliseconds)
    {
        if (milliseconds > MaxMilliseconds)
        {
            throw new ArgumentOutOfRangeException("milliseconds", milliseconds,
                "The milliseconds value cannot exceed 47,721,858.");
        }

        return new PgsTime(milliseconds * 90);
    }

    /// <summary>
    ///     Compares the value of this instance to the value of the <paramref name="other"/>
    ///     one.
    /// </summary>
    /// <returns>
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 A negative value if this instance is less than the
    ///                 <paramref name="other"/> one.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Zero if this instance is equal to the <paramref name="other"/> one.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 A positive value if this instance is greater than the
    ///                 <paramref name="other"/> one.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </returns>
    public int CompareTo(PgsTime other) =>
        Ticks.CompareTo(other.Ticks);

    /// <summary>
    ///     Determines if the state of the <paramref name="other"/> instance equals this one's.
    /// </summary>
    public bool Equals(PgsTime other) =>
        other.Ticks == Ticks;

    /// <summary>
    ///     Determines if the type and state of the <paramref name="other"/> instance equals
    ///     this one's.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == typeof(PgsTime) && Equals((PgsTime)other);

    /// <summary>
    ///     Calculates and returns the hash code of this instance using its immutable state.
    /// </summary>
    public override int GetHashCode()
    {
        unchecked
        {
            return (int)Ticks;
        }
    }

    /// <summary>
    ///     Determines if the state of the <paramref name="first"/> instance equals the
    ///     state of the <paramref name="second"/> one.
    /// </summary>
    public static bool operator ==(PgsTime first, PgsTime second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the state of the <paramref name="first"/> instance doesn't equal the
    ///     state of the <paramref name="second"/> one.
    /// </summary>
    public static bool operator !=(PgsTime first, PgsTime second) =>
        !first.Equals(second);

    /// <summary>
    ///     Determines if the value of the <paramref name="left"/> instance is greater than the
    ///     value the <paramref name="right"/> one.
    /// </summary>
    public static bool operator >(PgsTime left, PgsTime right) =>
        left.Ticks > right.Ticks;

    /// <summary>
    ///     Determines if the value of the <paramref name="left"/> instance is less than than
    ///     the value the <paramref name="right"/> one.
    /// </summary>
    public static bool operator <(PgsTime left, PgsTime right) =>
        left.Ticks < right.Ticks;

    /// <summary>
    ///     Determines if the value of the <paramref name="left"/> instance is greater than or
    ///     equal to the value the <paramref name="right"/> one.
    /// </summary>
    public static bool operator >=(PgsTime left, PgsTime right) =>
        left.Ticks >= right.Ticks;

    /// <summary>
    ///     Determines if the value of the <paramref name="left"/> instance is less than or
    ///     equal to the value the <paramref name="right"/> one.
    /// </summary>
    public static bool operator <=(PgsTime left, PgsTime right) =>
        left.Ticks <= right.Ticks;

    /// <summary>
    ///     Adds the value of the <paramref name="addend"/> to the value of the
    ///     <paramref name="augend"/>.
    /// </summary>
    public static PgsTime operator +(PgsTime augend, PgsTime addend) =>
        new PgsTime(augend.Ticks + addend.Ticks);

    /// <summary>
    ///     Subtracts the value of the <paramref name="subtrahend"/> from the value of the
    ///     <paramref name="minuend"/>.
    /// </summary>
    public static PgsTime operator -(PgsTime minuend, PgsTime subtrahend) =>
        new PgsTime(minuend.Ticks - subtrahend.Ticks);

    /// <summary>
    ///     Implicitly returns the value of the <see cref="Ticks"/> property.
    /// </summary>
    /// <returns>
    ///     The number of ticks as used by the PTS and DTS fields of PGS segments. 90,000 ticks
    ///     make one second.
    /// </returns>
    public static implicit operator long(PgsTime timeStamp) =>
        timeStamp.Ticks;

    /// <summary>
    ///     Implicitly creates a new instance using the provided <paramref name="ticks"/>
    ///     value.
    /// </summary>
    /// <param name="ticks">
    ///     The number of ticks as used by the PTS and DTS fields of PGS segments. 90,000 ticks
    ///     make one second.
    /// </param>
    public static implicit operator PgsTime(long ticks) =>
        new PgsTime(ticks);
}
