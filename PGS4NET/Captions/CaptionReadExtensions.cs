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
using System.Threading;
using System.Threading.Tasks;
using PGS4NET.DisplaySets;

namespace PGS4NET.Captions;

public static partial class CaptionExtensions
{
    public static IList<Caption> ReadAllCaptions(this Stream stream)
    {
        var returnValue = new List<Caption>();

        while (stream.ReadCaption() is Caption caption)
            returnValue.Add(caption);

        return returnValue;
    }

    public static Caption? ReadCaption(this Stream stream)
    {
        var read = false;
        var composer = new CaptionComposer();
        Caption? caption = null;

        composer.NewCaption += (_, caption_) =>
        {
            caption = caption_;
        };

        while (stream.ReadDisplaySet() is DisplaySet displaySet)
        {
            read = true;
            composer.Input(displaySet);

            if (caption is Caption caption_)
                return caption_;
        }

        if (read)
            throw new DisplaySetException("EOF during caption composition.");

        return null;
    }
}
