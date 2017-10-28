#ifndef RHMS_API_PCI_H
#define RHMS_API_PCI_H

// ReSharper disable CppInconsistentNaming
#include "RhmsExportsApi.h"

/**
 * \brief Set maximum PCI bus index
 * \param max Max PCI Bus to scan
 */
RHMS_API_EXPOSED VOID WINAPI RHMS_SetPciMaxBusIndex(
	BYTE max
);

/**
 * \brief Reading the PCI device configuration byte
 * \param pci_address PCI Device Address
 * \param reg_address Configuration Address 0-255
 * \return PCI config byte value
 */
RHMS_API_EXPOSED BYTE WINAPI RHMS_ReadPciConfigByte(
	DWORD pci_address,
	BYTE reg_address
);

/**
 * \brief Reading the PCI device configuration word
 * \param pci_address PCI Device Address
 * \param reg_address Configuration Address 0-255
 * \return PCI config word value
 */
RHMS_API_EXPOSED WORD WINAPI RHMS_ReadPciConfigWord(
	DWORD pci_address,
	BYTE reg_address
);

/**
* \brief Reading the PCI device configuration dword
* \param pci_address PCI Device Address
* \param reg_address Configuration Address 0-255
* \return PCI config dword value
 */
RHMS_API_EXPOSED DWORD WINAPI RHMS_ReadPciConfigDword(
	DWORD pci_address,
	BYTE reg_address
);

/**
 * \brief Extended reading the PCI device configuration byte
 * \param pci_address PCI Device Address
 * \param reg_address Configuration Address 0-whatever
 * \param value Output PCI config byte value
 * \return Success or fail
 */
RHMS_API_EXPOSED BOOL WINAPI RHMS_ReadPciConfigByteEx(
	DWORD pci_address,
	DWORD reg_address,
	PBYTE value
);

/**
* \brief Extended reading the PCI device configuration word
* \param pci_address PCI Device Address
* \param reg_address Configuration Address 0-whatever
* \param value Output PCI config word value
* \return Success or fail
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_ReadPciConfigWordEx(
	DWORD pci_address,
	DWORD reg_address,
	PWORD value
);

/**
* \brief Extended reading the PCI device configuration dword
* \param pci_address PCI Device Address
* \param reg_address Configuration Address 0-whatever
* \param value Output PCI config dword value
* \return Success or fail
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_ReadPciConfigDwordEx(
	DWORD pci_address,
	DWORD reg_address,
	PDWORD value
);

/**
* \brief Write PCI device configuration byte value
* \param pci_address PCI Device Address
* \param reg_address Configuration Address 0-255
* \param value Value to write
*/
RHMS_API_EXPOSED VOID WINAPI RHMS_WritePciConfigByte(
	DWORD pci_address,
	BYTE reg_address,
	BYTE value
);

/**
* \brief Write PCI device configuration word value
* \param pci_address PCI Device Address
* \param reg_address Configuration Address 0-255
* \param value Value to write
*/
RHMS_API_EXPOSED VOID WINAPI RHMS_WritePciConfigWord(
	DWORD pci_address,
	BYTE reg_address,
	WORD value
);

/**
* \brief Write PCI device configuration dword value
* \param pci_address PCI Device Address
* \param reg_address Configuration Address 0-255
* \param value Value to write
*/
RHMS_API_EXPOSED VOID WINAPI RHMS_WritePciConfigDword(
	DWORD pci_address,
	BYTE reg_address,
	DWORD value
);

/**
* \brief Extended write PCI device configuration byte value
* \param pci_address PCI Device Address
* \param reg_address Configuration Address 0-whatever
* \param value Value to write
* \return Success or fail
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_WritePciConfigByteEx(
	DWORD pci_address,
	DWORD reg_address,
	BYTE value
);

/**
* \brief Extended write PCI device configuration word value
* \param pci_address PCI Device Address
* \param reg_address Configuration Address 0-whatever
* \param value Value to write
* \return Success or fail
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_WritePciConfigWordEx(
	DWORD pci_address,
	DWORD reg_address,
	WORD value
);

/**
* \brief Extended write PCI device configuration dword value
* \param pci_address PCI Device Address
* \param reg_address Configuration Address 0-whatever
* \param value Value to write
* \return Success or fail
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_WritePciConfigDwordEx(
	DWORD pci_address,
	DWORD reg_address,
	DWORD value
);

/**
 * \brief Find PCI device by id
 * \param vendor_id Vendor ID
 * \param device_id Device ID
 * \param index Index
 * \return PCI device address
 */
RHMS_API_EXPOSED DWORD WINAPI RHMS_FindPciDeviceById(
	WORD vendor_id,
	WORD device_id,
	BYTE index
);

/**
* \brief Find PCI device by class
* \param base_class Base Class
* \param sub_class Sub Class
* \param program_if Program Interface
* \param index Index
* \return PCI device address
*/
RHMS_API_EXPOSED DWORD WINAPI RHMS_FindPciDeviceByClass(
	BYTE base_class,
	BYTE sub_class,
	BYTE program_if,
	BYTE index
);

// ReSharper restore CppInconsistentNaming
#endif
