#ifndef __LOGIC_H
#define __LOGIC_H

#include "gpio.h"
#include "main.h"

typedef struct
{
	GPIO_TypeDef *GPIOx;
	uint32_t GPIO_Pin;
}GPIO_Function;


typedef struct
{
	GPIO_Function Data;
	GPIO_Function CLK;
}HC164_GPIO;


void IR_PT_SysInit(void);

#endif
