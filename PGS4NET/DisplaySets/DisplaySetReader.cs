/*
* Copyright 2025 William Swartzendruber
*
* This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
* copy of the MPL was not distributed with this file, You can obtain one at
* https://mozilla.org/MPL/2.0/.
*
* SPDX-License-Identifier: MPL-2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

/// <summary>
///     Reads PGS <see cref="DisplaySet"/>s from an input <see cref="Stream"/>.
/// </summary>
#if NETSTANDARD2_1_OR_GREATER
public class DisplaySetReader : IDisposable, IAsyncDisposable
#else
public class DisplaySetReader : IDisposable
#endif
{
    internal static readonly DisplaySetException TrailingSegments
        = new("Display set is incomplete.");

    private readonly SegmentReader Reader;
    private readonly DisplaySetComposer Composer = new();
    private readonly Queue<DisplaySet> Queue = new();

    /// <summary>
    ///     The stream that <see cref="DisplaySet"/>s are being read from.
    /// </summary>
    public Stream Input { get; }

    /// <summary>
    ///     Whether or not the <see cref="Input"/> stream will be left open once all
    ///     <see cref="DisplaySet"/>s have been read.
    /// </summary>
    public bool LeaveOpen { get; }

    /// <summary>
    ///     Initializes a new instance.
    /// </summary>
    /// <param name="input">
    ///     The stream that <see cref="DisplaySet"/>s will be read from.
    /// </param>
    /// <param name="leaveOpen">
    ///     Whether or not the <paramref name="input"/> stream will be left open once all
    ///     <see cref="DisplaySet"/>s have been read.
    /// </param>
    public DisplaySetReader(Stream input, bool leaveOpen = false)
    {
        Input = input;
        LeaveOpen = leaveOpen;
        Reader = new(input, leaveOpen);
        Composer.Ready += Ready;
    }

    /// <summary>
    ///     Attempts to read the next PGS display set from the <see cref="Input"/> stream.
    /// </summary>
    /// <remarks>
    ///     The current position of the <see cref="Input"/> stream must be at the beginning of a
    ///     a display set. Internally, <see cref="Segment"/>s are read until a display set can
    ///     be composed.
    /// </remarks>
    /// <returns>
    ///     The PGS display set that was read or <see langword="null"/> if the
    ///     <see cref="Input"/> stream is already EOF.
    /// </returns>
    /// <exception cref="DisplaySetException">
    ///     A <see cref="DisplaySet"/> can not be composed from the individual
    ///     <see cref="Segment"/>s that are read.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     An encoded value within a <see cref="Segment"/> is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to read a <see cref="Segment"/>.
    /// </exception>
    public DisplaySet? Read()
    {
        if (Queue.Count > 0)
            return Queue.Dequeue();

        var segmentRead = false;

        while (Queue.Count < 1 && Reader.Read() is Segment segment)
        {
            segmentRead = true;
            Composer.Input(segment);
        }

        if (!segmentRead)
            return null;
        else if (Queue.Count > 0)
            return Queue.Dequeue();
        else
            throw TrailingSegments;
    }

    /// <summary>
    ///     Attempts to asynchronously read the next PGS display set from the
    ///     <see cref="Input"/> stream.
    /// </summary>
    /// <remarks>
    ///     The current position of the <see cref="Input"/> stream must be at the beginning of a
    ///     a display set. Internally, <see cref="Segment"/>s are read until a display set can
    ///     be composed.
    /// </remarks>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     The PGS display set that was read or <see langword="null"/> if the
    ///     <see cref="Input"/> stream is already EOF.
    /// </returns>
    /// <exception cref="DisplaySetException">
    ///     A <see cref="DisplaySet"/> can not be composed from the individual
    ///     <see cref="Segment"/>s that are read.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     An encoded value within a <see cref="Segment"/> is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to read a <see cref="Segment"/>.
    /// </exception>
    public async Task<DisplaySet?> ReadAsync(CancellationToken cancellationToken = default)
    {
        if (Queue.Count > 0)
            return Queue.Dequeue();

        var segmentRead = false;

        while (Queue.Count < 1 && await Reader.ReadAsync(cancellationToken) is Segment segment)
        {
            segmentRead = true;
            Composer.Input(segment);
        }

        if (!segmentRead)
            return null;
        else if (Queue.Count > 0)
            return Queue.Dequeue();
        else
            throw TrailingSegments;
    }

    /// <summary>
    ///     Disposes the <see cref="Input"/> stream if <see cref="LeaveOpen"/> is
    ///     <see langword="false"/>.
    /// </summary>
    public void Dispose()
    {
        if (!LeaveOpen)
            Input.Dispose();
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    ///     Asynchronously disposes the <see cref="Input"/> stream if <see cref="LeaveOpen"/> is
    ///     <see langword="false"/>.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (!LeaveOpen)
            await Input.DisposeAsync();
    }
#endif

    private void Ready(object sender, DisplaySet displaySet)
    {
        Queue.Enqueue(displaySet);
    }
}
