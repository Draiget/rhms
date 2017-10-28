#include <Windows.h>
#include "RhmsApiPci.h"
#include "../../driver/src/include/RhmsIOCTL.h"

// ReSharper disable CppInconsistentNaming
const BYTE g_PciNumberOfDevice = 32;
const BYTE g_PciNumberOfFunction = 8;

extern HANDLE g_Handle;
BYTE g_PciMaxNumberOfBus = 255;

BOOL RHMS_PciConfigRead(DWORD pci_address, DWORD reg_address, PBYTE value, DWORD size, PDWORD error) {
	if (g_Handle == INVALID_HANDLE_VALUE) {
		return false;
	}

	if (value == nullptr) {
		return false;
	}

	// alignment check
	if (size == 2 && (reg_address & 1) != 0) {
		return false;
	}

	if (size == 4 && (reg_address & 3) != 0) {
		return false;
	}

	DWORD returned_length = 0;
	rhms_read_pci_config_input in_buf;

	in_buf.PciAddress = pci_address;
	in_buf.PciOffset = reg_address;

	auto const result = DeviceIoControl(
		g_Handle,
		IOCTL_RHMS_READ_PCI_CONFIG,
		&in_buf,
		sizeof in_buf,
		value,
		size,
		&returned_length,
		nullptr
	);

	if (error != nullptr) {
		*error = GetLastError();
	}

	return result;
}

BOOL RHMS_PciConfigWrite(DWORD pci_address, DWORD reg_address, PBYTE value, DWORD size) {
	DWORD returned_length = 0;

	if (g_Handle == INVALID_HANDLE_VALUE) {
		return false;
	}

	if (value == nullptr) {
		return false;
	}

	// alignment check
	if (size == 2 && (reg_address & 1) != 0)	{
		return false;
	}

	if (size == 4 && (reg_address & 3) != 0)	{
		return false;
	}

	auto const input_size = offsetof(rhms_write_pci_config_input, Data) + size;
	auto const in_buf = static_cast<rhms_write_pci_config_input*>(malloc(input_size));
	if (in_buf == nullptr) {
		return false;
	}

	memcpy(in_buf->Data, value, size);
	in_buf->PciAddress = pci_address;
	in_buf->PciOffset = reg_address;
	auto const result = DeviceIoControl(
		g_Handle,
		IOCTL_RHMS_WRITE_PCI_CONFIG,
		in_buf,
		static_cast<DWORD>(input_size),
		nullptr,
		0,
		&returned_length,
		nullptr
	);

	free(in_buf);

	return result;
}

BYTE WINAPI RHMS_ReadPciConfigByte(DWORD pci_address, BYTE reg_address) {
	BYTE ret;
	if (RHMS_PciConfigRead(pci_address, reg_address, static_cast<PBYTE>(&ret), sizeof ret, nullptr)) {
		return ret;
	}

	return 0xFF;
}

WORD WINAPI RHMS_ReadPciConfigWord(DWORD pci_address, BYTE reg_address) {
	WORD ret;
	if (RHMS_PciConfigRead(pci_address, reg_address, reinterpret_cast<PBYTE>(&ret), sizeof ret, nullptr)) {
		return ret;
	}

	return 0xFFFF;
}

DWORD WINAPI RHMS_ReadPciConfigDword(DWORD pci_address, BYTE reg_address) {
	DWORD ret;
	if (RHMS_PciConfigRead(pci_address, reg_address, reinterpret_cast<PBYTE>(&ret), sizeof ret, nullptr)) {
		return ret;
	}

	return 0xFFFFFFFF;
}

BOOL WINAPI RHMS_ReadPciConfigByteEx(DWORD pci_address, DWORD reg_address, PBYTE value) {
	return RHMS_PciConfigRead(pci_address, reg_address, static_cast<PBYTE>(value), sizeof(BYTE), nullptr);
}

BOOL WINAPI RHMS_ReadPciConfigWordEx(DWORD pci_address, DWORD reg_address, PWORD value) {
	return RHMS_PciConfigRead(pci_address, reg_address, reinterpret_cast<PBYTE>(value), sizeof(WORD), nullptr);
}

BOOL WINAPI RHMS_ReadPciConfigDwordEx(DWORD pci_address, DWORD reg_address, PDWORD value) {
	return RHMS_PciConfigRead(pci_address, reg_address, reinterpret_cast<PBYTE>(value), sizeof(DWORD), nullptr);
}

VOID WINAPI RHMS_WritePciConfigByte(DWORD pci_address, BYTE reg_address, BYTE value) {
	RHMS_PciConfigWrite(pci_address, reg_address, static_cast<PBYTE>(&value), sizeof value);
}

VOID WINAPI RHMS_WritePciConfigWord(DWORD pci_address, BYTE reg_address, WORD value) {
	RHMS_PciConfigWrite(pci_address, reg_address, reinterpret_cast<PBYTE>(&value), sizeof value);
}

VOID WINAPI RHMS_WritePciConfigDword(DWORD pci_address, BYTE reg_address, DWORD value) {
	RHMS_PciConfigWrite(pci_address, reg_address, reinterpret_cast<PBYTE>(&value), sizeof value);
}

BOOL WINAPI RHMS_WritePciConfigByteEx(DWORD pci_address, DWORD reg_address, BYTE value) {
	return RHMS_PciConfigWrite(pci_address, reg_address, static_cast<PBYTE>(&value), sizeof value);
}

BOOL WINAPI RHMS_WritePciConfigWordEx(DWORD pci_address, DWORD reg_address, WORD value) {
	return RHMS_PciConfigWrite(pci_address, reg_address, reinterpret_cast<PBYTE>(&value), sizeof value);
}

BOOL WINAPI RHMS_WritePciConfigDwordEx(DWORD pci_address, DWORD reg_address, DWORD value) {
	return RHMS_PciConfigWrite(pci_address, reg_address, reinterpret_cast<PBYTE>(&value), sizeof value);
}

VOID WINAPI RHMS_SetPciMaxBusIndex(BYTE max){
	g_PciMaxNumberOfBus = max;
}

DWORD WINAPI RHMS_FindPciDeviceById(WORD vendor_id, WORD device_id, BYTE index) {
	if (g_Handle == INVALID_HANDLE_VALUE) {
		return 0xFFFFFFFF;
	}

	DWORD bus, dev, func;
	DWORD id = 0;
	DWORD error = 0;
	BYTE type = 0;
	DWORD count = 0;

	if (vendor_id == 0xFFFF) {
		return 0xFFFFFFFF;
	}

	for (bus = 0; bus <= g_PciMaxNumberOfBus; bus++) {
		for (dev = 0; dev < g_PciNumberOfDevice; dev++) {
			BOOL multi_func_flag = false;

			for (func = 0; func < g_PciNumberOfFunction; func++) {
				if (multi_func_flag == 0 && func > 0) {
					break;
				}

				auto const pci_address = PCI_BUS_DEV_FUNC(bus, dev, func);
				if (RHMS_PciConfigRead(pci_address, 0, reinterpret_cast<PBYTE>(&id), sizeof id, &error)) {
					// Is multi function device
					if (func == 0) {
						if (RHMS_PciConfigRead(pci_address, 0x0E, static_cast<PBYTE>(&type), sizeof type, nullptr)) {
							if (type & 0x80) {
								multi_func_flag = true;
							}
						}
					}

					if (id == (vendor_id | static_cast<DWORD>(device_id) << 16)) {
						if (count == index) {
							return pci_address;
						}

						count++;
					}
				}
			}
		}
	}

	return 0xFFFFFFFF;
}

DWORD WINAPI RHMS_FindPciDeviceByClass(BYTE base_class, BYTE sub_class, BYTE program_if, BYTE index) {
	if (g_Handle == INVALID_HANDLE_VALUE) {
		return 0xFFFFFFFF;
	}

	DWORD bus, dev, func;
	DWORD conf[3] = { 0 };
	DWORD error = 0;
	BYTE type = 0;
	DWORD count = 0;

	for (bus = 0; bus <= g_PciMaxNumberOfBus; bus++) {
		for (dev = 0; dev < g_PciNumberOfDevice; dev++) {
			BOOL multi_func_flag = false;

			for (func = 0; func < g_PciNumberOfFunction; func++) {
				if (!multi_func_flag && func > 0) {
					break;
				}

				auto const pciAddress = PCI_BUS_DEV_FUNC(bus, dev, func);
				if (RHMS_PciConfigRead(pciAddress, 0, reinterpret_cast<BYTE*>(conf), sizeof conf, &error)) {
					// Is multi function device
					if (func == 0) {
						if (RHMS_PciConfigRead(pciAddress, 0x0E, static_cast<BYTE*>(&type), sizeof type, nullptr)) {
							if (type & 0x80) {
								multi_func_flag = true;
							}
						}
					}

					if ((conf[2] & 0xFFFFFF00) ==
						  (static_cast<DWORD>(base_class) << 24 |
							static_cast<DWORD>(sub_class) << 16 |
						   static_cast<DWORD>(program_if) << 8)
						)
					{
						if (count == index) {
							return pciAddress;
						}

						count++;
					}
				}
			}
		}
	}

	return 0xFFFFFFFF;
}

// ReSharper restore CppInconsistentNaming