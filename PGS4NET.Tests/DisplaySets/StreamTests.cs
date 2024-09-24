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

using PGS4NET.DisplaySets;

namespace PGS4NET.Tests.DisplaySets;

public class StreamTests
{
    [Fact]
    public void EnumerateWriteAllDisplaySets()
    {
        using var inputStream = new MemoryStream(SintelSubtitles.Buffer);
        using var outputStream = new MemoryStream();

        outputStream.WriteAllDisplaySets(inputStream.DisplaySets());

        Assert.True(inputStream.ToArray().SequenceEqual(outputStream.ToArray()));
    }

#if TEST_NETSTANDARD2_1
    [Fact]
    public async Task EnumerateWriteAllDisplaySetsAsync()
    {
        using var inputStream = new MemoryStream(SintelSubtitles.Buffer);
        using var outputStream = new MemoryStream();

        await outputStream.WriteAllDisplaySetsAsync(inputStream.DisplaySetsAsync());

        Assert.True(inputStream.ToArray().SequenceEqual(outputStream.ToArray()));
    }
#endif

    [Fact]
    public void ReadWriteAllDisplaySets()
    {
        using var inputStream = new MemoryStream(SintelSubtitles.Buffer);
        using var outputStream = new MemoryStream();

        outputStream.WriteAllDisplaySets(inputStream.ReadAllDisplaySets());

        Assert.True(inputStream.ToArray().SequenceEqual(outputStream.ToArray()));
    }

    [Fact]
    public async Task ReadWriteAllDisplaySetsAsync()
    {
        using var inputStream = new MemoryStream(SintelSubtitles.Buffer);
        using var outputStream = new MemoryStream();

        await outputStream.WriteAllDisplaySetsAsync(
            await inputStream.ReadAllDisplaySetsAsync());

        Assert.True(inputStream.ToArray().SequenceEqual(outputStream.ToArray()));
    }
}
