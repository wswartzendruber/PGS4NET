/*
 * Copyright 2024 William Swartzendruber
 *
 * To the extent possible under law, the person who associated CC0 with this file has waived all
 * copyright and related or neighboring rights to this file.
 *
 * You should have received a copy of the CC0 legalcode along with this work. If not, see
 * <http://creativecommons.org/publicdomain/zero/1.0/>.
 *
 * SPDX-License-Identifier: CC0-1.0
 */

using PGS4NET.Captions;
using PGS4NET.DisplaySets;

namespace PGS4NET.Tests.Captions;

public class CaptionComposerTests
{
    [Fact]
    public void ComposerClearsAfterEnd()
    {
        using var inputStream = new MemoryStream(SintelSubtitles.Buffer);
        var composer = new CaptionComposer();
        var displaySets = inputStream.ReadAllDisplaySets();

        foreach (var displaySet in displaySets)
            composer.Input(displaySet);
        
        Assert.False(composer.Pending);
    }
}
