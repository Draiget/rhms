#ifndef RHMS_API_MSR_H
#define RHMS_API_MSR_H

#include "RhmsExportsApi.h"

/**
* \brief Read MSR data shared function
* \param index IO buffer index
* \param eax EAX out register (bit 0-31)
* \param edx EDX out register (bit 32-63)
* \return Success of failure
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_ReadMsr(
	DWORD index,
	PDWORD eax,
	PDWORD edx
);

/**
* \brief Read MSR data (thread context)
* \param index IO buffer index
* \param eax EAX out register (bit 0-31)
* \param edx EDX out register (bit 32-63)
* \param thread_affinity_mask Thread affinity mask
* \return Success of failure
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_ReadMsrTx(
	DWORD index,
	PDWORD eax,
	PDWORD edx,
	DWORD_PTR thread_affinity_mask
);

/**
* \brief Read MSR data (process context)
* \param index IO buffer index
* \param eax EAX out register (bit 0-31)
* \param edx EDX out register (bit 32-63)
* \param process_affinity_mask Process affinity mask
* \return Success of failure
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_ReadMsrPx(
	DWORD index,
	PDWORD eax,
	PDWORD edx,
	DWORD_PTR process_affinity_mask
);

/**
* \brief Write MSR data shared function
* \param index IO buffer index
* \param eax EAX out register (bit 0-31)
* \param edx EDX out register (bit 32-63)
* \return Success of failure
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_WriteMsr(
	DWORD index,
	DWORD eax,
	DWORD edx
);

/**
* \brief Write MSR data (thread context)
* \param index IO buffer index
* \param eax EAX out register (bit 0-31)
* \param edx EDX out register (bit 32-63)
* \param thread_affinity_mask Thread affinity mask
* \return Success of failure
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_WriteMsrTx(
	DWORD index,
	DWORD eax,
	DWORD edx,
	DWORD_PTR thread_affinity_mask
);

/**
* \brief Write MSR data (process context)
* \param index IO buffer index
* \param eax EAX out register (bit 0-31)
* \param edx EDX out register (bit 32-63)
* \param process_affinity_mask Process affinity mask
* \return Success of failure
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_WriteMsrPx(
	DWORD index,					// MSR index
	DWORD eax,						// bit  0-31
	DWORD edx,						// bit 32-63
	DWORD_PTR process_affinity_mask
);


#endif
