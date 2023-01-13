# ArduinoClockOS
Software for handling Arduino multi-device clock device.

**ATTENTION! Comments in the arduino code were written in Polish - changes soon.**

## Capabilities:

- Default mode (changes every 15s.):
  - Displaying Hour (all the time on right side).
  - Displaying Date (interchanges with other informations).
  - Displaying Inside temperature (interchanges with other informations).
  - Displaying Outside temperature (interchanges with other informations).
- Menu (after pressing "D"):
  - Settings that allow to set:
    - Date,
    - Time,
    - Brightness,
    - Beep frequency,
    - Alarm,
- Alarm with sleep option ("D" - stops alarm, any other key will enable sleep for 10 minutes).
- Beeping every (24h, 12h, 6h, 3h, 1h, this option can be disabled in menu).
- Performing configuration from serial port (additional debug informations can be send).
- Performing configuration from bluetooth (no additional debug informations will be send).
- It recognizes from which device message came and where send response.
- Saving and reading configuration from SD card:
  - Alarm,
  - Beeping hours,
  - Brightness,
- Screen can change it brightness basing on the ambient brightness.
- Showing message from serial/bluetooth ("/msg YOUR_MESSAGE").
- Weather forecast:  
  Weather forecast is working with files where lines have pattern: "yyyy:MM:dd n,i0,i1,i2,i3,...,i24"  
  - yyyy:MM:dd: indicates date of weather forecast.
  - n: is count of forecast hours (24 = 24h, 6 = 6h, etc.)
  - iX: is icon index, where:
    - 0: sunny,
    - 1: part cloudy,
    - 2: cloudy,
    - 3: rainy,
    - 4: snowy,
    - 5: fog,
    - 6: thunder

## Used components:

- Arduino Mega 2560
- Bluetooth module HC-06
- Buzzer 5V 12MM THT
- Dallas led matrix 8x 8x8 MAX7219
- Keypad 4x4 HX13B001
- 2x Photoresistor GL5528
- Real time clock DS3231
- SD Card module HW-125
- Temperature sensor DALLAS DS18B20

## Examples:

Overview photo showing from above:
- Default display mode (changes every 15 sec): Date & Time
- Default display mode (changes every 15 sec): In door temperature.
- Default display mode (changes every 15 sec): Outside temperature.
- Main menu, showing Settings menu elemnt to choose.
- Alarm settings, showing hour and minutes setting (highlihted minutes).
- Alarm settings, showing option to set alarm (entered hour and minutes and enables alarm).
- Alarm settings, showing option to disable alarm.

![Arduino default date and time (Images/arduino_default_datetime.png)](Images/arduino_default_datetime.png)
![Arduino default temperature in (Images/arduino_default_intemp.png)](Images/arduino_default_intemp.png)
![Arduino default temperature out (Images/arduino_default_outtemp.png)](Images/arduino_default_outtemp.png)
![Arduino main menu - settings item (Images/arduino_menu_settings.png)](Images/arduino_menu_settings.png)
![Arduino settings - alarm setting minutes (Images/arduino_settings_alarm.png)](Images/arduino_settings_alarm.png)
![Arduino settings - alarm disable option (Images/arduino_settings_alarm_disable.png)](Images/arduino_settings_alarm_disable.png)
![Arduino settings - alarm set option (Images/arduino_settings_alarm_set.png)](Images/arduino_settings_alarm_set.png)

## Controls:

Navigation is done by:  
"D" - Enter settings mode, and back to default display mode from any place.  
"A" - Enter or set.  
"B" - Back.  
"*" - Navigate left in menu (change item, change option in settings or set other item to edit such as hour, minutes, etc...)  
"#" - Navigate right in menu (change item, change option in settings or set other item to edit such as hour, minutes, etc...)  
(Numeric data can be set by pressing numeric keys 0..9)  

# ArduinoConnect (WPF application)

Application that allow to control Arduino from PC using serial communication.

## Capabilities:

- Console Tab
  - Showing messages and debug messages that came from Arduino.
  - Sending messages and commands to arduino as pure text.

- Weather Tab
  - Getting weather from http://wttr.in/
  - Showing weather (raw data in TreeView and user friendly view in ListView).
  - Sending weather to arduino.

- Settings Tab
  - Loading configuration from Arduino at once.
  - Setting Date & Time in Arduino.
  - Setting Alarm in Arduino.
  - Setting Brightness in Arduino.
  - Setting Hours beeping in Arduino.
  - Sending full configuration at once to Arduino.

## In development:

- Sending weather to arduino (+ thread that will work in background and send weather every 3 days - app will work in Windows Tray)
