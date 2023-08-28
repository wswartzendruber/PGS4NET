/*
 * Copyright 2023 William Swartzendruber
 *
 * To the extent possible under law, the person who associated CC0 with this file has waived all
 * copyright and related or neighboring rights to this file.
 *
 * You should have received a copy of the CC0 legalcode along with this work. If not, see
 * <http://creativecommons.org/publicdomain/zero/1.0/>.
 *
 * SPDX-License-Identifier: CC0-1.0
 */

using PGS4NET.DisplaySets;

namespace PGS4NET.Tests.DisplaySets;

internal static class DisplaySetInstances
{
    internal static readonly Dictionary<string, DisplaySet> Instances = new()
    {
        {
            "defaults", new DisplaySet
            {
            }
        },
    };
}
