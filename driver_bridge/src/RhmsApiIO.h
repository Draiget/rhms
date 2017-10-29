#ifndef RHMS_API_IO_H
#define RHMS_API_IO_H

#include "RhmsExportsApi.h"

/**
* \brief Read port data
* \param port I/O port address
* \return Port value
*/
RHMS_API_EXPOSED BYTE WINAPI RHMS_ReadIoPortByte(
	WORD port
);

/**
* \brief Read port data
* \param port I/O port address
* \return Port value
*/
RHMS_API_EXPOSED WORD WINAPI RHMS_ReadIoPortWord(
	WORD port
);

/**
* \brief Read port data
* \param port I/O port address
* \return Port value
*/
RHMS_API_EXPOSED DWORD WINAPI RHMS_ReadIoPortDword(
	WORD port
);

/**
* \brief Extended read port data
* \param port I/O port address
* \param value Out readed port value
* \return Success or fail
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_ReadIoPortByteEx(
	WORD port,
	PBYTE value
);

/**
* \brief Extended read port data
* \param port I/O port address
* \param value Out readed port value
* \return Success or fail
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_ReadIoPortWordEx(
	WORD port,
	PWORD value
);

/**
* \brief Extended read port data
* \param port I/O port address
* \param value Out readed port value
* \return Success or fail
*/
RHMS_API_EXPOSED BOOL WINAPI RHMS_ReadIoPortDwordEx(
	WORD port,
	PDWORD value
);

/**
* \brief Write port data
* \param port I/O port address
* \param value Value to write in the port
*/
RHMS_API_EXPOSED VOID WINAPI RHMS_WriteIoPortByte(
	WORD port,
	BYTE value
);

/**
* \brief Write port data
* \param port I/O port address
* \param value Value to write in the port
*/
RHMS_API_EXPOSED VOID WINAPI RHMS_WriteIoPortDword(
	WORD port,
	DWORD value
);

/**
* \brief Write port data
* \param port I/O port address
* \param value Value to write in the port
*/
RHMS_API_EXPOSED VOID WINAPI RHMS_WriteIoPortWord(
	WORD port,
	WORD value
);

/**
* \brief Extended write port data
* \param port I/O port address
* \param value Value to write in the port
* \return Success or fail
*/
BOOL WINAPI RHMS_WriteIoPortByteEx(
	WORD port,
	BYTE value
);

/**
* \brief Extended write port data
* \param port I/O port address
* \param value Value to write in the port
* \return Success or fail
*/
BOOL WINAPI RHMS_WriteIoPortWordEx(
	WORD port,
	WORD value
);

/**
* \brief Extended write port data
* \param port I/O port address
* \param value Value to write in the port
* \return Success or fail
*/
BOOL WINAPI RHMS_WriteIoPortDwordEx(
	WORD port,
	DWORD value
);

#endif
