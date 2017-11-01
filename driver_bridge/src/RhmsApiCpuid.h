#ifndef RHMS_API_CPUID_Ð
#define RHMS_API_CPUID_Ð

#include "RhmsExportsApi.h"

/**
* \brief Read PMC data shared function
* \param index CPUID index
* \param sub_leaf ECX subleaf value (Bh leaf)
* \param eax EAX register
* \param ebx EBX register
* \param ecx ECX register
* \param edx EDX register
* \return Success of failure
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_Cpuid(
	DWORD index,
	DWORD sub_leaf,
	PDWORD eax,
	PDWORD ebx,
	PDWORD ecx,
	PDWORD edx
);

/**
* \brief Read PMC data function (thread context)
* \param index CPUID index
* \param sub_leaf ECX subleaf value (Bh leaf)
* \param eax EAX register
* \param ebx EBX register
* \param ecx ECX register
* \param edx EDX register
* \param thread_affinity_mask Thread affinity mask
* \return Success of failure
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_CpuidTx(
	DWORD index,
	DWORD sub_leaf,
	PDWORD eax,
	PDWORD ebx,
	PDWORD ecx,
	PDWORD edx,
	DWORD_PTR thread_affinity_mask
);

/**
* \brief Read PMC data function (process context)
* \param index CPUID index
* \param sub_leaf ECX subleaf value (Bh leaf)
* \param eax EAX register
* \param ebx EBX register
* \param ecx ECX register
* \param edx EDX register
* \param process_affinity_mask Process affinity mask
* \return Success of failure
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_CpuidPx(
	DWORD index,
	DWORD sub_leaf,
	PDWORD eax,
	PDWORD ebx,
	PDWORD ecx,
	PDWORD edx,
	DWORD_PTR process_affinity_mask
);

#endif
