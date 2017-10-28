#include <Windows.h>
#include <intrin.h>
#include <cstdio>
#include "RhmsBridge.h"
#include "RhmsDefs.h"
#include "RhmsDriverManager.h"
#include "../../driver/src/include/RhmsIOCTL.h"

#pragma warning(disable: 4996)

/*
 * Forward typedefs
 */

typedef BOOL(WINAPI *LPFN_ISWOW64PROCESS) (HANDLE hProcess, PBOOL Wow64Process);

/*
 * Global variables
 */

HANDLE g_Handle = INVALID_HANDLE_VALUE;
bool g_DriverInitizlied = false;
int g_DriverStatus = RHMS_BRIDGE_UNKNOWN_ERROR;
char g_DriverFileName[MAX_PATH];
char g_DriverPath[MAX_PATH];
bool g_IsMSR = false;
bool g_IsNT = false;
bool g_IsCPUID = false;
bool g_IsTSC = false;

/*
 * API realisation
 */

bool RHMS_IsNT() {
	OSVERSIONINFO osvi;
	osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
	GetVersionExW(&osvi);

	return osvi.dwPlatformId == VER_PLATFORM_WIN32_NT;
}

bool RHMS_IsCPUIDSupported() {
	__try {
		int info[4];
		__cpuid(info, 0x0);
	} __except (EXCEPTION_EXECUTE_HANDLER) {
		return false;
	}

	return true;
}

bool RHMS_IsMSRSupported() {
	int info[4];
	__cpuid(info, 0x1);

	return info[3] >> 5 & 1;
}

bool RHMS_IsTSCSupported() {
	int info[4];
	__cpuid(info, 0x1);

	return info[3] >> 4 & 1;
}

bool RHMS_IsWow64() {
	BOOL is64bit = false;

	if (!IsWow64Process(GetCurrentProcess(), &is64bit)){
		is64bit = false;
	}

	return is64bit;
}

bool RHMS_IsArch64() {
	SYSTEM_INFO systemInfo;
	GetNativeSystemInfo(&systemInfo);
	return systemInfo.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_AMD64;
}

int RHMS_InitializeDriver() {
	if (g_DriverInitizlied){
		return g_DriverStatus;
	}

	g_IsMSR = RHMS_IsMSRSupported();
	g_IsNT = RHMS_IsNT();
	g_IsCPUID = RHMS_IsCPUIDSupported();
	g_IsTSC = RHMS_IsTSCSupported();

	g_DriverStatus = GetDriverInfo();
	if (g_DriverStatus == RHMS_BRIDGE_NO_ERROR){
		for (auto i = 0; i < 4; i++) {
			g_DriverStatus = LoadDriver();
			if (g_DriverStatus == RHMS_BRIDGE_NO_ERROR) {
				break;
			}
			Sleep(100 * i);
		}
	}

	g_DriverInitizlied = true;
	return g_DriverStatus;
}

void RHMS_DeinitializeDriver() {
	if (!g_DriverInitizlied){
		return;
	}

	if (RHMS_IsNT() && GetRefCount() == 1){
		CloseHandle(g_Handle);
		g_Handle = INVALID_HANDLE_VALUE;
		ManageDriver(RHMS_DRIVER_ID, g_DriverPath, RHMS_DRIVER_REMOVE);
	}

	if (g_Handle != INVALID_HANDLE_VALUE) {
		CloseHandle(g_Handle);
		g_Handle = INVALID_HANDLE_VALUE;
	}

	g_DriverInitizlied = false;
}

DWORD LoadDriver() {
	char dir[MAX_PATH];
	char* ptr;

	GetModuleFileNameA(nullptr, dir, MAX_PATH);
	if ((ptr = strrchr(dir, '\\')) != nullptr) {
		*ptr = '\0';
	}

	sprintf_s(g_DriverPath, "%s\\bin\\%s", dir, g_DriverFileName);

	if (!IsFileExist(g_DriverPath)) {
		return RHMS_BRIDGE_DRIVER_NOT_FOUND;
	}

	if (IsOnNetworkDrive(g_DriverPath)) {
		return RHMS_BRIDGE_DRIVER_NOT_LOADED_ON_NETWORK;
	}

	if (RHMS_IsNT()){
		if (OpenDriver()){
			return RHMS_BRIDGE_NO_ERROR;
		}

		ManageDriver(RHMS_DRIVER_ID, g_DriverPath, RHMS_DRIVER_REMOVE);
		auto const install_code = ManageDriver(RHMS_DRIVER_ID, g_DriverPath, RHMS_DRIVER_INSTALL);
		if (install_code != RHMS_DRIVER_MANAGER_OK) {
			ManageDriver(RHMS_DRIVER_ID, g_DriverPath, RHMS_DRIVER_REMOVE);
			return install_code;
		}

		if (OpenDriver()) {
			return RHMS_BRIDGE_NO_ERROR;
		}

		return RHMS_BRIDGE_DRIVER_NOT_LOADED;
	}

	return RHMS_BRIDGE_UNSUPPORTED_PLATFORM;
}

bool OpenDriver() {
	g_Handle = CreateFileA(
		"\\\\.\\" RHMS_DRIVER_ID,
		GENERIC_READ | GENERIC_WRITE,
		0,
		nullptr,
		OPEN_EXISTING,
		FILE_ATTRIBUTE_NORMAL,
		nullptr
	);

	if (g_Handle == INVALID_HANDLE_VALUE) {
		return false;
	}

	return true;
}

DWORD GetDriverInfo() {
	OSVERSIONINFO osvi;
	osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
	GetVersionExW(&osvi);

	if (osvi.dwPlatformId == VER_PLATFORM_WIN32s) {
		return RHMS_BRIDGE_UNSUPPORTED_PLATFORM;
	}

	if (osvi.dwPlatformId == VER_PLATFORM_WIN32_WINDOWS) {
		return RHMS_BRIDGE_UNSUPPORTED_PLATFORM;
	}

	if (osvi.dwPlatformId == VER_PLATFORM_WIN32_NT) {
#ifdef _WIN64
	#ifdef _M_X64
		strcpy_s(g_DriverFileName, MAX_PATH, RHMS_DRIVER_FILE_NAME_WIN_NT_X64);
	#else // IA64
		// strcpy_s(g_DriverFileName, MAX_PATH, RHMS_DRIVER_FILE_NAME_WIN_NT_IA64);
		return RHMS_BRIDGE_UNSUPPORTED_PLATFORM;
	#endif
#else
		strcpy_s(g_DriverFileName, MAX_PATH, RHMS_DRIVER_FILE_NAME_WIN_NT);

		if (RHMS_IsWow64()){
			if (!RHMS_IsArch64()){
				return RHMS_BRIDGE_UNSUPPORTED_PLATFORM;
			}
		}
#endif
		return RHMS_BRIDGE_NO_ERROR;
	}

	return RHMS_BRIDGE_UNKNOWN_ERROR;
}

DWORD GetRefCount() {
	if (g_Handle == INVALID_HANDLE_VALUE) {
		return 0;
	}

	DWORD length;
	DWORD ref_count = 0;

	auto const result = DeviceIoControl(
		g_Handle,
		IOCTL_RHMS_GET_REFCOUNT,
		nullptr,
		0,
		&ref_count,
		sizeof(ref_count),
		&length,
		nullptr
	);

	if (!result) {
		ref_count = 0;
	}

	return ref_count;
}

BOOL IsFileExist(const char* file_name) {
	WIN32_FIND_DATAA find_data;

	auto const file_handle = FindFirstFileA(file_name, &find_data);
	if (file_handle != INVALID_HANDLE_VALUE) {
		FindClose(file_handle);
		return true;
	}

	return false;
}

BOOL IsOnNetworkDrive(const char* file_name) {
	char root[4] = {
		file_name[0],
		':',
		'\\',
		'\0'
	};

	if (root[0] == '\\' || GetDriveTypeA(root) == DRIVE_REMOTE) {
		return true;
	}

	return false;
}
