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

namespace PGS4NET.Tests.Segments;

public class RoundTrip
{
    [Fact]
    public void CycleSegments()
    {
        foreach (var testSegment in SegmentBuffers.Buffers)
        {
            var testName = testSegment.Key;
            var testBuffer = testSegment.Value;
            using var testStream = new MemoryStream(testBuffer);
            using var resultStream = new MemoryStream();

            resultStream.WriteSegment(testStream.ReadSegment()
                ?? throw new Exception("Read stream came back null."));

            if (!resultStream.ToArray().SequenceEqual(testBuffer))
                throw new Exception($"Could not round trip '{testName}' segment.");
        }
    }

    [Fact]
    public async Task CycleSegmentsAsync()
    {
        foreach (var testSegment in SegmentBuffers.Buffers)
        {
            var testName = testSegment.Key;
            var testBuffer = testSegment.Value;
            using var testStream = new MemoryStream(testBuffer);
            using var resultStream = new MemoryStream();

            await resultStream.WriteSegmentAsync(await testStream.ReadSegmentAsync()
                ?? throw new Exception("Read stream came back null."));

            if (!resultStream.ToArray().SequenceEqual(testBuffer))
                throw new Exception($"Could not round trip '{testName}' segment.");
        }
    }

    [Fact]
    public void CycleAllSegments()
    {
        var byteCount = 0;
        using var inputStream = new MemoryStream();
        using var outputStream = new MemoryStream();

        foreach (var testSegment in SegmentBuffers.Buffers)
        {
            var testName = testSegment.Key;
            var testBuffer = testSegment.Value;

            inputStream.Write(testBuffer, 0, testBuffer.Length);
            byteCount += testBuffer.Length;
        }
        inputStream.Position = 0;

        var segments = inputStream.ReadAllSegments();

        outputStream.WriteAllSegments(segments);

        Assert.True(outputStream.ToArray().SequenceEqual(inputStream.ToArray()));
        Assert.True(byteCount == outputStream.Position);
    }

    [Fact]
    public async Task CycleAllSegmentsAsync()
    {
        var byteCount = 0;
        using var inputStream = new MemoryStream();
        using var outputStream = new MemoryStream();

        foreach (var testSegment in SegmentBuffers.Buffers)
        {
            var testName = testSegment.Key;
            var testBuffer = testSegment.Value;

            inputStream.Write(testBuffer, 0, testBuffer.Length);
            byteCount += testBuffer.Length;
        }
        inputStream.Position = 0;

        var segments = await inputStream.ReadAllSegmentsAsync();

        await outputStream.WriteAllSegmentsAsync(segments);

        Assert.True(outputStream.ToArray().SequenceEqual(inputStream.ToArray()));
        Assert.True(byteCount == outputStream.Position);
    }
}
