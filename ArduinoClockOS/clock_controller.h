////////////////////////////////////////////////////////////////////////////////
//  CLOCK DS3231
////////////////////////////////////////////////////////////////////////////////

#ifndef CLOCK_CONTROLLER_H
#define CLOCK_CONTROLLER_H

////////////////////////////////////////////////////////////////////////////////
//  *** INCLUDED LIBRARIES ***
////////////////////////////////////////////////////////////////////////////////

#include <DS3231.h>


////////////////////////////////////////////////////////////////////////////////
//  *** CONFIGURATION ***
////////////////////////////////////////////////////////////////////////////////

#define CLOCK_PIN_SDA   SDA
#define CLOCK_PIN_SCL   SCL

const String  week_names[7]   = {"Pon", "Wto", "Sro", "Czw", "Pia", "Sob", "Nie"};


////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class ClockController
{
    private:
        bool          day_changed     = false;
        Time          previous_time;
        DS3231        *rtc;

    public:
        ClockController(int sda, int scl);

        Time    Now();
        bool    GetBlink();
        bool    HasDayChanged();
        String  GetDate(String format, char separator);
        String  GetTime(String format, char separator, bool blinking = false);
        void    SetDate(int day, int day_week, int month, int year);
        void    SetDate(int day, int month, int year);
        void    SetTime(int hour, int min, int sec);
        void    SetTime(int hour, int min);

        int     ValidateDay(int day, int month, int year);
        int     ValidateMonth(int month);
        int     ValidateYear(int year);
        int     ValidateWeek(int week);
        int     ValidateHour(int hour);
        int     ValidateMinSec(int value);
};


////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/* Konstruktor klasy modulu zegara CLOCK DS3231.
 * @param sda: Pin sygnalu danych.
 * @param scl: Pin sygnalu zegarowego.
 */
ClockController::ClockController(int sda = CLOCK_PIN_SDA, int scl = CLOCK_PIN_SCL)
{
    this->rtc = new DS3231(sda, scl);
    this->rtc->begin();
    this->previous_time = this->rtc->getTime();
}

//  ----------------------------------------------------------------------------
/* Pobranie aktualnej daty i godziny jako struktury Time.
 * @return: Aktualna data i godzina jako struktura Time.
 */
Time ClockController::Now()
{
    //  Pobranie aktualej daty i godziny.
    Time current_time = this->rtc->getTime();

    //  Warunek definiujacy zmienna zmiany dnia.
    day_changed = !day_changed ? previous_time.date != current_time.date : day_changed;

    //  Aktualizacja zmiennej tymczasowej i zwrocenie aktualnej daty i godziny.
    this->previous_time.date = current_time.date;
    return current_time;
}

//  ----------------------------------------------------------------------------
/* Pobranie informacje o zmianie sekundy (czy sekunda jest liczba parzysta).
 * @return: True - aktualna sekunda jest liczba parzysta; False - w innym wypadku.
 */
bool ClockController::GetBlink()
{
    return this->Now().sec % 2;
}

//  ----------------------------------------------------------------------------
/* Pobranie informacje o tym czy data zostala zmieniona (wartosc mozna pobrac raz na dzien).
 * @return: Informacja o zmianie daty (po polnocy).
 */
bool ClockController::HasDayChanged()
{
    bool result = this->day_changed;
    this->day_changed = false;
    return result;
}

//  ----------------------------------------------------------------------------
/*  Pobiera aktualna date w odpowiednim formacie.
 *  @param format: Format daty:
 *    D/d - dzien (01 - 31)
 *    M/m - miesiac (01 - 12)
 *    Y - pelny rok (2021)
 *    y - krotki rok (21)
 *    W - nazwa tygodnia (Pon, Wto, Sro, itd)
 *    w - tydzien (1 - 7)
 *  @param separator: Znak odzielajacy kolejne segmenty day.
 *  @return: Aktualna data jako tekst.
 */
String ClockController::GetDate(String format, char separator)
{
    //  Inicjalizacja zmiennych roboczych/wynikowych.
    Time date_time = this->Now();
    String result = "";

    //  Formatowanie daty.
    for (int i = 0; i < format.length(); i++)
    {
        if (format[i] == 'D' || format[i] == 'd') result += ((date_time.date < 10) ? "0" : "") + String(date_time.date);
        if (format[i] == 'M' || format[i] == 'm') result += ((date_time.mon < 10) ? "0" : "") + String(date_time.mon);
        if (format[i] == 'Y') result += String(date_time.year);
        if (format[i] == 'y') result += String(date_time.year).substring(2, 4);
        if (format[i] == 'W') result += week_names[max(0, min(date_time.dow-1, 6))];
        if (format[i] == 'w') result += String(date_time.dow);

        if (i < format.length() - 1) result += separator;
    }

    return result;
}

//  ----------------------------------------------------------------------------
/*  Pobiera aktualny czas w odpowiednim formacie.
 *  @param format: Format czasu:
 *    H/h - godzina (00 - 23)
 *    M/m - minuta (00 - 59)
 *    S/s - sekunda (00 - 59)
 *  @param separator: Znak odzielajacy kolejne segmenty czasu.
 *  @return: Aktualny czas jako tekst.
 */
String ClockController::GetTime(String format, char separator, bool blinking = false)
{
    //  Inicjalizacja zmiennych roboczych/wynikowych.
    Time date_time = this->Now();
    String result = "";

    //  Formatowanie separatora i dostosowanie do migajacego wskaznika.
    char sep = blinking ? ' ' : separator;

    //  Formatowanie czasu.
    for (int i = 0; i < format.length(); i++)
    {
        if (format[i] == 'H' || format[i] == 'h') result += ((date_time.hour < 10) ? "0" : "") + String(date_time.hour);
        if (format[i] == 'M' || format[i] == 'm') result += ((date_time.min < 10) ? "0" : "") + String(date_time.min);
        if (format[i] == 'S' || format[i] == 's') result += ((date_time.sec < 10) ? "0" : "") + String(date_time.sec);

        if (i < format.length() - 1) result += sep;
    }

    return result;
}

//  ----------------------------------------------------------------------------
/*  Ustawienie nowej daty.
 *  @param day: Dzien w miesiacu od 1 do 31.
 *  @param day_week: Dzien tygodnia od 1 do 7.
 *  @param month: Miesiac od 1 do 12.
 *  @param year: Rok.
 */
void ClockController::SetDate(int day, int day_week, int month, int year)
{
    this->rtc->setDOW(day_week);
    this->rtc->setDate(day, month, year);
}

//  ----------------------------------------------------------------------------
/*  Ustawienie nowej daty.
 *  @param day: Dzien w miesiacu od 1 do 31.
 *  @param month: Miesiac od 1 do 12.
 *  @param year: Rok.
 */
void ClockController::SetDate(int day, int month, int year)
{
    this->rtc->setDate(day, month, year);
}

//  ----------------------------------------------------------------------------
/*  Ustawienie nowego czasu.
 *  @param hour: Godzina od 0 do 23.
 *  @param min: Minuta od 0 do 59.
 *  @param sec: Sekunda od 0 do 59.
 */
void ClockController::SetTime(int hour, int min, int sec)
{
    this->rtc->setTime(hour, min, sec);
}

//  ----------------------------------------------------------------------------
/*  Ustawienie nowego czasu.
 *  @param hour: Godzina od 0 do 23.
 *  @param min: Minuta od 0 do 59.
 */
void ClockController::SetTime(int hour, int min)
{
    this->rtc->setTime(hour, min, 0);
}

//  ----------------------------------------------------------------------------
/*  Sprawdzenie poprawnosci dnia i zwrocenie poprawnej wartosci.
 *  @param day: Dzien miesiace.
 *  @param month: Miesiac.
 *  @param year: Rok.
 *  @return: Poprawna wartosc dnia.
 */
int ClockController::ValidateDay(int day, int month, int year)
{
    int y = this->ValidateYear(year);
    int m = this->ValidateMonth(month);

    bool is_leap = (y % 4 == 0 && y % 100 != 0) || y % 400 == 0;
    
    if (m == 2)
        return max(1, min(day, is_leap ? 29 : 28));
    else if (m == 4 || m == 6 || m == 9 || m == 11)
        return max(1, min(day, 30));
    else
        return max(1, min(day, 31));
}

//  ----------------------------------------------------------------------------
/*  Sprawdzenie poprawnosci miesiace i zwrocenie poprawnej wartosci.
 *  @param month: Miesiac.
 *  @return: Poprawna wartosc miesiaca.
 */
int ClockController::ValidateMonth(int month)
{
    return max(1, min(month, 12));
}

//  ----------------------------------------------------------------------------
/*  Sprawdzenie poprawnosci roku i zwrocenie poprawnej wartosci.
 *  @param year: Rok.
 *  @return: Poprawna wartosc roku.
 */
int ClockController::ValidateYear(int year)
{
    return max(2000, min(year, 2035));
}

//  ----------------------------------------------------------------------------
/*  Sprawdzenie poprawnosci dnia tygodnia i zwrocenie poprawnej wartosci.
 *  @param year: Dzien tygodnia.
 *  @return: Poprawna wartosc dnia tygodnia.
 */
int ClockController::ValidateWeek(int week)
{
    return max(1, min(week, 7));
}

//  ----------------------------------------------------------------------------
/*  Sprawdzenie poprawnosci godziny i zwrocenie poprawnej wartosci.
 *  @param hour: Godzina.
 *  @return: Poprawna wartosc godziny.
 */
int ClockController::ValidateHour(int hour)
{
    return max(0, min(hour, 23));
}

//  ----------------------------------------------------------------------------
/*  Sprawdzenie poprawnosci minuty lub sekundy i zwrocenie poprawnej wartosci.
 *  @param value: Minuta lub sekunda.
 *  @return: Poprawna wartosc minuty lub sekundy.
 */
int ClockController::ValidateMinSec(int value)
{
    return max(0, min(value, 59));
}

#endif