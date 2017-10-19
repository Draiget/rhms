#ifndef ZONTWELG_MODULE_TEMPERATURE_BASE_STRUCTS_H
#define ZONTWELG_MODULE_TEMPERATURE_BASE_STRUCTS_H
#include <cstring>

namespace rhms
{
	enum
	{
		RHMS_TEMP_MODULE_OK = 1,
	};

	struct ApiModuleState
	{
		int error_code;
		char error_string[1024];

		ApiModuleState(int err_code = RHMS_TEMP_MODULE_OK, const char* err_str = nullptr) 
			: error_code(err_code)
		{
			if (err_str != nullptr) {
				strcpy_s(error_string, err_str);
				return;
			}

			*error_string = '\0';
		}
	};
}

#endif
