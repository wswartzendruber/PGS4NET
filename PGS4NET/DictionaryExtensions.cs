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

namespace PGS4NET;

public static class DictionaryExtensions
{
    public static bool TryGetLatestValue<TId, TValue>(
        this IDictionary<VersionedId<TId>, TValue> dictionary, TId id, out TValue? returnObject)
        where TId : IEquatable<TId>
    {
        returnObject = default;

        bool found = false;
        short version = -1;

        foreach (var entry in dictionary)
        {
            if (entry.Key.Id.Equals(id) && entry.Key.Version > version)
            {
                found = true;
                returnObject = entry.Value;
            }
        }

        return found;
    }
}
