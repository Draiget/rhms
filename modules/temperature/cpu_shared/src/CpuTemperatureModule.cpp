#include <cstdio>
#ifndef ZONTWELG_MODULE_TEMPERATURE_CPU_H
#define ZONTWELG_MODULE_TEMPERATURE_CPU_H

#define RHMS_TEMP_IMPORT
#include <BaseTemperatureModule.h>
#include "CpuTemperatureModule.h"
using namespace rhms;

RHMS_API_EXPOSED bool module_open(uint32_t api_version, ApiModuleState &out_state, BaseTemperatureModule* out_module) {
	if (api_version < 1){
		out_state = ApiModuleState(false, "Unsupported API version");
		out_module = nullptr;
		return false;
	}

	*out_module = CpuTemperatureModule("CPUID module");
	out_state = ApiModuleState(true);
	return true;
}

RHMS_API_EXPOSED bool module_close(ApiModuleState &out_state) {
	out_state = ApiModuleState();
	return true;
}


#endif
