#include "Algorithm.h"
#include "usb_device.h"
#include "usbd_cdc.h"
#include "usb_device.h"
#include "usbd_cdc_if.h"
#include "crc.h"
#include "protocol_interface_ext.h"
/* Private variables ---------------------------------------------------------*/
float Subtraction_Base[MAXLENG];
float Multiplication_Base[MAXLENG];

uint8_t USB_UART_Buff[8];
extern uint8_t Tigger_X;
extern uint8_t Tigger_Y;
extern volatile uint8_t System_Flag_X; //System State
extern volatile uint8_t System_Flag_Y; //System State
uint8_t Error_Index = 0;
/* Private functions ---------------------------------------------------------*/

//function: Initial some math matrix or vector
//in :
//out:
void MathBase_initialization(void)
{
	uint8_t i;

	for (i = 0; i < XLENGTH; i++)
	{
		Subtraction_Base[i] = 1000.0f;
		Multiplication_Base[i] = (float)i;
	}
}

/* Private functions ---------------------------------------------------------*/
//function: power on self test, Minimum allowable ADC Value is 50 / 255
//in : ADCBuff to be cheak
//out: 0 means ADCBuff is correct and the other Numbers indicate that an error has occurred
uint8_t ADC_cheak_error(int16_t *ADCBuff, uint8_t length)
{
	int16_t result;
	uint32_t Index;
	arm_min_q15(ADCBuff, length, &result, &Index);
	if (result <= 50)
	{
		return (uint8_t)(++Index);
	}
	return 0;
}

/* Private functions ---------------------------------------------------------*/
//function: Handle ADC if it fails when ADC_cheak_error
//in : Index: The sequence number of the failed light set
//out:
void ADC_process_error(volatile uint8_t *Flag, uint8_t Index)
{
	uint8_t i, n, axis;
	if (Flag == &System_Flag_X)
	{
		axis = 1;
	}
	else if (Flag == &System_Flag_Y)
	{
		axis = 2;
	}
	else
		axis = 0;
	for (i = 0; i < axis; i++)
	{
		HAL_GPIO_WritePin(LED_RED_GPIO_Port, LED_RED_Pin, GPIO_PIN_RESET);
		HAL_Delay(200);
		HAL_GPIO_WritePin(LED_RED_GPIO_Port, LED_RED_Pin, GPIO_PIN_SET);
		HAL_Delay(200);
	}

	HAL_Delay(1000);

	n = ((uint8_t)(Index - 1) / 8) + 1;
	for (i = 0; i < n; i++)
	{
		HAL_GPIO_WritePin(LED_RED_GPIO_Port, LED_RED_Pin, GPIO_PIN_RESET);
		HAL_Delay(100);
		HAL_GPIO_WritePin(LED_RED_GPIO_Port, LED_RED_Pin, GPIO_PIN_SET);
		HAL_Delay(100);
	}

	HAL_Delay(1000);
	n = ((Index - 1) % 8) + 1;
	for (i = 0; i < n; i++)
	{
		HAL_GPIO_WritePin(LED_RED_GPIO_Port, LED_RED_Pin, GPIO_PIN_RESET);
		HAL_Delay(50);
		HAL_GPIO_WritePin(LED_RED_GPIO_Port, LED_RED_Pin, GPIO_PIN_SET);
		HAL_Delay(50);
	}
	HAL_Delay(2000);
}

void ADC_Check_IR_Light(volatile uint8_t *Flag, int16_t (*ADCbuff)[MAXLENG], float *BaseGroup, uint8_t length)
{
	int16_t Original_data[length];
	float DataBuff[length];
Init:
	memset(BaseGroup, 0, length * 4); //clear all data
	do
	{
		*Flag = 0;
	} while (*Flag != 0); //clear flag

	while (1)
	{
		while (*Flag == 0)
			; //wait for ADC finish
		if (*Flag == 1)
		{
			memcpy(Original_data, *ADCbuff, length * 2);
		}
		else
		{
			memcpy(Original_data, *(ADCbuff + 1), length * 2);
		}

		//Error_Index = ADC_cheak_error(Original_data,length);   //骚诨謫远袞蠆廷矛堋薷色窄铣矛謫邖蕘烁
		if (Error_Index != 0)
		{
			ADC_process_error(Flag, Error_Index);
			goto Init;
		}

		arm_q15_to_float(Original_data, DataBuff, length);
		arm_add_f32(BaseGroup, DataBuff, BaseGroup, length);
		do
		{
			*Flag = 0;
		} while (*Flag != 0);
	}
	// for(i = 0;i < length;i++)
	// {
	// 	BaseGroup[i] = 100000.0f / ( BaseGroup[i] * 32768.0f );
	// }
	// do
	// {
	// 	*Flag = 10;
	// }while(*Flag != 10);
}

/* Private functions ---------------------------------------------------------*/
//function: calculating ADC value without shielding
//in : Flag:      Point to state Flag
//     ADCBuff:   point to ADCBuff Dyadic Array(or matrix)
//out: BaseGroup: Point to Ouput
//检测并且校准红外，将放大系数算出，并且可以用系数将所有灯进行放大到同等大小
void ADC_Base_Calculation(volatile uint8_t *Flag, int16_t (*ADCbuff)[MAXLENG], float *BaseGroup, uint8_t length)
{
	uint16_t j, i;
	int16_t Original_data[length];
	float DataBuff[length];
Init:
	memset(BaseGroup, 0, length * 4); //clear all data
	do
	{
		*Flag = 0;
	} while (*Flag != 0); //clear flag

	for (j = 0; j < 100; j++)
	{
		while (*Flag == 0)
			; //wait for ADC finish
		if (*Flag == 1)
		{
			memcpy(Original_data, *ADCbuff, length * 2);
		}
		else
		{
			memcpy(Original_data, *(ADCbuff + 1), length * 2);
		}
		//Error_Index = ADC_cheak_error(Original_data,length);   //如果灯出现问题，会进入错误，灯就闪烁
		if (Error_Index != 0)
		{
			ADC_process_error(Flag, Error_Index);
			goto Init;
		}

		arm_q15_to_float(Original_data, DataBuff, length);
		arm_add_f32(BaseGroup, DataBuff, BaseGroup, length);
		do
		{
			*Flag = 0;
		} while (*Flag != 0);
	}
	for (i = 0; i < length; i++)
	{
		BaseGroup[i] = 100000.0f / (BaseGroup[i] * 32768.0f);
	}
	do
	{
		*Flag = 10;
	} while (*Flag != 10);
}

/* Private functions ---------------------------------------------------------*/
//function: Calculate if whether there is a bullet strike
//in : Flag:      Point to state Flag
//     ADCBuff:   point to ADCBuff Dyadic Array(or matrix)
//     BaseGroup: Point to ADC_Base
//     src:       0 means ADC DMA is harf while 1  means complete
//out: result:    a big matrix to restore the degree of occlusion(0 - 100)
//Spend time :     < 60us
//Spare bandwidth: > 120us

void ADC_Touch_Calculation(volatile uint8_t *Flag, int16_t *ADCBuff, uint8_t src, float (*result)[MAXLENG], float *BaseGroup, uint8_t *Tigger, uint8_t length)
{
	float Result;
	uint32_t Index;
	float DataBuff[length];
	if (*Flag < 10)
		*Flag = src; // 初始化模式  初始化成功再进入后面的模式
	else
	{
		if (*Tigger == 0)
		{
			arm_q15_to_float(ADCBuff, DataBuff, length);
			arm_scale_f32(DataBuff, 32768.0f, DataBuff, length);
			arm_mult_f32(DataBuff, BaseGroup, DataBuff, length);					   //乘以校准系数
			arm_sub_f32(Subtraction_Base, DataBuff, *(result + MAX_ScanTime), length); //反过来处理，值越大，说明遮挡越严重
			arm_max_f32(*(result + MAX_ScanTime), length, &Result, &Index);			   //找出数值最大，判断是否存在遮挡
			if (Result > 400.0f)
			{
				if (*Flag < MAX_ScanTime + 10) //
				{
					memcpy(*(result + *Flag - 10), *(result + MAX_ScanTime), length * 4); //把打靶的数据拷贝进入对应位置的数组里面
					(*Flag)++;
				}
				else
				{
					*Tigger = *Flag - 10;
				}
			}

			if (*Flag >= 13 && Result <= 400.0f) //因为打击一次，实际上已经是扫描了三次
			{
				*Tigger = *Flag - 10;
			}
		}
	}
}

/* Private functions ---------------------------------------------------------*/
//function: Calculate sum of pSrc vector
//in : pSrc:      input vector
//     blockSize: number of vector
//out: pResult:   point to sum

void LLZ_Sum_f32(
	float32_t *pSrc,
	uint32_t blockSize,
	float32_t *pResult)
{
	float32_t sum = 0.0f; /* Temporary result storage */
	uint32_t blkCnt;	  /* loop counter */
	float32_t in1, in2, in3, in4;

	blkCnt = blockSize >> 2u;
	while (blkCnt > 0u)
	{
		/* C = (A[0] + A[1] + A[2] + ... + A[blockSize-1]) */
		in1 = *pSrc++;
		in2 = *pSrc++;
		in3 = *pSrc++;
		in4 = *pSrc++;

		sum += in1;
		sum += in2;
		sum += in3;
		sum += in4;

		/* Decrement the loop counter */
		blkCnt--;
	}

	blkCnt = blockSize % 0x4u;
	while (blkCnt > 0u)
	{
		/* C = (A[0] + A[1] + A[2] + ... + A[blockSize-1]) */
		sum += *pSrc++;

		/* Decrement the loop counter */
		blkCnt--;
	}

	/* C = (A[0] + A[1] + A[2] + ... + A[blockSize-1]) / blockSize  */
	/* Store the result to the destination */
	*pResult = sum;
}

/* Private functions ---------------------------------------------------------*/
//function: When a bullet strikes, calculate the strike coordinates
//in: result:    a big matrix to restore the degree of occlusion
//Spend time :     < 2.2ms
//Spare bandwidth: > 40ms
//	float X_Coordinate;
//  float Y_Coordinate;
void ADC_Axis_calculation(volatile uint8_t *Flag, float (*result)[MAXLENG], float *Coordinate, uint8_t *Tigger, uint8_t length)
{
	uint16_t i, j;
	float Result;
	uint32_t Index;
	float Axis_result[MAX_ScanTime]; //results of coordinate
	float Sum;
	float Multiply_Add;

	if (*Tigger == 0)
	{
	}
	else
	{
		HAL_GPIO_WritePin(LED_GREEN_GPIO_Port, LED_GREEN_Pin, GPIO_PIN_RESET);
		for (i = 1; i < *Tigger - 1; i++)
		{
			arm_max_f32(*(result + i), length, &Result, &Index); //找出打击的点，就是数值最大的那个
			for (j = 0; j < length; j++)
			{
				if ((*(*(result + i) + j) < 20.0f) || (abs((int)j - (int)Index) >= 7))
				{
					*(*(result + i) + j) = 0;
				}
			} //舍掉一些干扰点
			arm_dot_prod_f32(*(result + i), Multiplication_Base, length, &Multiply_Add);
			LLZ_Sum_f32(*(result + i), length, &Sum);
			Axis_result[i] = Multiply_Add / Sum; //以上是
		}
		arm_mean_f32(Axis_result + 1, *Tigger - 2, Axis_result);

		*Coordinate = Axis_result[0] * 20.0f;
	}
}

void Coordinate_Send(volatile uint8_t *Flag1, volatile uint8_t *Flag2, const float Coordinate1, const float Coordinate2, uint8_t *Tigger1, uint8_t *Tigger2)
{

	// USB_UART_Buff[0] = 0XA5;
	// USB_UART_Buff[1] = 0X01;
	// *(uint16_t *)(&USB_UART_Buff[2]) = (uint16_t)Coordinate1;
	// *(uint16_t *)(&USB_UART_Buff[4]) = (uint16_t)Coordinate2;
	// USB_UART_Buff[6] = 0x01;
	// // USB_UART_Buff[7] =(uint8_t) HAL_CRC_Accumulate(&hcrc, (uint32_t *)USB_UART_Buff, 7);
	// CDC_Transmit_FS(USB_UART_Buff,6);

	// TODO: 修改命令码
	// uint16_t cmdid = 0x0101;
	// module_t module = {0};
	// module.type = MODULE_TYPE_SERVER;
	// module.id = MODULE_ID_0;
	coordinate_data_t coor;
	coor.x = (uint16_t)Coordinate1;
	coor.y = (uint16_t)Coordinate2;
	coor.type = 1;
	protocol_ext_send_data(CMD_POS_DATA_PUSH, (uint8_t *)&coor, sizeof(coor));

	do
	{
		*Flag1 = 10;
		*Flag2 = 10;
	} while ((*Flag1 != 10) && (*Flag2 != 10));
	do
	{
		*Tigger1 = 0;
		*Tigger2 = 0;
	} while ((*Tigger1 != 0) && (*Tigger2 != 0));

	HAL_GPIO_WritePin(LED_GREEN_GPIO_Port, LED_GREEN_Pin, GPIO_PIN_SET);
}
