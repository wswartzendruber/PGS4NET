﻿/*
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
///     Writes <see cref="DisplaySet"/>s to a <see cref="Stream"/>.
/// </summary>
#if NETSTANDARD2_1_OR_GREATER
public class DisplaySetWriter : IDisposable, IAsyncDisposable
#else
public class DisplaySetWriter : IDisposable
#endif
{
    private readonly SegmentWriter Writer;
    private readonly DisplaySetDecomposer Decomposer = new();
    private readonly Queue<Segment> Queue = new();

    /// <summary>
    ///     The stream to write <see cref="DisplaySet"/>s to.
    /// </summary>
    public Stream Output { get; }

    /// <summary>
    ///     Whether or not the <see cref="Output"/> stream will be left open when this instance
    ///     is disposed.
    /// </summary>
    public bool LeaveOpen { get; }

    /// <summary>
    ///     Initializes a new instance.
    /// </summary>
    /// <param name="output">
    ///     The stream to write <see cref="DisplaySet"/>s to.
    /// </param>
    /// <param name="leaveOpen">
    ///     Whether or not the <paramref name="output"/> stream will be left open when this
    ///     instance is disposed.
    /// </param>
    public DisplaySetWriter(Stream output, bool leaveOpen = false)
    {
        Output = output;
        LeaveOpen = leaveOpen;
        Writer = new(output, leaveOpen);
        Decomposer.Ready += Ready;
    }

    /// <summary>
    ///     Attempts to write a <see cref="DisplaySet"/> to the <see cref="Output"/> stream.
    /// </summary>
    /// <remarks>
    ///     Internally, the <paramref name="displaySet"/> is decomposed into
    ///     <see cref="Segment"/>s where each one is then written to the <see cref="Output"/>
    ///     stream.
    /// </remarks>
    /// <param name="displaySet">
    ///     The display set to write.
    /// </param>
    /// <exception cref="SegmentException">
    ///     A property of a <see cref="Segment"/> is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to write a <see cref="Segment"/>.
    /// </exception>
    public void Write(DisplaySet displaySet)
    {
        Decomposer.Input(displaySet);

        while (Queue.Count > 0)
            Writer.Write(Queue.Dequeue());
    }

    /// <summary>
    ///     Attempts to asynchronously write a <see cref="DisplaySet"/> to the
    ///     <see cref="Output"/> stream.
    /// </summary>
    /// <remarks>
    ///     Internally, the <paramref name="displaySet"/> is decomposed into
    ///     <see cref="Segment"/>s where each one is then written to the <see cref="Output"/>
    ///     stream.
    /// </remarks>
    /// <param name="displaySet">
    ///     The display set to write.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <exception cref="SegmentException">
    ///     A property of a <see cref="Segment"/> is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to write a <see cref="Segment"/>.
    /// </exception>
    public async Task WriteAsync(DisplaySet displaySet,
        CancellationToken cancellationToken = default)
    {
        Decomposer.Input(displaySet);

        while (Queue.Count > 0)
            await Writer.WriteAsync(Queue.Dequeue(), cancellationToken);
    }

    /// <summary>
    ///     Disposes the <see cref="Output"/> stream if <see cref="LeaveOpen"/> is
    ///     <see langword="false"/>.
    /// </summary>
    public void Dispose()
    {
        if (!LeaveOpen)
            Output.Dispose();
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    ///     Asynchronously disposes the <see cref="Output"/> stream if <see cref="LeaveOpen"/>
    ///     is <see langword="false"/>.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (!LeaveOpen)
            await Output.DisposeAsync();
    }
#endif
    /// <summary>
    ///     Asynchronously disposes the <see cref="Output"/> stream if <see cref="LeaveOpen"/> is
    ///     <see langword="false"/>.
    /// </summary>
    private void Ready(object sender, Segment segment)
    {
        Queue.Enqueue(segment);
    }
}
