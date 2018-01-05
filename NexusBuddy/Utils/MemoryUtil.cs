using System;
using System.Runtime.InteropServices;
using System.IO;

namespace NexusBuddy.Utils
{
    public unsafe class MemoryUtil
    {
        // MemCpy code from www.abstractpath.com/2009/memcpy-in-c/
        public delegate void MemCpyFunction(void* des, void* src, uint bytes);

        public static readonly MemCpyFunction MemCpy;

        static MemoryUtil()
        {
            var dynamicMethod = new System.Reflection.Emit.DynamicMethod
            (
                "MemCpy",
                typeof(void),
                new[] { typeof(void*), typeof(void*), typeof(uint) },
                typeof(MemoryUtil)
            );

            var ilGenerator = dynamicMethod.GetILGenerator();

            ilGenerator.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
            ilGenerator.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
            ilGenerator.Emit(System.Reflection.Emit.OpCodes.Ldarg_2);

            ilGenerator.Emit(System.Reflection.Emit.OpCodes.Cpblk);
            ilGenerator.Emit(System.Reflection.Emit.OpCodes.Ret);

            MemCpy = (MemCpyFunction)dynamicMethod.CreateDelegate(typeof(MemCpyFunction));
        }

        public static void memLogLine(string messageLine)
        {
            StreamWriter logFileWriter = new StreamWriter(new FileStream("memlog.log", FileMode.Append));
            logFileWriter.WriteLine(messageLine);
            logFileWriter.Close();
        }

        public static void memLogClear()
        {
            File.WriteAllText("memlog.log", System.String.Empty);
        }

        public static void memLog(string message)
        {
            StreamWriter logFileWriter = new StreamWriter(new FileStream("memlog.log", FileMode.Append));
            logFileWriter.Write(message);
            logFileWriter.Close();
        }

        public static unsafe void memProbe(IntPtr probePtr)
        
        {
            memLogLine("Mem Probe - Start");
            for (int i = 0; i <= 10000; i++)
            {
                memLogLine("i: " + i + " " +
                    (*(int*)(probePtr) + i) + " / " +
                    *(uint*)(*(int*)(probePtr) + i) + " " +
                    *(int*)(*(int*)(probePtr) + i) + " " +
                    (*(float*)(*(int*)(probePtr) + i)).ToString("f6") + " " +
                    *(byte*)(*(int*)(probePtr) + i) + " " +
                    (NumberUtils.halfToFloat(*(UInt16*)(*(int*)(probePtr) + i))).ToString("f6") + " " +
                    Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)(*(int*)(probePtr) + i))
                 );
            }
            memLogLine("Mem Probe - End");
            memLogLine("---");
        }

        public static IntPtr getStringIntPtr(string inString)
        {
            sbyte* stringPointer = (sbyte*)Marshal.StringToHGlobalAnsi(inString).ToPointer();
            return new IntPtr((void*)stringPointer);
        }

        public static string getIntPtrString(int spointer)
        {
            IntPtr sIntPtr = new IntPtr((void*)spointer);
            return Marshal.PtrToStringAnsi(sIntPtr);
        }

        public static void memStringScan(IntPtr probePtr, string searchString, int lines)
        {
            memLogLine("Mem Scan - Start");
            for (int i = 0; i <= lines; i++)
            {
                string checkString = Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)(*(int*)(probePtr) + i));

                if (!System.String.IsNullOrEmpty(checkString) && checkString.Contains(searchString) && checkString.Length < 50)
                {
                    memLogLine("i: " + i + " " + checkString);
                }
            }
            memLogLine("Mem Scan - End");
            memLogLine("---");
        }

        public static void memUintScan(IntPtr probePtr, uint searchUint, int lines)
        {
            memLogLine("Mem Scan - Start");
            for (int i = 0; i <= lines; i++)
            {
                string checkString = Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)(*(int*)(probePtr) + i));

                uint checkUint = *(uint*)(*(int*)(probePtr) + i);

                if (checkUint == searchUint)
                {
                    memLogLine("i: " + i + " " + checkString);
                }
            }
            memLogLine("Mem Scan - End");
            memLogLine("---");
        }
    }
}
