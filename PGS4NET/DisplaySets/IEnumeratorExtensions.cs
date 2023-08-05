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
using System.Linq;
using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

/// <summary>
///     Contains extensions against <see cref="IEnumerator{Segment}" /> for reading PGS display
///     sets.
/// </summary>
public static partial class IEnumeratorExtensions
{
    private static readonly DisplaySetException DuplicateObjectVid
        = new("Duplicate object VID.");
    private static readonly DisplaySetException InconsistentObjectId
        = new("Object portions have inconsistent IDs.");
    private static readonly DisplaySetException InconsistentObjectVersion
        = new("Object portions have inconsistent versions.");
    private static readonly DisplaySetException InconsistentPts
        = new("PTS is not consistent with PCS.");
    private static readonly DisplaySetException InconsistentDts
        = new("DTS is not consistent with PCS.");
    private static readonly DisplaySetException InvalidObjectSequence
        = new("Invalid object sequence state.");

    /// <summary>
    ///     Builds a PGS display set from an enumerator over a collection of segments. The
    ///     enumerator will be advanced in order to parse the first segment. Advancement will
    ///     stop after the end segment (ES) is reached or an error occurs.
    /// </summary>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a sequence of segments cannot form a display set.
    /// </exception>
    public static DisplaySet BuildDisplaySet(this IEnumerator<Segment> segments)
    {
        InitialObjectDefinitionSegment? initialObject = null;
        List<MiddleObjectDefinitionSegment> middleObjects = new();
        Dictionary<byte, Window> windows = new();
        Dictionary<VersionedId<byte>, Palette> palettes = new();
        Dictionary<VersionedId<ushort>, DisplayObject> displayObjects = new();
        Dictionary<CompositionId, CompositionObject> compositionObjects = new();
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

                    if (initialObject is null)
                    {
                        var vid = new VersionedId<ushort>
                        {
                            Id = sods.Id,
                            Version = sods.Version,
                        };

                        if (displayObjects.ContainsKey(vid))
                            throw DuplicateObjectVid;

                        displayObjects[vid] = new DisplayObject
                        {
                            Width = sods.Width,
                            Height = sods.Height,
                            Lines = Rle.Decompress(sods.Data),
                        };
                    }
                    else
                    {
                        throw InvalidObjectSequence;
                    }

                    break;
                };
                case InitialObjectDefinitionSegment iods:
                {
                    if (iods.Pts != pcs.Pts)
                        throw InconsistentPts;
                    if (iods.Dts != pcs.Dts)
                        throw InconsistentDts;

                    if (initialObject is null)
                    {
                        var vid = new VersionedId<ushort>
                        {
                            Id = iods.Id,
                            Version = iods.Version,
                        };

                        if (displayObjects.ContainsKey(vid))
                            throw DuplicateObjectVid;

                        initialObject = iods;
                    }
                    else
                    {
                        throw InvalidObjectSequence;
                    }

                    break;
                }
                case MiddleObjectDefinitionSegment mods:
                {
                    if (mods.Pts != pcs.Pts)
                        throw InconsistentPts;
                    if (mods.Dts != pcs.Dts)
                        throw InconsistentDts;

                    if (initialObject is not null)
                    {
                        if (mods.Id != initialObject.Id)
                            throw InconsistentObjectId;
                        if (mods.Version != initialObject.Version)
                            throw InconsistentObjectVersion;

                        middleObjects.Add(mods);
                    }
                    else
                    {
                        throw InvalidObjectSequence;
                    }

                    break;
                }
                case FinalObjectDefinitionSegment fods:
                {
                    if (fods.Pts != pcs.Pts)
                        throw InconsistentPts;
                    if (fods.Dts != pcs.Dts)
                        throw InconsistentDts;

                    if (initialObject is not null)
                    {
                        if (fods.Id != initialObject.Id)
                            throw InconsistentObjectId;
                        if (fods.Version != initialObject.Version)
                            throw InconsistentObjectVersion;

                        var vid = new VersionedId<ushort>
                        {
                            Id = fods.Id,
                            Version = fods.Version,
                        };
                        var data = new List<byte>();

                        data.AddRange(initialObject.Data);
                        foreach (var middleObject in middleObjects)
                            data.AddRange(middleObject.Data);
                        data.AddRange(fods.Data);

                        displayObjects[vid] = new DisplayObject
                        {
                            Width = initialObject.Width,
                            Height = initialObject.Height,
                            Lines = Rle.Decompress(data),
                        };

                        initialObject = null;
                        middleObjects.Clear();
                    }
                    else
                    {
                        throw InvalidObjectSequence;
                    }

                    break;
                }
                case EndSegment es:
                {
                    if (initialObject is not null)
                        throw new DisplaySetException("Incomplete object sequence.");

                    if (es.Pts != pcs.Pts)
                        throw InconsistentPts;
                    if (es.Dts != pcs.Dts)
                        throw InconsistentDts;

                    foreach (var compositionObject in pcs.CompositionObjects)
                    {
                        var cid = new CompositionId
                        {
                            ObjectId = compositionObject.ObjectId,
                            WindowId = compositionObject.WindowId,
                        };

                        compositionObjects[cid] = new CompositionObject
                        {
                            X = compositionObject.X,
                            Y = compositionObject.Y,
                            Forced = compositionObject.Forced,
                            Crop = compositionObject.Crop,
                        };
                    }

                    var composition = new Composition
                    {
                        Number = pcs.Number,
                        State = pcs.State,
                        CompositionObjects = compositionObjects,
                    };

                    if (pcs.PaletteUpdateOnly)
                    {
                        if (!palettes.Keys.Any(vid => vid.Id == pcs.PaletteUpdateId))
                        {
                            throw new DisplaySetException("Palette update references unknown "
                                + $"palette ID: {pcs.PaletteUpdateId}");
                        }
                    }

                    return new DisplaySet
                    {
                        Pts = pcs.Pts,
                        Dts = pcs.Dts,
                        Width = pcs.Width,
                        Height = pcs.Height,
                        FrameRate = pcs.FrameRate,
                        PaletteUpdateOnly = pcs.PaletteUpdateOnly,
                        PaletteUpdateId = pcs.PaletteUpdateId,
                        Windows = windows,
                        Palettes = palettes,
                        DisplayObjects = displayObjects,
                        Composition = composition,
                    };
                }
                default:
                {
                    throw new DisplaySetException("Unrecognized segment kind.");
                }
            }
        }

        throw new DisplaySetException("EOF encountered before ES.");
    }
}
