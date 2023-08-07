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

using PGS4NET.Segments;

namespace PGS4NET.Tests;

public class DisposalTests
{
    [Fact]
    public void SegmentReader_Using()
    {
        var testStream = new TestStream();

        Assert.False(testStream.Disposed);

        using (var reader = new SegmentReader(testStream))
        {
            Assert.False(testStream.Disposed);
        }

        Assert.True(testStream.Disposed);
    }

#if TEST_NETSTANDARD2_1
    [Fact]
    public async Task SegmentReader_AwaitUsing()
    {
        var testStream = new TestStream();

        Assert.False(testStream.Disposed);

        await using (var reader = new SegmentReader(testStream))
        {
            Assert.False(testStream.Disposed);
        }

        Assert.True(testStream.Disposed);
    }
#endif

    [Fact]
    public void SegmentWriter_Using()
    {
        var testStream = new TestStream();

        Assert.False(testStream.Disposed);

        using (var writer = new SegmentWriter(testStream))
        {
            Assert.False(testStream.Disposed);
        }

        Assert.True(testStream.Disposed);
    }

#if TEST_NETSTANDARD2_1
    [Fact]
    public async Task SegmentWriter_AwaitUsing()
    {
        var testStream = new TestStream();

        Assert.False(testStream.Disposed);

        await using (var writer = new SegmentWriter(testStream))
        {
            Assert.False(testStream.Disposed);
        }

        Assert.True(testStream.Disposed);
    }
#endif
}
