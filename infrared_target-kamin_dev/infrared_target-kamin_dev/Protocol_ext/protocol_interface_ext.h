#ifndef _PROTOCOL_INTERFACE_EXT_H_
#define _PROTOCOL_INTERFACE_EXT_H_

#include <stdint.h>

#include "protocol_ext_cmd.h"

#pragma pack(push, 1)

#define EXT_RX_BUF_SIZE (256)
#define EXT_ENTRY_MAX_NUM (16)

typedef void (*cmd_handle_func)(uint8_t *buf);
typedef struct
{
    uint16_t cmd_id;
    cmd_handle_func p_func;
} cmd_entry_t;

#define CMD_GET_VERSION_REQ 0x0080
typedef struct
{
    uint32_t loader_version;
    uint32_t app_version;
    uint8_t dev_id[12];
    uint32_t reserved;
} version_info_rep_t;
#define CMD_GET_VERSION_RSP CMD_GET_VERSION_REQ

typedef struct
{
    uint16_t x;
    uint16_t y;
    uint8_t type;
} coordinate_data_t;
#define CMD_POS_DATA_PUSH 0x0003

void protocol_ext_user_receive_data(uint8_t *p_data, uint32_t len);
void protocol_ext_send_data(uint16_t cmd_id, void *p_buf, uint16_t len);
void protocol_ext_interface_init(void);
void protocol_ext_interface_loop(void);
void ext_module_cmd_handle(uint8_t *p_buf);
void protocol_ext_cmd_reg(uint16_t cmdid, cmd_handle_func callback);

#pragma pack(pop)

#endif
