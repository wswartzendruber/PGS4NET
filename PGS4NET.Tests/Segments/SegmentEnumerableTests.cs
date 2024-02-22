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

using System.Collections;
using System.IO;
using PGS4NET.Segments;

namespace PGS4NET.Tests.Segments;

public class SegmentEnumerableTests
{
    [Fact]
    public void EnumerateSegments()
    {
        using var inputStream = new MemoryStream();

        foreach (var testSegment in SegmentBuffers.Buffers)
            inputStream.Write(testSegment.Value);

        inputStream.Seek(0, SeekOrigin.Begin);
        var segments1 = inputStream.ReadAllSegments();
        inputStream.Seek(0, SeekOrigin.Begin);
        var segments2 = new List<Segment>();

        foreach (var segment in new SegmentEnumerable(inputStream))
            segments2.Add(segment);

        if (inputStream.CanRead == true)
            throw new Exception("Input stream was not disposed.");

        using var segments1Stream = new MemoryStream();
        using var segments2Stream = new MemoryStream();

        segments1Stream.WriteAllSegments(segments1);
        segments2Stream.WriteAllSegments(segments2);

        if (!segments1Stream.ToArray().SequenceEqual(segments2Stream.ToArray()))
            throw new Exception("Memory streams are not equal.");
    }

    [Fact]
    public void EnumerateSegmentsLeaveOpen()
    {
        using var inputStream = new MemoryStream();

        foreach (var testSegment in SegmentBuffers.Buffers)
            inputStream.Write(testSegment.Value);

        inputStream.Seek(0, SeekOrigin.Begin);
        var segments1 = inputStream.ReadAllSegments();
        inputStream.Seek(0, SeekOrigin.Begin);
        var segments2 = new List<Segment>();

        foreach (var segment in new SegmentEnumerable(inputStream, true))
            segments2.Add(segment);

        if (inputStream.CanRead == false)
            throw new Exception("Input stream was disposed.");

        using var segments1Stream = new MemoryStream();
        using var segments2Stream = new MemoryStream();

        segments1Stream.WriteAllSegments(segments1);
        segments2Stream.WriteAllSegments(segments2);

        if (!segments1Stream.ToArray().SequenceEqual(segments2Stream.ToArray()))
            throw new Exception("Memory streams are not equal.");
    }

    [Fact]
    public void EnumerateSegmentsLegacy()
    {
        using var inputStream = new MemoryStream();

        foreach (var testSegment in SegmentBuffers.Buffers)
            inputStream.Write(testSegment.Value);

        inputStream.Seek(0, SeekOrigin.Begin);
        var segments1 = inputStream.ReadAllSegments();
        inputStream.Seek(0, SeekOrigin.Begin);
        var segments2 = new List<Segment>();

        foreach (var segment in new SegmentEnumerable(inputStream) as IEnumerable)
            segments2.Add((Segment)segment);

        if (inputStream.CanRead == true)
            throw new Exception("Input stream was not disposed.");

        using var segments1Stream = new MemoryStream();
        using var segments2Stream = new MemoryStream();

        segments1Stream.WriteAllSegments(segments1);
        segments2Stream.WriteAllSegments(segments2);

        if (!segments1Stream.ToArray().SequenceEqual(segments2Stream.ToArray()))
            throw new Exception("Memory streams are not equal.");
    }

    [Fact]
    public void EnumerateSegmentsLegacyLeaveOpen()
    {
        using var inputStream = new MemoryStream();

        foreach (var testSegment in SegmentBuffers.Buffers)
            inputStream.Write(testSegment.Value);

        inputStream.Seek(0, SeekOrigin.Begin);
        var segments1 = inputStream.ReadAllSegments();
        inputStream.Seek(0, SeekOrigin.Begin);
        var segments2 = new List<Segment>();

        foreach (var segment in new SegmentEnumerable(inputStream, true) as IEnumerable)
            segments2.Add((Segment)segment);

        if (inputStream.CanRead == false)
            throw new Exception("Input stream was disposed.");

        using var segments1Stream = new MemoryStream();
        using var segments2Stream = new MemoryStream();

        segments1Stream.WriteAllSegments(segments1);
        segments2Stream.WriteAllSegments(segments2);

        if (!segments1Stream.ToArray().SequenceEqual(segments2Stream.ToArray()))
            throw new Exception("Memory streams are not equal.");
    }

#if TEST_NETSTANDARD2_1
    [Fact]
    public async void EnumerateSegmentsAsync()
    {
        using var inputStream = new MemoryStream();

        foreach (var testSegment in SegmentBuffers.Buffers)
            await inputStream.WriteAsync(testSegment.Value);

        inputStream.Seek(0, SeekOrigin.Begin);
        var segments1 = await inputStream.ReadAllSegmentsAsync();
        inputStream.Seek(0, SeekOrigin.Begin);
        var segments2 = new List<Segment>();

        await foreach (var segment in new SegmentEnumerable(inputStream))
            segments2.Add(segment);

        if (inputStream.CanRead == true)
            throw new Exception("Input stream was not disposed.");

        using var segments1Stream = new MemoryStream();
        using var segments2Stream = new MemoryStream();

        await segments1Stream.WriteAllSegmentsAsync(segments1);
        await segments2Stream.WriteAllSegmentsAsync(segments2);

        if (!segments1Stream.ToArray().SequenceEqual(segments2Stream.ToArray()))
            throw new Exception("Memory streams are not equal.");
    }

    [Fact]
    public async void EnumerateSegmentsLeaveOpenAsync()
    {
        using var inputStream = new MemoryStream();

        foreach (var testSegment in SegmentBuffers.Buffers)
            await inputStream.WriteAsync(testSegment.Value);

        inputStream.Seek(0, SeekOrigin.Begin);
        var segments1 = await inputStream.ReadAllSegmentsAsync();
        inputStream.Seek(0, SeekOrigin.Begin);
        var segments2 = new List<Segment>();

        await foreach (var segment in new SegmentEnumerable(inputStream, true))
            segments2.Add(segment);

        if (inputStream.CanRead == false)
            throw new Exception("Input stream was disposed.");

        using var segments1Stream = new MemoryStream();
        using var segments2Stream = new MemoryStream();

        await segments1Stream.WriteAllSegmentsAsync(segments1);
        await segments2Stream.WriteAllSegmentsAsync(segments2);

        if (!segments1Stream.ToArray().SequenceEqual(segments2Stream.ToArray()))
            throw new Exception("Memory streams are not equal.");
    }
#endif

    [Fact]
    public void ExtensionMethod()
    {
        using var inputStream = new MemoryStream();

        foreach (var testSegment in SegmentBuffers.Buffers)
            inputStream.Write(testSegment.Value);

        inputStream.Seek(0, SeekOrigin.Begin);
        var segments1 = inputStream.ReadAllSegments();
        inputStream.Seek(0, SeekOrigin.Begin);
        var segments2 = new List<Segment>();

        foreach (var segment in inputStream.Segments())
            segments2.Add(segment);

        if (inputStream.CanRead == false)
            throw new Exception("Input stream was disposed.");

        using var segments1Stream = new MemoryStream();
        using var segments2Stream = new MemoryStream();

        segments1Stream.WriteAllSegments(segments1);
        segments2Stream.WriteAllSegments(segments2);

        if (!segments1Stream.ToArray().SequenceEqual(segments2Stream.ToArray()))
            throw new Exception("Memory streams are not equal.");
    }
}
