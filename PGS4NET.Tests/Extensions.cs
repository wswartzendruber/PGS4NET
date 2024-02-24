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

namespace PGS4NET.Tests;

public static class Extensions
{
    public static async Task<IList<T>> ToListAsync<T>(this IAsyncEnumerable<T> enumerable)
    {
        var returnValue = new List<T>();

        await foreach (var item in enumerable)
            returnValue.Add(item);

        return returnValue;
    }
}
