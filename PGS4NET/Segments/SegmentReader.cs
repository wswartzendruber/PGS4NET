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

#if NETSTANDARD2_1_OR_GREATER
public class SegmentReader : IEnumerable<Segment>, IAsyncEnumerable<Segment>
#else
public class SegmentReader : IEnumerable<Segment>
#endif
{
    public readonly Stream Input;
    public readonly bool LeaveOpen;

    public SegmentReader(Stream input, bool leaveOpen = false)
    {
        Input = input;
        LeaveOpen = leaveOpen;
    }

    public IEnumerator<Segment> GetEnumerator()
    {
        return new Enumerator(Input, LeaveOpen);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

#if NETSTANDARD2_1_OR_GREATER
    public IAsyncEnumerator<Segment> GetAsyncEnumerator(
        CancellationToken cancellationToken = default)
    {
        return new AsyncEnumerator(Input, cancellationToken, LeaveOpen);
    }
#endif

    private class Enumerator : IEnumerator<Segment>
    {
        private readonly Stream Input;
        private readonly bool LeaveOpen;

        private Segment? CurrentSegment;

        public Enumerator(Stream input, bool leaveOpen)
        {
            Input = input;
            LeaveOpen = leaveOpen;
        }

        public Segment Current => CurrentSegment
            ?? throw new InvalidOperationException("No more segments available.");

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            if (!LeaveOpen)
                Input.Dispose();
        }

        public bool MoveNext()
        {
            CurrentSegment = Input.ReadSegment();

            return CurrentSegment is not null;
        }

        public void Reset()
        {
            Input.Seek(0, SeekOrigin.Begin);
        }
    }

#if NETSTANDARD2_1_OR_GREATER
    private class AsyncEnumerator : IAsyncEnumerator<Segment>
    {
        private readonly Stream Input;
        private readonly CancellationToken CancelToken;
        private readonly bool LeaveOpen;

        private Segment? CurrentSegment;

        public AsyncEnumerator(Stream input, CancellationToken cancellationToken
            , bool leaveOpen)
        {
            Input = input;
            CancelToken = cancellationToken;
            LeaveOpen = leaveOpen;
        }

        public Segment Current => CurrentSegment
            ?? throw new InvalidOperationException("No more segments available.");

        public async ValueTask DisposeAsync()
        {
            if (!LeaveOpen)
                await Input.DisposeAsync();
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            CurrentSegment = await Input.ReadSegmentAsync(CancelToken);

            return CurrentSegment is not null;
        }
    }
#endif
}
