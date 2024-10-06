/*
 *  ======== rfEasyLinkTx.c ========
 */

 /* Standard C Libraries */
#include <stdlib.h>
#include <string.h>

/* XDCtools Header files */
#include <xdc/std.h>
#include <xdc/runtime/Assert.h>
#include <xdc/runtime/Error.h>
#include <xdc/runtime/System.h>

/* Driver Header files */
#include <ti/drivers/UART.h>


/* BIOS Header files */
#include <ti/sysbios/BIOS.h>
#include <ti/sysbios/knl/Task.h>
#include <ti/sysbios/knl/Semaphore.h>
#include <ti/sysbios/knl/Clock.h>

/* TI-RTOS Header files */
#include <ti/drivers/PIN.h>

/* Board Header files */
#include "Board.h"

/* EasyLink API Header files */
#include "easylink/EasyLink.h"

/* Application header files */
#include "smartrf_settings/smartrf_settings.h"

/* Undefine to not use async mode */
#define RFEASYLINKTX_ASYNC

#define RFEASYLINKTX_TASK_STACK_SIZE    1024
#define RFEASYLINKTX_TASK_PRIORITY      2

#define RFEASYLINKTX_BURST_SIZE         10
#define RFEASYLINKTXPAYLOAD_LENGTH      40

Task_Struct txTask;    /* not static so you can see in ROV */
static Task_Params txTaskParams;
static uint8_t txTaskStack[RFEASYLINKTX_TASK_STACK_SIZE];

/* Pin driver handle */
static PIN_Handle pinHandle;
static PIN_State pinState;

/* my UART handles */
static UART_Handle uartHandle;
static UART_Params uartParams;

/*
 * Application LED pin configuration table:
 *   - All LEDs board LEDs are off.
 */
PIN_Config pinTable[] = {
    Board_PIN_LED1 | PIN_GPIO_OUTPUT_EN | PIN_GPIO_LOW | PIN_PUSHPULL | PIN_DRVSTR_MAX,
    Board_PIN_LED2 | PIN_GPIO_OUTPUT_EN | PIN_GPIO_LOW | PIN_PUSHPULL | PIN_DRVSTR_MAX,
    PIN_TERMINATE
};

static uint16_t seqNumber;
static Semaphore_Handle txDoneSem;


void txDoneCb(EasyLink_Status status)
{
    if (status == EasyLink_Status_Success)
    {
        /* Toggle LED1 to indicate TX */
        PIN_setOutputValue(pinHandle, Board_PIN_LED1,!PIN_getOutputValue(Board_PIN_LED1));
    }
    else if(status == EasyLink_Status_Aborted)
    {
        /* Toggle LED2 to indicate command aborted */
        PIN_setOutputValue(pinHandle, Board_PIN_LED2,!PIN_getOutputValue(Board_PIN_LED2));
    }
    else
    {
        /* Toggle LED1 and LED2 to indicate error */
        PIN_setOutputValue(pinHandle, Board_PIN_LED1,!PIN_getOutputValue(Board_PIN_LED1));
        PIN_setOutputValue(pinHandle, Board_PIN_LED2,!PIN_getOutputValue(Board_PIN_LED2));
    }

    Semaphore_post(txDoneSem);
}

static void initPayload(uint8_t payload[40]){
    //    ========   Payload Schema  ========
    //     0                   1
    //     0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6
    //    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    //    |   Id.   | Tmp | Hum | Prs | Lux |
    //    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

    /* Create packet Id with incrementing sequence number*/
    payload[0] = 'I';
    payload[1] = 'D';
    payload[2] = (uint8_t)(seqNumber >> 8);
    payload[3] = (uint8_t)(seqNumber++);
    payload[4] = '|';

    /* Temperature */
    payload[5] = rand()%10+'0';
    payload[6] = rand()%10+'0';
    payload[7] = '|';

    /* Humidity */
    payload[8] = rand()%10+'0';
    payload[9] = rand()%10+'0';
    payload[10] = '|';

    /* Pressure */
    payload[11] = rand()%10+'0';
    payload[12] = rand()%10+'0';
    payload[13] = '|';

    /* Lightness */
    payload[14] = rand()%10+'0';
    payload[15] = rand()%10+'0';

}

static void rfEasyLinkTxFnx(UArg arg0, UArg arg1)
{
    uint8_t txBurstSize = 0;
    uint32_t absTime;

    /* Create a semaphore for Async */
    Semaphore_Params params;
    Error_Block eb;

    /* Init params */
    Semaphore_Params_init(&params);
    Error_init(&eb);

    /* Create semaphore instance */
    txDoneSem = Semaphore_create(0, &params, &eb);
    if(txDoneSem == NULL)
    {
        System_abort("Semaphore creation failed");
    }

    // Initialize the EasyLink parameters to their default values
    EasyLink_Params easyLink_params;
    EasyLink_Params_init(&easyLink_params);

    /*
     * Initialize EasyLink with the settings found in easylink_config.h
     * Modify EASYLINK_PARAM_CONFIG in easylink_config.h to change the default
     * PHY
     */
    if(EasyLink_init(&easyLink_params) != EasyLink_Status_Success)
    {
        System_abort("EasyLink_init failed");
    }

    /*
     * If you wish to use a frequency other than the default, use
     * the following API:
     * EasyLink_setFrequency(868000000);
     */

    while(1) {
        EasyLink_TxPacket txPacket =  { {0}, 0, 0, {0} };

        initPayload(txPacket.payload);


        txPacket.len = RFEASYLINKTXPAYLOAD_LENGTH;

        /*
         * Address filtering is enabled by default on the Rx device with the
         * an address of 0xAA. This device must set the dstAddr accordingly.
         */
        txPacket.dstAddr[0] = 0xaa;

        /* Add a Tx delay for > 500ms, so that the abort kicks in and brakes the burst */
        if(EasyLink_getAbsTime(&absTime) != EasyLink_Status_Success)
        {
            // Problem getting absolute time
        }
        if(txBurstSize++ >= RFEASYLINKTX_BURST_SIZE)
        {
          /* Set Tx absolute time to current time + 1s */
          txPacket.absTime = absTime + EasyLink_ms_To_RadioTime(1000);
          txBurstSize = 0;
        }
        /* Else set the next packet in burst to Tx in 100ms */
        else
        {
          /* Set Tx absolute time to current time + 100ms */
          txPacket.absTime = absTime + EasyLink_ms_To_RadioTime(100);
        }

        EasyLink_transmitAsync(&txPacket, txDoneCb);
        /* Wait 300ms for Tx to complete */
        if(Semaphore_pend(txDoneSem, (300000 / Clock_tickPeriod)) == FALSE)
        {
            /* TX timed out, abort */
            if(EasyLink_abort() == EasyLink_Status_Success)
            {
                /*
                 * Abort will cause the txDoneCb to be called and the txDoneSem
                 * to be released, so we must consume the txDoneSem
                 */
               Semaphore_pend(txDoneSem, BIOS_WAIT_FOREVER);
            }
        }
    }
}

void txTask_init(PIN_Handle inPinHandle) {
    pinHandle = inPinHandle;

    Task_Params_init(&txTaskParams);
    txTaskParams.stackSize = RFEASYLINKTX_TASK_STACK_SIZE;
    txTaskParams.priority = RFEASYLINKTX_TASK_PRIORITY;
    txTaskParams.stack = &txTaskStack;
    txTaskParams.arg0 = (UInt)1000000;

    Task_construct(&txTask, rfEasyLinkTxFnx, &txTaskParams, NULL);
}

void myUART_init(){
    UART_init();
    UART_Params_init(&uartParams);
    uartParams.writeDataMode = UART_DATA_BINARY;
    uartParams.readDataMode = UART_DATA_BINARY;
    uartParams.readReturnMode = UART_RETURN_FULL;
    uartParams.readEcho = UART_ECHO_OFF;
    uartParams.baudRate = 115200;
    uartHandle = UART_open(Board_UART0, &uartParams);

    if (uartHandle == NULL) {
        /* UART_open() failed */
        while (1);
    }
}


/*
 *  ======== main ========
 */
int main(void)
{
    /* Call driver init functions. */
    Board_initGeneral();
    myUART_init();

//    const char message[] = "Board TX Init Succesful!\n\r";
//    UART_write(uartHandle, message, sizeof(message));

    /* Open LED pins */
    pinHandle = PIN_open(&pinState, pinTable);
    Assert_isTrue(pinHandle != NULL, NULL);

    /* Clear LED pins */
    PIN_setOutputValue(pinHandle, Board_PIN_LED1, 0);
    PIN_setOutputValue(pinHandle, Board_PIN_LED2, 0);

    txTask_init(pinHandle);

    /* Start BIOS */
    BIOS_start();

    return (0);
}
