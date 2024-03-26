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
    ///     The number of ticks. 90,000 ticks makes one second.
    /// </summary>
    public uint Ticks { get; private set; }

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

    /// <summary>
    ///     Determines if the state of another <see cref="PgsTimeStamp" /> matches this one's.
    /// </summary>
    public bool Equals(PgsTimeStamp other)
    {
        if (Object.ReferenceEquals(this, other))
            return true;

        return
            this.Ticks == other.Ticks;
    }

    /// <summary>
    ///     Checks if the <paramrem name="other" /> instance is of the same type as this one and
    ///     then returns the value of the implementation-specific function, otherwise returns
    ///     <see langword="false" />.
    /// </summary>
    public override bool Equals(object? other) =>
        other?.GetType() == typeof(PgsTimeStamp) && Equals((PgsTimeStamp)other);

    public int CompareTo(PgsTimeStamp other) =>
        this.Ticks.CompareTo(other.Ticks);

    /// <summary>
    ///     Returns the hash code of this instance taking into account the values of all
    ///     readonly properties.
    /// </summary>
    public override int GetHashCode()
    {
        unchecked
        {
            return (int)Ticks;
        }
    }

    /// <summary>
    ///     Determines if the state of two <see cref="PgsTimeStamp" />s match each other.
    /// </summary>
    public static bool operator ==(PgsTimeStamp first, PgsTimeStamp second) =>
        first.Equals(second);

    /// <summary>
    ///     Determines if the state of two <see cref="PgsTimeStamp" />s don't match each other.
    /// </summary>
    public static bool operator !=(PgsTimeStamp first, PgsTimeStamp second) =>
        !first.Equals(second);

    public static PgsTimeStamp operator +(PgsTimeStamp augend, PgsTimeStamp addend) =>
        new PgsTimeStamp(augend.Ticks - addend.Ticks);

    public static PgsTimeStamp operator -(PgsTimeStamp minuend, PgsTimeStamp subtrahend) =>
        new PgsTimeStamp(minuend.Ticks - subtrahend.Ticks);

    public static bool operator >(PgsTimeStamp left, PgsTimeStamp right) =>
        left.Ticks > right.Ticks;

    public static bool operator <(PgsTimeStamp left, PgsTimeStamp right) =>
        left.Ticks < right.Ticks;

    public static bool operator >=(PgsTimeStamp left, PgsTimeStamp right) =>
        left.Ticks >= right.Ticks;

    public static bool operator <=(PgsTimeStamp left, PgsTimeStamp right) =>
        left.Ticks <= right.Ticks;
}
