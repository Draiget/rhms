#ifndef ZONTWELG_MODULE_TEMPERATURE_CPU_H_
#define ZONTWELG_MODULE_TEMPERATURE_CPU_H_

#include "BaseTemperatureModule.h"

namespace rhms
{
	class CpuTemperatureModule : public BaseTemperatureModule
	{
	public:
		explicit CpuTemperatureModule(const char* module_name)
			: BaseTemperatureModule(module_name) {}
	};
}

#endif
