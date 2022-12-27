////////////////////////////////////////////////////////////////////////////////
//  ARDUINO CLOCK 3.0
//  KAMIL KARPIŃSKI
////////////////////////////////////////////////////////////////////////////////

/*  TO DO:
 *  - Migotanie podkreslenia w ustawieniach.
 *  - Rozdzielenie czyszczenia ekranu.
 *  - Beep godzinowy.
 *  - Wyswietlanie wiadomosci.
 *  - Pogoda (wgrywanie, przetrzymywanie, ladowanie)
 *    int[25] -> pierwszy element (ile godzin), nastepne to ustawienie pogody
 *    Dane:
 *      - 0 - brak pogody
 *      - 1 - na caly dzien
 *      - 2 - dzien/noc (18-5 i 6-17)
 *      - 3 - ...
 *      - 24 - co godzine inna
 *  - Alarm
 *  - Zapis ustawien na karte i ich wczytywanie
 */

////////////////////////////////////////////////////////////////////////////////
//  *** INCLUDED LIBRARIES ***
////////////////////////////////////////////////////////////////////////////////

#include "global_controller.h"
#include "menu_controller.h"
#include "command_processor.h"
#include "data_setter.h"

#include "buzzer_controller.h"
#include "clock_controller.h"
#include "clock_timer.h"
#include "display_controller.h"
#include "keypad_controller.h"
#include "photoresistor_controller.h"
#include "sd_card_controller.h"
#include "serial_controller.h"
#include "temperature_sensor_controller.h"


////////////////////////////////////////////////////////////////////////////////
//  *** CONTROLLERS ***
////////////////////////////////////////////////////////////////////////////////

CommandProcessor  * command_processor;
DataSetter        * data_setter;
MenuController    * menu_controller;
GlobalController  * controller;



////////////////////////////////////////////////////////////////////////////////
//  *** SETUP METHODS ***
////////////////////////////////////////////////////////////////////////////////

//  Ruzruch i inicjalizacja oprogramowania.
void setup()
{
    //  Zaladowanie globalnego kontrolera.
    controller = new GlobalController();
    command_processor = new CommandProcessor(controller);
    data_setter = new DataSetter(controller);
    menu_controller = new MenuController(controller);

    //  Wyswietlenie pierwszej opcji.
    controller->SetDisplayingState(DISPLAY_DATETIME_STATE);
}


////////////////////////////////////////////////////////////////////////////////
//  *** PROCESS INPUT WORK METHODS ***
////////////////////////////////////////////////////////////////////////////////

void ProcessInput()
{
    //  Odczytanie danych z sensorow jasności.
    if (controller->IsAutoBrightness())
        controller->ProcessAutoBrightness();
    
    //  Odczytanie danych przychodzacych z innych urzadzen.
    String _data = controller->serial_ctrl->ReadInputData();
    bool _processed_ok = command_processor->ProcessCommand(_data);

    if (_processed_ok)
        return;

    //  Przetworzenie sygnalu danych wejsciowych z klawiatury.
    controller->ProcessInput();

    if (controller->GetInputKey() > KEYPAD_NO_KEY)
    {
        String _control_message = "Keyboard input: [" + String(controller->GetInputKey()) + "]";
        controller->serial_ctrl->WriteRawData(_control_message, SERIAL_COM);
        return;
    }
}

////////////////////////////////////////////////////////////////////////////////
//  *** PROCESS DISPLAY WORK METHODS ***
////////////////////////////////////////////////////////////////////////////////

void DisplayClock()
{
    DisplayString *_ds = controller->GetDisplayString(TEXT_ALIGN_RIGHT);

    bool is_blinking = controller->clock_ctrl->GetBlink();
    
    _ds->text = controller->clock_ctrl->GetTime("HM", ':', is_blinking);
    _ds->offset = 1;

    digitalWrite(LED_BUILTIN, is_blinking ? LOW : HIGH);
    controller->display_ctrl->PrintDS(_ds, true);
}

//  ----------------------------------------------------------------------------
void DisplayDate()
{
    DisplayString *_ds = controller->GetDisplayString(TEXT_ALIGN_LEFT);

    _ds->text = controller->clock_ctrl->GetDate("YMD", '.');
    _ds->offset = 1;
    _ds->_xpos = 0;
    controller->display_ctrl->PrintDS(_ds, true);
}

//  ----------------------------------------------------------------------------
void DisplayTemperatureInside()
{
    DisplayString *_ds = controller->GetDisplayString(TEXT_ALIGN_LEFT);
    int temperature = controller->temp_sensor_ctrl_in->GetTemperature();

    _ds->text = temperature <= TEMPERATURE_SENSOR_NULL ? "-`C" : String(temperature) + "`C";
    _ds->offset = 10;
    _ds->_xpos = 8;
    _ds->_width += 2;

    controller->display_ctrl->DrawSprite(SPRITE_HOME, 0, 0);
    controller->display_ctrl->PrintDS(_ds, true);
}

//  ----------------------------------------------------------------------------
void DisplayTemperatureOutside()
{
    DisplayString *_ds = controller->GetDisplayString(TEXT_ALIGN_LEFT);
    int temperature = controller->temp_sensor_ctrl_out->GetTemperature();

    _ds->text = temperature <= TEMPERATURE_SENSOR_NULL ? "-`C" : String(temperature) + "`C";
    _ds->offset = 10;
    _ds->_xpos = 8;
    _ds->_width += 2;

    controller->display_ctrl->DrawSprite(SPRITE_WEATHER, 0, 0);
    controller->display_ctrl->PrintDS(_ds, true);
}

//  ----------------------------------------------------------------------------
void ProcessDisplay()
{
    int _state = controller->GetGlobalState();

    if (_state == GLOBAL_STATE_NORMAL)
    {
        if (controller->IsGlobalChangeRequested())
        {
            switch (controller->GetDisplayingState())
            {
                case DISPLAY_DATETIME_STATE:
                    DisplayDate();
                    break;
                
                case DISPLAY_TEMPERATURE_IN_STATE:
                    DisplayTemperatureInside();
                    break;
                
                case DISPLAY_TEMPERATURE_OUT_STATE:
                    DisplayTemperatureOutside();
                    break;
            }

            DisplayClock();
        }
    }
    else if (_state == GLOBAL_STATE_SETTER)
    {
        data_setter->UpdateDisplay();
    }
}

////////////////////////////////////////////////////////////////////////////////
//  *** PROCESS DATA WORK METHODS ***
////////////////////////////////////////////////////////////////////////////////

void ProcessCommand()
{
    switch (controller->GetGlobalState())
    {
        case GLOBAL_STATE_NORMAL:
            switch (controller->GetInputCommand())
            {
                case COMMAND_DISPLAY_DATETIME:
                    controller->SetDisplayingState(DISPLAY_DATETIME_STATE);
                    break;
                
                default:
                    break;
            }
    }
}

//  ----------------------------------------------------------------------------
void ProcessKeyInput()
{
    int global_state = controller->GetGlobalState();
    
    if (global_state == GLOBAL_STATE_NORMAL)
    {
        switch (controller->GetInputKey())
        {
            case KEYPAD_1_KEY:
                controller->SetDisplayingState(DISPLAY_DATETIME_STATE);
                break;
            
            case KEYPAD_2_KEY:
                controller->SetDisplayingState(DISPLAY_TEMPERATURE_IN_STATE);
                break;
            
            case KEYPAD_3_KEY:
                controller->SetDisplayingState(DISPLAY_TEMPERATURE_OUT_STATE);
                break;
            
            case KEYPAD_MENU_KEY:
                controller->SetGlobalState(GLOBAL_STATE_MENU);
                menu_controller->OpenMenu();
                break;
            
            case KEYPAD_NO_KEY:
            default:
                break;
        }
    }
    else if (global_state == GLOBAL_STATE_MENU)
    {
        int menu_output = menu_controller->ProcessInput();

        switch (menu_output)
        {
            case SETTINGS_ITEM_TIME:
                controller->SetGlobalState(GLOBAL_STATE_SETTER);
                data_setter->OpenSetter(TIME_SETTER);
                break;
            
            case SETTINGS_ITEM_DATE:
                controller->SetGlobalState(GLOBAL_STATE_SETTER);
                data_setter->OpenSetter(DATE_SETTER);
                break;
            
            case SETTINGS_ITEM_BRIGHTNESS:
                controller->SetGlobalState(GLOBAL_STATE_SETTER);
                data_setter->OpenSetter(BRIGHTNESS_SETTER);
                break;
            
            case SETTINGS_ITEM_BEEP:
                controller->SetGlobalState(GLOBAL_STATE_SETTER);
                data_setter->OpenSetter(BEEP_SETTER);
                break;
            
            case SETTINGS_ITEM_ALARM:
                controller->SetGlobalState(GLOBAL_STATE_SETTER);
                data_setter->OpenSetter(ALARM_SETTER);
                break;

            case MENU_EXIT:
                controller->SetGlobalState(GLOBAL_STATE_NORMAL);
                controller->SetDisplayingState(DISPLAY_DATETIME_STATE);                    
                break;
            
            case MENU_NOTHING:
            default:
                break;
        }
    }
    else if (global_state == GLOBAL_STATE_SETTER)
    {
        int setter_output = data_setter->ProcessInput();
        int setter_mode = data_setter->GetMode();

        switch (setter_output)
        {
            case SETTER_SAVE:
            case SETTER_EXIT:
                switch (setter_mode)
                {
                    case DATE_SETTER:
                        controller->SetGlobalState(GLOBAL_STATE_MENU);
                        menu_controller->OpenMenu(LEVEL_SETTINGS, SETTINGS_ITEM_DATE);
                        break;
                    
                    case TIME_SETTER:
                        controller->SetGlobalState(GLOBAL_STATE_MENU);
                        menu_controller->OpenMenu(LEVEL_SETTINGS, SETTINGS_ITEM_TIME);
                        break;
                    
                    case BRIGHTNESS_SETTER:
                        controller->SetGlobalState(GLOBAL_STATE_MENU);
                        menu_controller->OpenMenu(LEVEL_SETTINGS, SETTINGS_ITEM_BRIGHTNESS);
                        break;

                    case BEEP_SETTER:
                        controller->SetGlobalState(GLOBAL_STATE_MENU);
                        menu_controller->OpenMenu(LEVEL_SETTINGS, SETTINGS_ITEM_BEEP);
                        break;

                    case ALARM_SETTER:
                        controller->SetGlobalState(GLOBAL_STATE_MENU);
                        menu_controller->OpenMenu(LEVEL_SETTINGS, SETTINGS_ITEM_ALARM);
                        break;
                }
                break;
            
            case SETTER_FULL_EXIT:
                controller->SetGlobalState(GLOBAL_STATE_NORMAL);
                controller->SetDisplayingState(DISPLAY_DATETIME_STATE);
                break;
            
            case SETTER_NOTHING:
            default:
                break;
        }
    }
}

//  ----------------------------------------------------------------------------
void ProcessAutoUpdateState()
{
    switch (controller->GetGlobalState())
    {
        case GLOBAL_STATE_NORMAL:
            controller->SetNextDisplayingState();
            break;

        default:
            break;
    }
}

//  ----------------------------------------------------------------------------
void ProcessData()
{
    //  Pobranie zmiennych tymczasowych wymaganych do przetwarzania.
    Time _date_time = controller->clock_ctrl->Now();

    //  Process global functionalities.
    controller->BuzzerNotifyChangeHour();

    //  Przetworzenie odczytanej komendy.
    if (controller->GetInputCommand() > 0)
    {
        controller->update_timer->Reset();
        ProcessCommand();
    }

    //  Przetworzenie odczytanej komendy.
    else if (controller->GetInputKey() > KEYPAD_NO_KEY)
    {
        controller->update_timer->Reset();
        ProcessKeyInput();
    }

    //  Przetworzenie informacji o zmianie dnia.
    else if (controller->clock_ctrl->HasDayChanged())
    {
        controller->SetDisplayingState(DISPLAY_DATETIME_STATE);
    }

    //  Wykonanie standardowej metody aktualizacji.
    else if (controller->update_timer->Work(_date_time))
    {
        ProcessAutoUpdateState();
    }
}

////////////////////////////////////////////////////////////////////////////////
//  *** WORK METHODS ***
////////////////////////////////////////////////////////////////////////////////

//  Glowna petla programu.
void loop()
{
    ProcessInput();
    ProcessData();
    ProcessDisplay();

    controller->FinalizeGlobalStateChange();
}
