#include <Windows.h>
#include <intrin.h>
#include "RhmsApiCpuid.h"

extern bool g_IsCPUID;
extern bool g_IsNT;

BOOL WINAPI RHMS_Cpuid(DWORD index, PDWORD eax, PDWORD ebx, PDWORD ecx, PDWORD edx) {
	if (eax == nullptr || ebx == nullptr || ecx == nullptr || edx == nullptr || !g_IsCPUID) {
		return FALSE;
	}

	int info[4];
	__cpuid(info, index);
	*eax = info[0];
	*ebx = info[1];
	*ecx = info[2];
	*edx = info[3];

	return true;
}

BOOL WINAPI RHMS_CpuidTx(DWORD index, PDWORD eax, PDWORD ebx, PDWORD ecx, PDWORD edx, DWORD_PTR thread_affinity_mask) {
	DWORD_PTR mask = 0;
	HANDLE h_thread = nullptr;

	if (g_IsNT) {
		h_thread = GetCurrentThread();
		mask = SetThreadAffinityMask(h_thread, thread_affinity_mask);
		if (mask == 0) {
			return false;
		}
	}

	auto const result = RHMS_Cpuid(index, eax, ebx, ecx, edx);

	if (g_IsNT) {
		SetThreadAffinityMask(h_thread, mask);
	}

	return result;
}

BOOL WINAPI RHMS_CpuidPx(DWORD index, PDWORD eax, PDWORD ebx, PDWORD ecx, PDWORD edx, DWORD_PTR process_affinity_mask) {
	DWORD_PTR process_mask = 0;
	DWORD_PTR system_mask = 0;
	HANDLE h_process = nullptr;

	UNREFERENCED_PARAMETER(process_mask);
	UNREFERENCED_PARAMETER(system_mask);

	if (g_IsNT) {
		h_process = GetCurrentProcess();
		GetProcessAffinityMask(h_process, &process_mask, &system_mask);
		if (!SetProcessAffinityMask(h_process, process_affinity_mask)) {
			return false;
		}
	}

	auto const result = RHMS_Cpuid(index, eax, ebx, ecx, edx);
	
	if (g_IsNT) {
		SetProcessAffinityMask(h_process, process_mask);
	}

	return result;
}