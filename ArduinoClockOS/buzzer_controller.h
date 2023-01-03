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
        int pin_output  = BUZZER_PIN_OUT;
        
    public:
        BuzzerController(int pin_out);

        void PlayTone(int note, int duration);
        void PlayTone(BuzzerNote *note);
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
    //  Zapobiegniecie odtworzenie nuty 0 i podzielenia sekundy przez zero.
    if (duration <= 0 || duration <= 0)
        return;
    
    //  Obliczenie czasu odtwarzania tonu i pauzy.
    int note_duration = 1000 / duration;
    int note_pause = note_duration * 1.30;

    //  Odtworzenie okreslonego tonu przez okreslony czas.
    tone(this->pin_output, note, note_duration);
    delay(note_pause);

    //  Zakonczenie odtwarzania tonu.
    noTone(this->pin_output);
}

//  ----------------------------------------------------------------------------
/*  Uruchomienie odtworzenia dzwieku
 *  @param note: Struktura zwierajaca nute i czas jej odtwarzania w ms.
 */
void BuzzerController::PlayTone(BuzzerNote *note)
{
    this->PlayTone(note->note, note->duration);
}

#endif