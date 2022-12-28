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

        String  params_data   =   "";
        String  raw_data      =   "";

        //  Utility methods.
        void  Clear();
        bool  IsCharacterADigit(char c);
        int   ParseMultiNumberData(int *data_array, int data_size);
        bool  ValidateCommand(String command);

        //  Errors.
        void  RaiseInvalidCommandError();
        void  RaiseInvalidParameterError(String command);

        //  Management methods.
        int   ProcessBrightnessSetCommand();
        int   ProcessDateSetCommand();
        int   ProcessTimeSetCommand();
    
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
int CommandProcessor::ParseMultiNumberData(int *data_array, int data_size)
{
    //  Inicjalizacja zmiennych roboczych/wynikowych.
    int data_length = this->params_data.length();
    int step_index = 0;
    String worker = "";

    //  Przetworzenie danych wejsciowych na dane liczbowe.
    for (int c = 0; c <= data_length; c++)
    {
        if (this->IsCharacterADigit(this->params_data[c]))
            worker += this->params_data[c];

        else if (worker != "" && step_index < data_size)
        {
            data_array[step_index] = worker.toInt();
            step_index ++;
            worker = "";
        }
    }

    return step_index;
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
//  *** PRIVATE MANAGEMENT METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

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
        return COMMAND_PROCESSED_OK;
    }
    else if (this->IsCharacterADigit(this->params_data[0]))
    {
        int brightness = max(DISPLAY_MIN_BRIGHTNESS, min(this->params_data[0] - 48, DISPLAY_MAX_BRIGHTNESS));
        this->controller->SetAutoBrightness(false);
        this->controller->display_ctrl->SetBrightness(brightness);
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

        this->controller->clock_ctrl->SetDate(day, week, month, year);
        return COMMAND_DISPLAY_DATETIME;
    }
    else if (last_step == 3)
    {
        int day = max(1, min(31, data_array[0]));
        int month = max(1, min(12, data_array[1]));
        int year = max(2000, min(2035, data_array[2]));

        this->controller->clock_ctrl->SetDate(day, month, year);
        return COMMAND_DISPLAY_DATETIME;
    }
    
    this->RaiseInvalidParameterError("date set");
    return COMMAND_NONE;
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
            this->controller->clock_ctrl->SetTime(hour, min, sec);            
        }
        else
        {
            this->controller->clock_ctrl->SetTime(hour, min);
        }

        return COMMAND_DISPLAY_DATETIME;
    }

    this->RaiseInvalidParameterError("time set");
    return COMMAND_NONE;
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

    if (this->ValidateCommand("/brightness set"))
        return this->ProcessBrightnessSetCommand();
        
    else if (this->ValidateCommand("/date set"))
        return this->ProcessDateSetCommand();

    else if (this->ValidateCommand("/time set"))
        return this->ProcessTimeSetCommand();

    else
        RaiseInvalidCommandError();
    
    this->Clear();
    return COMMAND_NONE;
}

#endif