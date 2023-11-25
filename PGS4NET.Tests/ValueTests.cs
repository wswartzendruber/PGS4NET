/*
 * Copyright 2023 William Swartzendruber
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
    public void CompositionId()
    {
        var a = new CompositionId
        {
            ObjectId = 1,
            WindowId = 2,
        };
        var b = new CompositionId
        {
            ObjectId = 1,
            WindowId = 2,
        };
        var c = new CompositionId
        {
            ObjectId = 1,
            WindowId = 3,
        };

        Assert.True(a == a);
        Assert.True(a == b);
        Assert.True(a != c);
        Assert.True(a.GetHashCode() == a.GetHashCode());
        Assert.True(a.GetHashCode() == b.GetHashCode());
        Assert.True(a.GetHashCode() != c.GetHashCode());
    }

    [Fact]
    public void PgsTimeStamp()
    {
        var a = new PgsTimeStamp
        {
            Ticks = 1,
        };
        var b = new PgsTimeStamp
        {
            Ticks = 1,
        };
        var c = new PgsTimeStamp
        {
            Ticks = 2,
        };

        Assert.True(a == a);
        Assert.True(a == b);
        Assert.True(a != c);
        Assert.True(a.GetHashCode() == a.GetHashCode());
        Assert.True(a.GetHashCode() == b.GetHashCode());
        Assert.True(a.GetHashCode() != c.GetHashCode());
    }

    [Fact]
    public void VersionedId()
    {
        var a = new VersionedId<int>
        {
            Id = 1,
            Version = 2,
        };
        var b = new VersionedId<int>
        {
            Id = 1,
            Version = 2,
        };
        var c = new VersionedId<int>
        {
            Id = 1,
            Version = 3,
        };
        var d = new VersionedId<long>
        {
            Id = 1,
            Version = 2,
        };
        var e = new VersionedId<long>
        {
            Id = 1,
            Version = 2,
        };
        var f = new VersionedId<long>
        {
            Id = 1,
            Version = 3,
        };

        Assert.True(a == a);
        Assert.True(a == b);
        Assert.True(d == e);
        Assert.True(a != c);
        Assert.True(d != f);
        Assert.True(a.GetHashCode() == a.GetHashCode());
        Assert.True(a.GetHashCode() == b.GetHashCode());
        Assert.True(d.GetHashCode() == e.GetHashCode());
        Assert.True(a.GetHashCode() != c.GetHashCode());
        Assert.True(d.GetHashCode() != f.GetHashCode());
    }
}
