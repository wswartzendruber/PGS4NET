/*
 * Copyright 2024 William Swartzendruber
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
public struct PgsTimeStamp : IEquatable<PgsTimeStamp>, IComparable<PgsTimeStamp>
{
    /// <summary>
    ///     The number of ticks as used by the PTS and DTS fields of PGS segments. 90,000 ticks
    ///     make one second.
    /// </summary>
    public uint Ticks { get; private set; }

    /// <summary>
    ///     Initializes a new instance with the given tick count.
    /// </summary>
    /// <param name="ticks">
    ///     The number of ticks as used by the PTS and DTS fields of PGS segments. 90,000 ticks
    ///     make one second.
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
    ///     The millisecond count which must not exceed 47,721,858.
    /// </param>
    public static PgsTimeStamp FromMilliseconds(int milliseconds)
    {
        if (milliseconds > 47_721_858)
        {
            throw new ArgumentOutOfRangeException("milliseconds", milliseconds
                , "The milliseconds value cannot exceed 47,721,858.");
        }

        return new PgsTimeStamp(milliseconds * 90);
    }

    /// <summary>
    ///     Compares this instance to another <see cref="PgsTimeStamp" /> one.
    /// </summary>
    /// <returns>
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 A negative value if this instance is less than the
    ///                 <paramref name="other" /> one.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Zero if this instance is equal to the <paramref name="other" /> one.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 A positive value if this instance is greater than the
    ///                 <paramref name="other" /> one.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </returns>
    public int CompareTo(PgsTimeStamp other) =>
        this.Ticks.CompareTo(other.Ticks);

    /// <summary>
    ///     Determines if the value of this instance matches another one's.
    /// </summary>
    public bool Equals(PgsTimeStamp other) => other.Ticks == this.Ticks;

    /// <summary>
    ///     Determines if the type and value of this instance matches another one's.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == typeof(PgsTimeStamp) && Equals((PgsTimeStamp)other);

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
    ///     Determines if the values of two <see cref="PgsTimeStamp" />s match each other.
    /// </summary>
    public static bool operator ==(PgsTimeStamp first, PgsTimeStamp second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the values of two <see cref="PgsTimeStamp" />s don't match each other.
    /// </summary>
    public static bool operator !=(PgsTimeStamp first, PgsTimeStamp second) =>
        !first.Equals(second);

    /// <summary>
    ///     Determines if the <paramref name="left" /> value is greater than the
    ///     <paramref name="right" /> value.
    /// </summary>
    public static bool operator >(PgsTimeStamp left, PgsTimeStamp right) =>
        left.Ticks > right.Ticks;

    /// <summary>
    ///     Determines if the <paramref name="left" /> value is less than the
    ///     <paramref name="right" /> value.
    /// </summary>
    public static bool operator <(PgsTimeStamp left, PgsTimeStamp right) =>
        left.Ticks < right.Ticks;

    /// <summary>
    ///     Determines if the <paramref name="left" /> value is greater than or equal to the
    ///     <paramref name="right" /> value.
    /// </summary>
    public static bool operator >=(PgsTimeStamp left, PgsTimeStamp right) =>
        left.Ticks >= right.Ticks;

    /// <summary>
    ///     Determines if the <paramref name="left" /> value is less than or equal to the
    ///     <paramref name="right" /> value.
    /// </summary>
    public static bool operator <=(PgsTimeStamp left, PgsTimeStamp right) =>
        left.Ticks <= right.Ticks;

    /// <summary>
    ///     Adds the <paramref name="addend" /> to the <paramref name="augend" />.
    /// </summary>
    public static PgsTimeStamp operator +(PgsTimeStamp augend, PgsTimeStamp addend) =>
        new PgsTimeStamp(augend.Ticks + addend.Ticks);

    /// <summary>
    ///     Subtracts the <paramref name="subtrahend" /> from the <paramref name="minuend" />.
    /// </summary>
    public static PgsTimeStamp operator -(PgsTimeStamp minuend, PgsTimeStamp subtrahend) =>
        new PgsTimeStamp(minuend.Ticks - subtrahend.Ticks);

    /// <summary>
    ///     Implicitly returns the value of the <see cref="Ticks" /> property.
    /// </summary>
    /// <returns>
    ///     The number of ticks as used by the PTS and DTS fields of PGS segments. 90,000 ticks
    ///     make one second.
    /// </returns>
    public static implicit operator uint(PgsTimeStamp pts) => pts.Ticks;

    /// <summary>
    ///     Implicitly creates a new instance using the provided <paramref name="ticks" />
    ///     value.
    /// </summary>
    /// <param name="ticks">
    ///     The number of ticks as used by the PTS and DTS fields of PGS segments. 90,000 ticks
    ///     make one second.
    /// </param>
    public static implicit operator PgsTimeStamp(uint ticks) => new PgsTimeStamp(ticks);
}
