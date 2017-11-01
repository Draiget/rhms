#include <Windows.h>
#include "RhmsDriverManager.h"
#include "RhmsBridge.h"
#include <cstdio>
#include "RhmsDefs.h"

extern HANDLE g_Handle;
RHMS_USE_LOGGER();

static BOOL InstallDriver(SC_HANDLE h_sc_manager, const char* driver_id, const char* driver_path);
static BOOL RemoveDriver(SC_HANDLE h_sc_manager, const char* driver_id);
static int StartDriver(SC_HANDLE h_sc_manager, const char* driver_id);
static BOOL StopDriver(SC_HANDLE h_sc_manager, const char* driver_id);
static BOOL SystemInstallDriver(SC_HANDLE h_sc_manager, const char* driver_id, const char* driver_path);
static BOOL IsSystemInstallDriver(SC_HANDLE h_sc_manager, const char* driver_id, const char* driver_path);

/**
 * \brief Manage driver in system (install, remove, permanent/system install)
 * \param driver_id Driver identifer
 * \param driver_path Driver file path
 * \param function Target function to do
 * \return Status of target function
 */
int ManageDriver(const char* driver_id, const char* driver_path, USHORT function) {
	int retn_code = false;

	if (driver_id == nullptr || driver_path == nullptr) {
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
			if (InstallDriver(h_sc_manager, driver_id, driver_path)) {
				retn_code = StartDriver(h_sc_manager, driver_id);
			}
			break;

		case RHMS_DRIVER_REMOVE:
			if (!IsSystemInstallDriver(h_sc_manager, driver_id, driver_path)) {
				StopDriver(h_sc_manager, driver_id);
				retn_code = RemoveDriver(h_sc_manager, driver_id);
			}
			break;

		case RHMS_DRIVER_SYSTEM_INSTALL:
			if (IsSystemInstallDriver(h_sc_manager, driver_id, driver_path)) {
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
			if (!IsSystemInstallDriver(h_sc_manager, driver_id, driver_path)) {
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

		default:
			retn_code = FALSE;
			break;
	}

	if (h_sc_manager != nullptr) {
		CloseServiceHandle(h_sc_manager);
	}

	return retn_code;
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
 * \brief Does system install the driver
 * \param h_sc_manager Service control manager
 * \param driver_id Driver id
 * \param driver_path Driver file path
 * \return Return status is system install driver
 */
BOOL IsSystemInstallDriver(SC_HANDLE h_sc_manager, const char* driver_id, const char* driver_path) {
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