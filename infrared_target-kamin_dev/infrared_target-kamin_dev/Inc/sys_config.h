
#ifndef __SYSCONFIG_H__
#define __SYSCONFIG_H__

#include <stdarg.h>
#include "bsp_flash.h"
// #include "protocol.h"

/*
*********************************************************************************************************
*                                        �û�������
*********************************************************************************************************
*/

#define APP_VERSION_ID0                     (5)
#define APP_VERSION_ID1                     (0)
#define APP_VERSION_ID2                     (0)
#define APP_VERSION_ID3                     (10)
#define APP_VERSION                         ((APP_VERSION_ID0 << 24) | (APP_VERSION_ID1 << 16) | (APP_VERSION_ID2<<8) | APP_VERSION_ID3)

/* BootLoader�汾�Ŷ��� */
#define LOADER_VERSION_ID0                  (5)
#define LOADER_VERSION_ID1                  (0)
#define LOADER_VERSION_ID2                  (0)
#define LOADER_VERSION_ID3                  (10)
#define BOOTLOADER_VERSION                  ((LOADER_VERSION_ID0 << 24) | (LOADER_VERSION_ID1 << 16) | (LOADER_VERSION_ID2<<8) | LOADER_VERSION_ID3)

#define LOADER_PAGE_SIZE                    (0x0F)
#define LOADER_PARAM_START_ADDR             (FLASH_ADDR_START + FLASH_PAGE_SIZE * (LOADER_PAGE_SIZE - 1))/* bootloader������ʼ��ַ��ҳ�׵�ַ�� */
#define FLASH_SAFE_START_ADDR               (FLASH_ADDR_START + FLASH_PAGE_SIZE * LOADER_PAGE_SIZE)/* bootloader����ֹ��ַ��ҳ�׵�ַ�� */
#define FLASH_ADDR_CPUID              	    (0x1FFFF7AC)/* оƬΨһID��ַ */

#define MODULE_TYPE                         (MODULE_TYPE_IR_TARGET)
#define MODULE_SUB_TYPE                     (MODULE_SUBTYPE_DEFAULT)
#define TARGET_MCU                          (0x07)
#define TARGET_PERIPH_VER                   (0x00)
#define HARDWARE_CODE                       ((MODULE_TYPE << 24) | (MODULE_SUB_TYPE << 16) | (TARGET_MCU<<8) | TARGET_PERIPH_VER)

#define SEND_MODULE_CANID                   (CANID_SLAVE_UWB + MODULE_ID)
#define RECEIVE_MODULE_CANID                (CANID_MASTER_UWB + MODULE_ID)

#define MUTEX_DECLARE(mutex)                
#define MUTEX_INIT(mutex)                   do{/* nothing */}while(0)
#define MUTEX_LOCK(mutex)                   do{__disable_irq();}while(0)
#define MUTEX_UNLOCK(mutex)                 do{__enable_irq();}while(0)

#define PROTOCOL_BIGENDIAN_HOST 0
#if (PROTOCOL_BIGENDIAN_HOST == 1) /* for big endian machine */ 

#define HOST_ENDIAN16(x)    ((((x) >> 8) & 0xff) | (((x) & 0xff) << 8))
#define HOST_ENDIAN32(x)    (((x) >> 24) | (((x) & 0x00ff0000) >> 8) | \
                             (((x) & 0x0000ff00) << 8) | ((x) << 24))
#define HOST_ENDIAN64(x)    (((x) >> 56) | (((x) & 0x00ff000000000000) >> 40) |  \
                             (((x) & 0x0000ff0000000000) >> 24) |  \
                             (((x) & 0x000000ff00000000) >> 8)  |  \
                             (((x) & 0x00000000ff000000) << 8)  |  \
                             (((x) & 0x0000000000ff0000) << 24) |  \
                             (((x) & 0x000000000000ff00) << 40) | (((x) << 56)))

#else   /* for little endian machine */

#define HOST_ENDIAN16(x)    (x)
#define HOST_ENDIAN32(x)    (x)
#define HOST_ENDIAN64(x)    (x)

#endif

#define SOFTTIMER_TICK 1 //1ms
#define BOOT_DELAY            (2000/SOFTTIMER_TICK)
#define UPGRADE_WAIT_TIME     (5000/SOFTTIMER_TICK)       //the wait time of boot ms
#define UPGRADE_SUCCESS_DELAY (100/SOFTTIMER_TICK)        //wait end of transmit end cmd
#define LED_FLICK_CYCLE     (100/SOFTTIMER_TICK)

#define SET_STACK(addr)                              \
do                                               \
{                                                \
    __set_MSP(*(__IO uint32_t*) addr); \
}while(0)


#define APP_RUN(addr)                               \
do                                               \
{                                                \
    uint32_t ResetFun = *(__IO uint32_t*) (addr + 4);   \
    (*(void(*)(void))(ResetFun))();              \
}while(0)

void user_init(void);
uint8_t * get_cpuid(void);
uint32_t read_loader_param(void);
void user_calcu_md5(uint8_t * calcu_md5_value, uint32_t image_addr, uint32_t image_size);
void upgrade_status_callback(uint8_t upgrade_status, uint8_t *buf, uint32_t len);

#endif
