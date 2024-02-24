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

public class EnumerableTests
{
    [Fact]
    public void ReadDisplaySetInstances()
    {
        using var inputStream = new MemoryStream();
        using var outputStream1 = new MemoryStream();
        using var outputStream2 = new MemoryStream();

        foreach (var buffer in DisplaySetInstances.Instances.Values)
            inputStream.WriteDisplaySet(buffer);

        inputStream.Seek(0, SeekOrigin.Begin);
        var bulk = inputStream.ReadAllDisplaySets();

        inputStream.Seek(0, SeekOrigin.Begin);
        var enumerable = inputStream.DisplaySets();

        outputStream1.WriteAllDisplaySets(bulk);
        outputStream2.WriteAllDisplaySets(enumerable);

        Assert.True(outputStream1.ToArray().SequenceEqual(outputStream2.ToArray()));
    }

#if TEST_NETSTANDARD2_1
    [Fact]
    public async void ReadDisplaySetInstancesAsync()
    {
        using var inputStream = new MemoryStream();
        using var outputStream1 = new MemoryStream();
        using var outputStream2 = new MemoryStream();

        foreach (var buffer in DisplaySetInstances.Instances.Values)
            await inputStream.WriteDisplaySetAsync(buffer);

        inputStream.Seek(0, SeekOrigin.Begin);
        var bulk = await inputStream.ReadAllDisplaySetsAsync();

        inputStream.Seek(0, SeekOrigin.Begin);
        var asyncEnumerable = inputStream.DisplaySetsAsync();

        await outputStream1.WriteAllDisplaySetsAsync(bulk);
        await outputStream2.WriteAllDisplaySetsAsync(asyncEnumerable);

        Assert.True(outputStream1.ToArray().SequenceEqual(outputStream2.ToArray()));
    }
#endif
}
