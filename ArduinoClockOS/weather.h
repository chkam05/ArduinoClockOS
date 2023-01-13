////////////////////////////////////////////////////////////////////////////////
//  WEATHER
////////////////////////////////////////////////////////////////////////////////

#ifndef WEATHER_H
#define WEATHER_H

////////////////////////////////////////////////////////////////////////////////
//  *** INCLUDED LIBRARIES ***
////////////////////////////////////////////////////////////////////////////////

#include "sd_card_controller.h"


////////////////////////////////////////////////////////////////////////////////
//  *** CONFIGURATION ***
////////////////////////////////////////////////////////////////////////////////

const String WEATHER_FILE_NAME = "weat.ini";


////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class Weather
{
    private:
        SdCardController  * sdcard_ctrl;

        byte  * weather       = new byte[25] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int     year          = 2000;
        int     month         = 1;
        int     day           = 1;
        int     data_idx_pos  = 0;

        int   CheckDateValidity(int *date_array);
        bool  IsCharacterADigit(char c);
        int   ParseData(String file_data, int *data_array, int data_size, int start_index = 0);
        void  UpdateDate(Time date_time);

    public:
        Weather(SdCardController * sdcard_ctrl);
        
        void  ClearWeather();
        void  AddWeather(int year, int month, int day, int *data_array, int data_size, bool append = true);
        int   GetWeather(Time date_time);
};


////////////////////////////////////////////////////////////////////////////////
//  *** PRIVATE METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Sprawdzenie czy data w formacie tablicy jest aktualna data.
 *  @param date_array: Data zapisana w tablicy 3 elementowej.
 *  @return: -1: Stara data; 0: Obecna data; 1: Przyszla data.
 */
int Weather::CheckDateValidity(int *date_array)
{
    bool old = false;

    if (date_array[0] < this->year || date_array[1] < this->month || date_array[2] < this->day)
        return -1;
    
    if (date_array[0] > this->year || date_array[1] > this->month || date_array[2] > this->day)
        return 1;
    
    return 0;
}

//  ----------------------------------------------------------------------------
/* Sprawdzenie czy wprowadzony znak jest znakiem reprezentujacym liczbe.
 * @param c: Sprawdzany znak pod katem tego czy jest reprezentacja liczby.
 * @return: Informacja czy wprowadzony znak reprezentuje liczbe.
 */
bool Weather::IsCharacterADigit(char c)
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
int Weather::ParseData(String file_data, int *data_array, int data_size, int start_index = 0)
{
    if (file_data != NULL && file_data.length() > 0)
    {
        //  Inicjalizacja zmiennych roboczych/wynikowych.
        int data_length = file_data.length();
        int step_index = 0;
        String worker = "";

        //  Przetworzenie danych wejsciowych na dane liczbowe.
        if (start_index < data_length)
        {
            for (int c = start_index; c <= data_length; c++)
            {
                if (this->IsCharacterADigit(file_data[c]))
                    worker += file_data[c];

                else if (worker != "" && step_index < data_size)
                {
                    data_array[step_index] = worker.toInt();
                    step_index ++;
                    worker = "";

                    this->data_idx_pos = c;
                }
            }
        }

        return step_index;
    }

    return 0;
}

//  ----------------------------------------------------------------------------
/*  Zaktualizuj zmienne przetrzymujace date dnia wyswietlanej prognozy pogody.
 *  @param date_time: Aktualna data i czas.
 */
void Weather::UpdateDate(Time date_time)
{
    this->year = date_time.year;
    this->month = date_time.mon;
    this->day = date_time.date;
}

////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Konstruktor klasy kontrolera informacji pogodowych.
 *  @param sdcard_ctrl: Kontroler karty pamieci.
 */
Weather::Weather(SdCardController * sdcard_ctrl)
{
    this->sdcard_ctrl = sdcard_ctrl;
}

//  ----------------------------------------------------------------------------
//  Wyczyszczenie konfiguracji progrnozy pogody.
void Weather::ClearWeather()
{
    if (this->sdcard_ctrl->IsInitialized() && this->sdcard_ctrl->IsMounted() && this->sdcard_ctrl->FileExists(WEATHER_FILE_NAME))
        this->sdcard_ctrl->RemoveFile(WEATHER_FILE_NAME);
    
    this->weather[0] = 0;
}

//  ----------------------------------------------------------------------------
/*  Dodaj prognoze pogody do pamieci zewnetrznej SD.
 *  @param year: Rok daty prognozy pogody.
 *  @param month: Miesiac daty prognozy pogody.
 *  @param day: Dzien daty prognozy pogody.
 *  @param data_array: Tablica z prognoza pogody.
 *  @param data_size: Rozmiar tablicy z prognoza pogody.
 *  @param append: Dopisz (nie nadpisz) daneych do pamieci zewnetrznej SD.
 */
void Weather::AddWeather(int year, int month, int day, int *data_array, int data_size, bool append = true)
{
    if (this->sdcard_ctrl->IsInitialized() && this->sdcard_ctrl->IsMounted())
    {
        File weather_file = append
            ? this->sdcard_ctrl->OpenFileToAppend(WEATHER_FILE_NAME)
            : this->sdcard_ctrl->OpenFileToWrite(WEATHER_FILE_NAME);
        
        if (weather_file)
        {
            String date = String(year) + "." + String(month) + "." + String(day);
            String data = "";

            for (int i = 0; i <= data_size; i++)
            {
                data += String(data_array[i]);

                if (i < data_size)
                    data += ",";
            }

            weather_file.println(date + " " + data);
            weather_file.close();
        }      
    }
}

//  ----------------------------------------------------------------------------
/*  Pobranie indkesu ikony prognozy pogody.
 *  @param date_time: Aktualna data i czas.
 *  @return: Indeks ikony prognozy pogody.
 */
int Weather::GetWeather(Time date_time)
{
    if (date_time.year > this->year || date_time.mon > this->month || date_time.date > this->day || this->weather[0] == 0)
    {
        this->UpdateDate(date_time);
        
        if (this->sdcard_ctrl->IsInitialized() && this->sdcard_ctrl->IsMounted() && this->sdcard_ctrl->FileExists(WEATHER_FILE_NAME))
        {
            File weather_file = this->sdcard_ctrl->OpenFileToRead(WEATHER_FILE_NAME);

            char character = ' ';
            String line = "";
            bool stop_reading = false;

            while (weather_file.available() && !stop_reading) {
                while (weather_file.available() && character != '\n')
                {
                    character = weather_file.read();

                    if (character == '\t' || character == '\r')
                        continue;

                    if (character != '\n')
                        line += character;
                    else
                        break;
                }

                line.toLowerCase();

                int *date_array = new int[3] {0, 0, 0};
                int last_step = this->ParseData(line, date_array, 3);

                if (last_step == 3)
                {
                    if (CheckDateValidity(date_array))
                        continue;
                    
                    int *weather_array = new int[25] { 0 };
                    last_step = this->ParseData(line, weather_array, 25, this->data_idx_pos);

                    if (last_step > 0 && last_step <= 25)
                    {
                        for (int i = 0; i < last_step; i++)
                            weather[i] = weather_array[i];
                        
                        stop_reading = true;
                        break;
                    }
                }
            }

            weather_file.close();
        }
    }

    if (this->weather[0] <= 0)
        return 0;
    
    int hour_mapped = (((100 * date_time.hour) / 24) * this->weather[0]) / 100;
    int hour_index = max(0, min(24, hour_mapped));
    return max(0, min(6, this->weather[hour_index+1]));
}

#endif