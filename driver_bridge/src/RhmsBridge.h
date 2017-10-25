#ifndef RHMS_DRIVER_BRIDGE_H
#define RHMS_DRIVER_BRIDGE_H
#include "RhmsBridgeAPI.h"

/*
 * API functions
 */

/**
 * \brief Checking if current windows is based on NT version
 * \return BOOL
 */
RHMS_API_EXPOSED bool RHMS_IsNT();

/**
 * \brief Checking is installed CPU supports CPUID
 * \return BOOL
 */
RHMS_API_EXPOSED bool RHMS_IsCPUIDSupported();

/**
 * \brief Checking if system supports Model-Specific Register's
 * \link http://wiki.osdev.org/Model_Specific_Registers
 * \return BOOL
 */
RHMS_API_EXPOSED bool RHMS_IsMSRSupported();

/**
 * \brief Checking if system supports Time Stamp Counter
 * \link https://en.wikipedia.org/wiki/Time_Stamp_Counter
 * \return BOOL
 */
RHMS_API_EXPOSED bool RHMS_IsTSCSupported();

/**
 * \brief Determines whether the specified process is running under WOW64
 * \return BOOL
 */
RHMS_API_EXPOSED bool RHMS_IsWow64();

/**
 * \brief Determines installed processor architecture is x64
 * \return BOOL
 */
RHMS_API_EXPOSED bool RHMS_IsArch64();

/*
 * Driver exposed functions
 */

RHMS_API_EXPOSED int RHMS_InitializeDriver();
RHMS_API_EXPOSED void RHMS_DeinitializeDriver();

/*
 * MSR Stuff
 */

RHMS_API_EXPOSED BOOL WINAPI RHMS_ReadMsr(
	DWORD index,	// MSR index
	PDWORD eax,		// bit  0-31
	PDWORD edx		// bit 32-63
);

RHMS_API_EXPOSED BOOL WINAPI RHMS_ReadMsrTx(
	DWORD index,					// MSR index
	PDWORD eax,						// bit  0-31
	PDWORD edx,						// bit 32-63
	DWORD_PTR thread_affinity_mask
);

RHMS_API_EXPOSED BOOL WINAPI RHMS_ReadMsrPx(
	DWORD index,					// MSR index
	PDWORD eax,						// bit  0-31
	PDWORD edx,						// bit 32-63
	DWORD_PTR process_affinity_mask
);

/*
 * Internals
 */

DWORD LoadDriver();
bool OpenDriver();
DWORD GetDriverInfo();
DWORD GetRefCount();

BOOL IsFileExist(const char* file_name);
BOOL IsOnNetworkDrive(const char* file_name);

#endif
