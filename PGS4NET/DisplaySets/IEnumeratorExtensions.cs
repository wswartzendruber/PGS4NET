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
using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

/// <summary>
///     Contains extensions against <see cref="IEnumerator{Segment}" /> for reading PGS display
///     sets.
/// </summary>
public static partial class IEnumeratorExtensions
{
    private enum Sequence
    {
        Single,
        Middle,
        Initial,
        Final,
    }

    private static readonly DisplaySetException InconsistentPts = new("Inconsistent PTS.");
    private static readonly DisplaySetException InconsistentDts = new("Inconsistent DTS.");

    /// <summary>
    ///     Builds a PGS display set from an enumerator over a collection of segments. The
    ///     enumerator will be advanced in order to parse the first segment. Advancement will
    ///     stop after the end segment (ES) is reached or an error occurs.
    /// </summary>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a sequence of segments cannot form a display set.
    /// </exception>
    public static DisplaySet? BuildDisplaySet(this IEnumerator<Segment> segments)
    {
        EndSegment? es = null;
        Sequence sequence = Sequence.Single;
        InitialObjectDefinitionSegment? iods = null;
        List<MiddleObjectDefinitionSegment> middleObjects = new();
        Dictionary<byte, Window> windows = new();
        Dictionary<VersionedId<byte>, Palette> palettes = new();
        Dictionary<VersionedId<ushort>, DisplayObject> displayObjects = new();
        Dictionary<CompoundId, CompositionObject> compositionObjects = new();
        PresentationCompositionSegment pcs = segments.MoveNext()
            ? segments.Current as PresentationCompositionSegment
                ?? throw new DisplaySetException("Initial segment not PCS.")
            : throw new DisplaySetException("Initial segment not present.");

        while (segments.MoveNext())
        {
            switch (segments.Current)
            {
                case PresentationCompositionSegment:
                {
                    throw new DisplaySetException("Unexpected PCS.");
                }
                case WindowDefinitionSegment wds:
                {
                    if (wds.Pts != pcs.Pts)
                        throw InconsistentPts;
                    if (wds.Dts != pcs.Dts)
                        throw InconsistentDts;

                    foreach (var wd in wds.Definitions)
                    {
                        if (windows.ContainsKey(wd.Id))
                            throw new DisplaySetException($"Duplicate window ID: {wd.Id}");

                        windows[wd.Id] = new Window
                        {
                            X = wd.X,
                            Y = wd.Y,
                            Width = wd.Width,
                            Height = wd.Height,
                        };
                    }

                    break;
                }
                case PaletteDefinitionSegment pds:
                {
                    if (pds.Pts != pcs.Pts)
                        throw InconsistentPts;
                    if (pds.Dts != pcs.Dts)
                        throw InconsistentDts;

                    var vid = new VersionedId<byte>
                    {
                        Id = pds.Id,
                        Version = pds.Version,
                    };

                    if (palettes.ContainsKey(vid))
                    {
                        throw new DisplaySetException($"Duplicate palette VID: {vid}");
                    }

                    var entries = new Dictionary<byte, PaletteEntry>();

                    foreach (var entry in pds.Entries)
                    {
                        entries[entry.Id] = new PaletteEntry
                        {
                            Y = entry.Y,
                            Cr = entry.Cr,
                            Cb = entry.Cb,
                            Alpha = entry.Alpha,
                        };
                    }

                    palettes[vid] = new Palette
                    {
                        Entries = entries,
                    };

                    break;
                }
                case SingleObjectDefinitionSegment sods:
                {
                    if (sods.Pts != pcs.Pts)
                        throw InconsistentPts;
                    if (sods.Dts != pcs.Dts)
                        throw InconsistentDts;

                    if (sequence == Sequence.Single || sequence == Sequence.Final)
                    {
                        var vid = new VersionedId<ushort>
                        {
                            Id = sods.Id,
                            Version = sods.Version,
                        };

                        if (displayObjects.ContainsKey(vid))
                            throw new DisplaySetException($"Duplicate object VID: {vid}");

                        displayObjects[vid] = new DisplayObject
                        {
                            Width = sods.Width,
                            Height = sods.Height,
                            Lines = Rle.Decompress(sods.Data),
                        };

                        sequence = Sequence.Single;
                    }
                    else
                    {
                        throw new DisplaySetException("Invalid object sequence state.");
                    }

                    break;
                };
                default:
                {
                    break;
                }
            }
        }

        return null;
    }
}
