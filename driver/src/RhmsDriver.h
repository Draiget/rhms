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
	void * lp_in_buffer,
	ULONG n_in_buffer_size,
	void * lp_out_buffer,
	ULONG n_out_buffer_size,
	ULONG * lp_bytes_returned
);

NTSTATUS WriteMsr(
	void * lp_in_buffer,
	ULONG n_in_buffer_size,
	void * lp_out_buffer,
	ULONG n_out_buffer_size,
	ULONG * lp_bytes_returned
);

NTSTATUS ReadPmc(
	void * lp_in_buffer,
	ULONG n_in_buffer_size,
	void * lp_out_buffer,
	ULONG n_out_buffer_size,
	ULONG * lp_bytes_returned
);

NTSTATUS ReadIoPort(
	ULONG io_control_code,
	void * lp_in_buffer,
	ULONG n_in_buffer_size,
	void * lp_out_buffer,
	ULONG n_out_buffer_size,
	ULONG * lp_bytes_returned
);

NTSTATUS WriteIoPort(
	ULONG io_control_code,
	void * lp_in_buffer,
	ULONG n_in_buffer_size,
	void * lp_out_buffer,
	ULONG n_out_buffer_size,
	ULONG * lp_bytes_returned
);

NTSTATUS ReadPciConfig(
	void * lp_in_buffer,
	ULONG n_in_buffer_size,
	void * lp_out_buffer,
	ULONG n_out_buffer_size,
	ULONG * lp_bytes_returned
);

NTSTATUS WritePciConfig(
	void * lp_in_buffer,
	ULONG n_in_buffer_size,
	void * lp_out_buffer,
	ULONG n_out_buffer_size,
	ULONG * lp_bytes_returned
);

NTSTATUS ReadMemory(
	void * lp_in_buffer,
	ULONG n_in_buffer_size,
	void * lp_out_buffer,
	ULONG n_out_buffer_size,
	ULONG * lp_bytes_returned
);

NTSTATUS WriteMemory(
	void * lp_in_buffer,
	ULONG n_in_buffer_size,
	void * lp_out_buffer,
	ULONG n_out_buffer_size,
	ULONG * lp_bytes_returned
);

/*
 * PCI support function declarations
 */

NTSTATUS PciConfigRead(ULONG pci_address, ULONG offset, void *data, int length);
NTSTATUS PciConfigWrite(ULONG pci_address, ULONG offset, void *data, int length);

#endif
