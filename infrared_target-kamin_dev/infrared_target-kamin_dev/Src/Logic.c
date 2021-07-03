#include "Logic.h"

extern uint32_t Switch_DATA[MAXLENG];
extern uint32_t Switch_CLK[MAXLENG];

HC164_GPIO   PT1_164_DIV1_GPIO;   //Photosense individual control IOs
HC164_GPIO   IR1_164_DIV1_GPIO;   //infrared diode control IOs.
HC164_GPIO   IR2_164_DIV1_GPIO;  //infrared diode control IOs.
HC164_GPIO   PT2_164_DIV1_GPIO;  //Photosense group control IOs


static void IR_PT_IO_Init(void);
static void Scan_Logic_Init(uint32_t *Databuff , uint32_t *Databuff2 ,  HC164_GPIO  IR1_DIV1 , HC164_GPIO IR2_DIV1,  HC164_GPIO PT1_DIV1 , HC164_GPIO PT2_DIV1);


void IR_PT_SysInit(void)
{
	IR_PT_IO_Init();
	Scan_Logic_Init(Switch_DATA , Switch_CLK , IR1_164_DIV1_GPIO , IR2_164_DIV1_GPIO , PT1_164_DIV1_GPIO , PT2_164_DIV1_GPIO);
}


static void IR_PT_IO_Init(void)
{

	IR1_164_DIV1_GPIO.Data.GPIOx = TX1_DATA_GPIO_Port;
	IR1_164_DIV1_GPIO.Data.GPIO_Pin = TX1_DATA_Pin;
	IR1_164_DIV1_GPIO.CLK.GPIOx  = TX1_CLK_GPIO_Port;
  IR1_164_DIV1_GPIO.CLK.GPIO_Pin  = TX1_CLK_Pin; 	
	
	PT1_164_DIV1_GPIO.Data.GPIOx = RX1_DATA_GPIO_Port;
	PT1_164_DIV1_GPIO.Data.GPIO_Pin = RX1_DATA_Pin;
	PT1_164_DIV1_GPIO.CLK.GPIOx  = RX1_CLK_GPIO_Port;
  PT1_164_DIV1_GPIO.CLK.GPIO_Pin  = RX1_CLK_Pin;	
	
	IR2_164_DIV1_GPIO.Data.GPIOx = TX2_DATA_GPIO_Port;
	IR2_164_DIV1_GPIO.Data.GPIO_Pin = TX2_DATA_Pin;
	IR2_164_DIV1_GPIO.CLK.GPIOx = TX2_CLK_GPIO_Port;   
	IR2_164_DIV1_GPIO.CLK.GPIO_Pin = TX2_CLK_Pin;
	
	PT2_164_DIV1_GPIO.Data.GPIOx = RX2_DATA_GPIO_Port;
	PT2_164_DIV1_GPIO.Data.GPIO_Pin = RX2_DATA_Pin;
	PT2_164_DIV1_GPIO.CLK.GPIOx  = RX2_CLK_GPIO_Port;
	PT2_164_DIV1_GPIO.CLK.GPIO_Pin = RX2_CLK_Pin;
	
}


static void Scan_Logic_Init(uint32_t *Databuff , uint32_t *Databuff2 ,  HC164_GPIO  IR1_DIV1 , HC164_GPIO IR2_DIV1,  HC164_GPIO PT1_DIV1 , HC164_GPIO PT2_DIV1)
{
	uint8_t i;
	
	uint32_t IR1_Set;        //luminous diode setting
	uint32_t IR2_Set;        //luminous diode setting	
	uint32_t PT1_DIV1_Set;   //photosensor setting
	uint32_t PT2_DIV1_Set;   //photosensor setting   
	
	for(i = 0; i < MAXLENG; i++)
	{
		IR1_Set  = 0;
		IR2_Set  = 0;
		PT1_DIV1_Set = 0;
		PT2_DIV1_Set = 0;
		
		if ( i == 0 )
		{			
			IR1_Set      = IR1_DIV1.Data.GPIO_Pin;
			IR2_Set      = IR2_DIV1.Data.GPIO_Pin;			
			PT1_DIV1_Set = PT1_DIV1.Data.GPIO_Pin;
			PT2_DIV1_Set = PT2_DIV1.Data.GPIO_Pin;
		}
		else
		{
			IR1_Set      =   IR1_DIV1.Data.GPIO_Pin << 16;	
      IR2_Set      =   IR2_DIV1.Data.GPIO_Pin << 16;			
			PT1_DIV1_Set =   PT1_DIV1.Data.GPIO_Pin << 16;
			PT2_DIV1_Set =   PT2_DIV1.Data.GPIO_Pin << 16;	
		}
		
		
		Databuff[i]  =  IR1_Set  | IR2_Set | PT1_DIV1_Set | PT2_DIV1_Set | (IR1_DIV1.CLK.GPIO_Pin<<16) | (IR2_DIV1.CLK.GPIO_Pin<<16) | (PT1_DIV1.CLK.GPIO_Pin<<16) | (PT2_DIV1.CLK.GPIO_Pin<<16);
		
		Databuff2[i] =  IR1_DIV1.CLK.GPIO_Pin | IR2_DIV1.CLK.GPIO_Pin | PT1_DIV1.CLK.GPIO_Pin | PT2_DIV1.CLK.GPIO_Pin;
	
	}
	
}





