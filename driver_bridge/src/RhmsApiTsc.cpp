#include <Windows.h>
#include "RhmsApiTsc.h"

extern bool g_IsTSC;
extern bool g_IsNT;

BOOL WINAPI RHMS_ReadTsc(PDWORD eax, PDWORD edx) {
	if (eax == nullptr || edx == nullptr || g_IsTSC == false) {
		return false;
	}

	ULONGLONG value = 0;

	value = __rdtsc();
	*eax = static_cast<DWORD>((value >> 0) & 0xFFFFFFFF);
	*edx = static_cast<DWORD>((value >> 32) & 0xFFFFFFFF);

	return true;
}

BOOL WINAPI RHMS_ReadTscTx(PDWORD eax, PDWORD edx, DWORD_PTR thread_affinity_mask) {
	DWORD_PTR mask = 0;
	HANDLE h_thread = nullptr;

	if (g_IsNT) {
		h_thread = GetCurrentThread();
		mask = SetThreadAffinityMask(h_thread, thread_affinity_mask);
		if (mask == 0) {
			return false;
		}
	}

	auto const result = RHMS_ReadTsc(eax, edx);

	if (g_IsNT) {
		SetThreadAffinityMask(h_thread, mask);
	}

	return result;
}

BOOL WINAPI RHMS_ReadTscPx(PDWORD eax, PDWORD edx, DWORD_PTR process_affinity_mask) {
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

	auto const result = RHMS_ReadTsc(eax, edx);

	if (g_IsNT) {
		SetProcessAffinityMask(h_process, process_mask);
	}

	return result;
}