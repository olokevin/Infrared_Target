/**
*  @file ext_procotol_module.h
*  @version 0.9
*  @date 2017-11-13
*  @author sky.huang
*
*  @brief
*   此文件包含部门约定的：
*   2，命令码
*   3，命令上下行数据格式及长度
*
*  @attention
*   命令索引：EXT_CMD##_IDX是命令唯一的索引号，从0开始连续增长
*            顺序无关紧要，保证唯一性就行
*
*  @copyright 2017 RoboMaster. All right reserved.
*
*/

#ifndef __PROTOCOL_EXT_CMD_H__
#define __PROTOCOL_EXT_CMD_H__

#include "stdint.h"
#include "protocol_ext.h"

/*******************************************   协议命令id定义   ***********************************************/
/* 新增命令步骤：
    加入要加入CMD命令
    1，在 extCmdIndex_t 枚举类型中加入CMD_IDX项
    2，在此文件中define EXT_CMD,  EXT_CMD_LEN(如果命令可能带不同的数据结构，则按最长的计算), 请求和响应的数据结构
 */

/* 给命令分配连续唯一编号，从小到大排序，作为推送索引 */
/* NOTE: 为节省空间，主控只需发送、无需接收的命令可以注释掉 */
typedef enum
{
    EXT_CMD_SUPPLY_REQ_IDX,
    EXT_CMD_CAMMUNICATE_IDX,
    /* 统计命令数量 */
    EXT_CMD_RECEIVE_MAX_NUM,
} extCmdIndex_t;

/* 实时功率数据 20ms周期发送 */
#define EXT_CMD_POWER_HEAT_DATA 0x0202u
#define EXT_CMD_POWER_HEAT_DATA_LEN (EXT_HEADER_CRC_CMDID_LEN + sizeof(extPowerHeatData_t))
typedef __packed struct
{
    uint16_t chassis_volt;         /* 底盘输出电压 */
    uint16_t chassis_current;      /* 底盘输出电流 */
    float chassis_power;           /* 底盘输出功率 */
    uint16_t chassis_power_buffer; /* 底盘功率缓冲 */
} chassis_power_info_t;
typedef __packed struct
{
    uint16_t shooter_heat0; /* 小弹丸热量 */
    uint16_t shooter_heat1; /* 大弹丸热量 */
} shooter_heat_info_t;
typedef __packed struct
{
    chassis_power_info_t chassis_power_info;
    shooter_heat_info_t shooter_heat_info;
} ext_power_heat_info_t;

/* 请求服务器进行补给命令 */
#define EXT_CMD_SUPPLY_REQ 0x0103u
#define EXT_CMD_SUPPLY_REQ_PERIOD_MS 100

/* 交互数据命令 */
#define EXT_CMD_COMMUNICATE 0x0301u
#define EXT_CMD_COMMUNICATE_BANDWITH 1280

#endif // __PROTOCOL_EXT_CMD_H__
