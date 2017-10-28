#ifndef RHMS_DRIVER_H
#define RHMS_DRIVER_H

#include <ntddk.h>
#include <devioctl.h>
#include "include/RhmsIOCTL.h"

#define NT_DEVICE_NAME		L"\\Device\\rhms_kernel"
#define DOS_DEVICE_NAME		L"\\DosDevices\\rhms_kernel"

/*
 * Driver function declaration
 */

NTSTATUS DriverEntry(
	IN PDRIVER_OBJECT driver_object,
	IN PUNICODE_STRING registry_path
);

NTSTATUS RhmsDispatch(
	IN PDEVICE_OBJECT driver_device,
	IN PIRP p_irp
);

VOID Unload(
	IN PDRIVER_OBJECT driver_object
);

/*
 * Driver helper functions
 */
NTSTATUS DispatchMajorDriverFunction(
	IN PIRP p_irp,
	IN PIO_STACK_LOCATION p_irp_stack
);

NTSTATUS DispatchIoCtlDriverFunction(
	IN PIRP p_irp,
	IN PIO_STACK_LOCATION p_irp_stack
);

/*
 * MSR (Model Specific Registers) function declaration
 */

NTSTATUS ReadMsr(
	void *lpInBuffer,
	ULONG nInBufferSize,
	void *lpOutBuffer,
	ULONG nOutBufferSize,
	ULONG *lpBytesReturned
);

NTSTATUS WriteMsr(
	void *lpInBuffer,
	ULONG nInBufferSize,
	void *lpOutBuffer,
	ULONG nOutBufferSize,
	ULONG *lpBytesReturned
);

NTSTATUS ReadPmc(
	void *lpInBuffer,
	ULONG nInBufferSize,
	void *lpOutBuffer,
	ULONG nOutBufferSize,
	ULONG *lpBytesReturned
);

NTSTATUS ReadIoPort(
	ULONG ioControlCode,
	void *lpInBuffer,
	ULONG nInBufferSize,
	void *lpOutBuffer,
	ULONG nOutBufferSize,
	ULONG *lpBytesReturned
);

NTSTATUS WriteIoPort(
	ULONG ioControlCode,
	void *lpInBuffer,
	ULONG nInBufferSize,
	void *lpOutBuffer,
	ULONG nOutBufferSize,
	ULONG *lpBytesReturned
);

NTSTATUS ReadPciConfig(
	void *lpInBuffer,
	ULONG nInBufferSize,
	void *lpOutBuffer,
	ULONG nOutBufferSize,
	ULONG *lpBytesReturned
);

NTSTATUS WritePciConfig(
	void *lpInBuffer,
	ULONG nInBufferSize,
	void *lpOutBuffer,
	ULONG nOutBufferSize,
	ULONG *lpBytesReturned
);

NTSTATUS ReadMemory(
	void *lpInBuffer,
	ULONG nInBufferSize,
	void *lpOutBuffer,
	ULONG nOutBufferSize,
	ULONG *lpBytesReturned
);

NTSTATUS WriteMemory(
	void *lpInBuffer,
	ULONG nInBufferSize,
	void *lpOutBuffer,
	ULONG nOutBufferSize,
	ULONG *lpBytesReturned
);

/*
 * PCI support function declarations
 */

NTSTATUS PciConfigRead(ULONG pci_address, ULONG offset, void *data, int length);
NTSTATUS PciConfigWrite(ULONG pci_address, ULONG offset, void *data, int length);

#endif
