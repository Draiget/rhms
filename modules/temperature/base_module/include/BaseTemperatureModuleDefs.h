#ifndef ZONTWELG_MODULE_TEMPERATURE_BASE_DEFS_H
#define ZONTWELG_MODULE_TEMPERATURE_BASE_DEFS_H

namespace rhms
{

#define RHMS_API_EXPORT extern "C" __declspec(dllimport)

#ifdef RHMS_TEMP_IMPORT
#define RHMS_API_EXPOSED extern "C" __declspec(dllexport)
#pragma comment(lib, "base_temp_module.lib")
#else
#define RHMS_API_EXPOSED RHMS_API_EXPORT
#endif

}

#endif
