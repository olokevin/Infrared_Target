
#include "protocol_ext.h"
#include "protocol_ext_cmd.h"
#include "protocol_ext_crc.h"
#include "protocol_interface_ext.h"

static ext_send_func_t ext_send_func;

static void ext_protocol_pack(uint8_t *p_out, uint8_t *p_in, uint16_t len, uint16_t cmd_id)
{
    uint16_t head_size = EXT_PROTOCOL_HEADER_SIZE;
    uint16_t frame_size = len + EXT_HEADER_CRC_CMDID_LEN;

    memcpy(p_out + head_size, &cmd_id, EXT_PROTOCOL_CMD_SIZE);
    ext_append_crc8(p_out, head_size);
    memcpy(p_out + head_size + EXT_PROTOCOL_CMD_SIZE, p_in, len);
    ext_append_crc16(p_out, frame_size);
}

/**
*@func:     ext_protocol_transmit
*@brief:    pc send protocol to main control
*@param:    cmd_id: the protocol cmd id
*           pdata: the point about data buffer
*           len: the length of the data buffer
*/
void ext_protocol_transmit(uint16_t cmd_id, void *p_buf, uint16_t len)
{
    static uint8_t extSeqNum;
    uint16_t frameSize = len + EXT_HEADER_CRC_CMDID_LEN;
    uint8_t tx_buf[EXT_PROTOCOL_FRAME_MAX_SIZE];
    ext_protocol_header_t *pHeader = (ext_protocol_header_t *)tx_buf;

    pHeader->sof = EXT_PROTOCOL_HEADER;
    pHeader->length = len;
    pHeader->seq_num = extSeqNum++;

    ext_protocol_pack(tx_buf, p_buf, len, cmd_id);
    ext_send_func(tx_buf, frameSize);
}

void ext_send_handle_register(ext_send_func_t ext_send_fn)
{
    ext_send_func = ext_send_fn;
}

void ext_protocol_unpackobj_init(ext_unpack_obj_t *p_obj, fifo_s_t *fifo)
{
    p_obj->p_fifo = fifo;
    p_obj->index = 0;
    p_obj->unpack_step = 0;
    p_obj->data_len = 0;
    p_obj->p_head = (ext_protocol_header_t *)p_obj->protocol_packet;
}

/*
*@func:ext_protocol_unpack
*@brief:  unpack protocol package
*@param:
*@ret :1  crc8 or cr16 check failed
*      0  unpack successfed or receiving data is not finished.
*/
uint8_t ext_protocol_unpack(ext_unpack_obj_t *p_obj)
{
    uint8_t byte = 0;
    uint8_t head_len = EXT_PROTOCOL_HEADER_SIZE;
    uint8_t unpack_result = 0;

    while (fifo_s_used(p_obj->p_fifo))
    {
        byte = fifo_s_get(p_obj->p_fifo);
        switch (p_obj->unpack_step)
        {
        case 0: // sof
            if (byte == EXT_PROTOCOL_HEADER)
            {
                p_obj->unpack_step = 1;
                p_obj->protocol_packet[p_obj->index++] = byte;
            }
            else
            {
                p_obj->index = 0;
            }
            break;

        case 1:
            p_obj->data_len = byte;
            p_obj->protocol_packet[p_obj->index++] = byte;
            p_obj->unpack_step = 2;
            break;

        case 2: // data_len high
            p_obj->data_len |= (byte << 8);
            p_obj->protocol_packet[p_obj->index++] = byte;

            if (p_obj->data_len < (EXT_PROTOCOL_FRAME_MAX_SIZE - EXT_HEADER_CRC_LEN))
            {
                p_obj->unpack_step = 3;
            }
            else
            {
                p_obj->unpack_step = 0;
                p_obj->index = 0;
            }
            break;

        case 3:
            p_obj->protocol_packet[p_obj->index++] = byte;

            if (p_obj->index == head_len)
            {
                if (ext_verify_crc8(p_obj->protocol_packet, head_len))
                {
                    p_obj->unpack_step = 4;
                }
                else
                {
                    unpack_result = 1;
                    p_obj->unpack_step = 0;
                    p_obj->index = 0;
                }
            }
            break;

        case 4:
            if (p_obj->index < (p_obj->data_len + EXT_HEADER_CRC_CMDID_LEN))
            {
                p_obj->protocol_packet[p_obj->index++] = byte;
            }
            if (p_obj->index >= (p_obj->data_len + EXT_HEADER_CRC_CMDID_LEN))
            {
                p_obj->index = 0;
                p_obj->unpack_step = 0;

                if (ext_verify_crc16(p_obj->protocol_packet, p_obj->data_len + EXT_HEADER_CRC_CMDID_LEN))
                {
                    ext_module_cmd_handle(p_obj->protocol_packet);
                }
                else
                {
                    unpack_result = 1;
                }
            }
            break;

        default:
            p_obj->unpack_step = 0;
            p_obj->index = 0;
            break;
        }
    }

    return unpack_result;
}

/************************ (C) COPYRIGHT 2017 DJI RoboMaster *****END OF FILE****/
