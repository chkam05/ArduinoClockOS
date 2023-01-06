////////////////////////////////////////////////////////////////////////////////
//  DATA SETTER
////////////////////////////////////////////////////////////////////////////////

#ifndef DATA_SETTER_H
#define DATA_SETTER_H

////////////////////////////////////////////////////////////////////////////////
//  *** INCLUDED LIBRARIES ***
////////////////////////////////////////////////////////////////////////////////

#include "global_controller.h"


////////////////////////////////////////////////////////////////////////////////
//  *** CONFIGURATION ***
////////////////////////////////////////////////////////////////////////////////

#define SETTERS           5
#define DATE_SETTER       0
#define TIME_SETTER       1
#define BRIGHTNESS_SETTER 2
#define BEEP_SETTER       3
#define ALARM_SETTER      4

#define SETTER_NOTHING    -1
#define SETTER_SAVE       -2
#define SETTER_EXIT       -3
#define SETTER_FULL_EXIT  -4
#define SETTER_OFF        -5

#define SETTER_DATE_POSITIONS       6
#define SETTER_TIME_POSITIONS       5
#define SETTER_BRIGHNESS_POSITIONS  11
#define SETTER_BRIGHNESS_AUTO       10
#define SETTER_BEEP_POSITIONS       7
#define SETTER_ALARM_POSITIONS      5

#define SETTER_INPUT_MAX            2

const String SETTER_AUTO_STR = "<AUTO>";
const String SETTER_SAVE_STR = "<SAVE>";
const String SETTER_EXIT_STR = "<EXIT>";
const String SETTER_SET_STR = "<SET>";
const String SETTER_OFF_STR = "<OFF>";


////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class DataSetter
{
    private:
        GlobalController  * controller;

        int     mode                  = 0;
        bool    allow_keyboard_input  = false;
        String  current_input         = "";
        int   * data                  = new int[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        bool    edit_started          = true;
        int     input_index           = 0;
        int     setter_position       = 1;
        bool    underline_blink       = false;

        DisplayString * data_dsp_string;

        int     GetBeepValuePosition();
        int     GetBrightnessValuePosition();

        void    DisplayData(DisplayController * dsp_ctrl);
        void    DisplayTitle(DisplayController * dsp_ctrl);
        void    DisplaySetter(bool clear = false, bool only_data_update = false);
        String  GetEditableString(int pos, int spaces);

        int   NavigateForward();
        int   NavigateBack();
        int   NavigatePrevious();
        int   NavigateNext();

        bool  IsManualInputAllowed();
        int   NavigateManualInput(char input_key);
        void  SetManualInputValue();
        void  ResetManualInput();
    
    public:
        DataSetter(GlobalController * controller);
        
        int   GetMode();

        void  OpenSetter(int mode);
        int   ProcessInput(int input);
        void  UpdateDisplay();
};

////////////////////////////////////////////////////////////////////////////////
//  *** PRIVATE METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Konwersja wartosci brzeczyka godzinowego na indeks opcji w konfiguracji.
 *  @return: Indeks opcji w konfiguracji.
 */
int DataSetter::GetBeepValuePosition()
{
    switch (this->controller->GetBuzzerHourNotifierInterval())
    {
        case 1:
            return 2;
        case 3:
            return 3;
        case 6:
            return 4;
        case 12:
            return 5;
        case 24:
            return 6;
        default:
            return 1;
    }

    return 1;
}

//  ----------------------------------------------------------------------------
/*  Konwersja wartosci jasnosci na indeks opcji w konfiguracji.
 *  @return: Indeks opcji w konfiguracji.
 */
int DataSetter::GetBrightnessValuePosition()
{
    return controller->IsAutoBrightness() ? SETTER_BRIGHNESS_AUTO : controller->display_ctrl->GetBrightness() + 1;
}

//  ----------------------------------------------------------------------------
/*  Przejscie do nastepnego podmenu ustawien lub wybranie opcji w konfiguracji.
 *  @return: Indeks wybranego podmenu, opcji, badz 0 powrot.
 */
int DataSetter::NavigateForward()
{
    int value = this->data[this->setter_position];
    int pos = this->setter_position;
    
    switch (this->mode)
    {
        case DATE_SETTER:
            if (value == SETTER_SAVE)
            {
                int day = this->data[1];
                int mon = this->data[2];
                int year = ("20" + (this->data[3] < 10 ? "0" + String(this->data[3]) : String(this->data[3]))).toInt();
                int week = this->data[4];
                controller->SetDate(day, week, mon, year);
                return SETTER_EXIT;
            }
            else if (value == SETTER_EXIT)
                return SETTER_EXIT;

            break;
        
        case TIME_SETTER:
            if (value == SETTER_SAVE)
            {
                int hour = this->data[1];
                int mins = this->data[2];
                int secs = this->data[3];
                controller->SetTime(hour, mins, secs);
                return SETTER_EXIT;
            }
            else if (value == SETTER_EXIT)
                return SETTER_EXIT;
            
            break;
        
        case BRIGHTNESS_SETTER:
            if (value >= 0 && value <= 8)
            {
                controller->SetBrightness(value);
                return SETTER_EXIT;
            }
            else if (value == SETTER_BRIGHNESS_AUTO)
            {
                controller->SetAutoBrightness(true);
                return SETTER_EXIT;
            }
            else if (value == SETTER_EXIT)
                return SETTER_EXIT;

            break;
        
        case BEEP_SETTER:
            if (pos >= 1 && pos <= 6)
            {
                controller->SetBuzzerHourNotifierInterval(value);
                return SETTER_EXIT;
            }
            else if (value == SETTER_EXIT)
                return SETTER_EXIT;

            break;
        
        case ALARM_SETTER:
            if (value == SETTER_OFF)
            {
                this->controller->DisableAlarm();
                return SETTER_EXIT;
            }
            if (value == SETTER_SAVE)
            {
                int hour_alarm = this->data[1];
                int mins_alarm = this->data[2];
                this->controller->SetAlarm(hour_alarm, mins_alarm);
                return SETTER_EXIT;
            }
            else if (value == SETTER_EXIT)
                return SETTER_EXIT;
            
            break;
    }

    return SETTER_NOTHING;
}

//  ----------------------------------------------------------------------------
/*  Powrot do poprzedniego menu.
 *  @return: Indeks wybranego menu, opcji, badz exit.
 */
int DataSetter::NavigateBack()
{
    return SETTER_EXIT;
}

//  ----------------------------------------------------------------------------
//  Przejscie do poprzedniego elementu na liscie opcji konfiguracji.
int DataSetter::NavigatePrevious()
{
    if (this->IsManualInputAllowed() && this->input_index > 0)
        this->SetManualInputValue();
    
    if (this->setter_position <= 1)
        this->setter_position = data[0];
    else
        this->setter_position = this->setter_position - 1;

    this->edit_started = true;
    this->ResetManualInput();
    this->DisplaySetter(false, true);

    return SETTER_NOTHING;
}

//  ----------------------------------------------------------------------------
//  Przejscie do nastepnego elementu na liscie opcji konfiguracji.
int DataSetter::NavigateNext()
{
    if (this->IsManualInputAllowed() && this->input_index > 0)
        this->SetManualInputValue();

    if (this->setter_position >= data[0])
        this->setter_position = 1;
    else
        this->setter_position = this->setter_position + 1;

    this->edit_started = true;
    this->ResetManualInput();
    this->DisplaySetter(false, true);

    return SETTER_NOTHING;
}

//  ----------------------------------------------------------------------------
/*  Sprawdzenie czy dozwolone jest wpisywanie wartosci z klawiatury numerycznej.
 *  @return: True - dozwolone wpisanie wartosci z klawiatury numerycznej; False - w innym wypadku.
 */
bool DataSetter::IsManualInputAllowed()
{
    switch (this->mode)
    {
        case DATE_SETTER:
            return this->setter_position >= 1 && this->setter_position <= 4;

        case TIME_SETTER:
            return this->setter_position >= 1 && this->setter_position <= 3;
        
        case ALARM_SETTER:
            return this->setter_position >= 1 && this->setter_position <= 2;
    }

    return false;
}

//  ----------------------------------------------------------------------------
/*  Przetworzenie danych wejsciowych uzytkownika wprowadzonych z klawiatury numerycznej.
 *  @param input: Dane wejsciowe z klawiatury numerycznej (wcisniety klawisz 0..9).
 *  @return: Indeks wykonanego procesu przez konfiguratora.
 */
int DataSetter::NavigateManualInput(char input_key)
{
    if (this->IsManualInputAllowed() && this->input_index < SETTER_INPUT_MAX)
    {
        this->input_index = this->input_index + 1;
        this->current_input = this->current_input + String(input_key);

        if (this->input_index == 2 || (this->mode == DATE_SETTER && this->setter_position == 4))
        {
            this->SetManualInputValue();
            this->ResetManualInput();

            if (((this->mode == DATE_SETTER || this->mode == TIME_SETTER) && this->setter_position < 3) 
                    || (this->mode == ALARM_SETTER && this->setter_position < 2))
                return this->NavigateNext();
            else
                this->edit_started = false;          
        }

        this->DisplaySetter(false, true);
    }

    return SETTER_NOTHING;
}

//  ----------------------------------------------------------------------------
//  Przetworzenie i zatwierdzenie danych wejsciowych uzytkownika wprowadzonych z klawiatury numerycznej.
void DataSetter::SetManualInputValue()
{
    if (this->current_input == "" && this->current_input.length() <= 0)
        return;
    
    this->edit_started = true;

    int pos = this->setter_position;
    int value = this->current_input.toInt();

    switch (this->mode)
    {
        case DATE_SETTER:
            if (pos == 4)
                this->data[4] = controller->clock_ctrl->ValidateWeek(value);

            else if (pos == 3)
            {
                int year = ("20" + this->current_input).toInt();
                this->data[3] = String(controller->clock_ctrl->ValidateYear(year)).substring(2).toInt();
            }
                
            else if (pos == 2)
                this->data[2] = controller->clock_ctrl->ValidateMonth(value);
            
            else if (pos == 1)
                this->data[1] = controller->clock_ctrl->ValidateDay(value, this->data[2], this->data[3]);
                
            if (pos == 2 || pos == 3)
                this->data[1] = controller->clock_ctrl->ValidateDay(this->data[1], this->data[2], this->data[3]);

            break;

        case TIME_SETTER:
            if (pos == 3)
                this->data[3] = controller->clock_ctrl->ValidateMinSec(value);
            
            else if (pos == 2)
                this->data[2] = controller->clock_ctrl->ValidateMinSec(value);
            
            else if (pos == 1)
                this->data[1] = controller->clock_ctrl->ValidateHour(value);

            break;
        
        case ALARM_SETTER:
            if (pos == 2)
                this->data[2] = controller->clock_ctrl->ValidateMinSec(value);
            
            else if (pos == 1)
                this->data[1] = controller->clock_ctrl->ValidateHour(value);

            break;
    }
}

//  ----------------------------------------------------------------------------
//  Reset danych tymczasowych uzywanych podczas wprowadzania danych z klawiatury numerycznej.
void DataSetter::ResetManualInput()
{
    this->input_index = 0;
    this->current_input = "";    
}

//  ----------------------------------------------------------------------------
/*  Wygenerowanie pola edycji tekstu wraz z wpisanym tekstem i wolnymi polami.
 *  @param pos: Pozycja kursora.
 *  @param spaces: Puste miejsca.
 *  @return: Pole edycji tekstu wraz z wpisanym tekstem i wolnymi polami.
 */
String DataSetter::GetEditableString(int pos, int spaces)
{
    String display_string = "";
    String underlined = underline_blink
        ? (spaces >= 2 ? "    " : (spaces == 1 ? this->current_input + "  " : this->current_input))
        : (spaces >= 2 ? "__" : (spaces == 1 ? this->current_input + "_" : this->current_input));

    if (this->mode == DATE_SETTER)
    {
        display_string = pos == 0 && this->edit_started ? underlined : ((this->data[1] < 10) ? "0" : "") + String(this->data[1]);
        display_string += ".";
        display_string += pos == 1 && this->edit_started ? underlined : ((this->data[2] < 10) ? "0" : "") + String(this->data[2]);
        display_string += ".";
        display_string += pos == 2 && this->edit_started ? underlined : ((this->data[3] < 10) ? "0" : "") + String(this->data[3]);
    }
    else if (this->mode == TIME_SETTER)
    {
        display_string = pos == 0 && this->edit_started ? underlined : ((this->data[1] < 10) ? "0" : "") + String(this->data[1]);
        display_string += ":";
        display_string += pos == 1 && this->edit_started ? underlined : ((this->data[2] < 10) ? "0" : "") + String(this->data[2]);
        display_string += ":";
        display_string += pos == 2 && this->edit_started ? underlined : ((this->data[3] < 10) ? "0" : "") + String(this->data[3]);
    }
    else if (this->mode == ALARM_SETTER)
    {
        display_string = pos == 0 && this->edit_started ? underlined : ((this->data[1] < 10) ? "0" : "") + String(this->data[1]);
        display_string += ":";
        display_string += pos == 1 && this->edit_started ? underlined : ((this->data[2] < 10) ? "0" : "") + String(this->data[2]);
    }

    return display_string;
}

//  ----------------------------------------------------------------------------
/*  Wyswietlenie tylko czesci danych / wybranej opcji konfiguratora na ekranie.
 *  @param dsp_ctrl: Kontroler wyswietlacza.
 */
void DataSetter::DisplayData(DisplayController * dsp_ctrl)
{
    int pos = this->setter_position;
    String display_string = "";

    switch (this->mode)
    {
        case DATE_SETTER:
            if (pos >= 1 && pos <= 3)
                display_string = this->GetEditableString(pos - 1, min(2, max(0, 2 - input_index)));
                
            else if (pos == 4)
                display_string = "<" + week_names[max(0, min(this->data[4]-1, 6))] + ">";
            
            else if (pos == 5)
                display_string = SETTER_SAVE_STR;
            
            else if (pos == 6)
                display_string = SETTER_EXIT_STR;
                            
            break;
        
        case TIME_SETTER:
            if (pos >= 1 && pos <= 3)
                display_string = this->GetEditableString(pos - 1, min(2, max(0, 2 - input_index)));

            else if (pos == 4)
                display_string = SETTER_SAVE_STR;
            
            else if (pos == 5)
                display_string = SETTER_EXIT_STR;

            break;
        
        case BRIGHTNESS_SETTER:
            if (pos >= 1 && pos <= 9)
            {
                display_string = "<" + String(this->data[setter_position]) + ">";
            }
            else if (pos == 10)
                display_string = SETTER_AUTO_STR;

            else if (pos == 11)
                display_string = SETTER_EXIT_STR;
            
            break;
        
        case BEEP_SETTER:
            if (pos == 1)
                display_string = SETTER_OFF_STR;
            
            else if (pos >= 2 && pos <= 6)
                display_string = "<" + String(data[pos]) + "h>";
            
            else if (pos == 7)
                display_string = SETTER_EXIT_STR;

            break;
        
        case ALARM_SETTER:
            if (pos >= 1 && pos <= 2)
                display_string = this->GetEditableString(pos - 1, min(2, max(0, 2 - input_index)));
            
            else if (pos == 3)
                display_string = SETTER_OFF_STR;

            else if (pos == 4)
                display_string = SETTER_SET_STR;
            
            else if (pos == 5)
                display_string = SETTER_EXIT_STR;

            break;
    }

    this->data_dsp_string->text = display_string;
    dsp_ctrl->PrintDS(data_dsp_string);
}

//  ----------------------------------------------------------------------------
/*  Wyswietlenie tylko czesci tytulu konfiguratora na ekranie.
 *  @param dsp_ctrl: Kontroler wyswietlacza.
 */
void DataSetter::DisplayTitle(DisplayController * dsp_ctrl)
{
    int _text_offset = 10;

    switch (this->mode)
    {
        case DATE_SETTER:
            dsp_ctrl->DrawSprite(SPRITE_CALENDAR, 0, 0);
            dsp_ctrl->PrintText(0, _text_offset, "Date");
            break;
        
        case TIME_SETTER:
            dsp_ctrl->DrawSprite(SPRITE_CLOCK, 0, 0);
            dsp_ctrl->PrintText(0, _text_offset, "Time");
            break;
        
        case BRIGHTNESS_SETTER:
            dsp_ctrl->DrawSprite(SPRITE_BRIGHTNESS, 0, 0);
            dsp_ctrl->PrintText(0, _text_offset, "Bright...");
            break;
        
        case BEEP_SETTER:
            dsp_ctrl->DrawSprite(SPRITE_MUSIC, 0, 0);
            dsp_ctrl->PrintText(0, _text_offset, "Beep");
            break;
        
        case ALARM_SETTER:
            dsp_ctrl->DrawSprite(SPRITE_ALARM, 0, 0);
            dsp_ctrl->PrintText(0, _text_offset, "Alarm");
            break;
    }
}

//  ----------------------------------------------------------------------------
/*  Wyswietlenie konfiguratora na ekranie badz jego wyczyszczenie.
 *  @param clear: True - wyczyszczenie ekranu; False - w innym wypadku.
 *  @param only_data_update: True - aktualizacja tylko wybranej opcji na ekranie; False - w innym wypadku.
 */
void DataSetter::DisplaySetter(bool clear = false, bool only_data_update = false)
{
    DisplayController * dsp_ctrl = this->controller->display_ctrl;

    if (clear)
        dsp_ctrl->Clear();

    if (!only_data_update)
        this->DisplayTitle(dsp_ctrl);
    
    this->DisplayData(dsp_ctrl);
}

////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Konstruktor klasy konfiguratora.
 *  @param controller: Globalny kontroler.
 */
DataSetter::DataSetter(GlobalController * controller)
{
    this->controller = controller;

    this->data_dsp_string = new DisplayString(0, TEXT_ALIGN_RIGHT, "");
    this->data_dsp_string->offset = 1;
    this->data_dsp_string->_xpos = this->controller->display_ctrl->GetWidth()-1;
}

//  ----------------------------------------------------------------------------
/*  Pobranie indeksu aktualnie modyfikowanego ustawienia.
 *  @return: Indeks aktualnie modyfikowanego ustawienia.
 */
int DataSetter::GetMode()
{
    return this->mode;
}

//  ----------------------------------------------------------------------------
/*  Otwarcie konfiguratora i ustawienie indeksu ustawienia do modyfikacji.
 *  @param mode: Indeks ustawienia do modyfikacji.
 */
void DataSetter::OpenSetter(int mode)
{
    this->mode = mode % SETTERS;
    this->setter_position = 1;

    Time _date_time = controller->clock_ctrl->Now();

    switch (this->mode)
    {
        case DATE_SETTER:
            this->data[0] = SETTER_DATE_POSITIONS;
            this->data[1] = _date_time.date;
            this->data[2] = _date_time.mon;
            this->data[3] = String(_date_time.year).substring(2).toInt();
            this->data[4] = _date_time.dow;
            this->data[5] = SETTER_SAVE;
            this->data[6] = SETTER_EXIT;
            this->allow_keyboard_input = true;
            break;
        
        case TIME_SETTER:
            this->data[0] = SETTER_TIME_POSITIONS;
            this->data[1] = _date_time.hour;
            this->data[2] = _date_time.min;
            this->data[3] = _date_time.sec;
            this->data[4] = SETTER_SAVE;
            this->data[5] = SETTER_EXIT;
            this->allow_keyboard_input = true;
            break;
        
        case BRIGHTNESS_SETTER:
            this->data[0] = SETTER_BRIGHNESS_POSITIONS;
            this->data[1] = 0;
            this->data[2] = 1;
            this->data[3] = 2;
            this->data[4] = 3;
            this->data[5] = 4;
            this->data[6] = 5;
            this->data[7] = 6;
            this->data[8] = 7;
            this->data[9] = 8;
            this->data[10] = SETTER_BRIGHNESS_AUTO;
            this->data[11] = SETTER_EXIT;
            this->allow_keyboard_input = false;
            this->setter_position = this->GetBrightnessValuePosition();
            break;
        
        case BEEP_SETTER:
            this->data[0] = SETTER_BEEP_POSITIONS;
            this->data[1] = 0;
            this->data[2] = 1;
            this->data[3] = 3;
            this->data[4] = 6;
            this->data[5] = 12;
            this->data[6] = 24;
            this->data[7] = SETTER_EXIT;
            this->allow_keyboard_input = false;
            this->setter_position = this->GetBeepValuePosition();
            break;
        
        case ALARM_SETTER:
            this->data[0] = SETTER_ALARM_POSITIONS;
            this->data[1] = this->controller->alarm->hour;
            this->data[2] = this->controller->alarm->minute;
            this->data[3] = SETTER_OFF;
            this->data[4] = SETTER_SAVE;
            this->data[5] = SETTER_EXIT;            
            this->allow_keyboard_input = true;
            break;
    }

    this->DisplaySetter();
}

//  ----------------------------------------------------------------------------
/*  Przetworzenie danych wejsciowych uzytkownika wprowadzonych z klawiatury.
 *  @param input: Dane wejsciowe z klawiatury (wcisniety klawisz).
 *  @return: Indeks wykonanego procesu przez konfiguratora.
 */
int DataSetter::ProcessInput(int input)
{
    if (this->allow_keyboard_input && input >= KEYPAD_0_KEY && input <= KEYPAD_9_KEY)
        return this->NavigateManualInput(input);

    switch (input)
    {
        case KEYPAD_SELECT_KEY:
            return this->NavigateForward();
        
        case KEYPAD_BACK_KEY:
            return this->NavigateBack();
        
        case KEYPAD_MENU_KEY:
            this->controller->display_ctrl->Clear();
            return SETTER_FULL_EXIT;
        
        case KEYPAD_NEXT_KEY:
            return this->NavigateNext();
        
        case KEYPAD_PREV_KEY:
            return this->NavigatePrevious();
    }

    return SETTER_NOTHING;
}

//  ----------------------------------------------------------------------------
//  Odswiezenie ekranu i ponowne wyswietlenie konfiguratora.
void DataSetter::UpdateDisplay()
{
    if (this->IsManualInputAllowed() && this->edit_started)
    {
        bool _blink = this->controller->clock_ctrl->GetBlink();

        if (_blink != this->underline_blink)
            this->underline_blink = _blink;
        
        this->DisplaySetter(false, true);
    }
}

#endif
