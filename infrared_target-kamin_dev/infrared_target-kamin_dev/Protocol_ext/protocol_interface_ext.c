#include "protocol_interface_ext.h"
#include "usbd_cdc_if.h"
#include "sys_param.h"

static uint8_t ext_rx_buffer[EXT_RX_BUF_SIZE];
static fifo_s_t ext_rx_fifo;
static ext_unpack_obj_t ext_unpack_obj;
static cmd_entry_t cmd_entry_array[EXT_ENTRY_MAX_NUM];

static void protocol_ext_cmd_init(void);

void protocol_ext_user_receive_data(uint8_t *p_data, uint32_t len)
{
    fifo_s_puts_noprotect(&ext_rx_fifo, (char *)p_data, len);
}

static void protocol_ext_send_func(uint8_t *buf, uint32_t len)
{
    CDC_Transmit_FS(buf, len);
}

void protocol_ext_send_data(uint16_t cmd_id, void *p_buf, uint16_t len)
{
    ext_protocol_transmit(cmd_id, p_buf, len);
}

void protocol_ext_interface_init(void)
{
    for (size_t i = 0; i < sizeof(cmd_entry_array); i++)
    {
        cmd_entry_array[i].cmd_id = 0;
        cmd_entry_array[i].p_func = NULL;
    }
    fifo_s_init(&ext_rx_fifo, ext_rx_buffer, EXT_RX_BUF_SIZE);
    ext_protocol_unpackobj_init(&ext_unpack_obj, &ext_rx_fifo);

    ext_send_handle_register(protocol_ext_send_func);
    protocol_ext_cmd_init();
}

void protocol_ext_interface_loop(void)
{
    ext_protocol_unpack(&ext_unpack_obj);
}

void ext_module_cmd_handle(uint8_t *p_buf)
{
    ext_protocol_pack_t *pack = (ext_protocol_pack_t *)p_buf;
    for (size_t i = 0; i < sizeof(cmd_entry_array); i++)
    {
        if (cmd_entry_array[i].cmd_id == pack->cmd_id && cmd_entry_array[i].p_func != NULL)
        {
            cmd_entry_array[i].p_func(p_buf);
        }
    }
}

void protocol_ext_cmd_reg(uint16_t cmdid, cmd_handle_func callback)
{
    for (size_t i = 0; i < sizeof(cmd_entry_array); i++)
    {
        if (cmd_entry_array[i].cmd_id == 0 && cmd_entry_array[i].p_func == NULL)
        {
            cmd_entry_array[i].cmd_id = cmdid;
            cmd_entry_array[i].p_func = callback;
            break;
        }
    }
}

static void get_ver_handle(uint8_t *buf)
{
    //TODO: 修改成对外协议
    //    ext_protocol_pack_t *pack = (ext_protocol_pack_t*)buf;
    version_info_rep_t ver_ack;

    memset(&ver_ack, 0, sizeof(version_info_rep_t));
    ver_ack.app_version = HOST_ENDIAN32(APP_VERSION);
    ver_ack.loader_version = HOST_ENDIAN32(BOOTLOADER_VERSION);
    /* 适应loader中cpuid的定义 */
    //memset(ver_ack.dev_id,0xFF,12);
    memcpy(ver_ack.dev_id, get_cpuid(), DEVICE_ID_SIZE);
    protocol_ext_send_data(CMD_GET_VERSION_RSP, &ver_ack, sizeof(ver_ack));
}

static void protocol_ext_cmd_init(void)
{
    protocol_ext_cmd_reg(CMD_GET_VERSION_REQ, get_ver_handle);
}
