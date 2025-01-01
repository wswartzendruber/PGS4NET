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

public class VersionedIdTests
{
    [Fact]
    public void HashCodes()
    {
        var a = new VersionedId<int>(1, 2);
        var b = new VersionedId<int>(1, 2);
        var e = new VersionedId<long>(1, 2);
        var f = new VersionedId<long>(1, 2);

        Assert.True(a.GetHashCode() == a.GetHashCode());
        Assert.True(a.GetHashCode() == b.GetHashCode());
        Assert.True(e.GetHashCode() == e.GetHashCode());
        Assert.True(e.GetHashCode() == f.GetHashCode());
    }

    [Fact]
    public void Equality()
    {
        var a = new VersionedId<int>(1, 2);
        var b = new VersionedId<int>(1, 2);
        var c = new VersionedId<int>(1, 3);
        var d = new VersionedId<int>(2, 2);
        var e = new VersionedId<long>(1, 2);
        var f = new VersionedId<long>(1, 2);
        var g = new VersionedId<long>(1, 3);
        var h = new VersionedId<long>(2, 2);

        Assert.True(a.Equals(a));
        Assert.True(a.Equals(b));
        Assert.False(a.Equals(c));
        Assert.False(a.Equals(d));
        Assert.False(c.Equals(d));
        Assert.True(e.Equals(e));
        Assert.True(e.Equals(f));
        Assert.False(e.Equals(g));
        Assert.False(e.Equals(h));
        Assert.False(g.Equals(h));
        Assert.True(a == a);
        Assert.True(a == b);
        Assert.True(a != c);
        Assert.True(a != d);
        Assert.True(c != d);
        Assert.True(e == e);
        Assert.True(e == f);
        Assert.True(e != g);
        Assert.True(e != h);
        Assert.True(g != h);
    }
}
