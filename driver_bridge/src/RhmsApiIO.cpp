#include <Windows.h>
#include "RhmsApiIO.h"
#include "../../driver/src/include/RhmsIOCTL.h"

extern HANDLE g_Handle;
extern bool g_IsNT;

BYTE WINAPI RHMS_ReadIoPortByte(WORD port) {
	if (g_Handle == INVALID_HANDLE_VALUE) {
		return 0;
	}

	DWORD returned_length = 0;
	WORD value = 0;

	DeviceIoControl(
		g_Handle,
		IOCTL_RHMS_READ_IO_PORT_BYTE,
		&port,
		sizeof(port),
		&value,
		sizeof(value),
		&returned_length,
		nullptr
	);

	return static_cast<BYTE>(value);
}

WORD WINAPI RHMS_ReadIoPortWord(WORD port) {
	if (g_Handle == INVALID_HANDLE_VALUE) {
		return 0;
	}

	DWORD returned_length = 0;
	WORD value = 0;

	DeviceIoControl(
		g_Handle,
		IOCTL_RHMS_READ_IO_PORT_WORD,
		&port,
		sizeof(port),
		&value,
		sizeof(value),
		&returned_length,
		nullptr
	);

	return value;
}

DWORD WINAPI RHMS_ReadIoPortDword(WORD port) {
	if (g_Handle == INVALID_HANDLE_VALUE) {
		return 0;
	}

	DWORD returned_length = 0;
	DWORD port4 = port;
	DWORD value = 0;

	DeviceIoControl(
		g_Handle,
		IOCTL_RHMS_READ_IO_PORT_DWORD,
		&port4,
		sizeof(port4),	// required 4 bytes
		&value,
		sizeof(value),
		&returned_length,
		nullptr
	);

	return value;
}

BOOL WINAPI RHMS_ReadIoPortByteEx(WORD port, PBYTE value) {
	if (g_Handle == INVALID_HANDLE_VALUE) {
		return FALSE;
	}

	DWORD returned_length = 0;
	WORD val = 0;

	auto const result = DeviceIoControl(
		g_Handle,
		IOCTL_RHMS_READ_IO_PORT_BYTE,
		&port,
		sizeof(port),
		&val,
		sizeof(val),
		&returned_length,
		nullptr
	);

	if (result) {
		*value = static_cast<BYTE>(val);
		return true;
	}

	return false;
}

BOOL WINAPI RHMS_ReadIoPortWordEx(WORD port, PWORD value) {
	if (g_Handle == INVALID_HANDLE_VALUE) {
		return false;
	}

	DWORD returned_length = 0;
	WORD val = 0;

	auto const result = DeviceIoControl(
		g_Handle,
		IOCTL_RHMS_READ_IO_PORT_WORD,
		&port,
		sizeof(port),
		&val,
		sizeof(val),
		&returned_length,
		nullptr
	);

	if (result) {
		*value = val;
		return true;
	}

	return false;
}

BOOL WINAPI RHMS_ReadIoPortDwordEx(WORD port, PDWORD value) {
	if (g_Handle == INVALID_HANDLE_VALUE) {
		return false;
	}

	DWORD returned_length = 0;
	DWORD val = port;

	auto const result = DeviceIoControl(
		g_Handle,
		IOCTL_RHMS_READ_IO_PORT_DWORD,
		&val,
		sizeof(val), // required 4 bytes
		&val,
		sizeof(val),
		&returned_length,
		nullptr
	);

	if (result) {
		*value = val;
		return true;
	}

	return false;
}

VOID WINAPI RHMS_WriteIoPortByte(WORD port, BYTE value) {
	if (g_Handle == INVALID_HANDLE_VALUE){
		return;
	}

	DWORD returned_length = 0;
	rhms_write_io_port_input in_buf;

	in_buf.SharedData.CharData = value;
	in_buf.PortNumber = port;
	auto const length = offsetof(rhms_write_io_port_input, SharedData.CharData) + sizeof in_buf.SharedData.CharData;

	DeviceIoControl(
		g_Handle,
		IOCTL_RHMS_WRITE_IO_PORT_BYTE,
		&in_buf,
		length,
		nullptr,
		0,
		&returned_length,
		nullptr
	);
}

VOID WINAPI RHMS_WriteIoPortWord(WORD port, WORD value) {
	if (g_Handle == INVALID_HANDLE_VALUE){
		return;
	}

	DWORD returned_length = 0;
	rhms_write_io_port_input in_buf;

	in_buf.SharedData.ShortData = value;
	in_buf.PortNumber = port;
	auto const length = offsetof(rhms_write_io_port_input, SharedData.CharData) + sizeof in_buf.SharedData.ShortData;

	DeviceIoControl(
		g_Handle,
		IOCTL_RHMS_WRITE_IO_PORT_WORD,
		&in_buf,
		length,
		nullptr,
		0,
		&returned_length,
		nullptr
	);
}

VOID WINAPI RHMS_WriteIoPortDword(WORD port, DWORD value) {
	if (g_Handle == INVALID_HANDLE_VALUE){
		return;
	}

	DWORD returned_length = 0;
	rhms_write_io_port_input inBuf;

	inBuf.SharedData.LongData = value;
	inBuf.PortNumber = port;
	auto const length = offsetof(rhms_write_io_port_input, SharedData.CharData) + sizeof inBuf.SharedData.LongData;

	DeviceIoControl(
		g_Handle,
		IOCTL_RHMS_WRITE_IO_PORT_DWORD,
		&inBuf,
		length,
		nullptr,
		0,
		&returned_length,
		nullptr
	);
}

BOOL WINAPI RHMS_WriteIoPortByteEx(WORD port, BYTE value) {
	if (g_Handle == INVALID_HANDLE_VALUE) {
		return false;
	}

	DWORD returned_length = 0;
	rhms_write_io_port_input in_buf;

	in_buf.SharedData.CharData = value;
	in_buf.PortNumber = port;
	auto const length = offsetof(rhms_write_io_port_input, SharedData.CharData) + sizeof in_buf.SharedData.CharData;

	auto const result = DeviceIoControl(
		g_Handle,
		IOCTL_RHMS_WRITE_IO_PORT_BYTE,
		&in_buf,
		length,
		nullptr,
		0,
		&returned_length,
		nullptr
	);

	return result;
}

BOOL WINAPI RHMS_WriteIoPortWordEx(WORD port, WORD value) {
	if (g_Handle == INVALID_HANDLE_VALUE) {
		return false;
	}

	DWORD returned_length = 0;
	rhms_write_io_port_input in_buf;

	in_buf.SharedData.ShortData = value;
	in_buf.PortNumber = port;
	auto const length = offsetof(rhms_write_io_port_input, SharedData.CharData) + sizeof in_buf.SharedData.ShortData;

	auto const result = DeviceIoControl(
		g_Handle,
		IOCTL_RHMS_WRITE_IO_PORT_WORD,
		&in_buf,
		length,
		nullptr,
		0,
		&returned_length,
		nullptr
	);

	return result;
}

BOOL WINAPI RHMS_WriteIoPortDwordEx(WORD port, DWORD value) {
	if (g_Handle == INVALID_HANDLE_VALUE) {
		return FALSE;
	}

	DWORD returned_length = 0;
	rhms_write_io_port_input in_buf;

	in_buf.SharedData.LongData = value;
	in_buf.PortNumber = port;
	auto const length = offsetof(rhms_write_io_port_input, SharedData.CharData) + sizeof in_buf.SharedData.LongData;

	auto const result = DeviceIoControl(
		g_Handle,
		IOCTL_RHMS_WRITE_IO_PORT_DWORD,
		&in_buf,
		length,
		nullptr,
		0,
		&returned_length,
		nullptr
	);

	return result;
}