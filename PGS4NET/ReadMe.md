<!--
    Copyright 2023 William Swartzendruber

    To the extent possible under law, the person who associated CC0 with this file has waived
    all copyright and related or neighboring rights to this file.

    You should have received a copy of the CC0 legalcode along with this work. If not, see
    <http://creativecommons.org/publicdomain/zero/1.0/>.

    SPDX-License-Identifier: CC0-1.0
-->

# RuneEncoding

## Introduction

The RuneEncoding library eases the implementation of custom character encodings. It does this by
sitting as an abstraction layer on top of the `System.Text.Encoder` and `System.Text.Decoder`
classes, handling what are essentially boilerplate concerns to Unicode. Primarily, RuneEncoding
handles surrogate composition and decomposition for the implementor. The `RuneEncoder` and
`RuneDecoder` abstract classes allow implementors to only worry about complete Unicode scalar
values.

While a custom encoding built on top of RuneEncoding does not promise to be as fast or as
performant as an optimized one built directly on top of the native .NET classes, it does promise
to considerably simplify the implementation of such an encoding.

## Example

The code snippet below outlines how to implement the `RuneEncoder` class:

```csharp
using RuneEncoding.TextEncoder;

public class HypotheticalRuneEncoder : RuneEncoder
{
    protected override int ByteCount(int scalarValue)
    {
        // Return how many bytes are needed to encode [scalarValue].
        //
        // Take current state into account, but don't modify it.
    }

    protected override int WriteBytes(int scalarValue, byte[] bytes, int index)
    {
        // Write the encoded byte representation of [scalarValue] to [bytes], starting at the
        // provided [index], and then return how many bytes were written.
        //
        // Take current state into account and modify it as appropriate.
    }

    protected override void ResetState()
    {
        // If an encoder has to manage any internal state, that can be reset here.
    }
}
```

This next code snippet outlines how to implement the `RuneDecoder` class:

```csharp
using RuneEncoding.RuneDecoder;

public class HypotheticalRuneDecoder : RuneDecoder
{
    protected override int IsScalarValueBasic(byte[] bytes, int index, int limit, bool first
        , out bool? isBasic)
    {
        // Decode the [bytes] at the specified [index] into a Unicode scalar value; don't
        // exceed [index] + [limit]. If the decoded scalar value is BMP, then set [isBasic] to
        // true. If it requires a surrogate pair, then set it to false. If there are
        // insufficient bytes to decode the value, then set it to null. Return the number of
        // bytes read.
        //
        // Take current state into account, but don't modify it.
    }

    protected override int ReadScalarValue(byte[] bytes, int index, int limit
        , out int? scalarValue)
    {
        // Decode the [bytes] at the specified [index] into a Unicode scalar value; don't
        // exceed [index] + [limit]. Set [scalarValue] to the decoded value. If there are
        // insufficient bytes to decode the value, then set it to null. Return the number of
        // bytes read.
        //
        // Take current state into account and modify it as appropriate.
    }
}
```
