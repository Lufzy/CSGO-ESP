using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace Memorys
{
    internal class bufferByte
    {
        static Process m_iProcess;
        static IntPtr m_iProcessHandle;

        static int m_iBytesWritten;
        static int m_iBytesRead;

        public static void Initialize(string processName)
        {
            if (Process.GetProcessesByName(processName).Length > 0)
            {
                m_iProcess = Process.GetProcessesByName(processName)[0];
                m_iProcessHandle = Imports.OpenProcess(Flags.PROCESS_VM_OPERATION | Flags.PROCESS_VM_READ | Flags.PROCESS_VM_WRITE, false, m_iProcess.Id);
            }
            else
            {
                Environment.Exit(0);
                throw new Exception("Process not found.");
            }
        }

        public static void Write<T>(int Address, object Value)
        {
            byte[] buffer = StructureToByteArray(Value);

            Imports.WriteProcessMemory((int)m_iProcessHandle, Address, buffer, buffer.Length, out m_iBytesWritten);
        }

        public static void Write<T>(int Adress, char[] Value)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(Value);

            Imports.WriteProcessMemory((int)m_iProcessHandle, Adress, buffer, buffer.Length, out m_iBytesWritten);
        }

        public static T Read<T>(int address) where T : struct
        {
            var ByteSize = Marshal.SizeOf(typeof(T));

            byte[] buffer = new byte[ByteSize];

            Imports.ReadProcessMemory((int)m_iProcessHandle, address, buffer, buffer.Length, ref m_iBytesRead);

            return ByteArrayToStructure<T>(buffer);
        }

        public static byte[] Read(int offset, int size)
        {
            byte[] buffer = new byte[size];

            Imports.ReadProcessMemory((int)m_iProcessHandle, offset, buffer, size, ref m_iBytesRead);

            return buffer;
        }

        public static float[] ReadMatrix<T>(int Adress, int MatrixSize) where T : struct
        {
            var ByteSize = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[ByteSize * MatrixSize];
            Imports.ReadProcessMemory((int)m_iProcessHandle, Adress, buffer, buffer.Length, ref m_iBytesRead);

            return ConvertToFloatArray(buffer);
        }

        public static Vector2 WorldToScreen(Vector3 target, int width, int height, float[] viewMatrix)
        {
            Vector2 _worldToScreenPos;
            Vector3 to;
            float w = 0.0f;
            float[] viewmatrix = new float[16];
            viewmatrix = viewMatrix;

            to.X = viewmatrix[0] * target.X + viewmatrix[1] * target.Y + viewmatrix[2] * target.Z + viewmatrix[3];
            to.Y = viewmatrix[4] * target.X + viewmatrix[5] * target.Y + viewmatrix[6] * target.Z + viewmatrix[7];

            w = viewmatrix[12] * target.X + viewmatrix[13] * target.Y + viewmatrix[14] * target.Z + viewmatrix[15];

            // behind us
            if (w < 0.01f)
                return new Vector2(0, 0);

            to.X *= (1.0f / w);
            to.Y *= (1.0f / w);

            //int width = Main.ScreenSize.Width;
            //int height = Main.ScreenSize.Height;

            float x = width / 2;
            float y = height / 2;

            x += 0.5f * to.X * width + 0.5f;
            y -= 0.5f * to.Y * height + 0.5f;

            to.X = x;
            to.Y = y;

            _worldToScreenPos.X = to.X;
            _worldToScreenPos.Y = to.Y;
            return _worldToScreenPos;
        }

        public static IntPtr FindDMAAddy(IntPtr ptr, int[] offsets) // https://stackoverflow.com/questions/35788512/c-sharp-need-to-add-one-offset-to-two-addresses-that-leave-up-to-my-value
        {
            var buffer = new byte[IntPtr.Size];
            foreach (int i in offsets)
            {
                Imports.ReadProcessMemory((int)m_iProcessHandle, (int)ptr, buffer, buffer.Length, ref m_iBytesRead);

                ptr = (IntPtr.Size == 4)
                ? IntPtr.Add(new IntPtr(BitConverter.ToInt32(buffer, 0)), i)
                : ptr = IntPtr.Add(new IntPtr(BitConverter.ToInt64(buffer, 0)), i);
            }
            return ptr;
        }

        public static int GetModuleAddress(string ModuleName)
        {
            try
            {
                foreach (ProcessModule ProcMod in m_iProcess.Modules)
                {
                    if (ModuleName == ProcMod.ModuleName)
                    {
                        return (Int32)ProcMod.BaseAddress;
                    }
                }
            }
            catch
            {
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: Cannot find - " + ModuleName + " | Check file extension.");
            Console.ResetColor();

            return -1;
        }

        #region Signature Scanning

        public static int ScanPattern(string mModuleName, string mPattern, int Offset = 0, int Extra = 0, bool ModuleSubract = false)
        {
            IntPtr hProcess = m_iProcess.Handle;
            ProcessModule SelectedModule = null;
            foreach (ProcessModule module in m_iProcess.Modules)
            {
                if (Path.GetFileName(module.FileName) == mModuleName)
                {
                    SelectedModule = module;
                }
            }
            if (SelectedModule == null)
            {
                throw new Exception("Selected Module is Null !");
            }
            var scanner = new SigScan(hProcess);
            scanner.SelectModule(SelectedModule.BaseAddress, SelectedModule.ModuleMemorySize);
            int Scaned = (int)scanner.FindPattern(mPattern, out long time);
            if (Scaned != 0)
            {
                var Scan = BitConverter.ToInt32(Read(Scaned + Offset, 4), 0) + Extra;
                if (ModuleSubract) Scan -= (Int32)SelectedModule.BaseAddress;
                return Scan;
            }
            else
            {
                return 0;
            }
        }
        private class SigScan
        {
            private IntPtr g_hProcess { get; set; }
            private byte[] g_arrModuleBuffer { get; set; }
            private ulong g_lpModuleBase { get; set; }

            private Dictionary<string, string> g_dictStringPatterns { get; }

            public SigScan(IntPtr hProc)
            {
                g_hProcess = hProc;
                g_dictStringPatterns = new Dictionary<string, string>();
            }

            public bool SelectModule(IntPtr hModule, int SizeOfImage)
            {
                g_lpModuleBase = (ulong)hModule;
                g_arrModuleBuffer = new byte[SizeOfImage];

                g_dictStringPatterns.Clear();

                return NativeMethods.ReadProcessMemory(g_hProcess, hModule, g_arrModuleBuffer, SizeOfImage, out IntPtr a);
            }

            public void AddPattern(string szPatternName, string szPattern)
            {
                g_dictStringPatterns.Add(szPatternName, szPattern);
            }

            private bool PatternCheck(int nOffset, byte[] arrPattern)
            {
                for (int i = 0; i < arrPattern.Length; i++)
                {
                    if (arrPattern[i] == 0x0)
                        continue;

                    if (arrPattern[i] != this.g_arrModuleBuffer[nOffset + i])
                        return false;
                }

                return true;
            }

            public ulong FindPattern(string szPattern, out long lTime)
            {
                if (g_arrModuleBuffer == null || g_lpModuleBase == 0)
                    throw new Exception("Selected module is null");

                Stopwatch stopwatch = Stopwatch.StartNew();

                byte[] arrPattern = ParsePatternString(szPattern);

                for (int nModuleIndex = 0; nModuleIndex < g_arrModuleBuffer.Length; nModuleIndex++)
                {
                    if (this.g_arrModuleBuffer[nModuleIndex] != arrPattern[0])
                        continue;

                    if (PatternCheck(nModuleIndex, arrPattern))
                    {
                        lTime = stopwatch.ElapsedMilliseconds;
                        return g_lpModuleBase + (ulong)nModuleIndex;
                    }
                }

                lTime = stopwatch.ElapsedMilliseconds;
                return 0;
            }
            public Dictionary<string, ulong> FindPatterns(out long lTime)
            {
                if (g_arrModuleBuffer == null || g_lpModuleBase == 0)
                    throw new Exception("Selected module is null");

                Stopwatch stopwatch = Stopwatch.StartNew();

                byte[][] arrBytePatterns = new byte[g_dictStringPatterns.Count][];
                ulong[] arrResult = new ulong[g_dictStringPatterns.Count];

                // PARSE PATTERNS
                for (int nIndex = 0; nIndex < g_dictStringPatterns.Count; nIndex++)
                    arrBytePatterns[nIndex] = ParsePatternString(g_dictStringPatterns.ElementAt(nIndex).Value);

                // SCAN FOR PATTERNS
                for (int nModuleIndex = 0; nModuleIndex < g_arrModuleBuffer.Length; nModuleIndex++)
                {
                    for (int nPatternIndex = 0; nPatternIndex < arrBytePatterns.Length; nPatternIndex++)
                    {
                        if (arrResult[nPatternIndex] != 0)
                            continue;

                        if (this.PatternCheck(nModuleIndex, arrBytePatterns[nPatternIndex]))
                            arrResult[nPatternIndex] = g_lpModuleBase + (ulong)nModuleIndex;
                    }
                }

                Dictionary<string, ulong> dictResultFormatted = new Dictionary<string, ulong>();

                // FORMAT PATTERNS
                for (int nPatternIndex = 0; nPatternIndex < arrBytePatterns.Length; nPatternIndex++)
                    dictResultFormatted[g_dictStringPatterns.ElementAt(nPatternIndex).Key] = arrResult[nPatternIndex];

                lTime = stopwatch.ElapsedMilliseconds;
                return dictResultFormatted;
            }

            private byte[] ParsePatternString(string szPattern)
            {
                List<byte> patternbytes = new List<byte>();

                foreach (var szByte in szPattern.Split(' '))
                    patternbytes.Add(szByte == "?" ? (byte)0x0 : Convert.ToByte(szByte, 16));

                return patternbytes.ToArray();
            }
        }

        #endregion SigScanning

        #region Conversion

        public static float[] ConvertToFloatArray(byte[] bytes)
        {
            if (bytes.Length % 4 != 0)
                throw new ArgumentException();

            float[] floats = new float[bytes.Length / 4];

            for (var i = 0; i < floats.Length; i++)
                floats[i] = BitConverter.ToSingle(bytes, i * 4);

            return floats;
        }

        static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }

        static byte[] StructureToByteArray(object obj)
        {
            var length = Marshal.SizeOf(obj);

            byte[] array = new byte[length];

            var pointer = Marshal.AllocHGlobal(length);

            Marshal.StructureToPtr(obj, pointer, true);
            Marshal.Copy(pointer, array, 0, length);
            Marshal.FreeHGlobal(pointer);

            return array;
        }

        #endregion

        #region Other

        #region kernel32.dll

        private static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
            public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

            [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
            public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, AllocationType dwFreeType);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern int WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);
        }

        #endregion

        internal struct Flags
        {
            public const int PROCESS_VM_OPERATION = 0x0008;
            public const int PROCESS_VM_READ = 0x0010;
            public const int PROCESS_VM_WRITE = 0x0020;
        }

        #region Flags

        public const int INFINITE = -1;
        public const int WAIT_ABANDONED = 0x80;
        public const int WAIT_OBJECT_0 = 0x00;
        public const int WAIT_TIMEOUT = 0x102;
        public const int WAIT_FAILED = -1;

        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        #endregion

        internal class Imports
        {
            [DllImport("kernel32.dll")]
            public static extern IntPtr OpenProcess(int DesiredAccess, bool InheritHandle, int m_iProcessID);

            [DllImport("kernel32.dll")]
            public static extern bool ReadProcessMemory(int h_Process, int BaseAddress, byte[] buffer, int size, ref int BytesRead);

            [DllImport("kernel32.dll")]
            public static extern bool WriteProcessMemory(int h_Process, int BaseAddress, byte[] buffer, int size, out int BytesWritten);
        }

        #endregion
    }
}
