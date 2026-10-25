// Copyright (C) 2026 Moe S. / BlackNotepad Contributors
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

using System;
using System.Runtime.InteropServices;

namespace Savaged.BlackNotepad.Services
{
    /// <summary>
    /// Native P/Invoke codec for creating and applying binary diffs using Windows msdelta.dll.
    /// Provides safe pinning and memory cleanup, passing DELTA_INPUT by value to ensure x86 and x64 compatibility.
    /// Falls back gracefully when msdelta.dll is unavailable or fails.
    /// </summary>
    public static class MsDeltaCodec
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct DELTA_INPUT
        {
            /// <summary>Pointer to the start of memory buffer.</summary>
            public IntPtr lpStart;
            /// <summary>Size of the buffer in bytes.</summary>
            public UIntPtr uSize;
            /// <summary>True if buffer can be modified in place.</summary>
            [MarshalAs(UnmanagedType.Bool)]
            public bool Editable;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DELTA_OUTPUT
        {
            /// <summary>Pointer to the allocated output memory buffer.</summary>
            public IntPtr lpStart;
            /// <summary>Size of the output buffer in bytes.</summary>
            public UIntPtr uSize;
        }

        private const long DELTA_FILE_TYPE_RAW = 1;
        private const long DELTA_FLAG_NONE = 0;

        [DllImport("msdelta.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CreateDeltaB(
            long fileTypeSet,
            long setFlags,
            long resetFlags,
            DELTA_INPUT source,
            DELTA_INPUT target,
            DELTA_INPUT sourceOptions,
            DELTA_INPUT targetOptions,
            DELTA_INPUT globalOptions,
            IntPtr lpTargetFileTime,
            uint hashAlgId,
            out DELTA_OUTPUT lpDelta);

        [DllImport("msdelta.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ApplyDeltaB(
            long applyFlags,
            DELTA_INPUT source,
            DELTA_INPUT delta,
            out DELTA_OUTPUT lpTarget);

        [DllImport("msdelta.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeltaFree(IntPtr lpMemory);

        /// <summary>
        /// Computes a native binary delta between sourceBytes and targetBytes.
        /// </summary>
        /// <param name="sourceBytes">The baseline binary payload.</param>
        /// <param name="targetBytes">The updated binary payload.</param>
        /// <returns>Delta byte array if successfully created; null if msdelta fails, is unavailable, or either input is null or empty.</returns>
        public static byte[] CreateDelta(byte[] sourceBytes, byte[] targetBytes)
        {
            if (sourceBytes == null || targetBytes == null || sourceBytes.Length == 0 || targetBytes.Length == 0)
            {
                return null;
            }

            GCHandle sourceHandle = default(GCHandle);
            GCHandle targetHandle = default(GCHandle);

            try
            {
                sourceHandle = GCHandle.Alloc(sourceBytes, GCHandleType.Pinned);
                targetHandle = GCHandle.Alloc(targetBytes, GCHandleType.Pinned);

                var sourceInput = new DELTA_INPUT
                {
                    lpStart = sourceHandle.AddrOfPinnedObject(),
                    uSize = (UIntPtr)sourceBytes.Length,
                    Editable = false
                };

                var targetInput = new DELTA_INPUT
                {
                    lpStart = targetHandle.AddrOfPinnedObject(),
                    uSize = (UIntPtr)targetBytes.Length,
                    Editable = false
                };

                var emptyOptions = default(DELTA_INPUT);
                if (CreateDeltaB(
                    DELTA_FILE_TYPE_RAW,
                    DELTA_FLAG_NONE,
                    DELTA_FLAG_NONE,
                    sourceInput,
                    targetInput,
                    emptyOptions,
                    emptyOptions,
                    emptyOptions,
                    IntPtr.Zero,
                    0,
                    out var output))
                {
                    try
                    {
                        int size = checked((int)output.uSize);
                        var delta = new byte[size];
                        Marshal.Copy(output.lpStart, delta, 0, size);
                        return delta;
                    }
                    finally
                    {
                        DeltaFree(output.lpStart);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MsDeltaCodec.CreateDelta failed: {ex.Message}");
                return null;
            }
            finally
            {
                if (sourceHandle.IsAllocated)
                {
                    sourceHandle.Free();
                }
                if (targetHandle.IsAllocated)
                {
                    targetHandle.Free();
                }
            }
        }

        /// <summary>
        /// Reconstructs targetBytes by applying deltaBytes onto sourceBytes.
        /// </summary>
        /// <param name="sourceBytes">The baseline binary payload.</param>
        /// <param name="deltaBytes">The binary delta payload.</param>
        /// <returns>Reconstructed target byte array; null if application fails, is unavailable, or either input is null or empty.</returns>
        public static byte[] ApplyDelta(byte[] sourceBytes, byte[] deltaBytes)
        {
            if (sourceBytes == null || deltaBytes == null || sourceBytes.Length == 0 || deltaBytes.Length == 0)
            {
                return null;
            }

            GCHandle sourceHandle = default(GCHandle);
            GCHandle deltaHandle = default(GCHandle);

            try
            {
                sourceHandle = GCHandle.Alloc(sourceBytes, GCHandleType.Pinned);
                deltaHandle = GCHandle.Alloc(deltaBytes, GCHandleType.Pinned);

                var sourceInput = new DELTA_INPUT
                {
                    lpStart = sourceHandle.AddrOfPinnedObject(),
                    uSize = (UIntPtr)sourceBytes.Length,
                    Editable = false
                };

                var deltaInput = new DELTA_INPUT
                {
                    lpStart = deltaHandle.AddrOfPinnedObject(),
                    uSize = (UIntPtr)deltaBytes.Length,
                    Editable = false
                };

                if (ApplyDeltaB(DELTA_FLAG_NONE, sourceInput, deltaInput, out var output))
                {
                    try
                    {
                        int size = checked((int)output.uSize);
                        var target = new byte[size];
                        Marshal.Copy(output.lpStart, target, 0, size);
                        return target;
                    }
                    finally
                    {
                        DeltaFree(output.lpStart);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MsDeltaCodec.ApplyDelta failed: {ex.Message}");
                return null;
            }
            finally
            {
                if (sourceHandle.IsAllocated)
                {
                    sourceHandle.Free();
                }
                if (deltaHandle.IsAllocated)
                {
                    deltaHandle.Free();
                }
            }
        }
    }
}
