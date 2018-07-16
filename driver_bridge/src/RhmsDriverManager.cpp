#include <Windows.h>
#include "RhmsDriverManager.h"
#include "RhmsBridge.h"
#include <cstdio>
#include "RhmsDefs.h"

#include <setupapi.h>
#include <tlhelp32.h>
#include <devguid.h>
#pragma comment(lib, "setupapi.lib")

extern HANDLE g_Handle;
RHMS_USE_LOGGER();

static BOOL InstallDriver(SC_HANDLE h_sc_manager, const char* driver_id, const char* driver_path);
static BOOL RemoveDriver(SC_HANDLE h_sc_manager, const char* driver_id);
static int StartDriver(SC_HANDLE h_sc_manager, const char* driver_id);
static BOOL StopDriver(SC_HANDLE h_sc_manager, const char* driver_id);
static BOOL SystemInstallDriver(SC_HANDLE h_sc_manager, const char* driver_id, const char* driver_path);
static BOOL IsSystemDriverInstalled(SC_HANDLE h_sc_manager, const char* driver_id, const char* driver_path);
static BOOL IsServiceExists(SC_HANDLE h_sc_manager, const char* driver_id, const char* driver_path);
static DWORD SetDisplayDriverState(DWORD driver_state);
static BOOL RefreshNotifyIcons();
static BOOL RefreshNotifyWindow(const HWND window);

/**
 * \brief Manage driver in system (install, remove, permanent/system install)
 * \param driver_id Driver identifer
 * \param driver_path Driver file path
 * \param function Target function to do
 * \return Status of target function
 */
int ManageDriver(const char* driver_id, const char* driver_path, USHORT function) {
	int retn_code = false;

	if (function < RHMS_DISPLAY_DRIVER_STOP && (driver_id == nullptr || driver_path == nullptr)) {
		return RHMS_DRIVER_MANAGER_ID_OR_PATH_EMPTY;
	}

	const auto h_sc_manager = OpenSCManagerA(nullptr, nullptr, SC_MANAGER_ALL_ACCESS);
	if (h_sc_manager == nullptr) {
		auto const err = GetLastError();
		if (err == ERROR_ACCESS_DENIED){
			RHMS_Log(RHMS_LOGLEVEL_ERROR, "OpenSCManagerA: Access denied (user must have administrator rights)", err);
			return RHMS_DRIVER_MANAGER_OPENSCM_FAILED_ACCESS_DENIED;
		}
		
		if (err == ERROR_DATABASE_DOES_NOT_EXIST) {
			RHMS_Log(RHMS_LOGLEVEL_ERROR, "OpenSCManagerA: The specified database does not exist (ERROR_DATABASE_DOES_NOT_EXIST)", err);
			return RHMS_DRIVER_MANAGER_OPENSCM_FAILED_DATABASE_DOES_NOT_EXIST;
		}

		return RHMS_DRIVER_MANAGER_OPENSCM_FAILED;
	}

	switch (function) {
		case RHMS_DRIVER_INSTALL:
			if (IsServiceExists(h_sc_manager, driver_id, driver_path)) {
				if (InstallDriver(h_sc_manager, driver_id, driver_path)) {
					retn_code = StartDriver(h_sc_manager, driver_id);
				}
			}
			break;

		case RHMS_DRIVER_REMOVE:
			if (!IsSystemDriverInstalled(h_sc_manager, driver_id, driver_path)) {
				if (IsServiceExists(h_sc_manager, driver_id, driver_path)) {
					StopDriver(h_sc_manager, driver_id);
					retn_code = RemoveDriver(h_sc_manager, driver_id);
				} else {
					retn_code = TRUE;
				}
			}
			break;

		case RHMS_DRIVER_SYSTEM_INSTALL:
			if (IsSystemDriverInstalled(h_sc_manager, driver_id, driver_path)) {
				retn_code = TRUE;
			} else {
				if (!OpenDriver()) {
					StopDriver(h_sc_manager, driver_id);
					RemoveDriver(h_sc_manager, driver_id);
					if (InstallDriver(h_sc_manager, driver_id, driver_path)) {
						StartDriver(h_sc_manager, driver_id);
					}
					OpenDriver();
				}
				retn_code = SystemInstallDriver(h_sc_manager, driver_id, driver_path);
			}
			break;

		case RHMS_DRIVER_SYSTEM_UNINSTALL:
			if (!IsSystemDriverInstalled(h_sc_manager, driver_id, driver_path)) {
				retn_code = TRUE;
			} else {
				if (g_Handle != INVALID_HANDLE_VALUE) {
					CloseHandle(g_Handle);
					g_Handle = INVALID_HANDLE_VALUE;
				}

				if (StopDriver(h_sc_manager, driver_id)) {
					retn_code = RemoveDriver(h_sc_manager, driver_id);
				}
			}
			break;
		case RHMS_DISPLAY_DRIVER_STOP:
			if (!SetDisplayDriverState(DICS_DISABLE)) {
				retn_code = RHMS_DRIVER_MANAGER_DISPLAY_NOT_FOUND_OR_DISABLED_ALREADY;
			}
			else {
				RefreshNotifyIcons();
				retn_code = RHMS_DRIVER_MANAGER_DISPLAY_DRIVER_DISABLE_OK;
			}
			break;
		case RHMS_DISPLAY_DRIVER_START:
			if (!SetDisplayDriverState(DICS_ENABLE)){
				retn_code = RHMS_DRIVER_MANAGER_DISPLAY_NOT_FOUND_OR_ENABLED_ALREADY;
			} else {
				retn_code = RHMS_DRIVER_MANAGER_DISPLAY_DRIVER_ENABLE_OK;
			}

			break;

		default:
			retn_code = FALSE;
			break;
	}

	if (h_sc_manager != nullptr) {
		CloseServiceHandle(h_sc_manager);
	}

	return retn_code;
}

DWORD SetDisplayDriverState(DWORD driver_state) {
	BOOL retn_code = false;
	DWORD dw_size;

	const auto devices = SetupDiGetClassDevs(&GUID_DEVCLASS_DISPLAY, nullptr, nullptr, DIGCF_PRESENT);
	if (devices == INVALID_HANDLE_VALUE){
		return 0;
	}

	SP_DEVINFO_DATA device;
	SP_PROPCHANGE_PARAMS params;
	DWORD result = 0;

	for (auto i = 0; device.cbSize = sizeof device, SetupDiEnumDeviceInfo(devices, i, &device); i++) {
		params.ClassInstallHeader.cbSize = sizeof params.ClassInstallHeader;
		params.ClassInstallHeader.InstallFunction = DIF_PROPERTYCHANGE;
		params.StateChange = driver_state;
		params.Scope = DICS_FLAG_GLOBAL;
		params.HwProfile = 0;

		if (SetupDiSetClassInstallParams(devices, &device, &params.ClassInstallHeader, sizeof params)) {
			if (SetupDiCallClassInstaller(DIF_PROPERTYCHANGE, devices, &device)) {
				result++;
			}
		}
	}

	SetupDiDestroyDeviceInfoList(devices);
	return result;
}

BOOL RefreshNotifyIcons() {
	auto parent = FindWindowA("Shell_TrayWnd", nullptr);
	if (!parent) {
		return FALSE;
	}

	parent = FindWindowExA(parent, nullptr, "TrayNotifyWnd", nullptr);
	if (!parent) {
		return FALSE;
	}

	parent = FindWindowExA(parent, nullptr, "SysPager", nullptr);
	if (!parent) {
		return FALSE;
	}

	auto window = FindWindowExA(parent, nullptr, "ToolbarWindow32", "Notification Area");
	if (window) {
		RefreshNotifyWindow(window);
	}

	window = FindWindowExA(parent, nullptr, "ToolbarWindow32", "User Promoted Notification Area");
	if (window) {
		RefreshNotifyWindow(window);
	}

	parent = FindWindowA("NotifyIconOverflowWindow", nullptr);
	if (parent)	{
		window = FindWindowExA(parent, nullptr, "ToolbarWindow32", "Overflow Notification Area");
		if (window) {
			RefreshNotifyWindow(window);
		}
	}

	return TRUE;
}

BOOL RefreshNotifyWindow(const HWND window) {
	RECT rect;
	GetClientRect(window, &rect);

	for (auto y = 0; y < rect.bottom; y += 4) {
		for (auto x = 0; x < rect.right; x += 4) {
			PostMessage(window, WM_MOUSEMOVE, 0, (y << 16) | (x & 65535));
		}
	}

	return TRUE;
}

/**
 * \brief Create driver service (install driver) 
 * \param h_sc_manager Service control manager
 * \param driver_id Driver id
 * \param driver_path Driver file path
 * \return Status of installing driver
 */
BOOL InstallDriver(SC_HANDLE h_sc_manager, const char* driver_id, const char* driver_path) {
	RHMS_Log(RHMS_LOGLEVEL_DEBUG, "%s: Creating service", __FUNCTION__);
	auto const h_service = CreateServiceA(	h_sc_manager,
											driver_id,
											driver_id,
											SERVICE_ALL_ACCESS,
											SERVICE_KERNEL_DRIVER,
											SERVICE_DEMAND_START,
											SERVICE_ERROR_NORMAL,
											driver_path,
											nullptr,
											nullptr,
											nullptr,
											nullptr,
											nullptr
	);

	if (h_service == nullptr) {
		auto const error = GetLastError();
		RHMS_Log(RHMS_LOGLEVEL_ERROR, "%s: Creating service error: %d", __FUNCTION__, error);
		if (error == ERROR_SERVICE_EXISTS) {
			return true;
		}
	} else {
		RHMS_Log(RHMS_LOGLEVEL_DEBUG, "%s: Install done, close handle", __FUNCTION__);
		CloseServiceHandle(h_service);
		return true;
	}

	return false;
}

/**
 * \brief Installing driver into system (with service auto-start)
 * \param h_sc_manager Service control manager
 * \param driver_id Driver id
 * \param driver_path Driver file path
 * \return Status of installing driver
 */
BOOL SystemInstallDriver(SC_HANDLE h_sc_manager, const char* driver_id, const char* driver_path) {
	BOOL retn_code = false;

	auto const h_service = OpenServiceA(h_sc_manager, driver_id, SERVICE_ALL_ACCESS);
	if (h_service != nullptr) {
		retn_code = ChangeServiceConfigA(h_service,
			SERVICE_KERNEL_DRIVER,
			SERVICE_AUTO_START,
			SERVICE_ERROR_NORMAL,
			driver_path,
		    nullptr,
		    nullptr,
		    nullptr,
		    nullptr,
		    nullptr,
		    nullptr
		);
		CloseServiceHandle(h_service);
	}

	return retn_code;
}

/**
 * \brief Remove driver (and service) from system
 * \param h_sc_manager Service control manager
 * \param driver_id Driver id
 * \return Status of removing driver
 */
BOOL RemoveDriver(SC_HANDLE h_sc_manager, const char* driver_id) {
	RHMS_Log(RHMS_LOGLEVEL_DEBUG, "%s: Removing driver", __FUNCTION__);
	auto const service_handle = OpenServiceA(h_sc_manager, driver_id, SERVICE_ALL_ACCESS);
	if (service_handle == nullptr) {
		return true;
	} 

	auto const retn_code = DeleteService(service_handle);
	CloseServiceHandle(service_handle);
	return retn_code;
}


/**
 * \brief Start driver (open service)
 * \param h_sc_manager Service control manager
 * \param driver_id Driver id
 * \return Status of starting
 */
int StartDriver(SC_HANDLE h_sc_manager, const char* driver_id) {
	RHMS_Log(RHMS_LOGLEVEL_DEBUG, "%s: Starting driver", __FUNCTION__);
	int retn_code = false;

	auto const service_handle = OpenServiceA(h_sc_manager, driver_id, SERVICE_ALL_ACCESS);
	if (service_handle != nullptr) {
		if (!StartServiceA(service_handle, 0, nullptr)) {
			auto const error = GetLastError();
			if (error == ERROR_INVALID_IMAGE_HASH){
				retn_code = RHMS_DRIVER_MANAGER_INCORRECT_DRV_SIGNATURE;
			} else {
				RHMS_Log(RHMS_LOGLEVEL_ERROR, "%s: Starting driver error: %d", __FUNCTION__, error);
			}

			if (error == ERROR_SERVICE_ALREADY_RUNNING) {
				retn_code = RHMS_DRIVER_MANAGER_OK;
			}
		} else {
			RHMS_Log(RHMS_LOGLEVEL_DEBUG, "%s: Driver started!", __FUNCTION__);
			retn_code = RHMS_DRIVER_MANAGER_OK;
		}

		CloseServiceHandle(service_handle);
	}

	return retn_code;
}

/**
 * \brief Stop the driver
 * \param h_sc_manager Service control manager
 * \param driver_id Driver id
 * \return Does driver stops
 */
BOOL StopDriver(SC_HANDLE h_sc_manager, const char* driver_id) {
	BOOL retn_code = false;
	SERVICE_STATUS service_status;

	UNREFERENCED_PARAMETER(service_status);

	RHMS_Log(RHMS_LOGLEVEL_DEBUG, "%s: Stopping driver", __FUNCTION__);
	auto const service_handle = OpenServiceA(h_sc_manager, driver_id, SERVICE_ALL_ACCESS);
	if (service_handle != nullptr) {
		retn_code = ControlService(service_handle, SERVICE_CONTROL_STOP, &service_status);
		CloseServiceHandle(service_handle);
	}

	return retn_code;
}


/**
 * \brief Checking if system driver is installed in system
 * \param h_sc_manager Service control manager
 * \param driver_id Driver id
 * \param driver_path Driver file path
 * \return Installed status
 */
BOOL IsSystemDriverInstalled(SC_HANDLE h_sc_manager, const char* driver_id, const char* driver_path) {
	UNREFERENCED_PARAMETER(driver_path);

	BOOL retn_code = false;
	DWORD dw_size;

	auto const service_handle = OpenServiceA(h_sc_manager, driver_id, SERVICE_ALL_ACCESS);
	if (service_handle != nullptr) {
		QueryServiceConfig(service_handle, nullptr, 0, &dw_size);
		auto const lp_service_config = static_cast<LPQUERY_SERVICE_CONFIG>(HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY, dw_size));
		QueryServiceConfig(service_handle, lp_service_config, dw_size, &dw_size);

		if (lp_service_config->dwStartType == SERVICE_AUTO_START) {
			retn_code = TRUE;
		}

		CloseServiceHandle(service_handle);
		HeapFree(GetProcessHeap(), HEAP_NO_SERIALIZE, lp_service_config);
	}

	return retn_code;
}

BOOL IsServiceExists(SC_HANDLE h_sc_manager, const char* driver_id, const char* driver_path) {
	UNREFERENCED_PARAMETER(driver_path);

	auto const service_handle = OpenServiceA(h_sc_manager, driver_id, SERVICE_ALL_ACCESS);
	if (service_handle != nullptr) {
		CloseServiceHandle(service_handle);
		return true;
	}

	// TODO: Maybe check GetLastError() and ERROR_SERVICE_DOES_NOT_EXIST
	return true;
}
