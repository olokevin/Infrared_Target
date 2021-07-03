/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.c
  * @brief          : Main program body
  ******************************************************************************
  * @attention
  *
  * <h2><center>&copy; Copyright (c) 2019 STMicroelectronics.
  * All rights reserved.</center></h2>
  *
  * This software component is licensed by ST under Ultimate Liberty license
  * SLA0044, the "License"; You may not use this file except in compliance with
  * the License. You may obtain a copy of the License at:
  *                             www.st.com/SLA0044
  *
  ******************************************************************************
  */
/* USER CODE END Header */

/* Includes ------------------------------------------------------------------*/
#include "main.h"
#include "adc.h"
#include "crc.h"
#include "dma.h"
#include "opamp.h"
#include "tim.h"
#include "usb_device.h"
#include "gpio.h"

/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */
#include "Algorithm.h"
#include "Logic.h"
#include "protocol_interface_ext.h"
#include "sys_param.h"
/* USER CODE END Includes */

/* Private typedef -----------------------------------------------------------*/
/* USER CODE BEGIN PTD */

/* USER CODE END PTD */

/* Private define ------------------------------------------------------------*/
/* USER CODE BEGIN PD */

/* USER CODE END PD */

/* Private macro -------------------------------------------------------------*/
/* USER CODE BEGIN PM */

/* USER CODE END PM */

/* Private variables ---------------------------------------------------------*/

/* USER CODE BEGIN PV */
int16_t ADC_Buff_X[2][MAXLENG]={0};                   //ADC_Buff_X
int16_t ADC_Buff_Y[2][MAXLENG]={0};                   //ADC_Buff_Y
uint32_t Switch_DATA[MAXLENG]={0};
uint32_t Switch_CLK[MAXLENG]={0};
float   Result_Buff_X[MAX_ScanTime + 1][MAXLENG]={0};     //result buff
float   Result_Buff_Y[MAX_ScanTime + 1][MAXLENG]={0};     //result buff
float   ADC_Offset_X[XLENGTH]={0};                    //ADC_Base 1 / no shielding ADC value
float   ADC_Offset_Y[YLENGTH]={0};                    //ADC_Base 1 / no shielding ADC value
uint8_t Tigger_X;
uint8_t Tigger_Y;
volatile uint8_t System_Flag_X = 0;          //System State
volatile uint8_t System_Flag_Y = 0;          //System State

float X_Coordinate;
float Y_Coordinate;
/* USER CODE END PV */

/* Private function prototypes -----------------------------------------------*/
void SystemClock_Config(void);
/* USER CODE BEGIN PFP */
/* USER CODE END PFP */

/* Private user code ---------------------------------------------------------*/
/* USER CODE BEGIN 0 */

/* USER CODE END 0 */

/**
  * @brief  The application entry point.
  * @retval int
  */
int main(void)
{
  /* USER CODE BEGIN 1 */
//  int i=0;
  /* USER CODE END 1 */


  /* MCU Configuration--------------------------------------------------------*/

  /* Reset of all peripherals, Initializes the Flash interface and the Systick. */
  HAL_Init();

  /* USER CODE BEGIN Init */

  /* USER CODE END Init */

  /* Configure the system clock */
  SystemClock_Config();

  /* USER CODE BEGIN SysInit */
  // sys_param_init();
  protocol_ext_interface_init();
  /* USER CODE END SysInit */

  /* Initialize all configured peripherals */
  MX_GPIO_Init();
  MX_DMA_Init();
  MX_ADC1_Init();
  MX_ADC4_Init();
  MX_OPAMP1_Init();
  MX_OPAMP4_Init();
  MX_TIM2_Init();
  MX_TIM8_Init();
  MX_USB_DEVICE_Init();
  MX_CRC_Init();
  /* USER CODE BEGIN 2 */
    HAL_GPIO_WritePin(LED_GREEN_GPIO_Port , LED_GREEN_Pin , GPIO_PIN_SET);
    HAL_GPIO_WritePin(LED_RED_GPIO_Port   , LED_RED_Pin   , GPIO_PIN_SET);
    HAL_OPAMP_Start(&hopamp1);
    HAL_OPAMP_Start(&hopamp4);
	HAL_TIM_Base_Stop(&htim8);
	HAL_TIM_Base_Stop(&htim2);
	__HAL_TIM_SET_COUNTER(&htim8,0);
	__HAL_TIM_SET_COUNTER(&htim2,0);
	HAL_Delay(500);

	IR_PT_SysInit();

	HAL_DMA_Start(&hdma_tim8_ch1,(uint32_t)Switch_DATA ,(uint32_t)&GPIOB->BSRR,MAXLENG);
    __HAL_TIM_ENABLE_DMA(&htim8, TIM_DMA_CC1);
	HAL_DMA_Start(&hdma_tim8_ch2,(uint32_t)Switch_CLK,(uint32_t)&GPIOB->BSRR,MAXLENG);
	__HAL_TIM_ENABLE_DMA(&htim8, TIM_DMA_CC2);

	TIM_CCxChannelCmd(htim8.Instance, TIM_CHANNEL_1,TIM_CCx_ENABLE); //
	TIM_CCxChannelCmd(htim8.Instance, TIM_CHANNEL_2,TIM_CCx_ENABLE);
	HAL_ADC_Start_DMA(&hadc4,(uint32_t *)ADC_Buff_Y,MAXLENG * 2);
	HAL_ADC_Start_DMA(&hadc1,(uint32_t *)ADC_Buff_X,MAXLENG * 2);

	HAL_TIM_PWM_Start(&htim8,TIM_CHANNEL_1);//
	HAL_TIM_PWM_Start(&htim8,TIM_CHANNEL_2);
//  while(i<15)
//	{i++;}
	HAL_TIM_PWM_Start(&htim2,TIM_CHANNEL_3);//ADC4
//	HAL_TIM_PWM_Start(&htim2,TIM_CHANNEL_1);
	HAL_TIM_PWM_Start(&htim2,TIM_CHANNEL_2);	//ADC1

	HAL_Delay(100);
	MathBase_initialization();
	ADC_Base_Calculation(&System_Flag_X,ADC_Buff_X,ADC_Offset_X,XLENGTH);//计算没有遮挡的adc数据 方便后续对比
  ADC_Base_Calculation(&System_Flag_Y,ADC_Buff_Y,ADC_Offset_Y,YLENGTH);//计算没有遮挡的adc数据 方便后续对比
  /* USER CODE END 2 */

  /* Infinite loop */
  /* USER CODE BEGIN WHILE */
  while (1)
  {
    /* USER CODE END WHILE */
    protocol_ext_interface_loop();
    /* USER CODE BEGIN 3 */
		if(Tigger_X>0 && Tigger_Y>0)
		{
			ADC_Axis_calculation(&System_Flag_X,Result_Buff_X,&X_Coordinate,&Tigger_X,XLENGTH);
			ADC_Axis_calculation(&System_Flag_Y,Result_Buff_Y,&Y_Coordinate,&Tigger_Y,YLENGTH);
			Coordinate_Send(&System_Flag_X,&System_Flag_Y,X_Coordinate,Y_Coordinate,&Tigger_X,&Tigger_Y);
			HAL_Delay(20);
		}
  }
  /* USER CODE END 3 */
}

/**
  * @brief System Clock Configuration
  * @retval None
  */
void SystemClock_Config(void)
{
  RCC_OscInitTypeDef RCC_OscInitStruct = {0};
  RCC_ClkInitTypeDef RCC_ClkInitStruct = {0};
  RCC_PeriphCLKInitTypeDef PeriphClkInit = {0};

  /** Initializes the CPU, AHB and APB busses clocks
  */
  RCC_OscInitStruct.OscillatorType = RCC_OSCILLATORTYPE_HSE;
  RCC_OscInitStruct.HSEState = RCC_HSE_ON;
  RCC_OscInitStruct.HSEPredivValue = RCC_HSE_PREDIV_DIV1;
  RCC_OscInitStruct.HSIState = RCC_HSI_ON;
  RCC_OscInitStruct.PLL.PLLState = RCC_PLL_ON;
  RCC_OscInitStruct.PLL.PLLSource = RCC_PLLSOURCE_HSE;
  RCC_OscInitStruct.PLL.PLLMUL = RCC_PLL_MUL6;
  if (HAL_RCC_OscConfig(&RCC_OscInitStruct) != HAL_OK)
  {
    Error_Handler();
  }
  /** Initializes the CPU, AHB and APB busses clocks
  */
  RCC_ClkInitStruct.ClockType = RCC_CLOCKTYPE_HCLK|RCC_CLOCKTYPE_SYSCLK
                              |RCC_CLOCKTYPE_PCLK1|RCC_CLOCKTYPE_PCLK2;
  RCC_ClkInitStruct.SYSCLKSource = RCC_SYSCLKSOURCE_PLLCLK;
  RCC_ClkInitStruct.AHBCLKDivider = RCC_SYSCLK_DIV1;
  RCC_ClkInitStruct.APB1CLKDivider = RCC_HCLK_DIV2;
  RCC_ClkInitStruct.APB2CLKDivider = RCC_HCLK_DIV1;

  if (HAL_RCC_ClockConfig(&RCC_ClkInitStruct, FLASH_LATENCY_2) != HAL_OK)
  {
    Error_Handler();
  }
  PeriphClkInit.PeriphClockSelection = RCC_PERIPHCLK_USB|RCC_PERIPHCLK_TIM8
                              |RCC_PERIPHCLK_ADC12|RCC_PERIPHCLK_ADC34;
  PeriphClkInit.Adc12ClockSelection = RCC_ADC12PLLCLK_DIV1;
  PeriphClkInit.Adc34ClockSelection = RCC_ADC34PLLCLK_DIV1;
  PeriphClkInit.USBClockSelection = RCC_USBCLKSOURCE_PLL_DIV1_5;
  PeriphClkInit.Tim8ClockSelection = RCC_TIM8CLK_HCLK;
  if (HAL_RCCEx_PeriphCLKConfig(&PeriphClkInit) != HAL_OK)
  {
    Error_Handler();
  }
}

/* USER CODE BEGIN 4 */
void HAL_ADC_ConvHalfCpltCallback(ADC_HandleTypeDef* hadc)
{
	  if (hadc->Instance == ADC4)
        ADC_Touch_Calculation(&System_Flag_X,ADC_Buff_X[0],1,Result_Buff_X,ADC_Offset_X,&Tigger_X,XLENGTH);

		if (hadc->Instance == ADC1)
			  ADC_Touch_Calculation(&System_Flag_Y,ADC_Buff_Y[0],1,Result_Buff_Y,ADC_Offset_Y,&Tigger_Y,YLENGTH);
}
void HAL_ADC_ConvCpltCallback(ADC_HandleTypeDef* hadc)
{
	  if (hadc->Instance == ADC4)
        ADC_Touch_Calculation(&System_Flag_X,ADC_Buff_X[1],1,Result_Buff_X,ADC_Offset_X,&Tigger_X,XLENGTH);

		if (hadc->Instance == ADC1)
			  ADC_Touch_Calculation(&System_Flag_Y,ADC_Buff_Y[1],1,Result_Buff_Y,ADC_Offset_Y,&Tigger_Y,YLENGTH);

}


/* USER CODE END 4 */

/**
  * @brief  This function is executed in case of error occurrence.
  * @retval None
  */
void Error_Handler(void)
{
  /* USER CODE BEGIN Error_Handler_Debug */
  /* User can add his own implementation to report the HAL error return state */

  /* USER CODE END Error_Handler_Debug */
}

#ifdef  USE_FULL_ASSERT
/**
  * @brief  Reports the name of the source file and the source line number
  *         where the assert_param error has occurred.
  * @param  file: pointer to the source file name
  * @param  line: assert_param error line source number
  * @retval None
  */
void assert_failed(char *file, uint32_t line)
{
  /* USER CODE BEGIN 6 */
  /* User can add his own implementation to report the file name and line number,
     tex: printf("Wrong parameters value: file %s on line %d\r\n", file, line) */
  /* USER CODE END 6 */
}
#endif /* USE_FULL_ASSERT */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
