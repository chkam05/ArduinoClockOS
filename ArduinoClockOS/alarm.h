////////////////////////////////////////////////////////////////////////////////
//  ALARM
////////////////////////////////////////////////////////////////////////////////

#ifndef ALARM_H
#define ALARM_H

////////////////////////////////////////////////////////////////////////////////
//  *** INCLUDED LIBRARIES ***
////////////////////////////////////////////////////////////////////////////////

#include <DS3231.h>


////////////////////////////////////////////////////////////////////////////////
//  *** CONFIGURATION ***
////////////////////////////////////////////////////////////////////////////////

#define ALARM_NONE        0
#define ALARM_DISARMED    1
#define ALARM_RAISED      2
#define ALARM_SUSPENDED   3


////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class Alarm
{
    private:
        int   alarm_state       =   ALARM_DISARMED;
        bool  enabled           =   false;
        bool  is_led            =   false;
        bool  has_been_raised   =   false;

        bool  CheckBaseTrigger(Time now_time);
        bool  CheckSuspendedTrigger(Time now_time);
        void  StartSleep();
    
    public:
        int   hour    =   6;
        int   minute  =   30;
        
        int   sleep_raise   =   0;
        int   sleep_day     =   0;

        Alarm();

        bool  CheckTrigger(Time now_time);
        int   GetState();
        bool  IsEnabled();
        bool  IsLed();

        void  DisableAlarm();
        void  SetAlarm(int hour, int minute, bool enabled = true, bool is_led = false);
        int   ProcessInput(char key);
};


////////////////////////////////////////////////////////////////////////////////
//  *** PRIVATE METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Sprawdzenie godziny uruchomienia alarmu i jego uruchomienie
 *  @param now_time: Aktualna data i czas.
 *  @return: True - uruchomienie alarmu; False - w innym przypadku.
 */
bool Alarm::CheckBaseTrigger(Time now_time)
{
    if (!this->has_been_raised && now_time.hour == this->hour && now_time.min == this->minute)
    {
        this->alarm_state = ALARM_RAISED;
        this->has_been_raised = true;
        this->sleep_day = now_time.date;

        return true;
    }
    else if (this->has_been_raised && (now_time.hour != this->hour || now_time.min != this->minute))
    {
        this->has_been_raised = false;
    }

    return false;
}

//  ----------------------------------------------------------------------------
/*  Sprawdzenie czasu dzialania drzemki i przywrocenie dzialania alarmu.
 *  @param now_time: Aktualna data i czas.
 *  @return: True - ponowne uruchomienie alarmu; False - w innym przypadku.
 */
bool Alarm::CheckSuspendedTrigger(Time now_time)
{
    int current_time = (now_time.hour * 60 + now_time.min);
    bool raise = false;

    if (this->sleep_raise > 1440)
    {
        if (now_time.date > this->sleep_day && (this->sleep_raise % 1440) < current_time)
            raise = true;
    }
    else if (this->sleep_raise < current_time)
    {
        raise = true;
    }

    if (raise)
    {
        this->alarm_state = ALARM_RAISED;
        this->has_been_raised = true;
        this->sleep_day = now_time.date;
        return true;
    }

    return false;
}

//  ----------------------------------------------------------------------------
//  Uruchom tryb drzemki.
void Alarm::StartSleep()
{
    this->alarm_state = ALARM_SUSPENDED;
    this->sleep_raise = (this->hour * 60) + this->minute + 10;
}

////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

//  Konstruktor klasy modulu alarmu.
Alarm::Alarm()
{
    //
}

//  ----------------------------------------------------------------------------
/*  Sprawdzenie godziny uruchomienia alarmu i jego uruchomienie
 *  @param now_time: Aktualna data i czas.
 *  @return: True - uruchomienie alarmu; False - w innym przypadku.
 */
bool Alarm::CheckTrigger(Time now_time)
{
    if (this->enabled)
    {
        switch (this->alarm_state)
        {
            case ALARM_DISARMED:
                return this->CheckBaseTrigger(now_time);
            
            case ALARM_RAISED:
                return false;
            
            case ALARM_SUSPENDED:
                return this->CheckSuspendedTrigger(now_time);
        }
    }

    return false;
}

//  ----------------------------------------------------------------------------
/*  Pobranie aktualnego stanu alarmu (dzwoni, drzemka, nieaktywny).
 *  @return: Indeks stanu alarmu.
 */
int Alarm::GetState()
{
    return this->alarm_state;
}

//  ----------------------------------------------------------------------------
/*  Pobranie informacji o uzbrojeniu alarmu.
 *  @return: True - alarm uzbrojony; False - w innym przypadku.
 */
bool Alarm::IsEnabled()
{
    return this->enabled;
}

//  ----------------------------------------------------------------------------
bool Alarm::IsLed()
{
    return this->is_led;
}

//  ----------------------------------------------------------------------------
//  Rozbrojenie alarmu.
void Alarm::DisableAlarm()
{
    this->alarm_state = ALARM_DISARMED;
    this->enabled = false;
}

//  ----------------------------------------------------------------------------
/*  Ustawienie godziny uruchomienia alarmu.
 *  @param hour: Godzina uruchomienia alarmu.
 *  @param minute: Minuta uruchomienia alarmu.
 */
void Alarm::SetAlarm(int hour, int minute, bool enabled = true, bool is_led = false)
{
    this->alarm_state = ALARM_DISARMED;
    this->hour = hour;
    this->minute = minute;
    this->enabled = enabled;
    this->is_led = is_led;
}

//  ----------------------------------------------------------------------------
/*  Przetworzenie danych wejsciowych uzytkownika wprowadzonych z klawiatury.
 *  @param input: Dane wejsciowe z klawiatury (wcisniety klawisz).
 *  @return: Indeks wykonanego procesu przez kontroler alarmu.
 */
int Alarm::ProcessInput(char key)
{
    switch (key)
    {
        case KEYPAD_MENU_KEY:
            this->alarm_state = ALARM_DISARMED;
            return ALARM_DISARMED;
        
        default:
            this->StartSleep();
            return ALARM_SUSPENDED;
    }

    return ALARM_NONE;
}

#endif