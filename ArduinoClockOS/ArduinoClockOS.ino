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
        int proc_result = command_processor->ProcessCommand(controller->GetInputCommand());

        if (controller->IsServiceLocked())
            return;

        switch (proc_result)
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

        if (!controller->buzzer_ctrl->UpdateToneAsync())
            controller->buzzer_ctrl->PlayToneAsync(NOTE_C8, 2);
        
        if (controller->alarm->IsLed())
            controller->led_controller->On();
    }
    else
    {
        controller->display_ctrl->Clear();

        if (controller->alarm->IsLed())
            controller->led_controller->Off();
    }
}

//  ----------------------------------------------------------------------------
void ProcessLedsDisplay(bool ignore_change = false)
{
    if (ignore_change || controller->led_controller->HasChanged())
    {
        String color_name = controller->led_controller->GetName();
        int text_length = controller->display_ctrl->GetTextWidth(0, color_name) + 2;
        int text_xpos = controller->display_ctrl->GetWidth();
        
        controller->display_ctrl->Clear();
        controller->display_ctrl->DrawSprite(SPRITE_LEDS, 0, 0);
        controller->display_ctrl->PrintText(0, 9, "Leds");
        controller->display_ctrl->PrintText(0, text_xpos - text_length, color_name);
    }
}

//  ----------------------------------------------------------------------------
void ProcessDisplay()
{
    //  Przerwanie z powodu blokady serwisowej.
    if (controller->IsServiceLocked())
        return;
    
    int machine_state = controller->GetMachineState();

    if (machine_state == GLOBAL_STATE_NORMAL)
        controller->ProcessDisplay();

    else if (machine_state == GLOBAL_STATE_SETTER)
        data_setter->UpdateDisplay();
    
    else if (machine_state == GLOBAL_STATE_ALARM)
        ProcessAlarmDisplay();
    
    else if (machine_state == GLOBAL_STATE_MESSAGE)
        controller->msg_ctrl->UpdateDisplay();
    
    else if (machine_state == GLOBAL_STATE_SONG_PLAY)
        controller->song_controller->ProcessPlaying();
    
    else if (machine_state == GLOBAL_STATE_LEDS)
        ProcessLedsDisplay();
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
        
        case KEYPAD_0_KEY:
            controller->led_controller->OnOff();            
            break;
        
        case KEYPAD_NEXT_KEY:
            controller->led_controller->Brighter();
            break;
            
        case KEYPAD_PREV_KEY:
            controller->led_controller->Darker();
            break;
        
        case KEYPAD_SELECT_KEY:
            //  Open Alarm:
            controller->display_ctrl->Clear();
            controller->SetMachineState(GLOBAL_STATE_SETTER);
            data_setter->OpenSetter(ALARM_SETTER);
            return;
        
        case KEYPAD_BACK_KEY:
            //  Open Brightness:
            controller->display_ctrl->Clear();
            controller->SetMachineState(GLOBAL_STATE_SETTER);
            data_setter->OpenSetter(BRIGHTNESS_SETTER);
            return;
        
        case KEYPAD_OPTION_KEY:
            //  Open Clock:
            controller->display_ctrl->Clear();
            controller->SetMachineState(GLOBAL_STATE_SETTER);
            data_setter->OpenSetter(TIME_SETTER);
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
        
        case MENU_ITEM_LEDS:
            controller->SetMachineState(GLOBAL_STATE_LEDS);
            ProcessLedsDisplay(true);
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
        if (controller->buzzer_ctrl->UpdateToneAsync())
            controller->buzzer_ctrl->StopToneAsync();

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
void ProcessSongPlayState(int input)
{
    if (input == SONG_FINISHED)
    {
        controller->SetMachineState(GLOBAL_STATE_NORMAL);
        controller->display_ctrl->Clear();
        controller->SetDisplayingState(DISPLAY_DATETIME_STATE);
    }
}

//  ----------------------------------------------------------------------------
void ProcessLedsState(int input)
{
    switch (input)
    {
        case KEYPAD_0_KEY:
            controller->led_controller->OnOff();
            return;
          
        case KEYPAD_1_KEY:
            controller->led_controller->Red(0);
            return;
        
        case KEYPAD_2_KEY:
            controller->led_controller->Green(0);
            return;
        
        case KEYPAD_3_KEY:
            controller->led_controller->Blue(0);
            return;
        
        case KEYPAD_4_KEY:
            controller->led_controller->Red(2);
            return;
        
        case KEYPAD_5_KEY:
            controller->led_controller->Green(2);
            return;
        
        case KEYPAD_6_KEY:
            controller->led_controller->Blue(2);
            return;
        
        case KEYPAD_7_KEY:
            controller->led_controller->Red(4);
            return;

        case KEYPAD_8_KEY:
            controller->led_controller->Green(4);
            return;
        
        case KEYPAD_9_KEY:
            controller->led_controller->Blue(4);
            return;
        
        case KEYPAD_PREV_KEY:
            controller->led_controller->Darker();
            return;
        
        case KEYPAD_NEXT_KEY:
            controller->led_controller->Brighter();
            return;
        
        case KEYPAD_SELECT_KEY:
            controller->led_controller->White();
            return;
        
        case KEYPAD_BACK_KEY:
            controller->led_controller->Strobe();
            return;
        
        case KEYPAD_OPTION_KEY:
            controller->led_controller->Smooth();
            return;
        
        case KEYPAD_MENU_KEY:
            controller->display_ctrl->Clear();
            controller->SetMachineState(GLOBAL_STATE_NORMAL);
            controller->SetDisplayingState(DISPLAY_DATETIME_STATE);
            return;
        
        case KEYPAD_NO_KEY:
        default:
            return;
    }
}

//  ----------------------------------------------------------------------------
void ProcessFunctionalities()
{
    //  Przerwanie z powodu blokady serwisowej.
    if (controller->IsServiceLocked())
        return;
      
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
        else if (machine_state == GLOBAL_STATE_LEDS)
        {          
            ProcessLedsState(input_key);
        }
    }

    //  Tryby pozwalajace na 0 input.
    if (machine_state == GLOBAL_STATE_MESSAGE)
    {
        int message_output = controller->msg_ctrl->ProcessInput(input_key);
        ProcessMessageState(message_output);
    }
    else if (machine_state == GLOBAL_STATE_SONG_PLAY)
    {
        int song_output = controller->song_controller->ProcessInput(input_key);
        ProcessSongPlayState(song_output);
    }
    else if (machine_state == GLOBAL_STATE_VPLAYER)
    {
        //
    }
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
