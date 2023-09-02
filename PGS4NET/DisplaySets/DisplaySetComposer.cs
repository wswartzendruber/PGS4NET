/*
 * Copyright 2023 William Swartzendruber
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

/// <summary>
///     Statefully composes display sets using sequentially input segments. Also supports
///     immediate display set decomposition into segments.
/// </summary>
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

    /// <summary>
    ///     Inputs the next <see cref="Segment" /> into the composer, returning a new
    ///     <see cref="DisplaySet" /> instance if one can be composed, or
    ///     <see langword="null" /> if more segments are required.
    /// </summary>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a combination of otherwise valid PGS segments cannot be combined into a
    ///     display set.
    /// </exception>
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

                    var entries = new Dictionary<byte, DisplayPaletteEntry>();

                    foreach (var entry in pds.Entries)
                    {
                        entries[entry.Id] = new DisplayPaletteEntry
                        {
                            Y = entry.Y,
                            Cr = entry.Cr,
                            Cb = entry.Cb,
                            Alpha = entry.Alpha,
                        };
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
                            Lines = Rle.Decompress(data),
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
                        PaletteUpdateId = Pcs.PaletteUpdateId,
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

    /// <summary>
    ///     Resets the composer's internal state.
    /// </summary>
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

    /// <summary>
    ///     Decomposes a single display set into a collection of segments.
    /// </summary>
    public static IList<Segment> Decompose(DisplaySet displaySet)
    {
        var returnValue = new List<Segment>();
        var compositionObjects = new List<CompositionObject>();

        foreach (var item in displaySet.Compositions)
        {
            compositionObjects.Add(new CompositionObject
            {
                ObjectId = item.Key.ObjectId,
                WindowId = item.Key.WindowId,
                X = item.Value.X,
                Y = item.Value.Y,
                Forced = item.Value.Forced,
                Crop = item.Value.Crop,
            });
        }

        returnValue.Add(new PresentationCompositionSegment
        {
            Pts = displaySet.Pts,
            Dts = displaySet.Dts,
            Width = displaySet.Width,
            Height = displaySet.Height,
            FrameRate = displaySet.FrameRate,
            Number = displaySet.CompositionNumber,
            State = displaySet.CompositionState,
            PaletteUpdateOnly = displaySet.PaletteUpdateOnly,
            PaletteUpdateId = displaySet.PaletteUpdateId,
            CompositionObjects = compositionObjects,
        });

        if (displaySet.Windows.Count > 0)
        {
            var windowEntries = new List<WindowDefinitionEntry>();

            foreach (var window in displaySet.Windows)
            {
                windowEntries.Add(new WindowDefinitionEntry
                {
                    Id = window.Key,
                    X = window.Value.X,
                    Y = window.Value.Y,
                    Width = window.Value.Width,
                    Height = window.Value.Height,
                });
            }

            returnValue.Add(new WindowDefinitionSegment
            {
                Pts = displaySet.Pts,
                Dts = displaySet.Dts,
                Definitions = windowEntries,
            });
        }

        foreach (var palette in displaySet.Palettes)
        {
            var paletteEntries = new List<PaletteDefinitionEntry>();

            foreach (var paletteEntry in palette.Value.Entries)
            {
                paletteEntries.Add(new PaletteDefinitionEntry
                {
                    Id = paletteEntry.Key,
                    Y = paletteEntry.Value.Y,
                    Cr = paletteEntry.Value.Cr,
                    Cb = paletteEntry.Value.Cb,
                    Alpha = paletteEntry.Value.Alpha,
                });
            }

            returnValue.Add(new PaletteDefinitionSegment
            {
                Pts = displaySet.Pts,
                Dts = displaySet.Dts,
                Id = palette.Key.Id,
                Version = palette.Key.Version,
                Entries = paletteEntries,
            });
        }

        foreach (var displayObject in displaySet.Objects)
        {
            var data = Rle.Compress(displayObject.Value.Lines);

            if (data.Length > InitialObjectDefinitionSegment.MaxDataSize)
            {
                var index = 0;
                var size = data.Length;
                var iodsBuffer = new byte[InitialObjectDefinitionSegment.MaxDataSize];

                Array.Copy(data, iodsBuffer, iodsBuffer.Length);

                returnValue.Add(new InitialObjectDefinitionSegment
                {
                    Pts = displaySet.Pts,
                    Dts = displaySet.Dts,
                    Id = displayObject.Key.Id,
                    Version = displayObject.Key.Version,
                    Data = iodsBuffer,
                    Length = (uint)data.Length + 4,
                    Width = displayObject.Value.Width,
                    Height = displayObject.Value.Height,
                });

                index += InitialObjectDefinitionSegment.MaxDataSize;
                size -= InitialObjectDefinitionSegment.MaxDataSize;

                while (size > MiddleObjectDefinitionSegment.MaxDataSize)
                {
                    var modsBuffer = new byte[MiddleObjectDefinitionSegment.MaxDataSize];

                    Array.Copy(data, index, modsBuffer, 0, modsBuffer.Length);

                    returnValue.Add(new MiddleObjectDefinitionSegment
                    {
                        Pts = displaySet.Pts,
                        Dts = displaySet.Dts,
                        Id = displayObject.Key.Id,
                        Version = displayObject.Key.Version,
                        Data = modsBuffer,
                    });

                    index += MiddleObjectDefinitionSegment.MaxDataSize;
                    size -= MiddleObjectDefinitionSegment.MaxDataSize;
                }

                var fodsBuffer = new byte[size];

                Array.Copy(data, index, fodsBuffer, 0, fodsBuffer.Length);

                returnValue.Add(new FinalObjectDefinitionSegment
                {
                    Pts = displaySet.Pts,
                    Dts = displaySet.Dts,
                    Id = displayObject.Key.Id,
                    Version = displayObject.Key.Version,
                    Data = fodsBuffer,
                });
            }
            else
            {
                returnValue.Add(new SingleObjectDefinitionSegment
                {
                    Pts = displaySet.Pts,
                    Dts = displaySet.Dts,
                    Id = displayObject.Key.Id,
                    Version = displayObject.Key.Version,
                    Data = data,
                    Width = displayObject.Value.Width,
                    Height = displayObject.Value.Height,
                });
            }
        }

        returnValue.Add(new EndSegment
        {
            Pts = displaySet.Pts,
            Dts = displaySet.Dts,
        });

        return returnValue;
    }
}
