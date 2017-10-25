#ifndef RHMS_IO_CTL_H
#define RHMS_IO_CTL_H

#define RHMS_DEVICE_TYPE 40000

#define RHMS_DRIVER_ID				"rhms_kernel"

#define RHMS_DRV_MAJOR_VERSION		1
#define RHMS_DRV_MINOR_VERSION		0
#define RHMS_DRV_REVISION			0
#define RHMS_DRV_RELEASE			1

#define RHMS_DRV_VERSION \
		((RHMS_DRV_MAJOR_VERSION << 24) | (RHMS_DRV_MINOR_VERSION << 16) | (RHMS_DRV_REVISION << 8) | RHMS_DRV_RELEASE) 

/*
 *	IOCTL function codes in range from 0x800 to 0xFFF
 */
#define IOCTL_RHMS_GET_DRIVER_VERSION \
		CTL_CODE(RHMS_DEVICE_TYPE, 0x800, METHOD_BUFFERED, FILE_ANY_ACCESS)

#define IOCTL_RHMS_GET_REFCOUNT \
		CTL_CODE(RHMS_DEVICE_TYPE, 0x801, METHOD_BUFFERED, FILE_ANY_ACCESS)

#define IOCTL_RHMS_READ_MSR \
		CTL_CODE(RHMS_DEVICE_TYPE, 0x821, METHOD_BUFFERED, FILE_ANY_ACCESS)

#define IOCTL_RHMS_WRITE_MSR \
		CTL_CODE(RHMS_DEVICE_TYPE, 0x822, METHOD_BUFFERED, FILE_ANY_ACCESS)

#define IOCTL_RHMS_READ_PMC \
		CTL_CODE(RHMS_DEVICE_TYPE, 0x823, METHOD_BUFFERED, FILE_ANY_ACCESS)

#define IOCTL_RHMS_HALT \
		CTL_CODE(RHMS_DEVICE_TYPE, 0x824, METHOD_BUFFERED, FILE_ANY_ACCESS)

#define IOCTL_RHMS_READ_IO_PORT \
		CTL_CODE(RHMS_DEVICE_TYPE, 0x831, METHOD_BUFFERED, FILE_READ_ACCESS)

#define IOCTL_RHMS_WRITE_IO_PORT \
		CTL_CODE(RHMS_DEVICE_TYPE, 0x832, METHOD_BUFFERED, FILE_WRITE_ACCESS)

#define IOCTL_RHMS_READ_IO_PORT_BYTE \
		CTL_CODE(RHMS_DEVICE_TYPE, 0x833, METHOD_BUFFERED, FILE_READ_ACCESS)

#define IOCTL_RHMS_READ_IO_PORT_WORD \
		CTL_CODE(RHMS_DEVICE_TYPE, 0x834, METHOD_BUFFERED, FILE_READ_ACCESS)

#define IOCTL_RHMS_READ_IO_PORT_DWORD \
		CTL_CODE(RHMS_DEVICE_TYPE, 0x835, METHOD_BUFFERED, FILE_READ_ACCESS)

#define IOCTL_RHMS_WRITE_IO_PORT_BYTE \
		CTL_CODE(RHMS_DEVICE_TYPE, 0x836, METHOD_BUFFERED, FILE_WRITE_ACCESS)

#define IOCTL_RHMS_WRITE_IO_PORT_WORD \
		CTL_CODE(RHMS_DEVICE_TYPE, 0x837, METHOD_BUFFERED, FILE_WRITE_ACCESS)

#define IOCTL_RHMS_WRITE_IO_PORT_DWORD \
		CTL_CODE(RHMS_DEVICE_TYPE, 0x838, METHOD_BUFFERED, FILE_WRITE_ACCESS)

#define IOCTL_RHMS_READ_MEMORY \
		CTL_CODE(RHMS_DEVICE_TYPE, 0x841, METHOD_BUFFERED, FILE_READ_ACCESS)

#define IOCTL_RHMS_WRITE_MEMORY \
		CTL_CODE(RHMS_DEVICE_TYPE, 0x842, METHOD_BUFFERED, FILE_WRITE_ACCESS)

#define IOCTL_RHMS_READ_PCI_CONFIG \
		CTL_CODE(RHMS_DEVICE_TYPE, 0x851, METHOD_BUFFERED, FILE_READ_ACCESS)

#define IOCTL_RHMS_WRITE_PCI_CONFIG \
		CTL_CODE(RHMS_DEVICE_TYPE, 0x852, METHOD_BUFFERED, FILE_WRITE_ACCESS)


/**
 * PCI error codes
 */
#define RHMS_DRV_ERROR_PCI_BUS_NOT_EXIST	(0xE0000001L)
#define RHMS_DRV_ERROR_PCI_NO_DEVICE		(0xE0000002L)
#define RHMS_DRV_ERROR_PCI_WRITE_CONFIG		(0xE0000003L)
#define RHMS_DRV_ERROR_PCI_READ_CONFIG		(0xE0000004L)

/*
 * Bus Number, Device Number and Function Number to PCI Device Address
 */
#define PCI_BUS_DEV_FUNC(Bus, Dev, Func) \
		((Bus & 0xFF) << 8) | ((Dev & 0x1F) << 3) | (Func & 7)

/*
 * PCI Device Address to Bus Number
 */
#define PCI_GET_BUS(address) \
		((address >> 8) & 0xFF)

/*
 * PCI Device Address to Device Number
 */
#define PCI_GET_DEV(address) \
		((address >> 3) & 0x1F)

/*
 * PCI Device Address to Function Number
 */
#define PCI_GET_FUNC(address) \
		(address & 7)

#pragma pack(push,4)

typedef struct s_rhms_write_msr_input {
	ULONG Register;
	ULARGE_INTEGER Value;
} rhms_write_msr_input;

typedef struct s_rhms_write_io_port_input {
	ULONG PortNumber;
	union {
		ULONG LongData;
		USHORT ShortData;
		UCHAR CharData;
	} SharedData;
} rhms_write_io_port_input;

typedef struct s_rhms_read_pci_config_input {
	ULONG PciAddress;
	ULONG PciOffset;
} rhms_read_pci_config_input;

typedef struct s_rhms_write_pci_config_input {
	ULONG PciAddress;
	ULONG PciOffset;
	UCHAR Data[1];
} rhms_write_pci_config_input;

typedef LARGE_INTEGER PHYSICAL_ADDRESS;

typedef struct s_rhms_read_memory_input {
	PHYSICAL_ADDRESS Address;
	ULONG UnitSize;
	ULONG Count;
} rhms_read_memory_input;

typedef struct s_rhms_write_memory_input {
	PHYSICAL_ADDRESS Address;
	ULONG UnitSize;
	ULONG Count;
	UCHAR Data[1];
} rhms_write_memory_input;

#pragma pack(pop)

#endif
