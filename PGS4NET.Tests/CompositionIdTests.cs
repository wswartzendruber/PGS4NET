/*
 * Copyright 2024 William Swartzendruber
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

namespace PGS4NET.Tests;

public class CompositionIdTests
{
    [Fact]
    public void HashCodes()
    {
        var a = new CompositionId(1, 2);
        var b = new CompositionId(1, 2);

        Assert.True(a.GetHashCode() == a.GetHashCode());
        Assert.True(a.GetHashCode() == b.GetHashCode());
    }

    [Fact]
    public void Equality()
    {
        var a = new CompositionId(1, 2);
        var b = new CompositionId(1, 2);
        var c = new CompositionId(1, 3);
        var d = new CompositionId(2, 2);

        Assert.True(a.Equals(a));
        Assert.True(a.Equals(b));
        Assert.False(a.Equals(c));
        Assert.False(a.Equals(d));
        Assert.False(c.Equals(d));
        Assert.True(a == a);
        Assert.True(a == b);
        Assert.True(a != c);
        Assert.True(a != d);
        Assert.True(c != d);
    }
}
