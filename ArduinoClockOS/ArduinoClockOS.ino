////////////////////////////////////////////////////////////////////////////////
//  ARDUINO CLOCK 3.0
//  KAMIL KARPIŃSKI
////////////////////////////////////////////////////////////////////////////////

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

//  Przetworzenie danych wejsciowycyh.
void ProcessInput()
{
    controller->ProcessInput();

    //  Przetworzenie danych z konsoli.
    if (controller->IsCommandValueInputed())
    {
        switch (command_processor->ProcessCommand(controller->GetInputCommand()))
        {
            case COMMAND_NONE:
                break;
            
            case COMMAND_PROCESSED_OK:
                return;
            
            case COMMAND_DISPLAY_DATETIME:
                controller->SetMachineState(GLOBAL_STATE_NORMAL);
                controller->SetDisplayingState(DISPLAY_DATETIME_STATE);
                return;
        }
    }

    //  Przetworzenie danych z klawiatury.
    if (controller->GetInputKey() > KEYPAD_NO_KEY)
    {
        controller->serial_ctrl->WriteRawData("Keyboard input: [" + String(controller->GetInputKey()) + "]", SERIAL_COM);
        return;
    }
}

////////////////////////////////////////////////////////////////////////////////
//  *** PROCESS DATA WORK METHODS ***
////////////////////////////////////////////////////////////////////////////////

void ProcessNormalState(int input)
{
    switch (input)
    {
        case KEYPAD_1_KEY:
            controller->SetDisplayingState(DISPLAY_DATETIME_STATE);
            return;
        
        case KEYPAD_2_KEY:
            controller->SetDisplayingState(DISPLAY_TEMPERATURE_IN_STATE);
            return;
        
        case KEYPAD_3_KEY:
            controller->SetDisplayingState(DISPLAY_TEMPERATURE_OUT_STATE);
            return;
        
        case KEYPAD_MENU_KEY:
            controller->SetMachineState(GLOBAL_STATE_MENU);
            menu_controller->OpenMenu();
            return;
        
        case KEYPAD_NO_KEY:
        default:
            return;
    }
}

//  ----------------------------------------------------------------------------
void ProcessMenuState(int input)
{
    switch (input)
    {
        case MENU_EXIT:
            controller->SetMachineState(GLOBAL_STATE_NORMAL);
            controller->SetDisplayingState(DISPLAY_DATETIME_STATE);                    
            break;

        case SETTINGS_ITEM_TIME:
            controller->SetMachineState(GLOBAL_STATE_SETTER);
            data_setter->OpenSetter(TIME_SETTER);
            break;
        
        case SETTINGS_ITEM_DATE:
            controller->SetMachineState(GLOBAL_STATE_SETTER);
            data_setter->OpenSetter(DATE_SETTER);
            break;
        
        case SETTINGS_ITEM_BRIGHTNESS:
            controller->SetMachineState(GLOBAL_STATE_SETTER);
            data_setter->OpenSetter(BRIGHTNESS_SETTER);
            break;
        
        case SETTINGS_ITEM_BEEP:
            controller->SetMachineState(GLOBAL_STATE_SETTER);
            data_setter->OpenSetter(BEEP_SETTER);
            break;
        
        case SETTINGS_ITEM_ALARM:
            controller->SetMachineState(GLOBAL_STATE_SETTER);
            data_setter->OpenSetter(ALARM_SETTER);
            break;
        
        case MENU_NOTHING:
        default:
            break;
    }
}

//  ----------------------------------------------------------------------------
void ProcessSetterState(int input, int mode)
{
    if (input == SETTER_NOTHING)
        return;
    
    else if (input == SETTER_SAVE || input == SETTER_EXIT)
    {
        switch (mode)
        {
            case DATE_SETTER:
                controller->SetMachineState(GLOBAL_STATE_MENU);
                menu_controller->OpenMenu(LEVEL_SETTINGS, SETTINGS_ITEM_DATE);
                break;
            
            case TIME_SETTER:
                controller->SetMachineState(GLOBAL_STATE_MENU);
                menu_controller->OpenMenu(LEVEL_SETTINGS, SETTINGS_ITEM_TIME);
                break;
            
            case BRIGHTNESS_SETTER:
                controller->SetMachineState(GLOBAL_STATE_MENU);
                menu_controller->OpenMenu(LEVEL_SETTINGS, SETTINGS_ITEM_BRIGHTNESS);
                break;

            case BEEP_SETTER:
                controller->SetMachineState(GLOBAL_STATE_MENU);
                menu_controller->OpenMenu(LEVEL_SETTINGS, SETTINGS_ITEM_BEEP);
                break;

            case ALARM_SETTER:
                controller->SetMachineState(GLOBAL_STATE_MENU);
                menu_controller->OpenMenu(LEVEL_SETTINGS, SETTINGS_ITEM_ALARM);
                break;
        }
    }
    else if (input == SETTER_FULL_EXIT)
    {
        controller->SetMachineState(GLOBAL_STATE_NORMAL);
        controller->SetDisplayingState(DISPLAY_DATETIME_STATE);
    }
}

//  ----------------------------------------------------------------------------
void ProcessAlarmState(int input)
{
    if (input == ALARM_DISARMED || input == ALARM_SUSPENDED)
    {
        controller->SetMachineState(GLOBAL_STATE_NORMAL);
        controller->SetDisplayingState(DISPLAY_DATETIME_STATE);
    }
}

//  ----------------------------------------------------------------------------
void ProcessMessageState(int input)
{
    if (input == MESSAGE_FINISHED)
    {
        controller->SetMachineState(GLOBAL_STATE_NORMAL);
        controller->display_ctrl->Clear();
        controller->SetDisplayingState(DISPLAY_DATETIME_STATE);
    }
}

//  ----------------------------------------------------------------------------
void ProcessFunctionalities()
{
    //  Przetworzenie globalnych funkcjonlanosci.
    controller->ProcessFunctionalities();

    //  Dodatkowe przetworzenie danych wejsciowych.
    int input_key = controller->GetInputKey();
    int machine_state = controller->GetMachineState();

    if (input_key > KEYPAD_NO_KEY)
    {
        controller->update_timer->Reset();

        if (machine_state == GLOBAL_STATE_NORMAL)
        {
            ProcessNormalState(input_key);
        }
        else if (machine_state == GLOBAL_STATE_MENU)
        {
            int menu_output = menu_controller->ProcessInput(input_key);
            ProcessMenuState(menu_output);
        }
        else if (machine_state == GLOBAL_STATE_SETTER)
        {
            int setter_output = data_setter->ProcessInput(input_key);
            int setter_mode = data_setter->GetMode();
            ProcessSetterState(setter_output, setter_mode);
        }
        else if (machine_state == GLOBAL_STATE_ALARM)
        {
            int alarm_output = controller->alarm->ProcessInput(input_key);
            ProcessAlarmState(alarm_output);
        }
    }

    //  Tryby pozwalajace na 0 input.
    if (machine_state == GLOBAL_STATE_MESSAGE)
    {
        int message_output = controller->msg_ctrl->ProcessInput(input_key);
        ProcessMessageState(message_output);
    }
}

////////////////////////////////////////////////////////////////////////////////
//  *** PROCESS DISPLAY WORK METHODS ***
////////////////////////////////////////////////////////////////////////////////

void ProcessAlarmDisplay()
{
    bool blink = controller->clock_ctrl->GetBlink();
    DisplayString * dsp_str_l = controller->GetDisplayString(TEXT_ALIGN_LEFT);
    DisplayString * dsp_str_r = controller->GetDisplayString(TEXT_ALIGN_RIGHT);

    if (blink)
    {
        dsp_str_l->text = "ALARM";
        dsp_str_l->offset =   10;
        dsp_str_l->_xpos  =   8;
        dsp_str_l->_width +=  2;
            
        dsp_str_r->text = controller->clock_ctrl->GetTime("HM", ':', false);
        dsp_str_r->offset = 1;

        controller->display_ctrl->DrawSprite(SPRITE_CLOCK, 0, 0);
        controller->display_ctrl->PrintDS(dsp_str_l, true);
        controller->display_ctrl->PrintDS(dsp_str_r, true);

        controller->buzzer_ctrl->PlayTone(NOTE_C8, 1);
    }
    else
    {
        controller->display_ctrl->Clear();
    }
}

//  ----------------------------------------------------------------------------
void ProcessDisplay()
{
    int machine_state = controller->GetMachineState();

    if (machine_state == GLOBAL_STATE_NORMAL)
        controller->ProcessDisplay();

    else if (machine_state == GLOBAL_STATE_SETTER)
        data_setter->UpdateDisplay();
    
    else if (machine_state == GLOBAL_STATE_ALARM)
        ProcessAlarmDisplay();
    
    else if (machine_state == GLOBAL_STATE_MESSAGE)
        controller->msg_ctrl->UpdateDisplay();
}

////////////////////////////////////////////////////////////////////////////////
//  *** WORK METHODS ***
////////////////////////////////////////////////////////////////////////////////

//  Glowna petla programu.
void loop()
{
    ProcessInput();
    ProcessFunctionalities();
    ProcessDisplay();

    controller->FinalizeCycle();
}
