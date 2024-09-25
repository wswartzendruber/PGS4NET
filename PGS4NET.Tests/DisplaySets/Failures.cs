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

using PGS4NET.DisplaySets;
using PGS4NET.Segments;

namespace PGS4NET.Tests.DisplaySets;

public class Failures
{
    [Fact]
    public void EmptyStream()
    {
        using var stream = new MemoryStream();

        if (stream.ReadDisplaySet() is not null)
            throw new Exception("Returned display set is not null.");
    }

    [Fact]
    public async Task EmptyStreamAsync()
    {
        using var stream = new MemoryStream();

        if (await stream.ReadDisplaySetAsync() is not null)
            throw new Exception("Returned display set is not null.");
    }

    [Fact]
    public void IncompleteAll()
    {
        using var stream = new MemoryStream();
        var pcs = new PresentationCompositionSegment();

        stream.WriteSegment(pcs);
        stream.Position = 0;

        try
        {
            stream.ReadAllDisplaySets();

            throw new Exception("Successfully read an incomplete display set.");
        }
        catch (DisplaySetException dse)
        {
            if (dse.Message != "EOF during display set composition.")
                throw new Exception("Expected specific error message on composition.");
        }
    }

    [Fact]
    public async Task IncompleteAllAsync()
    {
        using var stream = new MemoryStream();
        var pcs = new PresentationCompositionSegment();

        stream.WriteSegment(pcs);
        stream.Position = 0;

        try
        {
            await stream.ReadAllDisplaySetsAsync();

            throw new Exception("Successfully read an incomplete display set.");
        }
        catch (DisplaySetException dse)
        {
            if (dse.Message != "EOF during display set composition.")
                throw new Exception("Expected specific error message on composition.");
        }
    }

    [Fact]
    public void IncompleteSingle()
    {
        using var stream = new MemoryStream();
        var pcs = new PresentationCompositionSegment();

        stream.WriteSegment(pcs);
        stream.Position = 0;

        try
        {
            stream.ReadDisplaySet();

            throw new Exception("Successfully read an incomplete display set.");
        }
        catch (DisplaySetException dse)
        {
            if (dse.Message != "EOF during display set composition.")
                throw new Exception("Expected specific error message on composition.");
        }
    }

    [Fact]
    public async Task IncompleteSingleAsync()
    {
        using var stream = new MemoryStream();
        var pcs = new PresentationCompositionSegment();

        stream.WriteSegment(pcs);
        stream.Position = 0;

        try
        {
            await stream.ReadDisplaySetAsync();

            throw new Exception("Successfully read an incomplete display set.");
        }
        catch (DisplaySetException dse)
        {
            if (dse.Message != "EOF during display set composition.")
                throw new Exception("Expected specific error message on composition.");
        }
    }

    [Fact]
    public void NullByte()
    {
        using var stream = new MemoryStream();

        stream.WriteByte(0x00);
        stream.Position = 0;

        try
        {
            stream.ReadDisplaySet();

            throw new Exception("Successfully read a display set with a trailing null byte.");
        }
        catch (IOException ioe)
        {
            if (ioe.Message != "EOF reading segment header.")
                throw new Exception("Expected specific error message on header EOF.");
        }
    }

    [Fact]
    public async Task NullByteAsync()
    {
        using var stream = new MemoryStream();

        stream.WriteByte(0x00);
        stream.Position = 0;

        try
        {
            await stream.ReadDisplaySetAsync();

            throw new Exception("Successfully read a display set with a trailing null byte.");
        }
        catch (IOException ioe)
        {
            if (ioe.Message != "EOF reading segment header.")
                throw new Exception("Expected specific error message on header EOF.");
        }
    }

    [Fact]
    public void TrailingNullByte()
    {
        using var stream = new MemoryStream();
        var pcs = new PresentationCompositionSegment();
        var es = new EndSegment();

        stream.WriteSegment(pcs);
        stream.WriteSegment(es);
        stream.WriteByte(0x00);
        stream.Position = 0;

        try
        {
            stream.ReadAllDisplaySets();

            throw new Exception("Successfully read a display set with a trailing null byte.");
        }
        catch (IOException ioe)
        {
            if (ioe.Message != "EOF reading segment header.")
                throw new Exception("Expected specific error message on header EOF.");
        }
    }

    [Fact]
    public async Task TrailingNullByteAsync()
    {
        using var stream = new MemoryStream();
        var pcs = new PresentationCompositionSegment();
        var es = new EndSegment();

        stream.WriteSegment(pcs);
        stream.WriteSegment(es);
        stream.WriteByte(0x00);
        stream.Position = 0;

        try
        {
            await stream.ReadAllDisplaySetsAsync();

            throw new Exception("Successfully read a display set with a trailing null byte.");
        }
        catch (IOException ioe)
        {
            if (ioe.Message != "EOF reading segment header.")
                throw new Exception("Expected specific error message on header EOF.");
        }
    }
}
