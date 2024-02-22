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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PGS4NET.Segments;

/// <summary>
///     Enumerates through the segments in a <see cref="Stream" />.
/// </summary>
/// <remarks>
///     Segments are read one by one until the end of the stream is reached. The stream must
///     contain only valid segment data from its current position onward. Any trailing data will
///     cause an exception to be thrown.
/// </remarks>
#if NETSTANDARD2_1_OR_GREATER
public class SegmentEnumerable : IEnumerable<Segment>, IAsyncEnumerable<Segment>
#else
public class SegmentEnumerable : IEnumerable<Segment>
#endif
{
    private readonly Stream Input;
    private readonly bool LeaveOpen;

    /// <summary>
    ///     Initializes a new instance against the specified <paramref name="input" />
    ///     <see cref="Stream" />, optionally leaving the stream open when done.
    /// </summary>
    /// <param name="input">
    ///     The <see cref="Stream" /> to read segment data from.
    /// </param>
    /// <param name="leaveOpen">
    ///     If set to true, the <paramref name="input" /> <see cref="Stream" /> will not be
    ///     disposed when enumeration completes.
    /// </param>
    public SegmentEnumerable(Stream input, bool leaveOpen = false)
    {
        Input = input;
        LeaveOpen = leaveOpen;
    }

    /// <summary>
    ///     Returns an <see cref="IEnumerator{Segment}" /> allowing enumeration through the
    ///     segments in the <see cref="Stream" />
    /// </summary>
    public IEnumerator<Segment> GetEnumerator()
    {
        return new Enumerator(Input, LeaveOpen);
    }

    /// <summary>
    ///     Returns an <see cref="IEnumerator" /> allowing enumeration through the segments in
    ///     the <see cref="Stream" />
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    ///     Returns a <see cref="IAsyncEnumerator{Segment}" /> allowing asynchronous enumeration
    ///     through the segments in the <see cref="Stream" />
    /// </summary>
    public IAsyncEnumerator<Segment> GetAsyncEnumerator(
        CancellationToken cancellationToken = default)
    {
        return new Enumerator(Input, cancellationToken, LeaveOpen);
    }
#endif

#if NETSTANDARD2_1_OR_GREATER
    private class Enumerator : IEnumerator<Segment>, IAsyncEnumerator<Segment>
#else
    private class Enumerator : IEnumerator<Segment>
#endif
    {
        private readonly Stream Input;
#if NETSTANDARD2_1_OR_GREATER
        private readonly CancellationToken CancelToken;
#endif
        private readonly bool LeaveOpen;

        private Segment? CurrentSegment;

        public Enumerator(Stream input, bool leaveOpen)
        {
            Input = input;
            LeaveOpen = leaveOpen;
        }

#if NETSTANDARD2_1_OR_GREATER
        public Enumerator(Stream input, CancellationToken cancellationToken, bool leaveOpen)
        {
            Input = input;
            CancelToken = cancellationToken;
            LeaveOpen = leaveOpen;
        }
#endif

        public Segment Current => CurrentSegment
            ?? throw new InvalidOperationException("No more segments available.");

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            if (!LeaveOpen)
                Input.Dispose();
        }

#if NETSTANDARD2_1_OR_GREATER
        public async ValueTask DisposeAsync()
        {
            if (!LeaveOpen)
                await Input.DisposeAsync();
        }
#endif

        public bool MoveNext()
        {
            CurrentSegment = Input.ReadSegment();

            return CurrentSegment is not null;
        }

#if NETSTANDARD2_1_OR_GREATER
        public async ValueTask<bool> MoveNextAsync()
        {
            CurrentSegment = await Input.ReadSegmentAsync(CancelToken);

            return CurrentSegment is not null;
        }
#endif

        public void Reset()
        {
            Input.Seek(0, SeekOrigin.Begin);
        }
    }
}
