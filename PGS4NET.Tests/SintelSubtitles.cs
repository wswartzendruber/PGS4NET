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

using System.Reflection;

namespace PGS4NET.Tests;

public static class SintelSubtitles
{
    public static readonly byte[] Buffer;

    static SintelSubtitles()
    {
        var pwd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var subtitlesFile = Path.Join(pwd, "sintel-en.sup");

        Buffer = File.ReadAllBytes(subtitlesFile);
    }
}
