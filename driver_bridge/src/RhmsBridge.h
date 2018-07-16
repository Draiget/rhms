#ifndef RHMS_DRIVER_BRIDGE_H
#define RHMS_DRIVER_BRIDGE_H

#include <string>
#include "RhmsExportsApi.h"

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
 * Display driver spechial
 */
RHMS_API_EXPOSED int RHMS_ManageGraphicsDriver(int code);

/*
 * Logging agent
 */

enum e_rhms_loglevel
{
	RHMS_LOGLEVEL_DEBUG = 0,
	RHMS_LOGLEVEL_ERROR,
	RHMS_LOGLEVEL_WARNING
};

typedef void(__stdcall *logger_callback_fn)(e_rhms_loglevel level, const char* message);

RHMS_API_EXPOSED bool RHMS_RegisterLoggerCallback(logger_callback_fn callback_ref);
void RHMS_Log(e_rhms_loglevel level, const char* message, ...);

/*
 * MSR Stuff
 */

#include "RhmsApiMsr.h"

/**
 * PMC functions (Performance Monitor Count Registers)
 * \link http://wiki.dreamrunner.org/public_html/Embedded-System/Cortex-A8/PerformanceMonitorControlRegister.html
 */

#include "RhmsApiPmc.h"

/**
 * CPUID functions
 * \link https://ru.wikipedia.org/wiki/CPUID
 */

#include "RhmsApiCpuid.h"

/**
 * TSC (Time Stamp Counter) registers
 * \link https://ru.wikipedia.org/wiki/Rdtsc
 */

#include "RhmsApiTsc.h"

/**
 * HLT (Halt)
 * \link https://en.wikipedia.org/wiki/HLT_(x86_instruction)
 */

#include "RhmsApiHlt.h"

/*
 * I/O
 */

#include "RhmsApiIO.h"

/*
 * PCI
 */

#include "RhmsApiPci.h"

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
