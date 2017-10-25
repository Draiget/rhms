#include <ntddk.h>
#include <stddef.h>
#include <intrin.h>
#include "RhmsDriver.h"
#include "include/RhmsIOCTL.h"

static ULONG g_RefCount;

/*
	Try to initialize driver
 */
NTSTATUS DriverEntry(IN PDRIVER_OBJECT driver_object, IN PUNICODE_STRING registry_path) {
	UNREFERENCED_PARAMETER(registry_path);

	UNICODE_STRING nt_device_name;
	UNICODE_STRING win32_device_name;
	PDEVICE_OBJECT device_object = NULL;

	RtlInitUnicodeString(&nt_device_name, NT_DEVICE_NAME);

	auto status = IoCreateDevice(
		driver_object,					// Our Driver Object
		0,								// We don't use a device extension
		&nt_device_name,				// Device name 
		RHMS_DEVICE_TYPE,				// Device type
		FILE_DEVICE_SECURE_OPEN,		// Device characteristics
		FALSE,							// Not an exclusive device
		&device_object);				// Returned ptr to Device Object

	if (!NT_SUCCESS(status)) {
		g_RefCount = (ULONG)-1;
		return status;
	}

	g_RefCount = 0;

	// Initialize our driver object instance and set entry point function
	driver_object->MajorFunction[IRP_MJ_CREATE] = RhmsDispatch;
	driver_object->MajorFunction[IRP_MJ_CLOSE] = RhmsDispatch;
	driver_object->MajorFunction[IRP_MJ_DEVICE_CONTROL] = RhmsDispatch;
	driver_object->DriverUnload = Unload;

	// Initialize a unicode string with win32 name for driver device
	RtlInitUnicodeString(&win32_device_name, DOS_DEVICE_NAME);

	// Make symbolic link between device name and win32 name
	status = IoCreateSymbolicLink(&win32_device_name, &nt_device_name);

	if (!NT_SUCCESS(status)) {
		// Cleanup allocated stuff
		IoDeleteDevice(device_object);
	}

	return status;
}

NTSTATUS RhmsDispatch(IN PDEVICE_OBJECT driver_device, IN PIRP p_irp) {
	UNREFERENCED_PARAMETER(driver_device);

	// Initi irp information field, used to return the number of transfered bytes
	p_irp->IoStatus.Information = 0;
	const PIO_STACK_LOCATION p_irp_stack = IoGetCurrentIrpStackLocation(p_irp);

	auto const status = DispatchMajorDriverFunction(p_irp, p_irp_stack);

	// We're done with I/O request, record the status of the I/O action
	p_irp->IoStatus.Status = status;

	// Don't boost priority when returning since this took little time
	IoCompleteRequest(p_irp, IO_NO_INCREMENT);

	return status;
}

NTSTATUS DispatchMajorDriverFunction(IN PIRP p_irp, IN PIO_STACK_LOCATION p_irp_stack) {
	switch (p_irp_stack->MajorFunction) {
		case IRP_MJ_CREATE:
			if (g_RefCount != (ULONG)-1){
				g_RefCount++;
			}
			return STATUS_SUCCESS;

		case IRP_MJ_CLOSE:
			if (g_RefCount != (ULONG)-1){
				g_RefCount--;
			}
			return STATUS_SUCCESS;

		case IRP_MJ_DEVICE_CONTROL:
			return DispatchIoCtlDriverFunction(p_irp, p_irp_stack);

		default:
			return STATUS_NOT_IMPLEMENTED;
	}
}

NTSTATUS DispatchIoCtlDriverFunction(IN PIRP p_irp, IN PIO_STACK_LOCATION p_irp_stack) {
	switch (p_irp_stack->Parameters.DeviceIoControl.IoControlCode) {
		case IOCTL_RHMS_GET_DRIVER_VERSION:
			*(PULONG)p_irp->AssociatedIrp.SystemBuffer = RHMS_DRV_VERSION;
			p_irp->IoStatus.Information = 4;
			return STATUS_SUCCESS;

		case IOCTL_RHMS_GET_REFCOUNT:
			*(PULONG)p_irp->AssociatedIrp.SystemBuffer = g_RefCount;
			p_irp->IoStatus.Information = sizeof(g_RefCount);
			return STATUS_SUCCESS;

		case IOCTL_RHMS_READ_MSR:
			return ReadMsr(
				p_irp->AssociatedIrp.SystemBuffer,
				p_irp_stack->Parameters.DeviceIoControl.InputBufferLength,
				p_irp->AssociatedIrp.SystemBuffer,
				p_irp_stack->Parameters.DeviceIoControl.OutputBufferLength,
				(ULONG*)&p_irp->IoStatus.Information
			);

		case IOCTL_RHMS_WRITE_MSR:
			return WriteMsr(
				p_irp->AssociatedIrp.SystemBuffer,
				p_irp_stack->Parameters.DeviceIoControl.InputBufferLength,
				p_irp->AssociatedIrp.SystemBuffer,
				p_irp_stack->Parameters.DeviceIoControl.OutputBufferLength,
				(ULONG*)&p_irp->IoStatus.Information
			);
			
		case IOCTL_RHMS_READ_PMC:
			return ReadPmc(
				p_irp->AssociatedIrp.SystemBuffer,
				p_irp_stack->Parameters.DeviceIoControl.InputBufferLength,
				p_irp->AssociatedIrp.SystemBuffer,
				p_irp_stack->Parameters.DeviceIoControl.OutputBufferLength,
				(ULONG*)&p_irp->IoStatus.Information
			);

		case IOCTL_RHMS_HALT:
			__halt();

			return STATUS_SUCCESS;

		case IOCTL_RHMS_READ_IO_PORT:
		case IOCTL_RHMS_READ_IO_PORT_BYTE:
		case IOCTL_RHMS_READ_IO_PORT_WORD:
		case IOCTL_RHMS_READ_IO_PORT_DWORD:
			return ReadIoPort(
				p_irp_stack->Parameters.DeviceIoControl.IoControlCode,
				p_irp->AssociatedIrp.SystemBuffer,
				p_irp_stack->Parameters.DeviceIoControl.InputBufferLength,
				p_irp->AssociatedIrp.SystemBuffer,
				p_irp_stack->Parameters.DeviceIoControl.OutputBufferLength,
				(ULONG*)&p_irp->IoStatus.Information
			);

		case IOCTL_RHMS_WRITE_IO_PORT:
		case IOCTL_RHMS_WRITE_IO_PORT_BYTE:
		case IOCTL_RHMS_WRITE_IO_PORT_WORD:
		case IOCTL_RHMS_WRITE_IO_PORT_DWORD:
			return WriteIoPort(
				p_irp_stack->Parameters.DeviceIoControl.IoControlCode,
				p_irp->AssociatedIrp.SystemBuffer,
				p_irp_stack->Parameters.DeviceIoControl.InputBufferLength,
				p_irp->AssociatedIrp.SystemBuffer,
				p_irp_stack->Parameters.DeviceIoControl.OutputBufferLength,
				(ULONG*)&p_irp->IoStatus.Information
			);

		case IOCTL_RHMS_READ_PCI_CONFIG:
			return ReadPciConfig(
				p_irp->AssociatedIrp.SystemBuffer,
				p_irp_stack->Parameters.DeviceIoControl.InputBufferLength,
				p_irp->AssociatedIrp.SystemBuffer,
				p_irp_stack->Parameters.DeviceIoControl.OutputBufferLength,
				(ULONG*)&p_irp->IoStatus.Information
			);

		case IOCTL_RHMS_WRITE_PCI_CONFIG:
			return WritePciConfig(
				p_irp->AssociatedIrp.SystemBuffer,
				p_irp_stack->Parameters.DeviceIoControl.InputBufferLength,
				p_irp->AssociatedIrp.SystemBuffer,
				p_irp_stack->Parameters.DeviceIoControl.OutputBufferLength,
				(ULONG*)&p_irp->IoStatus.Information
			);

		case IOCTL_RHMS_READ_MEMORY:
			return ReadMemory(
				p_irp->AssociatedIrp.SystemBuffer,
				p_irp_stack->Parameters.DeviceIoControl.InputBufferLength,
				p_irp->AssociatedIrp.SystemBuffer,
				p_irp_stack->Parameters.DeviceIoControl.OutputBufferLength,
				(ULONG*)&p_irp->IoStatus.Information
			);

		case IOCTL_RHMS_WRITE_MEMORY:
			return WriteMemory(
				p_irp->AssociatedIrp.SystemBuffer,
				p_irp_stack->Parameters.DeviceIoControl.InputBufferLength,
				p_irp->AssociatedIrp.SystemBuffer,
				p_irp_stack->Parameters.DeviceIoControl.OutputBufferLength,
				(ULONG*)&p_irp->IoStatus.Information
			);

		default:
			return STATUS_NOT_IMPLEMENTED;
	}
}

VOID Unload(PDRIVER_OBJECT driver_object) {
	const PDEVICE_OBJECT device_object = driver_object->DeviceObject;
	UNICODE_STRING win32_name_string;

	PAGED_CODE();

	// Create counted string version of our Win32 device name
	RtlInitUnicodeString(&win32_name_string, DOS_DEVICE_NAME);

	// Delete the link from our device name to a name in the Win32 namespace
	IoDeleteSymbolicLink(&win32_name_string);

	if (device_object != NULL) {
		IoDeleteDevice(device_object);
	}
}

/*
 * CPU functions
 */

NTSTATUS ReadMsr(void* lp_in_buffer, ULONG n_in_buffer_size, void* lp_out_buffer, ULONG n_out_buffer_size, ULONG* lp_bytes_returned) {
	UNREFERENCED_PARAMETER(n_in_buffer_size);
	UNREFERENCED_PARAMETER(n_out_buffer_size);
	__try {
		ULONGLONG data = __readmsr(*(ULONG*)lp_in_buffer);
		memcpy((PULONG)lp_out_buffer, &data, 8);
		*lp_bytes_returned = 8;
		return STATUS_SUCCESS;
	} __except (EXCEPTION_EXECUTE_HANDLER) {
		*lp_bytes_returned = 0;
		return STATUS_UNSUCCESSFUL;
	}
}

NTSTATUS WriteMsr(void* lp_in_buffer, ULONG n_in_buffer_size, void* lp_out_buffer, ULONG n_out_buffer_size, ULONG* lp_bytes_returned) {
	UNREFERENCED_PARAMETER(n_in_buffer_size);
	UNREFERENCED_PARAMETER(lp_out_buffer);
	UNREFERENCED_PARAMETER(n_out_buffer_size);

	__try {
		 rhms_write_msr_input* param = (rhms_write_msr_input*)lp_in_buffer;

		__writemsr(param->Register, param->Value.QuadPart);
		*lp_bytes_returned = 0;
		return STATUS_SUCCESS;
	} __except (EXCEPTION_EXECUTE_HANDLER) {
		*lp_bytes_returned = 0;
		return STATUS_UNSUCCESSFUL;
	}
}

NTSTATUS ReadPmc(void* lp_in_buffer, ULONG n_in_buffer_size,	void* lp_out_buffer, ULONG n_out_buffer_size, ULONG* lp_bytes_returned) {
	UNREFERENCED_PARAMETER(n_in_buffer_size);
	UNREFERENCED_PARAMETER(n_out_buffer_size);

	__try 	{
		ULONGLONG data = __readpmc(*(ULONG*)lp_in_buffer);
		memcpy((PULONG)lp_out_buffer, &data, 8);
		*lp_bytes_returned = 8;
		return STATUS_SUCCESS;
	} __except (EXCEPTION_EXECUTE_HANDLER) {
		*lp_bytes_returned = 0;
		return STATUS_UNSUCCESSFUL;
	}
}

/*
 * IO port functions
 */

NTSTATUS ReadIoPort(ULONG io_control_code, void* lp_in_buffer, ULONG n_in_buffer_size, void* lp_out_buffer, ULONG n_out_buffer_size, ULONG* lp_bytes_returned) {
	UNREFERENCED_PARAMETER(n_out_buffer_size);

	const ULONG n_port = *(ULONG*)lp_in_buffer;

	switch (io_control_code) {
		case IOCTL_RHMS_READ_IO_PORT_BYTE:
			*(PUCHAR)lp_out_buffer = READ_PORT_UCHAR((PUCHAR)(ULONG_PTR)n_port);
			break;
		case IOCTL_RHMS_READ_IO_PORT_WORD:
			*(PUSHORT)lp_out_buffer = READ_PORT_USHORT((PUSHORT)(ULONG_PTR)n_port);
			break;
		case IOCTL_RHMS_READ_IO_PORT_DWORD:
			*(PULONG)lp_out_buffer = READ_PORT_ULONG((PULONG)(ULONG_PTR)n_port);
			break;
		default:
			*lp_bytes_returned = 0;
			return STATUS_INVALID_PARAMETER;
	}

	*lp_bytes_returned = n_in_buffer_size;
	return STATUS_SUCCESS;
}

NTSTATUS WriteIoPort(ULONG io_control_code, void* lp_in_buffer, ULONG n_in_buffer_size, void* lp_out_buffer, ULONG n_out_buffer_size, ULONG* lp_bytes_returned) {
	UNREFERENCED_PARAMETER(n_in_buffer_size);
	UNREFERENCED_PARAMETER(lp_out_buffer);
	UNREFERENCED_PARAMETER(n_out_buffer_size);
	UNREFERENCED_PARAMETER(lp_bytes_returned);

	rhms_write_io_port_input* param = (rhms_write_io_port_input*)lp_in_buffer;
	const ULONG n_port = param->PortNumber;

	switch (io_control_code) {
		case IOCTL_RHMS_WRITE_IO_PORT_BYTE:
			WRITE_PORT_UCHAR((PUCHAR)(ULONG_PTR)n_port, param->SharedData.CharData);
			break;
		case IOCTL_RHMS_WRITE_IO_PORT_WORD:
			WRITE_PORT_USHORT((PUSHORT)(ULONG_PTR)n_port, param->SharedData.ShortData);
			break;
		case IOCTL_RHMS_WRITE_IO_PORT_DWORD:
			WRITE_PORT_ULONG((PULONG)(ULONG_PTR)n_port, param->SharedData.LongData);
			break;
		default:
			return STATUS_INVALID_PARAMETER;
	}

	return STATUS_SUCCESS;
}

/*
 * PCI functions
 */

NTSTATUS ReadPciConfig(void* lp_in_buffer, ULONG n_in_buffer_size, void* lp_out_buffer, ULONG n_out_buffer_size, ULONG* lp_bytes_returned) {
	if (n_in_buffer_size != sizeof(rhms_read_pci_config_input)) {
		return STATUS_INVALID_PARAMETER;
	}

	rhms_read_pci_config_input* param = (rhms_read_pci_config_input*)lp_in_buffer;
	const NTSTATUS status = pciConfigRead(param->PciAddress, param->PciOffset, lp_out_buffer, n_out_buffer_size);

	if (status == STATUS_SUCCESS) {
		*lp_bytes_returned = n_out_buffer_size;
	} else {
		*lp_bytes_returned = 0;
	}

	return status;
}

NTSTATUS WritePciConfig(void* lp_in_buffer, ULONG n_in_buffer_size, void* lp_out_buffer, ULONG n_out_buffer_size, ULONG* lp_bytes_returned) {
	UNREFERENCED_PARAMETER(lp_out_buffer);
	UNREFERENCED_PARAMETER(n_out_buffer_size);

	if (n_in_buffer_size < offsetof(rhms_write_pci_config_input, Data)) {
		return STATUS_INVALID_PARAMETER;
	}

	rhms_write_pci_config_input* param = (rhms_write_pci_config_input*)lp_in_buffer;
	const ULONG write_size = n_in_buffer_size - offsetof(rhms_write_pci_config_input, Data);

	*lp_bytes_returned = 0;

	return pciConfigWrite(param->PciAddress, param->PciOffset, &param->Data, write_size);
}

/*
 * PCI support functions
 */

NTSTATUS pciConfigRead(ULONG pci_address, ULONG offset, void *data, int length) {
	PCI_SLOT_NUMBER slot;

	const ULONG bus_number = PCI_GET_BUS(pci_address);
	slot.u.AsULONG = 0;
	slot.u.bits.DeviceNumber = PCI_GET_DEV(pci_address);
	slot.u.bits.FunctionNumber = PCI_GET_FUNC(pci_address);
	const int error = HalGetBusDataByOffset(PCIConfiguration, bus_number, slot.u.AsULONG, data, offset, length);

	if (error == 0) {
		return RHMS_DRV_ERROR_PCI_BUS_NOT_EXIST;
	}

	if (length != 2 && error == 2) {
		return RHMS_DRV_ERROR_PCI_NO_DEVICE;
	}

	if (length != error) {
		return RHMS_DRV_ERROR_PCI_READ_CONFIG;
	}

	return STATUS_SUCCESS;
}

NTSTATUS pciConfigWrite(ULONG pci_address, ULONG offset, void *data, int length)
{
	PCI_SLOT_NUMBER slot;

	const ULONG bus_number = PCI_GET_BUS(pci_address);

	slot.u.AsULONG = 0;
	slot.u.bits.DeviceNumber = PCI_GET_DEV(pci_address);
	slot.u.bits.FunctionNumber = PCI_GET_FUNC(pci_address);
	const int error = HalSetBusDataByOffset(PCIConfiguration, bus_number, slot.u.AsULONG, data, offset, length);

	if (error != length) {
		return RHMS_DRV_ERROR_PCI_WRITE_CONFIG;
	}

	return STATUS_SUCCESS;
}

/*
 * Physical memory functions
 */

NTSTATUS ReadMemory(void* lp_in_buffer, ULONG n_in_buffer_size, void* lp_out_buffer, ULONG n_out_buffer_size, ULONG* lp_bytes_returned) {
	PHYSICAL_ADDRESS address;

	if (n_in_buffer_size != sizeof(rhms_read_memory_input)) {
		return STATUS_INVALID_PARAMETER;
	}

	rhms_read_memory_input* param = (rhms_read_memory_input*)lp_in_buffer;
	const ULONG size = param->UnitSize * param->Count;

	if (n_out_buffer_size < size) {
		return STATUS_INVALID_PARAMETER;
	}

	address.QuadPart = param->Address.QuadPart;

#ifndef _PHYSICAL_MEMORY_SUPPORT
	if (0x000C0000 > address.QuadPart
		|| (address.QuadPart + size - 1) > 0x000FFFFF)
	{
		return STATUS_INVALID_PARAMETER;
	}
#endif

	const PVOID maped = MmMapIoSpace(address, size, FALSE);

	BOOLEAN error = FALSE;
	switch (param->UnitSize) {
		case 1:
			READ_REGISTER_BUFFER_UCHAR(maped, lp_out_buffer, param->Count);
			break;
		case 2:
			READ_REGISTER_BUFFER_USHORT(maped, lp_out_buffer, param->Count);
			break;
		case 4:
			READ_REGISTER_BUFFER_ULONG(maped, lp_out_buffer, param->Count);
			break;
		default:
			error = TRUE;
			break;
	}

	MmUnmapIoSpace(maped, size);

	if (error) {
		return STATUS_INVALID_PARAMETER;
	}

	*lp_bytes_returned = n_out_buffer_size;
	return STATUS_SUCCESS;
}

NTSTATUS WriteMemory(void* lp_in_buffer, ULONG n_in_buffer_size, void* lp_out_buffer, ULONG n_out_buffer_size, ULONG* lp_bytes_returned) {
#ifdef _PHYSICAL_MEMORY_SUPPORT
	PHYSICAL_ADDRESS address;

	if (nInBufferSize < offsetof(rhms_write_memory_input, Data)) {
		return STATUS_INVALID_PARAMETER;
	}

	rhms_write_memory_input* param = (rhms_write_memory_input*)lpInBuffer;

	const ULONG size = param->UnitSize * param->Count;
	if (nInBufferSize < size + offsetof(rhms_write_memory_input, Data)) {
		return STATUS_INVALID_PARAMETER;
	}

	address.QuadPart = param->Address.QuadPart;

	const PVOID maped = MmMapIoSpace(address, size, FALSE);

	BOOLEAN error = FALSE;
	switch (param->UnitSize) {
		case 1:
			WRITE_REGISTER_BUFFER_UCHAR(maped,
				(UCHAR*)&param->Data, param->Count);
			break;
		case 2:
			WRITE_REGISTER_BUFFER_USHORT(maped,
				(USHORT*)&param->Data, param->Count);
			break;
		case 4:
			WRITE_REGISTER_BUFFER_ULONG(maped,
				(ULONG*)&param->Data, param->Count);
			break;
		default:
			error = TRUE;
			break;
	}

	MmUnmapIoSpace(maped, size);

	if (error) {
		return STATUS_INVALID_PARAMETER;
	}

	*lpBytesReturned = 0;
	return STATUS_SUCCESS;
#else
	UNREFERENCED_PARAMETER(lp_in_buffer);
	UNREFERENCED_PARAMETER(n_in_buffer_size);
	UNREFERENCED_PARAMETER(lp_out_buffer);
	UNREFERENCED_PARAMETER(n_out_buffer_size);

	*lp_bytes_returned = 0;
	return STATUS_INVALID_PARAMETER;
#endif
}