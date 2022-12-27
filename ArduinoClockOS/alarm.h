////////////////////////////////////////////////////////////////////////////////
//  ALARM
////////////////////////////////////////////////////////////////////////////////

#ifndef ALARM_H
#define ALARM_H

////////////////////////////////////////////////////////////////////////////////
//  *** INCLUDED LIBRARIES ***
////////////////////////////////////////////////////////////////////////////////

#include <Arduino.h>
#include <avr/pgmspace.h>
#include <DS3231.h>


////////////////////////////////////////////////////////////////////////////////
//  *** CONFIGURATION ***
////////////////////////////////////////////////////////////////////////////////

#define ALARM_DISARMED    0
#define ALARM_RAISED      1
#define ALARM_SUSPENDED   2


////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class Alarm
{
    private:
        int   alarm_state       =   ALARM_DISARMED;
        bool  enabled           =   false;
        bool  has_been_raised   =   false;
        int   sleep_time        =   10;

        bool  CheckBaseTrigger(Time now_time);
        bool  CheckSuspendedTrigger(Time now_time);
    
    public:
        int   hour    =   0;
        int   minute  =   0;

        Alarm();

        bool  CheckTrigger(Time now_time);
        int   GetState();
        bool  IsEnabled();

        void  DisableAlarm();
        void  SetAlarm(int hour, int minute);
        int   ProcessInput(char key);
};


////////////////////////////////////////////////////////////////////////////////
//  *** PRIVATE METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

bool Alarm::CheckBaseTrigger(Time now_time)
{
    if (!this->has_been_raised && now_time.hour == this->hour && now_time.min == this->minute)
    {
        this->alarm_state = ALARM_RAISED;
        this->has_been_raised = true;

        return true;
    }
    else if (this->has_been_raised && (now_time.hour != this->hour || now_time.min != this->minute))
    {
        this->has_been_raised = false;
    }

    return false;
}

//  ----------------------------------------------------------------------------
bool Alarm::CheckSuspendedTrigger(Time now_time)
{
    int _all_minutes = this->hour * 60 + this->minute + this->sleep_time;

    int _hour = _all_minutes / 60;
    int _minute = _all_minutes % 60;

    if (_hour > 23)
        _hour = _hour % 24;
    
    if (!this->has_been_raised && now_time.hour == _hour && now_time.min == _minute)
    {
        this->alarm_state = ALARM_RAISED;
        this->has_been_raised = true;

        return true;
    }

    return false;
}


////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

Alarm::Alarm()
{
    //
}

//  ----------------------------------------------------------------------------
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
int Alarm::GetState()
{
    return this->alarm_state;
}

//  ----------------------------------------------------------------------------
bool Alarm::IsEnabled()
{
    return this->enabled;
}

//  ----------------------------------------------------------------------------
void Alarm::DisableAlarm()
{
    this->enabled = false;
}

//  ----------------------------------------------------------------------------
void Alarm::SetAlarm(int hour, int minute)
{
    this->hour = hour;
    this->minute = minute;
    this->enabled = true;
}

//  ----------------------------------------------------------------------------
int Alarm::ProcessInput(char key)
{
    switch (key)
    {
        case KEYPAD_MENU_KEY:
            return ALARM_DISARMED;
        
        default:
            return ALARM_SUSPENDED;
    }
}

#endif