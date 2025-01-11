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

namespace PGS4NET.Segments;

/// <summary>
///     Represents a PGS bitstream error.
/// </summary>
public class SegmentException : PgsException
{
    /// <summary>
    ///     Initializes a new instance of the class.
    /// </summary>
    public SegmentException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the class with the specified error message.
    /// </summary>
    /// <param name="message">
    ///     The message that describes the error.
    /// </param>
    public SegmentException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the class with a specified error message and a
    ///     reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">
    ///     The message that describes the error.
    /// </param>
    /// <param name="inner">
    ///     The exception that is the cause of the current exception.
    /// </param>
    public SegmentException(string message, Exception inner) : base(message, inner)
    {
    }
}
