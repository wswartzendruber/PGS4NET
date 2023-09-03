/*
 * Copyright 2023 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PGS4NET.DisplaySets;
using PGS4NET.Segments;

namespace PGS4NET.Epochs;

/// <summary>
///     Contains extensions against different classes for intuitively handling epochs.
/// </summary>
public static partial class DisplaySetExtensions
{
    /// <summary>
    ///     Writes all <see cref="Epochs" />s in a collection to a <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     Internally, this method iterates through each <see cref="Epoch" /> and:
    ///     <list type="number">
    ///         <item>
    ///             <description>
    ///                 Decomposes the <see cref="Epoch" /> into a collection of
    ///                 <see cref="DisplaySet" />s.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Decomposes each <see cref="DisplaySet" /> into a collection of
    ///                 <see cref="Segments" />s.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Writes each <see cref="Segment" /> to the <paramref name="stream" />.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <exception cref="EpochException">
    ///     Thrown when an <see cref="Epoch" /> cannot be decomposed into a collection of
    ///     <see cref="DisplaySet" />s.
    /// </exception>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a <see cref="DisplaySet" /> cannot be decomposed into a collection of
    ///     <see cref="Segment" />s.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     Thrown when the properties of a <see cref="Segment" /> cannot be written to the
    ///     <paramref name="stream" />.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to write a
    ///     <see cref="Segment" /> to the <paramref name="stream" />.
    /// </exception>
    public static void WriteAllEpochs(this Stream stream, IEnumerable<Epoch> epochs)
    {
        foreach (var epoch in epochs)
            stream.WriteEpoch(epoch);
    }

    /// <summary>
    ///     Asynchronously writes all <see cref="Epochs" />s in a collection to a
    ///     <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     Internally, this method iterates through each <see cref="Epoch" /> and:
    ///     <list type="number">
    ///         <item>
    ///             <description>
    ///                 Decomposes the <see cref="Epoch" /> into a collection of
    ///                 <see cref="DisplaySet" />s.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Decomposes each <see cref="DisplaySet" /> into a collection of
    ///                 <see cref="Segments" />s.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Writes each <see cref="Segment" /> to the <paramref name="stream" />.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <exception cref="EpochException">
    ///     Thrown when an <see cref="Epoch" /> cannot be decomposed into a collection of
    ///     <see cref="DisplaySet" />s.
    /// </exception>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a <see cref="DisplaySet" /> cannot be decomposed into a collection of
    ///     <see cref="Segment" />s.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     Thrown when the properties of a <see cref="Segment" /> cannot be written to the
    ///     <paramref name="stream" />.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to write a
    ///     <see cref="Segment" /> to the <paramref name="stream" />.
    /// </exception>
    public static async Task WriteAllEpochsAsync(this Stream stream, IEnumerable<Epoch> epochs)
    {
        foreach (var epoch in epochs)
            await stream.WriteEpochAsync(epoch);
    }

    /// <summary>
    ///     Writes an <see cref="Epoch" /> to a <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     Internally, this method:
    ///     <list type="number">
    ///         <item>
    ///             <description>
    ///                 Decomposes the <see cref="Epoch" /> into a collection of
    ///                 <see cref="DisplaySet" />s.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Decomposes each <see cref="DisplaySet" /> into a collection of
    ///                 <see cref="Segments" />s.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Writes each <see cref="Segment" /> to the <paramref name="stream" />.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <exception cref="EpochException">
    ///     Thrown when an <see cref="Epoch" /> cannot be decomposed into a collection of
    ///     <see cref="DisplaySet" />s.
    /// </exception>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a <see cref="DisplaySet" /> cannot be decomposed into a collection of
    ///     <see cref="Segment" />s.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     Thrown when the properties of a <see cref="Segment" /> cannot be written to the
    ///     <paramref name="stream" />.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to write a
    ///     <see cref="Segment" /> to the <paramref name="stream" />.
    /// </exception>
    public static void WriteEpoch(this Stream stream, Epoch epoch)
    {
        foreach (var displaySet in EpochComposer.Decompose(epoch))
            stream.WriteDisplaySet(displaySet);
    }

    /// <summary>
    ///     Asynchronously writes an <see cref="Epoch" /> to a <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     Internally, this method:
    ///     <list type="number">
    ///         <item>
    ///             <description>
    ///                 Decomposes the <see cref="Epoch" /> into a collection of
    ///                 <see cref="DisplaySet" />s.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Decomposes each <see cref="DisplaySet" /> into a collection of
    ///                 <see cref="Segments" />s.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Writes each <see cref="Segment" /> to the <paramref name="stream" />.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <exception cref="EpochException">
    ///     Thrown when an <see cref="Epoch" /> cannot be decomposed into a collection of
    ///     <see cref="DisplaySet" />s.
    /// </exception>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a <see cref="DisplaySet" /> cannot be decomposed into a collection of
    ///     <see cref="Segment" />s.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     Thrown when the properties of a <see cref="Segment" /> cannot be written to the
    ///     <paramref name="stream" />.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to write a
    ///     <see cref="Segment" /> to the <paramref name="stream" />.
    /// </exception>
    public static async Task WriteEpochAsync(this Stream stream, Epoch epoch)
    {
        foreach (var displaySet in EpochComposer.Decompose(epoch))
            await stream.WriteDisplaySetAsync(displaySet);
    }

    /// <summary>
    ///     Decomposes a collection of <see cref="Epoch" />s into a collection of
    ///     <see cref="DisplaySet" />s.
    /// </summary>
    /// <exception cref="EpochException">
    ///     Thrown when an <see cref="Epoch" /> cannot be decomposed into a collection of
    ///     <see cref="DisplaySet" />s.
    /// </exception>
    public static IList<DisplaySet> ToDisplaySetList(this IEnumerable<Epoch> epochs)
    {
        var returnValue = new List<DisplaySet>();

        foreach (var epoch in epochs)
            returnValue.AddRange(EpochComposer.Decompose(epoch));

        return returnValue;
    }
}
