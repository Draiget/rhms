#ifndef ZONTWELG_MODULE_TEMPERATURE_BASE_H
#define ZONTWELG_MODULE_TEMPERATURE_BASE_H

#include <cstdint>
#include "BaseTemperatureModuleDefs.h"
#include "BaseTemperatureModuleStructs.h"

namespace rhms
{
	RHMS_API_EXPOSED class BaseTemperatureModule
	{
	public:
		BaseTemperatureModule(const char* module_name);

		char m_ModuleName[1024];
	};

	RHMS_API_EXPOSED bool module_open(uint32_t api_version, ApiModuleState& out_state);
	RHMS_API_EXPOSED bool module_close(ApiModuleState& out_state);
	RHMS_API_EXPOSED bool module_get_instance(BaseTemperatureModule* out_module);

}

#endif
