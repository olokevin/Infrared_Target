/**
* @file     ExternalProtocolOut_Crc.c
* @brief    crc checkout about student's datas.
* @author   damom.li
* @date     2015-11-16
* @version  1.0.0.0
* @license  Copyright(c) 2015 DJI Corporation Robomasters Department.
*/
#ifndef __PROTOCOL_EXT_CRC_H__
#define __PROTOCOL_EXT_CRC_H__

#include "stdint.h"
#include "stdio.h"

uint8_t ext_get_crc8(uint8_t *p_msg, uint32_t len, uint8_t crc8);
uint32_t ext_verify_crc8(uint8_t *p_msg, uint32_t len);
void ext_append_crc8(uint8_t *p_msg, uint32_t len);

uint16_t ext_get_crc16(uint8_t *p_msg, uint16_t len, uint16_t crc16);
uint32_t ext_verify_crc16(uint8_t *p_msg, uint16_t len);
void ext_append_crc16(uint8_t *p_msg, uint32_t len);

#endif //__PROTOCOL_EXT_CRC_H__
