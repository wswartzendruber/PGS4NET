/*
* Copyright 2024 William Swartzendruber
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
using PGS4NET.DisplaySets;
using PGS4NET.Segments;

namespace PGS4NET.Captions;

/// <summary>
///     Reads PGS <see cref="Caption"/>s from an input <see cref="Stream"/>.
/// </summary>
#if NETSTANDARD2_1_OR_GREATER
public class CaptionReader : IDisposable, IAsyncDisposable
#else
public class CaptionReader : IDisposable
#endif
{
    internal static readonly CaptionException TrailingDisplaySets
        = new("Caption is incomplete.");

    private readonly DisplaySetReader Reader;
    private readonly CaptionComposer Composer = new();
    private readonly Queue<Caption> Queue = new();

    /// <summary>
    ///     The stream that <see cref="Caption"/>s are being read from.
    /// </summary>
    public Stream Input { get; }

    /// <summary>
    ///     Whether or not the <see cref="Input"/> stream will be left open once all
    ///     <see cref="Caption"/>s have been read.
    /// </summary>
    public bool LeaveOpen { get; }

    /// <summary>
    ///     Initializes a new instance.
    /// </summary>
    /// <param name="input">
    ///     The stream that <see cref="Caption"/>s will be read from.
    /// </param>
    /// <param name="leaveOpen">
    ///     Whether or not the <paramref name="input"/> stream will be left open once all
    ///     <see cref="Caption"/>s have been read.
    /// </param>
    public CaptionReader(Stream input, bool leaveOpen = false)
    {
        Input = input;
        LeaveOpen = leaveOpen;
        Reader = new(input, leaveOpen);
        Composer.Ready += Ready;
    }

    /// <summary>
    ///     Attempts to read the next PGS caption from the <see cref="Input"/> stream.
    /// </summary>
    /// <remarks>
    ///     The current position of the <see cref="Input"/> stream must be at the beginning of a
    ///     a caption. Internally, <see cref="Segments"/>s are read and composed into
    ///     <see cref="DisplaySet"/>s, which is done until a caption can in turn be composed.
    /// </remarks>
    /// <returns>
    ///     The PGS caption that was read or <see langword="null"/> if the <see cref="Input"/>
    ///     stream is already EOF.
    /// </returns>
    /// <exception cref="CaptionException">
    ///     A caption can not be composed from the individual <see cref="DisplaySet"/>s that are
    ///     composed.
    /// </exception>
    /// <exception cref="DisplaySetException">
    ///     A display set can not be composed from the individual <see cref="Segment"/>s that
    ///     are read.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     An encoded value within a <see cref="Segment"/> is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to read a <see cref="Segment"/>.
    /// </exception>
    public Caption? Read()
    {
        if (Queue.Count > 0)
            return Queue.Dequeue();

        var displaySetRead = false;

        while (Queue.Count < 1 && Reader.Read() is DisplaySet displaySet)
        {
            displaySetRead = true;
            Composer.Input(displaySet);
        }

        if (!displaySetRead)
            return null;
        else if (Queue.Count > 0)
            return Queue.Dequeue();
        else
            throw TrailingDisplaySets;
    }

    /// <summary>
    ///     Attempts to asynchronously read the next PGS caption from the <see cref="Input"/>
    ///     stream.
    /// </summary>
    /// <remarks>
    ///     The current position of the <see cref="Input"/> stream must be at the beginning of a
    ///     a caption. Internally, <see cref="Segments"/>s are read and composed into
    ///     <see cref="DisplaySet"/>s, which is done until a caption can in turn be composed.
    /// </remarks>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     The PGS caption that was read or <see langword="null"/> if the <see cref="Input"/>
    ///     stream is already EOF.
    /// </returns>
    /// <exception cref="CaptionException">
    ///     A caption can not be composed from the individual <see cref="DisplaySet"/>s that are
    ///     composed.
    /// </exception>
    /// <exception cref="DisplaySetException">
    ///     A display set can not be composed from the individual <see cref="Segment"/>s that
    ///     are read.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     An encoded value within a <see cref="Segment"/> is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to read a <see cref="Segment"/>.
    /// </exception>
    public async Task<Caption?> ReadAsync(CancellationToken cancellationToken = default)
    {
        if (Queue.Count > 0)
            return Queue.Dequeue();

        var displaySetRead = false;

        while (Queue.Count < 1 && await Reader.ReadAsync(cancellationToken) is DisplaySet displaySet)
        {
            displaySetRead = true;
            Composer.Input(displaySet);
        }

        if (!displaySetRead)
            return null;
        else if (Queue.Count > 0)
            return Queue.Dequeue();
        else
            throw TrailingDisplaySets;
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

    private void Ready(object sender, Caption caption)
    {
        Queue.Enqueue(caption);
    }
}
