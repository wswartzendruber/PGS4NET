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

namespace PGS4NET.Tests;

internal class TestStream : Stream
{
    public bool Disposed = false;

    public override bool CanRead
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public override bool CanSeek
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public override bool CanWrite
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override long Length
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public override long Position
    {
        get
        {
            throw new NotImplementedException();
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override async ValueTask DisposeAsync()
    {
        Disposed = true;

        await base.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    protected override void Dispose(bool disposing)
    {
        Disposed = true;

        base.Dispose(disposing);
    }
}
