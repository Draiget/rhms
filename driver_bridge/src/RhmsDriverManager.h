#ifndef RHMS_DRIVER_MANAGER_H
#define RHMS_DRIVER_MANAGER_H

/*
 * Driver status codes
 */

#define RHMS_DRIVER_INSTALL				1
#define RHMS_DRIVER_REMOVE				2
#define RHMS_DRIVER_SYSTEM_INSTALL		3
#define	RHMS_DRIVER_SYSTEM_UNINSTALL	4

/*
 * Driver manager statuses
 */

#define RHMS_DRIVER_MANAGER_UNKNOWN_ERR								-2
#define RHMS_DRIVER_MANAGER_OK										10
#define RHMS_DRIVER_MANAGER_ID_OR_PATH_EMPTY						11
#define RHMS_DRIVER_MANAGER_OPENSCM_FAILED							12
#define RHMS_DRIVER_MANAGER_OPENSCM_FAILED_ACCESS_DENIED			13
#define RHMS_DRIVER_MANAGER_OPENSCM_FAILED_DATABASE_DOES_NOT_EXIST	14
#define RHMS_DRIVER_MANAGER_INCORRECT_DRV_SIGNATURE					15


/*
 * Driver manage functions
 */

int ManageDriver(const char* driver_id, const char* driver_path, USHORT function);

#endif
