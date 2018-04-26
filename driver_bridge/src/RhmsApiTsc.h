#ifndef RHMS_API_RSC_H
#define RHMS_API_RSC_H

#include "RhmsExportsApi.h"

/**
* \brief Read TSC data shared
* \param ret Total return value (not register-specific)
* \return Success of failure
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_ReadTscRaw(
	PULONGLONG ret
);

/**
* \brief Read TSC data shared
* \param eax EAX register (bit 0-31)
* \param edx EDX register (bit 32-63)
* \return Success of failure
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_ReadTsc(
	PDWORD eax,
	PDWORD edx
);

/**
* \brief Read TSC data (thread context)
* \param eax EAX register (bit 0-31)
* \param edx EDX register (bit 32-63)
* \param thread_affinity_mask Thread affinity mask
* \return Success of failure
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_ReadTscTx(
	PDWORD eax,
	PDWORD edx,
	DWORD_PTR thread_affinity_mask
);

/**
* \brief Read TSC data (process context)
* \param eax EAX register (bit 0-31)
* \param edx EDX register (bit 32-63)
* \param process_affinity_mask Process affinity mask
* \return Success of failure
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_ReadTscPx(
	PDWORD eax,
	PDWORD edx,
	DWORD_PTR process_affinity_mask
);

#endif
