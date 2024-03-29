﻿/*
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

using PGS4NET;

namespace PGS4NET.Tests;

public class ValueTests
{
    [Fact]
    public void Area()
    {
        var a = new Area(1, 2, 3, 4);
        var b = new Area(1, 2, 3, 4);
        var c = new Area(1, 2, 3, 5);

        Assert.True(a == a);
        Assert.True(a == b);
        Assert.True(a != c);
        Assert.True(a.GetHashCode() == a.GetHashCode());
        Assert.True(a.GetHashCode() == b.GetHashCode());
    }

    [Fact]
    public void CompositionId()
    {
        var a = new CompositionId(1, 2);
        var b = new CompositionId(1, 2);
        var c = new CompositionId(1, 3);

        Assert.True(a == a);
        Assert.True(a == b);
        Assert.True(a != c);
        Assert.True(a.GetHashCode() == a.GetHashCode());
        Assert.True(a.GetHashCode() == b.GetHashCode());
    }

    [Fact]
    public void PgsPixel()
    {
        var a = new PgsPixel(1, 2, 3, 4);
        var b = new PgsPixel(1, 2, 3, 4);
        var c = new PgsPixel(1, 2, 3, 5);

        Assert.True(a == a);
        Assert.True(a == b);
        Assert.True(a != c);
        Assert.True(a.GetHashCode() == a.GetHashCode());
        Assert.True(a.GetHashCode() == b.GetHashCode());
    }

    [Fact]
    public void PgsTimeStamp()
    {
        var a = new PgsTimeStamp(1);
        var b = new PgsTimeStamp(1);
        var c = new PgsTimeStamp(2);

        Assert.True(a == a);
        Assert.True(a == b);
        Assert.True(a != c);
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
        Assert.True(a.GetHashCode() == a.GetHashCode());
        Assert.True(a.GetHashCode() == b.GetHashCode());
    }

    [Fact]
    public void VersionedId()
    {
        var a = new VersionedId<int>(1, 2);
        var b = new VersionedId<int>(1, 2);
        var c = new VersionedId<int>(1, 3);
        var d = new VersionedId<long>(1, 2);
        var e = new VersionedId<long>(1, 2);
        var f = new VersionedId<long>(1, 3);

        Assert.True(a == a);
        Assert.True(a == b);
        Assert.True(d == e);
        Assert.True(a != c);
        Assert.True(d != f);
        Assert.True(a.GetHashCode() == a.GetHashCode());
        Assert.True(a.GetHashCode() == b.GetHashCode());
        Assert.True(d.GetHashCode() == e.GetHashCode());
    }
}
