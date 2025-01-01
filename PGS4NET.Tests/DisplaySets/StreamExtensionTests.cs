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

using PGS4NET.DisplaySets;
using PGS4NET.Tests.Segments;

namespace PGS4NET.Tests.DisplaySets;

public class StreamExtensionTests
{
    [Fact]
    public void ReadWriteAllDisplaySetsEnumerable()
    {
        using var inputStream = new MemoryStream(SintelSubtitles.Buffer);
        using var outputStream = new MemoryStream();

        outputStream.WriteAllDisplaySets(inputStream.DisplaySets());

        Assert.Equal(inputStream.ToArray(), outputStream.ToArray());
    }

#if NETCOREAPP3_0_OR_GREATER
    [Fact]
    public async Task ReadWriteAllDisplaySetsAsyncEnumerable()
    {
        using var inputStream = new MemoryStream(SintelSubtitles.Buffer);
        using var outputStream = new MemoryStream();

        await outputStream.WriteAllDisplaySetsAsync(inputStream.DisplaySetsAsync());

        Assert.Equal(inputStream.ToArray(), outputStream.ToArray());
    }
#endif

    [Fact]
    public void ReadWriteAllDisplaySets()
    {
        using var inputStream = new MemoryStream(SintelSubtitles.Buffer);
        using var outputStream = new MemoryStream();

        outputStream.WriteAllDisplaySets(inputStream.ReadAllDisplaySets());

        Assert.Equal(inputStream.ToArray(), outputStream.ToArray());
    }

    [Fact]
    public async Task ReadWriteAllDisplaySetsAsync()
    {
        using var inputStream = new MemoryStream(SintelSubtitles.Buffer);
        using var outputStream = new MemoryStream();

        await outputStream.WriteAllDisplaySetsAsync(
            await inputStream.ReadAllDisplaySetsAsync());

        Assert.Equal(inputStream.ToArray(), outputStream.ToArray());
    }

    [Fact]
    public void PartialDisplaySetEnumerable()
    {
        using var stream = new MemoryStream();
        var buffer = SegmentBuffers.Buffers["pcs-es"];

        stream.Write(buffer, 0, buffer.Length);
        stream.Position = 0;

        try
        {
            stream.DisplaySets().ToList();

            Assert.Fail("Was able to read an incomplete display set.");
        }
        catch (DisplaySetException dse)
        {
            Assert.Equal("Display set is incomplete.", dse.Message);
        }
    }

#if NETCOREAPP3_0_OR_GREATER
    [Fact]
    public async Task PartialDisplaySetAsyncEnumerable()
    {
        using var stream = new MemoryStream();
        var buffer = SegmentBuffers.Buffers["pcs-es"];

        stream.Write(buffer, 0, buffer.Length);
        stream.Position = 0;

        try
        {
            await stream.DisplaySetsAsync().ToListAsync();

            Assert.Fail("Was able to read an incomplete display set.");
        }
        catch (DisplaySetException dse)
        {
            Assert.Equal("Display set is incomplete.", dse.Message);
        }
    }
#endif

    [Fact]
    public void PartialDisplaySet()
    {
        using var stream = new MemoryStream();
        var buffer = SegmentBuffers.Buffers["pcs-es"];

        stream.Write(buffer, 0, buffer.Length);
        stream.Position = 0;

        try
        {
            stream.ReadAllDisplaySets();

            Assert.Fail("Was able to read an incomplete display set.");
        }
        catch (DisplaySetException dse)
        {
            Assert.Equal("Display set is incomplete.", dse.Message);
        }
    }

    [Fact]
    public async Task PartialDisplaySetAsync()
    {
        using var stream = new MemoryStream();
        var buffer = SegmentBuffers.Buffers["pcs-es"];

        stream.Write(buffer, 0, buffer.Length);
        stream.Position = 0;

        try
        {
            await stream.ReadAllDisplaySetsAsync();

            Assert.Fail("Was able to read an incomplete display set.");
        }
        catch (DisplaySetException dse)
        {
            Assert.Equal("Display set is incomplete.", dse.Message);
        }
    }
}
