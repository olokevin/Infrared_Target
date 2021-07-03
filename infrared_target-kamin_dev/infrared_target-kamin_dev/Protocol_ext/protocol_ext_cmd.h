/**
*  @file ext_procotol_module.h
*  @version 0.9
*  @date 2017-11-13
*  @author sky.huang
*
*  @brief
*   ���ļ���������Լ���ģ�
*   2��������
*   3���������������ݸ�ʽ������
*
*  @attention
*   ����������EXT_CMD##_IDX������Ψһ�������ţ���0��ʼ��������
*            ˳���޹ؽ�Ҫ����֤Ψһ�Ծ���
*
*  @copyright 2017 RoboMaster. All right reserved.
*
*/

#ifndef __PROTOCOL_EXT_CMD_H__
#define __PROTOCOL_EXT_CMD_H__

#include "stdint.h"
#include "protocol_ext.h"

/*******************************************   Э������id����   ***********************************************/
/* ��������裺
    ����Ҫ����CMD����
    1���� extCmdIndex_t ö�������м���CMD_IDX��
    2���ڴ��ļ���define EXT_CMD,  EXT_CMD_LEN(���������ܴ���ͬ�����ݽṹ������ļ���), �������Ӧ�����ݽṹ
 */

/* �������������Ψһ��ţ���С����������Ϊ�������� */
/* NOTE: Ϊ��ʡ�ռ䣬����ֻ�跢�͡�������յ��������ע�͵� */
typedef enum
{
    EXT_CMD_SUPPLY_REQ_IDX,
    EXT_CMD_CAMMUNICATE_IDX,
    /* ͳ���������� */
    EXT_CMD_RECEIVE_MAX_NUM,
} extCmdIndex_t;

/* ʵʱ�������� 20ms���ڷ��� */
#define EXT_CMD_POWER_HEAT_DATA 0x0202u
#define EXT_CMD_POWER_HEAT_DATA_LEN (EXT_HEADER_CRC_CMDID_LEN + sizeof(extPowerHeatData_t))
typedef __packed struct
{
    uint16_t chassis_volt;         /* ���������ѹ */
    uint16_t chassis_current;      /* ����������� */
    float chassis_power;           /* ����������� */
    uint16_t chassis_power_buffer; /* ���̹��ʻ��� */
} chassis_power_info_t;
typedef __packed struct
{
    uint16_t shooter_heat0; /* С�������� */
    uint16_t shooter_heat1; /* �������� */
} shooter_heat_info_t;
typedef __packed struct
{
    chassis_power_info_t chassis_power_info;
    shooter_heat_info_t shooter_heat_info;
} ext_power_heat_info_t;

/* ������������в������� */
#define EXT_CMD_SUPPLY_REQ 0x0103u
#define EXT_CMD_SUPPLY_REQ_PERIOD_MS 100

/* ������������ */
#define EXT_CMD_COMMUNICATE 0x0301u
#define EXT_CMD_COMMUNICATE_BANDWITH 1280

#endif // __PROTOCOL_EXT_CMD_H__
