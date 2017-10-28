#ifndef RHMS_API_PMC_H
#define RHMS_API_PMC_H

#include "RhmsExportsApi.h"

/**
* \brief Read PMC data shared function
* \param index IO buffer index
* \param eax EAX out register (bit 0-31)
* \param edx EDX out register (bit 32-63)
* \return Success of failure
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_ReadPmc(
	DWORD index,
	PDWORD eax,
	PDWORD edx
);

/**
* \brief Read PMC data shared function (thread context)
* \param index IO buffer index
* \param eax EAX register (bit 0-31)
* \param edx EDX register (bit 32-63)
* \param thread_affinity_mask Thread affinity mask
* \return Success of failure
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_ReadPmcTx(
	DWORD index,
	PDWORD eax,
	PDWORD edx,
	DWORD_PTR thread_affinity_mask
);

/**
* \brief Read PMC data shared function (process context)
* \param index IO buffer index
* \param eax EAX register (bit 0-31)
* \param edx EDX register (bit 32-63)
* \param process_affinity_mask Process affinity mask
* \return Success of failure
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_ReadPmcPx(
	DWORD index,
	PDWORD eax,
	PDWORD edx,
	DWORD_PTR process_affinity_mask
);

#endif
