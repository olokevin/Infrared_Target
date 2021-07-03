#include "sys_param.h"
#include "string.h"

app_param_t app_param;
bootloader_param_t bootloader_param;
MUTEX_DECLARE(param_mutex)


app_param_t* get_app_param(void)
{
    return &app_param;
}

bootloader_param_t* get_loader_param(void)
{
    return &bootloader_param;
}


// 0:成功 1:失败
uint8_t sys_param_save(void)
{
    uint8_t     retval        = 1;
    uint8_t     writeCount    = 0;
    uint8_t     loader_param_len = 0;
    bootloader_param_t tmp_param;
    
    loader_param_len = sizeof(bootloader_param_t);
    
    while (retval != 0)
    {
        MUTEX_LOCK(param_mutex);
        if(!bsp_flash_erase(LOADER_PARAM_START_ADDR, loader_param_len))
        {
            retval = bsp_flash_write_words(LOADER_PARAM_START_ADDR, (uint32_t *)&bootloader_param, (sizeof(bootloader_param_t) + 3) / 4);
        }
        MUTEX_UNLOCK(param_mutex);
        
        memset(&tmp_param, 0, loader_param_len);
        memcpy(&tmp_param, (void *)LOADER_PARAM_START_ADDR, loader_param_len);
        if((memcmp(&tmp_param, &bootloader_param, loader_param_len) != 0) && (writeCount < 3))
        {
            retval = 1;
            writeCount++;
        }
    }

    return retval;
}



uint8_t sys_param_init(void)
{

    int8_t ret = 0;
    uint8_t loader_param_save_flag = 0;
    
    MUTEX_INIT(param_mutex);

    memcpy((uint8_t *)&bootloader_param, (uint8_t *)LOADER_PARAM_START_ADDR, sizeof(bootloader_param_t));
    if(bootloader_param.loader_version != BOOTLOADER_VERSION)        
    {
        bootloader_param.loader_version = BOOTLOADER_VERSION;

        bootloader_param.app_size = 0;
        bootloader_param.app_startup_address  = FLASH_SAFE_START_ADDR;
        bootloader_param.app_param_start_addr = FLASH_SAFE_START_ADDR;
        memset((void *)&bootloader_param.app_version, 0xff, sizeof(uint32_t));
        
        bootloader_param.module_type     = 0;
        bootloader_param.module_sub_type = 0;
        
        loader_param_save_flag = 1;
    }
    
    if((bootloader_param.app_param_start_addr > (FLASH_SAFE_START_ADDR)) &&
       (bootloader_param.app_param_start_addr < (FLASH_ADDR_END)))
    {
        memcpy(&app_param, (void *)bootloader_param.app_param_start_addr, sizeof(app_param_t));
    }
    else
    {
        memset(&app_param, 0x00, sizeof(app_param));
    }
    
    if(app_param.loader_version != BOOTLOADER_VERSION)
    {		
        app_param.module_id = 0;
    }
    
    if(loader_param_save_flag)
    {
        ret = sys_param_save();
    }
    
    return ret;
}

