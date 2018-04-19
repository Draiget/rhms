using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace server.Drivers.Kernel
{
    /// <summary>
    /// PCI functions (read\write device config)
    /// </summary>
    internal static class Pci
    {
        #region Read Functions

        /// <summary>
        /// Set maximum PCI bus index
        /// </summary>
        /// <param name="maxIndex">Max PCI Bus to scan</param>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_SetPciMaxBusIndex", CallingConvention = CallingConvention.Winapi)]
        public static extern void SetMaxBusIndex(byte maxIndex);

        /// <summary>
        /// Reading the PCI device configuration byte
        /// </summary>
        /// <param name="pciAddress">PCI Device Address</param>
        /// <param name="regAddress">Configuration Address 0-255</param>
        /// <returns>PCI config BYTE (byte) value</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadPciConfigByte", CallingConvention = CallingConvention.Winapi)]
        public static extern byte ReadConfigByte(uint pciAddress, byte regAddress);

        /// <summary>
        /// Reading the PCI device configuration word
        /// </summary>
        /// <param name="pciAddress">PCI Device Address</param>
        /// <param name="regAddress">Configuration Address 0-255</param>
        /// <returns>PCI config WORD (ushort) value</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadPciConfigWord", CallingConvention = CallingConvention.Winapi)]
        public static extern ushort ReadConfigWord(uint pciAddress, byte regAddress);

        /// <summary>
        /// Reading the PCI device configuration dword
        /// </summary>
        /// <param name="pciAddress">PCI Device Address</param>
        /// <param name="regAddress">Configuration Address 0-255</param>
        /// <returns>PCI config DWORD (uint) value</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadPciConfigDword", CallingConvention = CallingConvention.Winapi)]
        public static extern uint ReadConfigDWord(uint pciAddress, byte regAddress);

        /// <summary>
        /// Extended reading the PCI device configuration byte
        /// </summary>
        /// <param name="pciAddress">PCI Device Address</param>
        /// <param name="regAddress">Configuration Address (0 - whatever)</param>
        /// <param name="value">Output PCI config BYTE (byte) value</param>
        /// <returns>Success or fail</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadPciConfigByteEx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ReadConfigByteEx(uint pciAddress, byte regAddress, ref byte value);

        /// <summary>
        /// Extended reading the PCI device configuration word
        /// </summary>
        /// <param name="pciAddress">PCI Device Address</param>
        /// <param name="regAddress">Configuration Address (0 - whatever)</param>
        /// <param name="value">Output PCI config WORD (ushort) value</param>
        /// <returns>Success or fail</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadPciConfigWordEx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ReadConfigWordEx(uint pciAddress, byte regAddress, ref ushort value);

        /// <summary>
        /// Extended reading the PCI device configuration dword
        /// </summary>
        /// <param name="pciAddress">PCI Device Address</param>
        /// <param name="regAddress">Configuration Address (0 - whatever)</param>
        /// <param name="value">Output PCI config DWORD (uint) value</param>
        /// <returns>Success or fail</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadPciConfigDwordEx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ReadConfigDWordEx(uint pciAddress, byte regAddress, ref uint value);

        #endregion

        #region Write Functions

        /// <summary>
        /// Write PCI device configuration byte value
        /// </summary>
        /// <param name="pciAddress">PCI Device Address</param>
        /// <param name="regAddress">Configuration Address (0 - 255)</param>
        /// <param name="value">BYTE (byte) value to write</param>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_WritePciConfigByte", CallingConvention = CallingConvention.Winapi)]
        public static extern void WriteConfigByte(uint pciAddress, byte regAddress, byte value);

        /// <summary>
        /// Write PCI device configuration word value
        /// </summary>
        /// <param name="pciAddress">PCI Device Address</param>
        /// <param name="regAddress">Configuration Address (0 - 255)</param>
        /// <param name="value">WORD (ushort) value to write</param>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_WritePciConfigWord", CallingConvention = CallingConvention.Winapi)]
        public static extern void WriteConfigWord(uint pciAddress, byte regAddress, ushort value);

        /// <summary>
        /// Write PCI device configuration dword value
        /// </summary>
        /// <param name="pciAddress">PCI Device Address</param>
        /// <param name="regAddress">Configuration Address (0 - 255)</param>
        /// <param name="value">DWORD (uint) value to write</param>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_WritePciConfigDword", CallingConvention = CallingConvention.Winapi)]
        public static extern void WriteConfigDWord(uint pciAddress, byte regAddress, uint value);

        /// <summary>
        /// Extended write PCI device configuration byte value
        /// </summary>
        /// <param name="pciAddress">PCI Device Address</param>
        /// <param name="regAddress">Configuration Address (0 - whatever)</param>
        /// <param name="value">BYTE (byte) value to write</param>
        /// <returns>Success or fail</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_WritePciConfigByteEx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool WriteConfigByteEx(uint pciAddress, byte regAddress, byte value);

        /// <summary>
        /// Extended write PCI device configuration word value
        /// </summary>
        /// <param name="pciAddress">PCI Device Address</param>
        /// <param name="regAddress">Configuration Address (0 - whatever)</param>
        /// <param name="value">WORD (ushort) value to write</param>
        /// <returns>Success or fail</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_WritePciConfigWordEx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool WriteConfigWordEx(uint pciAddress, byte regAddress, ushort value);

        /// <summary>
        /// Extended write PCI device configuration dword value
        /// </summary>
        /// <param name="pciAddress">PCI Device Address</param>
        /// <param name="regAddress">Configuration Address (0 - whatever)</param>
        /// <param name="value">DWORD (uint) value to write</param>
        /// <returns>Success or fail</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_WritePciConfigDwordEx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool WriteConfigDWordEx(uint pciAddress, byte regAddress, uint value);

        #endregion

        /// <summary>
        /// Find PCI device by id
        /// </summary>
        /// <param name="vendorId">Vendor ID</param>
        /// <param name="deviceId">Device ID</param>
        /// <param name="index">Index</param>
        /// <returns>PCI device address</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_FindPciDeviceById", CallingConvention = CallingConvention.Winapi)]
        public static extern uint FindDeviceById(ushort vendorId, ushort deviceId, byte index);

        /// <summary>
        /// Find PCI device by class
        /// </summary>
        /// <param name="baseClass">Base Class</param>
        /// <param name="subClass">Sub Class</param>
        /// <param name="programIf">Program Interface</param>
        /// <param name="index">Index</param>
        /// <returns>PCI device address</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_FindPciDeviceByClass", CallingConvention = CallingConvention.Winapi)]
        public static extern uint FindDeviceByClass(byte baseClass, byte subClass, byte programIf, byte index);
    }
}
