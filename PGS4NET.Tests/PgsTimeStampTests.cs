/*
 * Copyright 2025 William Swartzendruber
 *
 * To the extent possible under law, the person who associated CC0 with this file has waived all
 * copyright and related or neighboring rights to this file.
 *
 * You should have received a copy of the CC0 legalcode along with this work. If not, see
 * <http://creativecommons.org/publicdomain/zero/1.0/>.
 *
 * SPDX-License-Identifier: CC0-1.0
 */

#pragma warning disable 1718

using PGS4NET;

namespace PGS4NET.Tests;

public class PgsTimeStampTests
{
    [Fact]
    public void Ticks()
    {
        var a = new PgsTimeStamp(1);
        var b = new PgsTimeStamp(1);
        var c = new PgsTimeStamp(2);

        Assert.Equal(1, a.Ticks);
        Assert.Equal(1, b.Ticks);
        Assert.Equal(2, c.Ticks);
    }

    [Fact]
    public void Milliseconds()
    {
        for (long ms = 0; ms <= PgsTimeStamp.MaxMilliseconds; ms++)
            Assert.Equal(ms, PgsTimeStamp.FromMilliseconds(ms).ToMilliseconds());
    }

    [Fact]
    public void Comparison()
    {
        var a = new PgsTimeStamp(1);
        var b = new PgsTimeStamp(1);
        var c = new PgsTimeStamp(2);

        Assert.True(a.CompareTo(a) == 0);
        Assert.True(a.CompareTo(b) == 0);
        Assert.True(a.CompareTo(c) < 0);
        Assert.True(c.CompareTo(a) > 0);
        Assert.False(a > a);
        Assert.False(a < a);
        Assert.True(a >= a);
        Assert.True(a <= a);
        Assert.False(a > b);
        Assert.False(a < b);
        Assert.True(a >= b);
        Assert.True(a <= b);
        Assert.False(a > c);
        Assert.True(a < c);
        Assert.False(a >= c);
        Assert.True(a <= c);
    }

    [Fact]
    public void HashCodes()
    {
        var a = new PgsTimeStamp(1);
        var b = new PgsTimeStamp(1);

        Assert.True(a.GetHashCode() == a.GetHashCode());
        Assert.True(a.GetHashCode() == b.GetHashCode());
    }

    [Fact]
    public void Equality()
    {
        var a = new PgsTimeStamp(1);
        var b = new PgsTimeStamp(1);
        var c = new PgsTimeStamp(2);

        Assert.True(a.Equals(a));
        Assert.True(a.Equals(b));
        Assert.False(a.Equals(c));
        Assert.True(a == a);
        Assert.True(a == b);
        Assert.True(a != c);
    }

    [Fact]
    public void Casting()
    {
        var a = new PgsTimeStamp(1);
        var c = new PgsTimeStamp(2);
        long au = a;
        long cu = c;
        PgsTimeStamp ap = 1;
        PgsTimeStamp cp = 2;

        Assert.Equal(1, au);
        Assert.Equal(2, cu);
        Assert.True(ap == a);
        Assert.True(cp == c);
    }

    [Fact]
    public void Exceptions()
    {
        try
        {
            _ = PgsTimeStamp.FromMilliseconds(PgsTimeStamp.MaxMilliseconds + 1);

            Assert.Fail("Was able to create PgsTimeStamp with excessive millisecond count.");
        }
        catch (ArgumentOutOfRangeException aoore)
        {
            Assert.Contains("The milliseconds value cannot exceed 47,721,858.", aoore.Message);
        }
    }
}
