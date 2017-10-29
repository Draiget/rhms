#ifndef RHMS_API_HLT_H
#define RHMS_API_HLT_H

#include "RhmsExportsApi.h"

/**
* \brief Halt to the next instruction
* \return Success of failure
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_Hlt();

/**
* \brief Halt to the next instruction (thread context)
* \param thread_affinity_mask Thread affinity mask
* \return Success of failure
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_HltTx(
	DWORD_PTR thread_affinity_mask
);

/**
* \brief Halt to the next instruction (process context)
* \param process_affinity_mask Process affinity mask
* \return Success of failure
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_HltPx(
	DWORD_PTR process_affinity_mask
);

#endif
