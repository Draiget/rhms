#ifndef RHMS_EXPORTS_API_H
#define RHMS_EXPORTS_API_H

#define RHMS_API_IMPORT extern "C" __declspec(dllimport)
#define RHMS_API_EXPORT extern "C" __declspec(dllexport)

#ifdef RHMS_TEMP_IMPORT
#define RHMS_API_EXPOSED RHMS_API_IMPORT
#pragma comment(lib, "driver_bridge.lib")
#else
#define RHMS_API_EXPOSED RHMS_API_EXPORT
#endif

#endif
