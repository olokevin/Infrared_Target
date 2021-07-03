/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.h
  * @brief          : Header for main.c file.
  *                   This file contains the common defines of the application.
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

/* Define to prevent recursive inclusion -------------------------------------*/
#ifndef __MAIN_H
#define __MAIN_H

#ifdef __cplusplus
extern "C" {
#endif

/* Includes ------------------------------------------------------------------*/
#include "stm32f3xx_hal.h"

/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */

/* USER CODE END Includes */

/* Exported types ------------------------------------------------------------*/
/* USER CODE BEGIN ET */

/* USER CODE END ET */

/* Exported constants --------------------------------------------------------*/
/* USER CODE BEGIN EC */

/* USER CODE END EC */

/* Exported macro ------------------------------------------------------------*/
/* USER CODE BEGIN EM */

/* USER CODE END EM */

/* Exported functions prototypes ---------------------------------------------*/
void Error_Handler(void);

/* USER CODE BEGIN EFP */

/* USER CODE END EFP */

/* Private defines -----------------------------------------------------------*/
#define LED_GREEN_Pin GPIO_PIN_14
#define LED_GREEN_GPIO_Port GPIOC
#define LED_RED_Pin GPIO_PIN_15
#define LED_RED_GPIO_Port GPIOC
#define RX2_DATA_Pin GPIO_PIN_10
#define RX2_DATA_GPIO_Port GPIOB
#define RX2_CLK_Pin GPIO_PIN_11
#define RX2_CLK_GPIO_Port GPIOB
#define TX2_CLK_Pin GPIO_PIN_4
#define TX2_CLK_GPIO_Port GPIOB
#define TX2_DATA_Pin GPIO_PIN_5
#define TX2_DATA_GPIO_Port GPIOB
#define TX1_CLK_Pin GPIO_PIN_6
#define TX1_CLK_GPIO_Port GPIOB
#define TX1_DATA_Pin GPIO_PIN_7
#define TX1_DATA_GPIO_Port GPIOB
#define RX1_CLK_Pin GPIO_PIN_8
#define RX1_CLK_GPIO_Port GPIOB
#define RX1_DATA_Pin GPIO_PIN_9
#define RX1_DATA_GPIO_Port GPIOB
/* USER CODE BEGIN Private defines */
#define 	MAXLENG   64 //x���y����󳤶�
#define 	XLENGTH   64//x�᳤��
#define 	YLENGTH   40//y�᳤��

#if MAXLENG<XLENGTH
#error "Please check xlength"
#elsif MAXLENG<YLENGTH
#error "Please check ylength"
#endif
/* USER CODE END Private defines */

#ifdef __cplusplus
}
#endif

#endif /* __MAIN_H */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
