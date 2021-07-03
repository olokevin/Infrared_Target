/**
* @file     bsp_flash.c
* @brief    flash function implement file
* @author   sky
* @date     2015-10-23
* @version  1.0
* @license  Copyright(c) 2015 DJI Corporation Robomasters Department.
*/

/*
*********************************************************************************************************
*                                             INCLUDE FILES
*********************************************************************************************************
*/
#include <string.h>
#include "bsp_flash.h"

uint16_t bsp_flash_get_sector(uint32_t addr)
{
    if (addr < FLASH_ADDR_START || addr >= FLASH_ADDR_END)
    {
        return 0xffff;
    }

    uint32_t relative_addr = FLASH_ADDR_START;
    uint16_t sector_num = 0;

    relative_addr = addr - FLASH_ADDR_START;
    sector_num = relative_addr / FLASH_PAGE_SIZE;

    return sector_num;
}

//addr must be page start address!!!
int bsp_flash_erase(uint32_t addr, uint32_t len)
{
    HAL_StatusTypeDef status = HAL_ERROR;
    uint32_t page_errs;
    uint32_t addr_s = addr;
    uint32_t addr_e = addr_s + len;
    // uint32_t erase_pages = bsp_flash_get_sector(addr);

    HAL_FLASH_Unlock();

    for (uint32_t i = addr_s; i < addr_e; i += FLASH_PAGE_SIZE)
    {
        FLASH_EraseInitTypeDef EraseInitStruct;
        EraseInitStruct.TypeErase = FLASH_TYPEERASE_PAGES;
        EraseInitStruct.PageAddress = i;
        EraseInitStruct.NbPages = 1;
        status = HAL_FLASHEx_Erase(&EraseInitStruct, &page_errs);
        if (status != HAL_OK)
        {
            HAL_FLASH_Lock();
            return -1;
        }
    }

    HAL_FLASH_Lock();

    return 0;
}

// int bsp_flash_erase(uint32_t addr, uint32_t len)
// {
// 	FLASH_Status FLASHStatus = FLASH_COMPLETE;
//     uint32_t addr_s = addr;
//     uint32_t addr_e = addr_s + len;
//     HAL_FLASH_Unlock();
//     for(uint32_t i = addr_s; i < addr_e; i += FLASH_PAGE_SIZE)
//     {
//         FLASHStatus = FLASH_ErasePage(i);
//         if(FLASHStatus!=FLASH_COMPLETE)
//         {
//             HAL_FLASH_Lock();
//             return  -1;
//         }
//     }
//     HAL_FLASH_Lock();
//     return 0;
// }

int bsp_flash_write_bytes(uint32_t addr, void *p_data, uint32_t byte_num)
{
    return 0;
}

int bsp_flash_write_words(uint32_t addr, void *p_data, uint32_t word_num)
{
    // FLASH_Status FLASHStatus = FLASH_COMPLETE;
    HAL_StatusTypeDef status;
    uint32_t *p_temp = p_data;

    HAL_FLASH_Unlock();

    for (uint32_t i = 0; i < word_num; i++)
    {
        status = HAL_FLASH_Program(FLASH_TYPEPROGRAM_WORD, addr + i * 4, p_temp[i]);
        // FLASHStatus = FLASH_ProgramWord(addr + i*4, p_temp[i]);
        if (status != HAL_OK)
        {
            // Error
            HAL_FLASH_Lock();
            return 1;
        }
    }

    HAL_FLASH_Lock();

    return 0;
}

// uint32_t BSP_FLASH_ErasePage(uint32_t addr, uint32_t len)
// {
//     FLASH_Status FLASHStatus = FLASH_COMPLETE;
//     uint32_t addr_s = addr;
//     uint32_t addr_e = addr_s + len*FLASH_PAGE_SIZE;

//     FLASH_Unlock();

//     for(uint32_t i=addr_s; i < addr_e; i += FLASH_PAGE_SIZE)
//     {
//         FLASHStatus = FLASH_ErasePage(i);
//         if(FLASHStatus!=FLASH_COMPLETE)
//         {
//             FLASH_Lock();
//             return  1;
//         }
//     }

//     FLASH_Lock();

//     return 0;
// }

// void bsp_flash_clear_flag(void)
// {
// 	FLASH_ClearFlag(FLASH_FLAG_BSY|FLASH_FLAG_PGERR|FLASH_FLAG_WRPERR|FLASH_FLAG_EOP);
// }
