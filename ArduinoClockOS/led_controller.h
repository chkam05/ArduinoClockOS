////////////////////////////////////////////////////////////////////////////////
//  LED CONTROLLER
////////////////////////////////////////////////////////////////////////////////

#ifndef LED_CONTROLLER_H
#define LED_CONTROLLER_H

////////////////////////////////////////////////////////////////////////////////
//  *** INCLUDED LIBRARIES ***
////////////////////////////////////////////////////////////////////////////////

#include "ir_controller.h"


////////////////////////////////////////////////////////////////////////////////
//  *** CONFIGURATION ***
////////////////////////////////////////////////////////////////////////////////

#define LED_COMMAND_OK    0
#define LED_COMMAND_BAD   1

#define IR_LDS_ON       0xF807FF00
#define IR_LDS_OFF      0xF906FF00

#define IR_LDS_WHITE    0xF40BFF00
#define IR_LDS_RED_0    0xF609FF00
#define IR_LDS_RED_1    0xF20DFF00
#define IR_LDS_RED_2    0xEA15FF00
#define IR_LDS_RED_3    0xE619FF00
#define IR_LDS_RED_4    0xEE11FF00
#define IR_LDS_GREEN_0  0xF708FF00
#define IR_LDS_GREEN_1  0xF30CFF00
#define IR_LDS_GREEN_2  0xEB14FF00
#define IR_LDS_GREEN_3  0xE718FF00
#define IR_LDS_GREEN_4  0xEF10FF00
#define IR_LDS_BLUE_0   0xF50AFF00
#define IR_LDS_BLUE_1   0xF10EFF00
#define IR_LDS_BLUE_2   0xE916FF00
#define IR_LDS_BLUE_3   0xE51AFF00
#define IR_LDS_BLUE_4   0xED12FF00

#define IR_LDS_DOWN     0xFB04FF00
#define IR_LDS_UP       0xFA05FF00
#define IR_LDS_FLASH    0xF00FFF00
#define IR_LDS_STROBE   0xE817FF00
#define IR_LDS_FADE     0xE41BFF00
#define IR_LDS_SMOOTH   0xEC13FF00


////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class LedController
{
    private:
        IRController * ir_controller;

        String  color_name;
        bool    on_off_state  = false;
        bool    has_changed   = true;

        bool ParseColorHue(String command, int &value);
        void SendIRCommand(uint32_t data);

    public:
        LedController(IRController * ir_controller, String color_name = "White");

        int   ProcessCommand(String command);
        int   On();
        int   Off();
        int   OnOff();
        int   White();
        int   Red(int hue);
        int   Green(int hue);
        int   Blue(int hue);
        int   Flash();
        int   Strobe();
        int   Fade();
        int   Smooth();
        int   Brighter();
        int   Darker();

        String  GetName();
        void    SetName(String color_name);
        bool    HasChanged();
};


////////////////////////////////////////////////////////////////////////////////
//  *** PRIVATE METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

bool LedController::ParseColorHue(String command, int &value)
{
    char last_char = command[command.length()-1];
    
    if (last_char >= 48 && last_char <=52)
    {
        value = String(last_char).toInt();
        return true;
    }

    return false;
}

//  ----------------------------------------------------------------------------
void LedController::SendIRCommand(uint32_t data)
{
    this->ir_controller->Send(data);
    this->has_changed = true;
}


////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

//  Konstruktor klasy modulu kontrolera tasm led.
LedController::LedController(IRController * ir_controller, String color_name = "White")
{
    this->ir_controller = ir_controller;
    this->color_name = color_name;
}

//  ----------------------------------------------------------------------------
int LedController::ProcessCommand(String command)
{
    command.toLowerCase();
    int hue = -1;

    if (command == "on")
        return this->On();

    else if (command == "off")
        return this->Off();
    
    else if (command == "flash")
        return this->Flash();
    
    else if (command == "strobe")
        return this->Strobe();
    
    else if (command == "fade")
        return this->Fade();
    
    else if (command == "smooth")
        return this->Smooth();
    
    else if (command == "+")
        return this->Brighter();
    
    else if (command == "-")
        return this->Darker();

    else if (command == "w" || command == "white")
        return this->White();
        
    else if ((command.startsWith("r") || command.startsWith("red")) && this->ParseColorHue(command, hue))
        return this->Red(hue);

    else if ((command.startsWith("g") || command.startsWith("green")) && this->ParseColorHue(command, hue))
        return this->Green(hue);
    
    else if ((command.startsWith("b") || command.startsWith("blue")) && this->ParseColorHue(command, hue))
        return this->Blue(hue);
    
    return LED_COMMAND_BAD;
}

//  ----------------------------------------------------------------------------
int LedController::On()
{  
    this->SendIRCommand(IR_LDS_ON);
    return LED_COMMAND_OK;
}

//  ----------------------------------------------------------------------------
int LedController::Off()
{
    this->SendIRCommand(IR_LDS_OFF);
    return LED_COMMAND_OK;
}

//  ----------------------------------------------------------------------------
int LedController::OnOff()
{
    if (this->on_off_state)
    {
        this->Off();
        this->on_off_state = false;
    }
    else
    {
        this->On();
        this->on_off_state = true;
    }
    return LED_COMMAND_OK;
}

//  ----------------------------------------------------------------------------
int LedController::White()
{
    this->SendIRCommand(IR_LDS_WHITE);
    this->SetName("White");
    return LED_COMMAND_OK;
}

//  ----------------------------------------------------------------------------
int LedController::Red(int hue)
{
    switch(hue)
    {
        case 0:
            this->SendIRCommand(IR_LDS_RED_0);
            this->SetName("Red");
            break;

        case 1:
            this->SendIRCommand(IR_LDS_RED_1);
            this->SetName("Tomato");
            break;

        case 2:
            this->SendIRCommand(IR_LDS_RED_2);
            this->SetName("Orange");
            break;

        case 3:
            this->SendIRCommand(IR_LDS_RED_3);
            this->SetName("Gold");
            break;

        case 4:
            this->SendIRCommand(IR_LDS_RED_4);
            this->SetName("Yellow");
            break;
        
        default:
            return LED_COMMAND_BAD;
    }

    return LED_COMMAND_OK;
}

//  ----------------------------------------------------------------------------
int LedController::Green(int hue)
{
    switch(hue)
    {
        case 0:
            this->SendIRCommand(IR_LDS_GREEN_0);
            this->SetName("Green");
            break;

        case 1:
            this->SendIRCommand(IR_LDS_GREEN_1);
            this->SetName("Mint");
            break;

        case 2:
            this->SendIRCommand(IR_LDS_GREEN_2);
            this->SetName("Sea");
            break;

        case 3:
            this->SendIRCommand(IR_LDS_GREEN_3);
            this->SetName("Teal");
            break;

        case 4:
            this->SendIRCommand(IR_LDS_GREEN_4);
            this->SetName("Aqua");
            break;
        
        default:
            return LED_COMMAND_BAD;
    }

    return LED_COMMAND_OK;
}

//  ----------------------------------------------------------------------------
int LedController::Blue(int hue)
{
    switch(hue)
    {
        case 0:
            this->SendIRCommand(IR_LDS_BLUE_0);
            this->SetName("Blue");
            break;

        case 1:
            this->SendIRCommand(IR_LDS_BLUE_1);
            this->SetName("Purple");
            break;

        case 2:
            this->SendIRCommand(IR_LDS_BLUE_2);
            this->SetName("Violet");
            break;

        case 3:
            this->SendIRCommand(IR_LDS_BLUE_3);
            this->SetName("Fuchsia");
            break;

        case 4:
            this->SendIRCommand(IR_LDS_BLUE_4);
            this->SetName("Pink");
            break;
        
        default:
            return LED_COMMAND_BAD;
    }

    return LED_COMMAND_OK;
}

//  ----------------------------------------------------------------------------
int LedController::Brighter()
{
    this->SendIRCommand(IR_LDS_UP);
    return LED_COMMAND_OK;
}

//  ----------------------------------------------------------------------------
int LedController::Darker()
{
    this->SendIRCommand(IR_LDS_DOWN);
    return LED_COMMAND_OK;
}

//  ----------------------------------------------------------------------------
int LedController::Flash()
{
    this->SendIRCommand(IR_LDS_FLASH);
    return LED_COMMAND_OK;
}

//  ----------------------------------------------------------------------------
int LedController::Strobe()
{
    this->SendIRCommand(IR_LDS_STROBE);
    return LED_COMMAND_OK;
}

//  ----------------------------------------------------------------------------
int LedController::Fade()
{
    this->SendIRCommand(IR_LDS_FADE);
    return LED_COMMAND_OK;
}

//  ----------------------------------------------------------------------------
int LedController::Smooth()
{
    this->SendIRCommand(IR_LDS_SMOOTH);
    return LED_COMMAND_OK;
}

//  ----------------------------------------------------------------------------
String LedController::GetName()
{
    if (!this->on_off_state)
        return "Off";
    return this->color_name;
}

//  ----------------------------------------------------------------------------
void LedController::SetName(String color_name)
{
    this->color_name = color_name == "Off" ? "White" : color_name;
    this->has_changed = true;
}

//  ----------------------------------------------------------------------------
bool LedController::HasChanged()
{
    bool current_state = this->has_changed;
    this->has_changed = false;
    return current_state;
}

#endif