////////////////////////////////////////////////////////////////////////////////
//  TIMER
////////////////////////////////////////////////////////////////////////////////

#ifndef CLOCK_TIMER_H
#define CLOCK_TIMER_H

////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class ClockTimer
{
    private:
        int   counter         =   0;
        int   interval        =   0;
        Time  previous_time;

        int   MeasureDiffrence(Time current_time);

    public:
        ClockTimer(Time initial_time, int interval);

        int   GetCounter();
        bool  IsFinished();
        void  Reset();
        void  SetTimeToEnd(int to_end);
        void  Update(int interval);
        bool  Work(Time current_time);
};


////////////////////////////////////////////////////////////////////////////////
//  *** PRIVATE METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Sprawdzenie roznic czasu - miedzy czasem aktualnym a czasomierzem.
 *  @return: Roznica czasu w sekundach.
 */
int ClockTimer::MeasureDiffrence(Time current_time)
{
    int diffrence_seconds = (current_time.hour * 3600 + current_time.min * 60 + current_time.sec)
      - (this->previous_time.hour * 3600 + this->previous_time.min * 60 + this->previous_time.sec);

    if (diffrence_seconds > 0)
        return diffrence_seconds;
    else
        return 0;
}

////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/* Konstruktor klasy czasomierza bazujacego na danych zegara czasu rzeczywistego CLOCK DS3231.
 * @param initial_time: Aktualna data i godzina jako struktura Time pobrana z modulu ModuleClock.
 * @param interval: Ilosc czasu w sekundach ktore ma odmierzac czasomierz.
 */
ClockTimer::ClockTimer(Time initial_time, int interval)
{
    this->previous_time = initial_time;
    this->interval = max(0, interval);
    this->counter = 0;
}

//  ----------------------------------------------------------------------------
/* Zwrocenie aktualnego czasu odliczania w sekundach.
 * @return: Aktualny czas odliczania w sekundach.
 */
int ClockTimer::GetCounter()
{
    return this->counter;
}

//  ----------------------------------------------------------------------------
/* Sprawdzenie czy czasomierz zakonczyl swoja prace.
 * @return: Informacja o tym czy czasomierz zakonczyl swoja prace.
 */
bool ClockTimer::IsFinished()
{
    return this->counter >= this->interval;
}

//  ----------------------------------------------------------------------------
// Zresetowanie czasomierza i ustawienie jego licznika na 0.
void ClockTimer::Reset()
{
    this->counter = 0;
}

//  ----------------------------------------------------------------------------
/* Przyspieszenie dzialania czasomierza poprzez ustawienie czasu do konca odliczania.
 * @param to_end: Ilosc czasu w sekundach jaka pozostanie do odmierzenia.
 */
void ClockTimer::SetTimeToEnd(int to_end)
{
    this->counter = max(0, min(to_end, interval));
}

//  ----------------------------------------------------------------------------
/* Aktualizacja limitu czasomierza.
 * @param interval: Ilosc czasu w sekundach ktore ma odmierzac czasomierz.
 */
void ClockTimer::Update(int interval)
{
    this->interval = max(0, interval);
}

//  ----------------------------------------------------------------------------
/* Metoda aktualizujaca czasomierz i umozliwiajaca jego prace.
 * @param current_time: Aktualna data i godzina jako struktura Time pobrana z modulu ModuleClock.
 * @return: Informacja o tym czy czasomierz zakonczyl swoja prace.
 */
bool ClockTimer::Work(Time current_time)
{
    int new_counter = this->counter + this->MeasureDiffrence(current_time);
    int finished = new_counter >= this->interval;
    
    this->counter = finished ? this->interval : new_counter;
    this->previous_time = current_time;

    return finished;
}

#endif