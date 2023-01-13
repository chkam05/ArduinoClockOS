////////////////////////////////////////////////////////////////////////////////
//  COMMAND PROCESSOR
////////////////////////////////////////////////////////////////////////////////

#ifndef COMMAND_PROCESSOR_H
#define COMMAND_PROCESSOR_H

////////////////////////////////////////////////////////////////////////////////
//  *** INCLUDED LIBRARIES ***
////////////////////////////////////////////////////////////////////////////////

#include "global_controller.h"


////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class CommandProcessor
{
    private:
        GlobalController * controller;

        String  params_data     =   "";
        String  raw_data        =   "";
        int     params_idx_pos  =   0;

        //  Utility methods.
        void  Clear();
        bool  IsCharacterADigit(char c);
        int   ParseMultiNumberData(int *data_array, int data_size, int start_index = 0);
        int   ParseNumberData(int &value);
        bool  ValidateCommand(String command);

        //  Errors.
        void  NotifyConfigurationUpdated();
        void  RaiseInvalidCommandError();
        void  RaiseInvalidParameterError(String command);

        //  Management methods.
        int   ProcessAlarmGetCommand();
        int   ProcessBeepGetCommand();
        int   ProcessBrightnessGetCommand();
        int   ProcessDateGetCommand();
        int   ProcessIsInitializedCommand();
        int   ProcessTimeGetCommand();

        int   ProcessAlarmSetCommand();
        int   ProcessBeepSetCommand();
        int   ProcessBrightnessSetCommand();
        int   ProcessDateSetCommand();
        int   ProcessMessageCommand();
        int   ProcessTimeSetCommand();
        int   ProcessWeatherAddCommand();
        int   ProcessWeatherClearCommand();
        int   ProcessWeatherSetCommand();
    
    public:
        CommandProcessor(GlobalController * controller);

        int   ProcessCommand(String raw_data);
};


////////////////////////////////////////////////////////////////////////////////
//  *** PRIVATE UTILITY METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

//  Wyczyszczenie danych po zakonczeniu przetwarzania polecenia.
void CommandProcessor::Clear()
{
    params_data = "";
    raw_data = "";
}

//  ----------------------------------------------------------------------------
/* Sprawdzenie czy wprowadzony znak jest znakiem reprezentujacym liczbe.
 * @param c: Sprawdzany znak pod katem tego czy jest reprezentacja liczby.
 * @return: Informacja czy wprowadzony znak reprezentuje liczbe.
 */
bool CommandProcessor::IsCharacterADigit(char c)
{
    if (c >= 48 && c <=57)
        return true;
    return false;
}

//  ----------------------------------------------------------------------------
/* Konwersja ciaglych argumentow na argumenty liczbowe w postaci tablicy.
 * @param *data_array: Wskaznik do tablicy bedacej tablica wynikowa.
 * @param data_size: Wielkosc tablicy wynikowej.
 * @return: Ilosc wypelnionych pol w tablicy wynikowej.
 */
int CommandProcessor::ParseMultiNumberData(int *data_array, int data_size, int start_index = 0)
{
    //  Inicjalizacja zmiennych roboczych/wynikowych.
    int data_length = this->params_data.length();
    int step_index = 0;
    String worker = "";

    //  Przetworzenie danych wejsciowych na dane liczbowe.
    if (start_index < data_length)
    {
        for (int c = start_index; c <= data_length; c++)
        {
            if (this->IsCharacterADigit(this->params_data[c]))
                worker += this->params_data[c];

            else if (worker != "" && step_index < data_size)
            {
                data_array[step_index] = worker.toInt();
                step_index ++;
                worker = "";

                this->params_idx_pos = c;
            }
        }
    }

    return step_index;
}

//  ----------------------------------------------------------------------------
/* Konwersja ciaglych argumentow na argumenty liczbowe w postaci tablicy.
 * @param *data_array: Wskaznik do tablicy bedacej tablica wynikowa.
 * @param data_size: Wielkosc tablicy wynikowej.
 * @return: Ilosc wypelnionych pol w tablicy wynikowej.
 */
int CommandProcessor::ParseNumberData(int &value)
{
    //  Inicjalizacja zmiennych roboczych/wynikowych.
    int data_length = this->params_data.length();
    String worker = "";

    //  Przetworzenie danych wejsciowych na dane liczbowe.
    for (int c = 0; c <= data_length; c++)
    {
        if (this->IsCharacterADigit(this->params_data[c]))
            worker += this->params_data[c];

        else if (worker != "")
        {
            value = worker.toInt();
            return c;
        }
    }

    return 0;
}

//  ----------------------------------------------------------------------------
/* Sprawdzenie czy wprowadzone dane zawieraja okreslone polecenie.
 * @param command: Polecenie do sprawdzenia (czy istnieje w wprowadzonych danych).
 * @return: Informacja o tym czy polecenie zostalo wprowadzone.
 */
bool CommandProcessor::ValidateCommand(String command)
{
    //  Sprawdzenie czy polecenie zostalo poprawnie wprowadzone.
    if (this->raw_data.length() >= command.length())
        if (this->raw_data.substring(0, command.length()) == command)
        {
            //  Odczytanie danych zawierajacych parametry komendy.
            if (this->raw_data.length() > command.length() + 1)
                this->params_data = this->raw_data.substring(command.length() + 1);
            
            return true;
        }
            
    return false;
}

////////////////////////////////////////////////////////////////////////////////
//  *** PRIVATE ERRORS METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

//  Powiadomienie o poprawnym wykonaniu polecenia.
void CommandProcessor::NotifyConfigurationUpdated()
{
    this->controller->serial_ctrl->WriteRawData(
        "OK",
        this->controller->serial_ctrl->GetLastInputDevice());
}

//  ----------------------------------------------------------------------------
//  Wyswietlenie bledu - niepoprawne polecenie.
void CommandProcessor::RaiseInvalidCommandError()
{
    this->controller->serial_ctrl->WriteRawData(
        "Entered invalid command.",
        this->controller->serial_ctrl->GetLastInputDevice());
}

//  ----------------------------------------------------------------------------
/*  Wyswietlenie bledu - niepoprawne parametry polecenia.
 *  @param command: Polecenie.
 */
void CommandProcessor::RaiseInvalidParameterError(String command)
{
    this->controller->serial_ctrl->WriteRawData(
        "Entered invalid parameters for '" + command + "' command.",
        this->controller->serial_ctrl->GetLastInputDevice());
}

////////////////////////////////////////////////////////////////////////////////
//  *** PRIVATE GET MANAGEMENT METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

//  Przetworzenie polecenia pobrania ustawien alarmu.
int CommandProcessor::ProcessAlarmGetCommand()
{
    String output = String(this->controller->alarm->hour) + ":" + String(this->controller->alarm->minute);
    output += this->controller->alarm->IsEnabled() ? " ON" : " OFF";

    this->controller->serial_ctrl->WriteRawData(
        output,
        this->controller->serial_ctrl->GetLastInputDevice());
    
    return COMMAND_NONE;
}

//  ----------------------------------------------------------------------------
//  Przetworzenie polecenia pobrania ustawien brzeczyka godzinowego.
int CommandProcessor::ProcessBeepGetCommand()
{
    int value = this->controller->GetBuzzerHourNotifierInterval();

    this->controller->serial_ctrl->WriteRawData(
        (value == 0 ? "OFF" : String(value)),
        this->controller->serial_ctrl->GetLastInputDevice());
    
    return COMMAND_NONE;
}

//  ----------------------------------------------------------------------------
//  Przetworzenie polecenia pobrania ustawien jasnosci ekranu.
int CommandProcessor::ProcessBrightnessGetCommand()
{
    int value = this->controller->display_ctrl->GetBrightness();

    this->controller->serial_ctrl->WriteRawData(
        (this->controller->IsAutoBrightness() ? "AUTO" : String(value)),
        this->controller->serial_ctrl->GetLastInputDevice());

    return COMMAND_NONE;
}

//  ----------------------------------------------------------------------------
//  Przetworzenie polecenia pobrania ustawien daty.
int CommandProcessor::ProcessDateGetCommand()
{
    this->controller->serial_ctrl->WriteRawData(
        this->controller->clock_ctrl->GetDate("wDMY", '.'),
        this->controller->serial_ctrl->GetLastInputDevice());
    
    return COMMAND_NONE;
}

//  ----------------------------------------------------------------------------
//  Pobranie informacji czy oprogramowanie zostalo zainicjalizowane.
int CommandProcessor::ProcessIsInitializedCommand()
{
    this->controller->serial_ctrl->WriteRawData(
        (this->controller->IsInitialized() ? "YES" : "NO"),
        this->controller->serial_ctrl->GetLastInputDevice());
    
    return COMMAND_NONE;
}

//  ----------------------------------------------------------------------------
//  Przetworzenie polecenia pobrania ustawien czasu.
int CommandProcessor::ProcessTimeGetCommand()
{
    this->controller->serial_ctrl->WriteRawData(
        this->controller->clock_ctrl->GetTime("HMS", ':'),
        this->controller->serial_ctrl->GetLastInputDevice());
    
    return COMMAND_NONE;
}

////////////////////////////////////////////////////////////////////////////////
//  *** PRIVATE SET MANAGEMENT METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

//  Przetworzenie polecenia ustawienia alarmu.
int CommandProcessor::ProcessAlarmSetCommand()
{
    if (this->params_data == NULL || this->params_data == "")
    {
        this->RaiseInvalidParameterError("alarm set");
        return COMMAND_NONE;
    }

    if (this->params_data == "off" || this->params_data == "disable")
    {
        this->controller->DisableAlarm();
        this->NotifyConfigurationUpdated();
        return COMMAND_DISPLAY_DATETIME;
    }

    int *data_array = new int[2] {0, 0};
    int last_step = this->ParseMultiNumberData(data_array, 23);

    if (last_step >= 2)
    {
        int hour = max(0, min(23, data_array[0]));
        int min = max(0, min(59, data_array[1]));
        
        this->controller->SetAlarm(hour, min);
        this->NotifyConfigurationUpdated();
        return COMMAND_DISPLAY_DATETIME;
    }

    this->RaiseInvalidParameterError("alarm set");
    return COMMAND_NONE;
}

//  ----------------------------------------------------------------------------
//  Przetworzenie polecenia ustawienia brzeczyka godzinowego.
int CommandProcessor::ProcessBeepSetCommand()
{
    if (this->params_data == NULL || this->params_data == "")
    {
        this->RaiseInvalidParameterError("beep set");
        return COMMAND_NONE;
    }

    if (this->params_data == "off" || this->params_data == "disable")
    {
        this->controller->SetBuzzerHourNotifierInterval(0);
        this->NotifyConfigurationUpdated();
        return COMMAND_PROCESSED_OK;
    }
    else
    {
        int value = -1;
        int last_step = this->ParseNumberData(value);

        if (last_step > 0)
        {
            if (value == 0)
            {            
                this->controller->SetBuzzerHourNotifierInterval(0);
                this->NotifyConfigurationUpdated();
                return COMMAND_PROCESSED_OK;
            }

            else if (value == 1 || value == 3 || value == 6 || value == 12 || value == 24)
            {
                this->controller->SetBuzzerHourNotifierInterval(value);
                this->NotifyConfigurationUpdated();
                return COMMAND_PROCESSED_OK;
            }
        }
    }

    this->RaiseInvalidParameterError("beep set");
    return COMMAND_NONE;
}

//  ----------------------------------------------------------------------------
//  Przetworzenie polecenia ustawienia jasnosci ekranu.
int CommandProcessor::ProcessBrightnessSetCommand()
{
    if (this->params_data == NULL || this->params_data == "")
    {
        this->RaiseInvalidParameterError("brightness set");
        return COMMAND_NONE;
    }

    this->params_data.toLowerCase();

    if (this->params_data == "a" || this->params_data == "auto")
    {
        this->controller->SetAutoBrightness(true);
        this->NotifyConfigurationUpdated();
        return COMMAND_PROCESSED_OK;
    }
    else if (this->IsCharacterADigit(this->params_data[0]))
    {
        int brightness = max(DISPLAY_MIN_BRIGHTNESS, min(this->params_data[0] - 48, DISPLAY_MAX_BRIGHTNESS));
        this->controller->SetAutoBrightness(false);
        this->controller->display_ctrl->SetBrightness(brightness);
        this->NotifyConfigurationUpdated();
        return COMMAND_PROCESSED_OK;
    }
    
    this->RaiseInvalidParameterError("brightness set");
    return COMMAND_NONE;
}

//  ----------------------------------------------------------------------------
//  Przetworzenie polecenia ustawienia daty.
int CommandProcessor::ProcessDateSetCommand()
{
    if (this->params_data == NULL || this->params_data == "")
    {
        this->RaiseInvalidParameterError("date set");
        return COMMAND_NONE;
    }
    
    int *data_array = new int[4] {0, 0, 0, 0};
    int last_step = this->ParseMultiNumberData(data_array, 4);

    if (last_step == 4)
    {
        int day = max(1, min(31, data_array[0]));
        int week = max(1, min(7, data_array[1]));
        int month = max(1, min(12, data_array[2]));
        int year = max(2000, min(2035, data_array[3]));

        this->controller->SetDate(day, week, month, year);
        this->NotifyConfigurationUpdated();
        return COMMAND_DISPLAY_DATETIME;
    }
    else if (last_step == 3)
    {
        int day = max(1, min(31, data_array[0]));
        int month = max(1, min(12, data_array[1]));
        int year = max(2000, min(2035, data_array[2]));

        this->controller->SetDate(day, month, year);
        this->NotifyConfigurationUpdated();
        return COMMAND_DISPLAY_DATETIME;
    }
    
    this->RaiseInvalidParameterError("date set");
    return COMMAND_NONE;
}

//  ----------------------------------------------------------------------------
//  Przetworzenie polecenia wiadomosci.
int CommandProcessor::ProcessMessageCommand()
{
    if (this->params_data == NULL || this->params_data == "")
    {
        this->RaiseInvalidParameterError("date set");
        return COMMAND_NONE;
    }

    int setup_result = this->controller->msg_ctrl->SetupMessage(this->params_data);

    if (setup_result == MESSAGE_DISPLAYING)
        this->controller->SetMachineState(GLOBAL_STATE_MESSAGE);

    return COMMAND_PROCESSED_OK;
}

//  ----------------------------------------------------------------------------
//  Przetworzenie polecenia ustawienia czasu.
int CommandProcessor::ProcessTimeSetCommand()
{
    if (this->params_data == NULL || this->params_data == "")
    {
        this->RaiseInvalidParameterError("time set");
        return COMMAND_NONE;
    }
    
    int *data_array = new int[3] {0, 0, 0};
    int last_step = this->ParseMultiNumberData(data_array, 3);

    if (last_step >= 2)
    {
        int hour = max(0, min(23, data_array[0]));
        int min = max(0, min(59, data_array[1]));
        
        if (last_step >= 3)
        {
            int sec = max(0, min(59, data_array[1]));
            this->controller->SetTime(hour, min, sec);
        }
        else
        {
            this->controller->SetTime(hour, min);
        }

        this->NotifyConfigurationUpdated();
        return COMMAND_DISPLAY_DATETIME;
    }

    this->RaiseInvalidParameterError("time set");
    return COMMAND_NONE;
}

//  ----------------------------------------------------------------------------
//  Przetworzenie polecenia dodania prognozy pogody.
int CommandProcessor::ProcessWeatherAddCommand()
{
    if (this->params_data == NULL || this->params_data == "")
    {
        this->RaiseInvalidParameterError("weather add");
        return COMMAND_NONE;
    }

    int *date_array = new int[3] {0, 0, 0};
    int last_step = this->ParseMultiNumberData(date_array, 3);

    if (last_step == 3)
    {
        int *weather_array = new int[25] { 0 };
        last_step = this->ParseMultiNumberData(weather_array, 25, this->params_idx_pos);

        if (last_step > 0 && last_step <= 25)
        {
            this->controller->weather->AddWeather(
                this->controller->clock_ctrl->ValidateYear(date_array[0]),
                this->controller->clock_ctrl->ValidateMonth(date_array[1]),
                this->controller->clock_ctrl->ValidateDay(date_array[2], date_array[1], date_array[0]),
                weather_array,
                weather_array[0]);
            
            this->NotifyConfigurationUpdated();
            return COMMAND_PROCESSED_OK;
        }
    }

    this->RaiseInvalidParameterError("weather add");
    return COMMAND_NONE;
}

//  ----------------------------------------------------------------------------
//  Przetworzenie polecenia ustawienia prognozy pogody na dzisiaj.
int CommandProcessor::ProcessWeatherClearCommand()
{
    this->controller->weather->ClearWeather();
    this->NotifyConfigurationUpdated();
    return COMMAND_PROCESSED_OK;
}

////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Konstruktor klasy przetwarzajacej polecenia.
 *  @param controller: Globalny kontroler.
 */
CommandProcessor::CommandProcessor(GlobalController * controller)
{
    this->controller = controller;
}

//  ----------------------------------------------------------------------------
/* Przetworzenie wprowadzonego polecenia wraz z argumentami i wykonanie okreslonego dzialania.
 * @return: Numer/identyfikator wprowadzonego polecenia.
 */
int CommandProcessor::ProcessCommand(String raw_data)
{
    //  Zwrocenie -1 w przypadku braku polecenia.
    if (raw_data == NULL || raw_data == "")
        return false;
    
    //  Inicjalizacja zmiennych roboczych/wynikowych.
    this->params_data = "";
    this->raw_data = raw_data;

    if (this->ValidateCommand("/alarm get"))
        return this->ProcessAlarmGetCommand();
        
    else if (this->ValidateCommand("/alarm set"))
        return this->ProcessAlarmSetCommand();
    
    else if (this->ValidateCommand("/beep get"))
        return this->ProcessBeepGetCommand();
        
    else if (this->ValidateCommand("/beep set"))
        return this->ProcessBeepSetCommand();
    
    else if (this->ValidateCommand("/brightness get"))
        return this->ProcessBrightnessGetCommand();
    
    else if (this->ValidateCommand("/brightness set"))
        return this->ProcessBrightnessSetCommand();

    else if (this->ValidateCommand("/date get"))
        return this->ProcessDateGetCommand();
        
    else if (this->ValidateCommand("/date set"))
        return this->ProcessDateSetCommand();
    
    else if (this->ValidateCommand("/init"))
        return this->ProcessIsInitializedCommand();
    
    else if (this->ValidateCommand("/msg"))
        return this->ProcessMessageCommand();
    
    else if (this->ValidateCommand("/time get"))
        return this->ProcessTimeGetCommand();

    else if (this->ValidateCommand("/time set"))
        return this->ProcessTimeSetCommand();
    
    else if (this->ValidateCommand("/weather add"))
        return this->ProcessWeatherAddCommand();
    
    else if (this->ValidateCommand("/weather clear"))
        return this->ProcessWeatherClearCommand();

    else
        RaiseInvalidCommandError();
    
    this->Clear();
    return COMMAND_NONE;
}

#endif