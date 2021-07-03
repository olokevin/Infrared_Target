#ifndef __Algorithm_H
#define __Algorithm_H

#include "stm32f3xx.h"
#include "arm_math.h"
#include "stdlib.h"
#include "main.h"

/* Defines -------------------------------------------------------------------*/
#define MAX_ScanTime 20  //if system scanned an object,it will scan it a few more times to ensure precision.
                         //The value shall be within 3 - 100
//#define 	Length   32		

extern void MathBase_initialization(void);
extern uint8_t ADC_cheak_error(int16_t *ADCBuff,uint8_t length);
extern void ADC_Base_Calculation(volatile uint8_t *Flag,int16_t (*ADCbuff)[MAXLENG],float *BaseGroup,uint8_t length);
extern void ADC_Touch_Calculation(volatile uint8_t *Flag,int16_t *ADCBuff,uint8_t src,float (*result)[MAXLENG],float *BaseGroup,uint8_t*Tigger,uint8_t length);
extern void ADC_Axis_calculation(volatile uint8_t *Flag,float (*result)[MAXLENG],float* Coordinate,uint8_t* Tigger,uint8_t length);
extern void Coordinate_Send(volatile uint8_t* Flag1,volatile uint8_t* Flag2,const float Coordinate1,const float Coordinate2,uint8_t* Tigger1,uint8_t* Tigger2);
#endif 
