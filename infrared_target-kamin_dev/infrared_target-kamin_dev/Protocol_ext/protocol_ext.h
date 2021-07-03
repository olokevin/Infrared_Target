

#ifndef __PROTOCOL_EXT_H__
#define __PROTOCOL_EXT_H__

#ifdef __cpluscplus
extern "C"
{
#endif

/* Includes ------------------------------------------------------------------*/
#include <stdint.h>
#include <stdlib.h>
#include <string.h>
#include "fifo.h"
#include "protocol_ext_crc.h"

/* Exported macro ------------------------------------------------------------*/

/**
  * @brief  protocol relate
  */
#define EXT_PROTOCOL_HEADER (0xA5)
#define EXT_PROTOCOL_HEADER_SIZE (sizeof(ext_protocol_header_t))
#define EXT_PROTOCOL_CMD_SIZE (sizeof(uint16_t))
#define EXT_PROTOCOL_CRC16_SIZE (sizeof(uint16_t))
#define EXT_HEADER_CRC_LEN (EXT_PROTOCOL_HEADER_SIZE + EXT_PROTOCOL_CRC16_SIZE)
#define EXT_HEADER_CMDID_LEN (EXT_PROTOCOL_HEADER_SIZE + EXT_PROTOCOL_CMD_SIZE)
#define EXT_HEADER_CRC_CMDID_LEN (EXT_PROTOCOL_HEADER_SIZE + EXT_PROTOCOL_CRC16_SIZE + EXT_PROTOCOL_CMD_SIZE)

#define EXT_PROTOCOL_FRAME_MAX_SIZE 128u

    typedef void (*ext_send_func_t)(uint8_t *buf, uint32_t len);

    typedef __packed struct
    {
        uint8_t sof;
        uint16_t length;
        uint8_t seq_num;
        uint8_t crc8;
    } ext_protocol_header_t;

    typedef __packed struct
    {
        ext_protocol_header_t header;
        uint16_t cmd_id;
        uint8_t data[];
    } ext_protocol_pack_t;

    typedef struct
    {
        fifo_s_t *p_fifo;
        ext_protocol_header_t *p_head;                        //指向协议头指针
        uint16_t data_len;                                    //协议数据部分长度
        uint8_t protocol_packet[EXT_PROTOCOL_FRAME_MAX_SIZE]; //存放完整的协议包
        uint8_t unpack_step;
        uint16_t index; //解包过程中，protocolPacket正在写下标
    } ext_unpack_obj_t;

    void ext_protocol_transmit(uint16_t cmd_id, void *p_buf, uint16_t len);
    void ext_send_handle_register(ext_send_func_t ext_send_fn);
    void ext_protocol_unpackobj_init(ext_unpack_obj_t *p_obj, fifo_s_t *fifo);
    uint8_t ext_protocol_unpack(ext_unpack_obj_t *p_obj);

#endif //__PROTOCOL_EXT_H__

    /************************ (C) COPYRIGHT 2017 DJI RoboMaster *****END OF FILE****/
