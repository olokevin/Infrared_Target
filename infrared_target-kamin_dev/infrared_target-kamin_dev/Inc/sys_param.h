
#ifndef __SYSPARAM_H__
#define __SYSPARAM_H__

#include <stdarg.h>
#include "sys_config.h"
#include "bsp_flash.h"



#define DEVICE_ID_SIZE                      (12)
#define MD5_SIZE                            (16)
#define MODULE_ID                           (get_app_param()->module_id)

typedef enum
{
	UPGRADE_SUCCEED,
	UPGRADE_FAILED,
	UPGRADE_NEVER    = 0XFF,
} firmware_status_e;

#pragma pack(push, 4)
typedef struct 
{        
    uint32_t  loader_version;
    uint32_t  app_version;
    uint8_t   module_id;		
} app_param_t;

typedef struct
{
    uint32_t  loader_version;
    uint32_t  app_version;
	uint8_t   module_type;
	uint8_t   module_sub_type;
	uint8_t   app_status;			/*!< firmware_status_e */
	uint32_t  app_size;
	uint32_t  app_startup_address;
	uint32_t  app_param_start_addr;
	uint8_t   app_md5[MD5_SIZE];
} bootloader_param_t;
#pragma pack(pop)

uint8_t sys_param_init(void);
uint8_t sys_param_save(void);
bootloader_param_t* get_loader_param(void);
app_param_t* get_app_param(void);

#endif
