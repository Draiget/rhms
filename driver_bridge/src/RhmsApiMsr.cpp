#include <Windows.h>
#include "RhmsApiMsr.h"
#include "../../driver/src/include/RhmsIOCTL.h"
#include <cstdio>

extern HANDLE g_Handle;
extern bool g_IsMSR;
extern bool g_IsNT;

BOOL WINAPI RHMS_ReadMsr(DWORD index, PDWORD eax, PDWORD edx) {
	if (g_Handle == INVALID_HANDLE_VALUE) {
		return false;
	}

	if (eax == nullptr || edx == nullptr || !g_IsMSR) {
		return false;
	}

	DWORD returned_length = 0;
	BYTE out_buf[8] = { 0 };

	auto const result = DeviceIoControl(
		g_Handle,
		IOCTL_RHMS_READ_MSR,
		&index,
		sizeof(index),
		&out_buf,
		sizeof(out_buf),
		&returned_length,
		nullptr
	);

	if (result) {
		memcpy(eax, out_buf, 4);
		memcpy(edx, out_buf + 4, 4);
		return true;
	}

	printf("result DeviceIoControl is false\n");
	return false;
}

BOOL WINAPI RHMS_ReadMsrTx(DWORD index, PDWORD eax, PDWORD edx, DWORD_PTR thread_affinity_mask) {
	DWORD_PTR mask = 0;
	HANDLE h_thread = nullptr;

	if (g_IsNT) {
		h_thread = GetCurrentThread();
		mask = SetThreadAffinityMask(h_thread, thread_affinity_mask);
		if (mask == 0) {
			return false;
		}
	}

	auto const result = RHMS_ReadMsr(index, eax, edx);

	if (g_IsNT) {
		SetThreadAffinityMask(h_thread, mask);
	}

	return result;
}

BOOL WINAPI RHMS_ReadMsrPx(DWORD index, PDWORD eax, PDWORD edx, DWORD_PTR process_affinity_mask) {
	DWORD_PTR process_mask = 0;
	DWORD_PTR system_mask = 0;
	HANDLE h_process = nullptr;

	if (g_IsNT) {
		h_process = GetCurrentProcess();
		GetProcessAffinityMask(h_process, &process_mask, &system_mask);
		if (!SetProcessAffinityMask(h_process, process_affinity_mask)) {
			return false;
		}
	}

	auto const result = RHMS_ReadMsr(index, eax, edx);

	if (g_IsNT) {
		SetProcessAffinityMask(h_process, process_mask);
	}

	return result;
}