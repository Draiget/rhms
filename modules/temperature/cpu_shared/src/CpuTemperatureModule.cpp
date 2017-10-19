#include <cstdio>
#ifndef ZONTWELG_MODULE_TEMPERATURE_CPU_H
#define ZONTWELG_MODULE_TEMPERATURE_CPU_H

#define RHMS_TEMP_IMPORT
#include <BaseTemperatureModule.h>
#include "CpuTemperatureModule.h"
using namespace rhms;

extern BaseTemperatureModule* g_Module;

RHMS_API_EXPOSED bool module_open(uint32_t api_version, ApiModuleState &out_state) {
	if (api_version < 1){
		out_state = ApiModuleState(false, "Unsupported API version");
		return false;
	}

	g_Module = new CpuTemperatureModule("CPUID module");
	out_state = ApiModuleState();
	return true;
}

RHMS_API_EXPOSED bool module_close(ApiModuleState &out_state) {
	out_state = ApiModuleState();
	return true;
}

RHMS_API_EXPOSED bool module_get_instance(BaseTemperatureModule* out_module) {
	*out_module = *g_Module;
	return true;
}


#endif
