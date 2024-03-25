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
using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

public class DisplaySetComposer
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

    private InitialObjectDefinitionSegment? InitialObject = null;
    private List<MiddleObjectDefinitionSegment> MiddleObjects = new();
    private Dictionary<byte, DisplayWindow> Windows = new();
    private Dictionary<VersionedId<byte>, DisplayPalette> Palettes = new();
    private Dictionary<VersionedId<ushort>, DisplayObject> Objects = new();
    private Dictionary<CompositionId, DisplayComposition> Compositions = new();
    private PresentationCompositionSegment? Pcs = null;

    public DisplaySet? Input(Segment segment)
    {
        if (Pcs is null)
        {
            Pcs = segment as PresentationCompositionSegment
                ?? throw new DisplaySetException("Starting segment not PCS.");

            return null;
        }
        else
        {
            switch (segment)
            {
                case PresentationCompositionSegment:
                {
                    throw new DisplaySetException("Unexpected PCS.");
                }
                case WindowDefinitionSegment wds:
                {
                    if (wds.Pts != Pcs.Pts)
                        throw InconsistentPts;
                    if (wds.Dts != Pcs.Dts)
                        throw InconsistentDts;

                    foreach (var wd in wds.Definitions)
                    {
                        if (Windows.ContainsKey(wd.Id))
                            throw new DisplaySetException($"Duplicate window ID: {wd.Id}");

                        Windows[wd.Id] = new DisplayWindow
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
                    if (pds.Pts != Pcs.Pts)
                        throw InconsistentPts;
                    if (pds.Dts != Pcs.Dts)
                        throw InconsistentDts;

                    var vid = new VersionedId<byte>
                    {
                        Id = pds.Id,
                        Version = pds.Version,
                    };

                    if (Palettes.ContainsKey(vid))
                    {
                        throw new DisplaySetException($"Duplicate palette VID: {vid}");
                    }

                    var entries = new Dictionary<byte, PgsPixel>();

                    foreach (var entry in pds.Entries)
                    {
                        entries[entry.Id] = new PgsPixel(entry.Pixel.Y, entry.Pixel.Cr
                            , entry.Pixel.Cb, entry.Pixel.Alpha);
                    }

                    Palettes[vid] = new DisplayPalette
                    {
                        Entries = entries,
                    };

                    break;
                }
                case SingleObjectDefinitionSegment sods:
                {
                    if (sods.Pts != Pcs.Pts)
                        throw InconsistentPts;
                    if (sods.Dts != Pcs.Dts)
                        throw InconsistentDts;

                    if (InitialObject is null)
                    {
                        var vid = new VersionedId<ushort>
                        {
                            Id = sods.Id,
                            Version = sods.Version,
                        };

                        if (Objects.ContainsKey(vid))
                            throw DuplicateObjectVid;

                        Objects[vid] = new DisplayObject
                        {
                            Width = sods.Width,
                            Height = sods.Height,
                            Data = Rle.Decompress(sods.Data, sods.Width, sods.Height),
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
                    if (iods.Pts != Pcs.Pts)
                        throw InconsistentPts;
                    if (iods.Dts != Pcs.Dts)
                        throw InconsistentDts;

                    if (InitialObject is null)
                    {
                        var vid = new VersionedId<ushort>
                        {
                            Id = iods.Id,
                            Version = iods.Version,
                        };

                        if (Objects.ContainsKey(vid))
                            throw DuplicateObjectVid;

                        InitialObject = iods;
                    }
                    else
                    {
                        throw InvalidObjectSequence;
                    }

                    break;
                }
                case MiddleObjectDefinitionSegment mods:
                {
                    if (mods.Pts != Pcs.Pts)
                        throw InconsistentPts;
                    if (mods.Dts != Pcs.Dts)
                        throw InconsistentDts;

                    if (InitialObject is not null)
                    {
                        if (mods.Id != InitialObject.Id)
                            throw InconsistentObjectId;
                        if (mods.Version != InitialObject.Version)
                            throw InconsistentObjectVersion;

                        MiddleObjects.Add(mods);
                    }
                    else
                    {
                        throw InvalidObjectSequence;
                    }

                    break;
                }
                case FinalObjectDefinitionSegment fods:
                {
                    if (fods.Pts != Pcs.Pts)
                        throw InconsistentPts;
                    if (fods.Dts != Pcs.Dts)
                        throw InconsistentDts;

                    if (InitialObject is not null)
                    {
                        if (fods.Id != InitialObject.Id)
                            throw InconsistentObjectId;
                        if (fods.Version != InitialObject.Version)
                            throw InconsistentObjectVersion;

                        var vid = new VersionedId<ushort>
                        {
                            Id = fods.Id,
                            Version = fods.Version,
                        };
                        var data = new List<byte>();

                        data.AddRange(InitialObject.Data);
                        foreach (var middleObject in MiddleObjects)
                            data.AddRange(middleObject.Data);
                        data.AddRange(fods.Data);

                        Objects[vid] = new DisplayObject
                        {
                            Width = InitialObject.Width,
                            Height = InitialObject.Height,
                            Data = Rle.Decompress(data.ToArray(), InitialObject.Width
                                , InitialObject.Height),
                        };

                        InitialObject = null;
                        MiddleObjects.Clear();
                    }
                    else
                    {
                        throw InvalidObjectSequence;
                    }

                    break;
                }
                case EndSegment es:
                {
                    if (InitialObject is not null)
                        throw new DisplaySetException("Incomplete object sequence.");

                    if (es.Pts != Pcs.Pts)
                        throw InconsistentPts;
                    if (es.Dts != Pcs.Dts)
                        throw InconsistentDts;

                    foreach (var compositionObject in Pcs.CompositionObjects)
                    {
                        var cid = new CompositionId
                        {
                            ObjectId = compositionObject.ObjectId,
                            WindowId = compositionObject.WindowId,
                        };

                        Compositions[cid] = new DisplayComposition
                        {
                            X = compositionObject.X,
                            Y = compositionObject.Y,
                            Forced = compositionObject.Forced,
                            Crop = compositionObject.Crop,
                        };
                    }

                    var returnValue = new DisplaySet
                    {
                        Pts = Pcs.Pts,
                        Dts = Pcs.Dts,
                        Width = Pcs.Width,
                        Height = Pcs.Height,
                        FrameRate = Pcs.FrameRate,
                        PaletteUpdateOnly = Pcs.PaletteUpdateOnly,
                        PaletteId = Pcs.PaletteId,
                        Windows = Windows,
                        Palettes = Palettes,
                        Objects = Objects,
                        CompositionNumber = Pcs.Number,
                        CompositionState = Pcs.State,
                        Compositions = Compositions,
                    };

                    Reset();

                    return returnValue;
                }
                default:
                {
                    throw new DisplaySetException("Unrecognized segment kind.");
                }
            }
        }

        return null;
    }

    public void Reset()
    {
        InitialObject = null;
        MiddleObjects = new();
        Windows = new();
        Palettes = new();
        Objects = new();
        Compositions = new();
        Pcs = null;
    }
}
