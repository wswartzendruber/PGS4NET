/*
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
using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

/// <summary>
///     Constructs <see cref="DisplaySet"/>s from <see cref="Segment"/>s.
/// </summary>
public sealed class DisplaySetComposer
{
    private static readonly DisplaySetException DuplicateObjectVid
        = new("Duplicate object VID.");
    private static readonly DisplaySetException InconsistentObjectId
        = new("Object portions have inconsistent IDs.");
    private static readonly DisplaySetException InconsistentObjectVersion
        = new("Object portions have inconsistent versions.");
    private static readonly DisplaySetException InconsistentPresentationTime
        = new("PTS is not consistent with PCS.");
    private static readonly DisplaySetException InconsistentDecodeTime
        = new("DTS is not consistent with PCS.");
    private static readonly DisplaySetException InvalidObjectSequence
        = new("Invalid object sequence state.");

    private InitialObjectDefinitionSegment? InitialObject = null;
    private List<MiddleObjectDefinitionSegment> MiddleObjects = new();
    private Dictionary<byte, DisplayWindow> Windows = new();
    private Dictionary<VersionedId<byte>, DisplayPalette> Palettes = new();
    private Dictionary<VersionedId<int>, DisplayObject> Objects = new();
    private Dictionary<CompositionId, DisplayComposition> Compositions = new();
    private PresentationCompositionSegment? Pcs = null;

    /// <summary>
    ///     Returns <see langword="true"/> if the composer has any buffered
    ///     <see cref="Segment"/>s that have not been written out as a <see cref="DisplaySet"/>.
    /// </summary>
    public bool Pending
    {
        get
        {
            return InitialObject is not null
                || MiddleObjects.Count > 0
                || Windows.Count > 0
                || Palettes.Count > 0
                || Objects.Count > 0
                || Compositions.Count > 0
                || Pcs is not null;
        }
    }

    /// <summary>
    ///     Inputs a <see cref="Segment"/> into the composer, causing <see cref="Ready"/> to
    ///     fire for any new <see cref="DisplaySet"/> that becomes available as a result.
    /// </summary>
    /// <param name="segment">
    ///     The segment to input.
    /// </param>
    /// <exception cref="DisplaySetException">
    ///     The <paramref name="segment"/> is not valid given the composer's state.
    /// </exception>
    public void Input(Segment segment)
    {
        if (Pcs is null)
        {
            Pcs = segment as PresentationCompositionSegment
                ?? throw new DisplaySetException("Starting segment not PCS.");
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
                    if (wds.PresentationTime != Pcs.PresentationTime)
                        throw InconsistentPresentationTime;
                    if (wds.DecodeStartTime != Pcs.DecodeStartTime)
                        throw InconsistentDecodeTime;

                    foreach (var wd in wds.Definitions)
                    {
                        if (Windows.ContainsKey(wd.Id))
                            throw new DisplaySetException($"Duplicate window ID: {wd.Id}");

                        Windows[wd.Id] = new DisplayWindow(wd.X, wd.Y, wd.Width, wd.Height);
                    }

                    break;
                }
                case PaletteDefinitionSegment pds:
                {
                    if (pds.PresentationTime != Pcs.PresentationTime)
                        throw InconsistentPresentationTime;
                    if (pds.DecodeStartTime != Pcs.DecodeStartTime)
                        throw InconsistentDecodeTime;

                    var vid = pds.VersionedId;

                    if (Palettes.ContainsKey(vid))
                    {
                        throw new DisplaySetException($"Duplicate palette VID: {vid}");
                    }

                    var entries = new Dictionary<byte, YcbcraPixel>();

                    foreach (var entry in pds.Entries)
                    {
                        entries[entry.Id] = new YcbcraPixel(entry.Pixel.Y, entry.Pixel.Cb,
                            entry.Pixel.Cr, entry.Pixel.Alpha);
                    }

                    Palettes[vid] = new DisplayPalette(entries);

                    break;
                }
                case SingleObjectDefinitionSegment sods:
                {
                    if (sods.PresentationTime != Pcs.PresentationTime)
                        throw InconsistentPresentationTime;
                    if (sods.DecodeStartTime != Pcs.DecodeStartTime)
                        throw InconsistentDecodeTime;

                    if (InitialObject is null)
                    {
                        var vid = sods.VersionedId;

                        if (Objects.ContainsKey(vid))
                            throw DuplicateObjectVid;

                        var pixels = Rle.Decompress(sods.Data, sods.Width, sods.Height);

                        Objects[vid] = new DisplayObject(sods.Width, sods.Height, pixels);
                    }
                    else
                    {
                        throw InvalidObjectSequence;
                    }

                    break;
                };
                case InitialObjectDefinitionSegment iods:
                {
                    if (iods.PresentationTime != Pcs.PresentationTime)
                        throw InconsistentPresentationTime;
                    if (iods.DecodeStartTime != Pcs.DecodeStartTime)
                        throw InconsistentDecodeTime;

                    if (InitialObject is null)
                    {
                        var vid = iods.VersionedId;

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
                    if (mods.PresentationTime != Pcs.PresentationTime)
                        throw InconsistentPresentationTime;
                    if (mods.DecodeStartTime != Pcs.DecodeStartTime)
                        throw InconsistentDecodeTime;

                    if (InitialObject is not null)
                    {
                        if (mods.VersionedId.Id != InitialObject.VersionedId.Id)
                            throw InconsistentObjectId;
                        if (mods.VersionedId.Version != InitialObject.VersionedId.Version)
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
                    if (fods.PresentationTime != Pcs.PresentationTime)
                        throw InconsistentPresentationTime;
                    if (fods.DecodeStartTime != Pcs.DecodeStartTime)
                        throw InconsistentDecodeTime;

                    if (InitialObject is not null)
                    {
                        if (fods.VersionedId.Id != InitialObject.VersionedId.Id)
                            throw InconsistentObjectId;
                        if (fods.VersionedId.Version != InitialObject.VersionedId.Version)
                            throw InconsistentObjectVersion;

                        var vid = fods.VersionedId;
                        var data = new List<byte>();

                        data.AddRange(InitialObject.Data);
                        foreach (var middleObject in MiddleObjects)
                            data.AddRange(middleObject.Data);
                        data.AddRange(fods.Data);

                        var pixels = Rle.Decompress(data.ToArray(), InitialObject.Width,
                            InitialObject.Height);

                        Objects[vid] = new DisplayObject(InitialObject.Width,
                            InitialObject.Height, pixels);

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

                    if (es.PresentationTime != Pcs.PresentationTime)
                        throw InconsistentPresentationTime;

                    foreach (var compositionObject in Pcs.CompositionObjects)
                    {
                        var cid = new CompositionId(compositionObject.Id.ObjectId,
                            compositionObject.Id.WindowId);

                        Compositions[cid] = new DisplayComposition(compositionObject.X,
                            compositionObject.Y, compositionObject.Forced,
                            compositionObject.Crop);
                    }

                    var newDisplaySet = new DisplaySet(Pcs.PresentationTime, Pcs.Width,
                        Pcs.Height, Pcs.FrameRate, Pcs.PaletteUpdateOnly, Pcs.PaletteId,
                        Windows, Palettes, Objects, Pcs.Number, Pcs.State, Compositions);

                    Reset();

                    OnReady(newDisplaySet);

                    break;
                }
                default:
                {
                    throw new DisplaySetException("Unrecognized segment kind.");
                }
            }
        }
    }

    /// <summary>
    ///     Resets the state of the composer.
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

    private void OnReady(DisplaySet displaySet)
    {
        Ready?.Invoke(this, displaySet);
    }

    /// <summary>
    ///     Fires when a new <see cref="DisplaySet"/> is ready.
    /// </summary>
    public event EventHandler<DisplaySet>? Ready;
}
