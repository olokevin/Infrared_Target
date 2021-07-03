#include "sys_config.h"
#include "sys_param.h"
#include "sys_param.h"
// #include "md5.h"
#include <string.h>

uint8_t cpu_id_buff[DEVICE_ID_SIZE + 1];
uint8_t loader_param_buff[sizeof(bootloader_param_t) + 1];
uint8_t app_param_buff[sizeof(app_param_t) + 1];

static void cpuid_init(void)
{
	memcpy(cpu_id_buff, (uint8_t *)FLASH_ADDR_CPUID, DEVICE_ID_SIZE);
}

uint8_t * get_cpuid(void)
{
    return cpu_id_buff;
}

static void loader_param_init(void)
{

}

uint32_t read_loader_param(void)
{
    return (uint32_t)loader_param_buff;
}

static void app_param_init(void)
{
	((bootloader_param_t *)loader_param_buff)->app_param_start_addr = (uint32_t)app_param_buff;
}

void user_init(void)
{
	cpuid_init();
	loader_param_init();
	app_param_init();
	
}

void user_calcu_md5(uint8_t * calcu_md5_value, uint32_t image_addr, uint32_t image_size)
{
	// MD5_CTX md5_handle;
	
    // MD5Init(&md5_handle);
    // MD5Update(&md5_handle, (unsigned char *)image_addr, image_size);
    // MD5Final(&md5_handle, calcu_md5_value);
}

void upgrade_status_callback(uint8_t upgrade_status, uint8_t *buf, uint32_t len)
{

}

