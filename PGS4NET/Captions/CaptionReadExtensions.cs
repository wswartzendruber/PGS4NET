/*
 * Copyright 2024 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using PGS4NET.DisplaySets;
using PGS4NET.Segments;

namespace PGS4NET.Captions;

/// <summary>
///     Extension methods against <see cref="System.IO.Stream" /> for reading captions.
/// </summary>
public static partial class CaptionExtensions
{
    /// <summary>
    ///     Attempts to read all captions from a <paramref name="stream" />, one at a time, as
    ///     each one is consumed by an enumerator.
    /// </summary>
    /// <remarks>
    ///     The current position of the <paramref name="stream" /> must be at the beginning of a
    ///     display set. The stream must contain only complete PGS display sets from this point
    ///     on and there must be no trailing data.
    /// </remarks>
    /// <param name="stream">
    ///     The stream to read all captions from.
    /// </param>
    /// <returns>
    ///     An enumerator over captions being read.
    /// </returns>
    /// <exception cref="CaptionException">
    ///     Multiple display sets cannot be composed into a caption.
    /// </exception>
    /// <exception cref="DisplaySetException">
    ///     An encoded value within a display set is invalid.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     An encoded value within a segment is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to read a segment.
    /// </exception>
    public static IEnumerable<Caption> Captions(this Stream stream)
    {
        var composer = new CaptionComposer();
        Caption? caption = null;

        composer.NewCaption += (_, caption_) =>
        {
            caption = caption_;
        };

        while (stream.ReadDisplaySet() is DisplaySet displaySet)
        {
            composer.Input(displaySet);

            if (caption is Caption caption_)
            {
                yield return caption_;

                caption = null;
            }
        }
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    ///     Attempts to asynchronously read all captions from a <paramref name="stream" />, one
    ///     at a time, as each one is consumed by an asynchronous enumerator.
    /// </summary>
    /// <remarks>
    ///     The current position of the <paramref name="stream" /> must be at the beginning of a
    ///     display set. The stream must contain only complete PGS display sets from this point
    ///     on and there must be no trailing data.
    /// </remarks>
    /// <param name="stream">
    ///     The stream to read all captions from.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     An asynchronous enumerator over captions being read.
    /// </returns>
    /// <exception cref="CaptionException">
    ///     Multiple display sets cannot be composed into a caption.
    /// </exception>
    /// <exception cref="DisplaySetException">
    ///     An encoded value within a display set is invalid.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     An encoded value within a segment is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to read a segment.
    /// </exception>
    public static async IAsyncEnumerable<Caption> CaptionsAsync(this Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var composer = new CaptionComposer();
        Caption? caption = null;

        composer.NewCaption += (_, caption_) =>
        {
            caption = caption_;
        };

        while (await stream.ReadDisplaySetAsync() is DisplaySet displaySet)
        {
            composer.Input(displaySet);

            if (caption is Caption caption_)
            {
                yield return caption_;

                caption = null;
            }
        }
    }
#endif

    /// <summary>
    ///     Attempts to read all captions from a <paramref name="stream" /> in a single
    ///     operation.
    /// </summary>
    /// <remarks>
    ///     The current position of the <paramref name="stream" /> must be at the beginning of a
    ///     display set. The stream must contain only complete PGS display sets from this point
    ///     on and there must be no trailing data.
    /// </remarks>
    /// <param name="stream">
    ///     The stream to read all captions from.
    /// </param>
    /// <returns>
    ///     The collection of captions that were read.
    /// </returns>
    /// <exception cref="CaptionException">
    ///     Multiple display sets cannot be composed into a caption.
    /// </exception>
    /// <exception cref="DisplaySetException">
    ///     An encoded value within a display set is invalid.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     An encoded value within a segment is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to read a segment.
    /// </exception>
    public static IList<Caption> ReadAllCaptions(this Stream stream)
    {
        var returnValue = new List<Caption>();
        var composer = new CaptionComposer();

        composer.NewCaption += (_, caption_) =>
        {
            returnValue.Add(caption_);
        };

        while (stream.ReadDisplaySet() is DisplaySet displaySet)
            composer.Input(displaySet);

        return returnValue;
    }

    /// <summary>
    ///     Attempts to asynchronously read all captions from a <paramref name="stream" /> in a
    ///     single operation.
    /// </summary>
    /// <remarks>
    ///     The current position of the <paramref name="stream" /> must be at the beginning of a
    ///     display set. The stream must contain only complete PGS display sets from this point
    ///     on and there must be no trailing data.
    /// </remarks>
    /// <param name="stream">
    ///     The stream to read all captions from.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     The collection of captions that were read.
    /// </returns>
    /// <exception cref="CaptionException">
    ///     Multiple display sets cannot be composed into a caption.
    /// </exception>
    /// <exception cref="DisplaySetException">
    ///     An encoded value within a display set is invalid.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     An encoded value within a segment is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to read a segment.
    /// </exception>
    public static async Task<IList<Caption>> ReadAllCaptionsAsync(this Stream stream
        , CancellationToken cancellationToken = default)
    {
        var returnValue = new List<Caption>();
        var composer = new CaptionComposer();

        composer.NewCaption += (_, caption_) =>
        {
            returnValue.Add(caption_);
        };

        while (await stream.ReadDisplaySetAsync() is DisplaySet displaySet)
            composer.Input(displaySet);

        return returnValue;
    }
}
