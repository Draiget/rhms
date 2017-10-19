#include "../include/BaseTemperatureModule.h"

rhms::BaseTemperatureModule* g_Module;

rhms::BaseTemperatureModule::BaseTemperatureModule(const char* module_name) {
	if (module_name == nullptr) {
		*m_ModuleName = '\0';
	} else {
		strcpy_s(m_ModuleName, module_name);
	}
}
