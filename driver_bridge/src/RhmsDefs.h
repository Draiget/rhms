#ifndef RHMS_DEFINIONS_H
#define RHMS_DEFINIONS_H 

/*
 * Driver status codes
 */

#define RHMS_BRIDGE_UNKNOWN_ERROR					-1
#define RHMS_BRIDGE_NO_ERROR						0
#define RHMS_BRIDGE_UNSUPPORTED_PLATFORM			1
#define RHMS_BRIDGE_DRIVER_NOT_LOADED				2
#define RHMS_BRIDGE_DRIVER_NOT_FOUND				3
#define RHMS_BRIDGE_DRIVER_UNLOADED					4
#define RHMS_BRIDGE_DRIVER_NOT_LOADED_ON_NETWORK	5

/*
 * Driver types
 */

#define RHMS_DRIVER_TYPE_UNKNOWN			0
#define RHMS_DRIVER_TYPE_WIN_9X				1
#define RHMS_DRIVER_TYPE_WIN_NT				2
#define RHMS_DRIVER_TYPE_WIN_NT4			3	// Obsolete
#define RHMS_DRIVER_TYPE_WIN_NT_X64			4
#define RHMS_DRIVER_TYPE_WIN_NT_IA64		5	// Reseved

/*
 * Driver file names
 */

#define RHMS_DRIVER_FILE_NAME_WIN_NT			RHMS_DRIVER_ID ".sys"
#define RHMS_DRIVER_FILE_NAME_WIN_NT_X64		RHMS_DRIVER_ID "64.sys"
#define RHMS_DRIVER_FILE_NAME_WIN_NT_IA64		RHMS_DRIVER_ID "ia64.sys"  // Reserved

#define RHMS_USE_LOGGER() \
		extern void RHMS_Log(e_rhms_loglevel level, const char* message, ...)

#endif
