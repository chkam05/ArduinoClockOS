////////////////////////////////////////////////////////////////////////////////
//  BUZZER 5V 12MM THT
////////////////////////////////////////////////////////////////////////////////

#ifndef BUZZER_CONTROLLER_H
#define BUZZER_CONTROLLER_H

////////////////////////////////////////////////////////////////////////////////
//  *** INCLUDED LIBRARIES ***
////////////////////////////////////////////////////////////////////////////////

#include "src/notes.h"


////////////////////////////////////////////////////////////////////////////////
//  *** CONFIGURATION ***
////////////////////////////////////////////////////////////////////////////////

#define BUZZER_PIN_OUT    A12


////////////////////////////////////////////////////////////////////////////////
//  *** STRUCT DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

struct BuzzerNote
{
    //  --- VARIABLES: ---
    int duration;
    int note;

    //  --- METHODS: ---
    BuzzerNote(int note, int duration)
    {
        this->note = note;
        this->duration = duration;
    }
};


////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class BuzzerController
{
    private:
        unsigned long start_time;
        int   current_note_duration;

        int   pin_output  = BUZZER_PIN_OUT;
        bool  is_playing  = false;
        
    public:
        BuzzerController(int pin_out);

        void PlayTone(int note, int duration);
        void PlayToneAsync(int note, int duration);
        void StopToneAsync();
        bool UpdateToneAsync();
};


////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Konstruktor klasy modulu buzzera 5V 12MM THT.
 *  @param pin_out: Pin wyjsciowy buzzera.
 */
BuzzerController::BuzzerController(int pin_out = BUZZER_PIN_OUT)
{
    this->pin_output = pin_out;
    pinMode(this->pin_output, OUTPUT);
}

//  ----------------------------------------------------------------------------
/*  Uruchomienie odtworzenia dzwieku.
 *  @param note: Nuta.
 *  @param duration: Czas odtwarzania w milisekundach.
 */ 
void BuzzerController::PlayTone(int note, int duration)
{
    if (this->is_playing)
        this->StopToneAsync();
    
    //  Zapobiegniecie odtworzenie nuty 0 i podzielenia sekundy przez zero.
    if (duration <= 0 || duration <= 0)
        return;
    
    //  Obliczenie pauzy.
    int note_pause = (1000 / duration) * 1.30;

    //  Odtworzenie okreslonego tonu przez okreslony czas.
    this->PlayToneAsync(note, duration);
    delay(note_pause);

    //  Zakonczenie odtwarzania tonu.
    this->StopToneAsync();
}

//  ----------------------------------------------------------------------------
void BuzzerController::PlayToneAsync(int note, int duration)
{
    //  Obliczenie czasu odtwarzania tonu.    
    this->current_note_duration = 1000 / duration;

    //  Odtworzenie okreslonego tonu przez okreslony czas.
    this->is_playing = true;
    this->start_time = millis();
    tone(this->pin_output, note, this->current_note_duration);
}

//  ----------------------------------------------------------------------------
void BuzzerController::StopToneAsync()
{
    if (this->is_playing)
    {
        this->is_playing = false;
        noTone(this->pin_output);        
    }
}

//  ----------------------------------------------------------------------------
bool BuzzerController::UpdateToneAsync()
{
    if (this->is_playing)
    {
        if (millis() - this->start_time >= this->current_note_duration)
        {
            this->StopToneAsync();
            return false;            
        }

        return true;
    }

    return false;
}

#endif