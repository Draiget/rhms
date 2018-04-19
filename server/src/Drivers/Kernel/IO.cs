using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace server.Drivers.Kernel
{
    /// <summary>
    /// I/O Functions (ports read\write)
    /// </summary>
    internal static class Io
    {
        #region Read Functions

        /// <summary>
        /// Read port data
        /// </summary>
        /// <param name="port">I/O port address</param>
        /// <returns>Port BYTE (byte) value</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadIoPortByte", CallingConvention = CallingConvention.Winapi)]
        public static extern byte ReadPortByte(ushort port);

        /// <summary>
        /// Read port data
        /// </summary>
        /// <param name="port">I/O port address</param>
        /// <returns>Port WORD (ushort) value</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadIoPortWord", CallingConvention = CallingConvention.Winapi)]
        public static extern ushort ReadPortWord(ushort port);

        /// <summary>
        /// Read port data
        /// </summary>
        /// <param name="port">I/O port address</param>
        /// <returns>Port DWORD (uint) value</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadIoPortDword", CallingConvention = CallingConvention.Winapi)]
        public static extern uint ReadPortDWord(ushort port);

        /// <summary>
        /// Extended read port data
        /// </summary>
        /// <param name="port">I/O port address</param>
        /// <param name="value">Out readed port BYTE (byte) value</param>
        /// <returns>Success or fail</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadIoPortByteEx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ReadPortByteEx(ushort port, ref byte value);

        /// <summary>
        /// Extended read port data
        /// </summary>
        /// <param name="port">I/O port address</param>
        /// <param name="value">Out readed port WORD (ushort) value</param>
        /// <returns>Success or fail</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadIoPortWordEx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ReadPortWordEx(ushort port, ref ushort value);

        /// <summary>
        /// Extended read port data
        /// </summary>
        /// <param name="port">I/O port address</param>
        /// <param name="value">Out readed port DWORD (uint) value</param>
        /// <returns>Success or fail</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadIoPortDwordEx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ReadPortDWordEx(ushort port, ref uint value);

        #endregion

        #region Write Functions

        /// <summary>
        /// Write port data
        /// </summary>
        /// <param name="port">I/O port address</param>
        /// <param name="value">BYTE (byte) value to write in the port</param>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_WriteIoPortByte", CallingConvention = CallingConvention.Winapi)]
        public static extern void WritePortByte(ushort port, byte value);

        /// <summary>
        /// Write port data
        /// </summary>
        /// <param name="port">I/O port address</param>
        /// <param name="value">WORD (ushort) value to write in the port</param>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_WriteIoPortWord", CallingConvention = CallingConvention.Winapi)]
        public static extern void WritePortWord(ushort port, ushort value);

        /// <summary>
        /// Write port data
        /// </summary>
        /// <param name="port">I/O port address</param>
        /// <param name="value">DWORD (uint) value to write in the port</param>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_WriteIoPortDword", CallingConvention = CallingConvention.Winapi)]
        public static extern void WritePortDWord(ushort port, uint value);

        /// <summary>
        /// Extended write port data
        /// </summary>
        /// <param name="port">I/O port address</param>
        /// <param name="value">BYTE (byte) value to write in the port</param>
        /// <returns>Success or fail</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_WriteIoPortByteEx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool WritePortByteEx(ushort port, byte value);

        /// <summary>
        /// Extended write port data
        /// </summary>
        /// <param name="port">I/O port address</param>
        /// <param name="value">WORD (ushort) value to write in the port</param>
        /// <returns>Success or fail</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_WriteIoPortWordEx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool WritePortWordEx(ushort port, ushort value);

        /// <summary>
        /// Extended write port data
        /// </summary>
        /// <param name="port">I/O port address</param>
        /// <param name="value">DWORD (uint) value to write in the port</param>
        /// <returns>Success or fail</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_WriteIoPortDwordEx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool WritePortDWordEx(ushort port, uint value);

        #endregion
    }
}
