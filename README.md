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
- Playing song with buzzer.
- Controling leds strip by IR module.

## Used components:

- Arduino Mega 2560
- Bluetooth module HC-06
- Buzzer 5V 12MM THT
- Dallas led matrix 8x 8x8 MAX7219
- GRL-12509 (IR Transmitter)
- Keypad 4x4 HX13B001
- 2x Photoresistor GL5528
- Real time clock DS3231
- SD Card module HW-125
- Temperature sensor DALLAS DS18B20

## Commands:
/alarm get - Getting alarm configuration.  
/alarm set [off/disable] - Disable alarm.  
/alarm set hh:mm - Set alarm.  
/beep get - Getting hourly beep configuration.  
/beep set [off/disable] - Disable hourly beep.  
/beep set [0/1/3/6/12/24] - Set hourly beep every x hours.  
/brightness get - Getting brightness configuration.  
/brightness set [a/auto] - Set auto brightness.  
/brightness set [0..8] - Set brightness to x value.  
/date get - Getting date configuration.  
/date set [dd.MM.yyyy/dd.w.MM.yyyy] - Set date by sending day, month, year or day, week number, month year.  
/init - Check if everything has been loaded after restart.  
/lock [message] - Lock all functionalities to keep fast communication with PC. You can add message.  
/msg [message] - Showing message.  
/play note,duration;note,duration;note,duration;...; - Play song by sending notes and its duration. 0 note is pause.  
/time get - Getting time configuration.  
/time set [hh:mm:ss/hh:mm] - Set time by sending hour, minutes, seconds or just hour, minutes.  
/unlock - Unlock all functionalities.  
/weather clear - Clear current weather data.  
/weather add yyyy.MM.dd 4,1,2,2,3 - Set weather by sending weather date, number of hours, and after comma, index of icon for weather forecast.  

Extended description:  
4 - is the number of hours, if in weahter forecast is weather for 00:00, 06:00, 12:00, 18:00  
1 - is the weather icon for 00:00 hour  
2 - is the weahter icon for 06:00, then for 12:00  
3 - is the weather icon for 18:00  
If there are less hours than 24, for example 4. Icon will be displayed from current weather forecast hour to next hour with available weather forecast.  

Icons:  
0 - Sunny,  
1 - Part cloudy,  
2 - Cloudy/Very cloudy,  
3 - Shower/Sleet shower/Heavy shower/Sleet/Light rain/Heavy rain/Rain,  
4 - Light snow/Heavy snow/Light snow shower/Heavy snow shower/Snow,  
5 - Fog,  
6 - Thundery shower/Thundery heavy rain/Thundery snow shower/Thunder,  

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

- Lights Tab
  - Controling LedStrip via IR in Arduino.

- Piano Tab
  - Composing songs.
  - Saving and opening compositions.
  - Playing songs.
  - Sending song to Arduino to play via buzzer.

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

- Minimalizing application to Windows Tray
- Reacting to "/get weahter" command from Arduino when data are not presented on SDCard, or are not up to date (no data for current date).
