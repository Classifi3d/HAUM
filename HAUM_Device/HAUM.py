from time import sleep
import serial
import asyncio
import websockets
import sys
import json

display_debug_prints = True

def startup_HAUM():
    print('')
    print('===========================================')
    print('##\   ##\  ######\  ##\   ##\ ##\      ##\ ')
    print('## |  ## |##  __##\ ## |  ## |###\    ### |')
    print('## |  ## |## /  ## |## |  ## |####\  #### |')
    print('######## |######## |## |  ## |##\##\## ## |')
    print('##  __## |##  __## |## |  ## |## \###  ## |')
    print('## |  ## |## |  ## |## |  ## |## |\#  /## |')
    print('## |  ## |## |  ## |\######  |## | \_/ ## |')
    print('\__|  \__|\__|  \__| \______/ \__|     \__|')
    print('===========================================')
    print('1. Start HAUM')
    print('2. Configure UART Connections')
    print('3. Configure Server Connections')
    print('4. Exit')

async def payloadJSONifier(payload):
    payload_str = str(payload[5:])[2:-1]
    if display_debug_prints:
        print(payload_str)
    data_split = payload_str.split('|')
    if display_debug_prints:
        print(data_split)
    data = {
        'Temperature': data_split[0],
        'Humidity': data_split[1],
        'Pressure': data_split[2],
        'Illumination': data_split[3]
    }
    json_data = json.dumps(data)
    if display_debug_prints:
        print(json_data)
    return json_data

#  ====== UART ======

uart_port = "/dev/ttyS0"
baudrate = 115200
ser = None

async def uart_ports_init():
    global uart_port
    uart_port = input('Insert UART Port. ex:"/dev/ttyS0"')
    global baudrate
    baudrate = input('Insert UART Baudrate. ex:"9600"')

async def uart_read(websocket, path):
    try:
        # Open the serial port
        ser = serial.Serial(uart_port, baudrate)
        print(f"Connected to {uart_port} at {baudrate} baud rate.")
        while True:
                payload = ser.read()
                await asyncio.sleep(0.03)
                data_remaining = ser.inWaiting()
                payload += ser.read(data_remaining)
                if display_debug_prints:
                    print(payload)
                json_payload = await payloadJSONifier(payload)
                await websocket.send(str(json_payload))
    except serial.SerialException as e:
        print(f"Serial error: {e}")
    except KeyboardInterrupt:
        print("Keyboard interrupt received, exiting.")
    finally:
        # Close the serial port
        if ser.is_open:
            ser.close()
            print(f"Disconnected from {uart_port}.")

#  ====== WebSocket ======

ip_addr = '192.168.1.130'
ws_port = 8765

async def web_ports_init():
    global ip_addr
    ip_addr = input('Insert Server IP ex:"192.168.0.45"')
    global ws_port
    ws_port = int(input('Insert Server Port. ex:"8765"'))

async def websocket_server():
    async with websockets.serve(uart_read, ip_addr, ws_port):
        await asyncio.Future()  # run forever

if __name__ == '__main__':
    while True:
        startup_HAUM()
        option = input('Select an option: ').lower()
        if option == '1':
            asyncio.run(websocket_server())
        elif option == '2':
            asyncio.run(uart_ports_init())
        elif option == '3':
            asyncio.run(web_ports_init())
        elif option == '4':
            sys.exit(0)