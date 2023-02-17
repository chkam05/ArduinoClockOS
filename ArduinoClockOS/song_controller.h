////////////////////////////////////////////////////////////////////////////////
//  SONG CONTROLLER
////////////////////////////////////////////////////////////////////////////////

#ifndef SONG_CONTROLLER_H
#define SONG_CONTROLLER_H

//89

////////////////////////////////////////////////////////////////////////////////
//  *** INCLUDED LIBRARIES ***
////////////////////////////////////////////////////////////////////////////////

#include "display_controller.h"
#include "buzzer_controller.h"
#include "keypad_controller.h"


////////////////////////////////////////////////////////////////////////////////
//  *** CONFIGURATION ***
////////////////////////////////////////////////////////////////////////////////

#define SONG_NOTHING    -1
#define SONG_FINISHED   0
#define SONG_PLAYING    1

#define SONG_MODE_NONE    0
#define SONG_MODE_LOADED  1
#define SONG_MODE_INPUT   2

#define SONG_LAST_POS   9


////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class SongController
{
    private:
        DisplayController * display_ctrl;
        BuzzerController  * buzzer_ctrl;

        int     mode = 0;
        int     position = 0;
        String  song = "";
        int     song_length = 0;

        void    ClearSong();
    
    public:
        SongController(DisplayController * display_ctrl, BuzzerController * buzzer_ctrl);

        int   CheckState();
        int   SetupSong(String song);
        int   ProcessInput(int input);
        void  ProcessPlaying();
};

////////////////////////////////////////////////////////////////////////////////
//  *** PRIVATE METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

//  Usuniecie starej wiadomosci z pamieci i przygotowanie do wyswietlenia nowej.
void SongController::ClearSong()
{
    this->position = 0;
    this->song = "";
    this->song_length = 0;
}

////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Konstruktor klasy modulu piosenki.
 *  @param display_ctrl: Kontroler wyswietlacza.
 *  @param buzzer_ctrl: Kontroler brzeczyka.
 */
SongController::SongController(DisplayController * display_ctrl, BuzzerController * buzzer_ctrl)
{
    this->display_ctrl = display_ctrl;
    this->buzzer_ctrl = buzzer_ctrl;
}

//  ----------------------------------------------------------------------------
/*  Sprawdzenie stanu przetwarzania odtwarzania piosenki.
 *  @return: Indeks stanu przetwarzania odtwarzania piosenki.
 */
int SongController::CheckState()
{
    return this->song_length > position ? SONG_PLAYING : SONG_FINISHED;
}

//  ----------------------------------------------------------------------------
/*  Uruchomienie odtwarzania piosenki.
 *  @param message: Piosenka do odegrania.
 *  @return: Indeks stanu przetwarzania odtwarzania piosenki.
 */
int SongController::SetupSong(String song)
{
    this->ClearSong();

    if (song != NULL && song.length() > 0)
    {
        this->mode = SONG_MODE_LOADED;
        this->song = song;
        this->song_length = song.length();

        this->display_ctrl->Clear();
        this->display_ctrl->DrawSprite(SPRITE_MUSIC, 0, 0);
        this->display_ctrl->PrintText(0, 9, "Playing...");
        
        return SONG_PLAYING;
    }

    return SONG_FINISHED;
}

//  ----------------------------------------------------------------------------
/*  Przetworzenie danych wejsciowych uzytkownika wprowadzonych z klawiatury.
 *  @param input: Dane wejsciowe z klawiatury (wcisniety klawisz).
 *  @return: Indeks stanu przetwarzania odtwarzania piosenki.
 */
int SongController::ProcessInput(int input)
{
    if (input >= KEYPAD_0_KEY && input <= KEYPAD_9_KEY)
    {
        this->ClearSong();
        return MESSAGE_FINISHED;
    }

    switch (input)
    {
        case KEYPAD_NEXT_KEY:
        case KEYPAD_PREV_KEY:
        case KEYPAD_SELECT_KEY:
        case KEYPAD_BACK_KEY:
        case KEYPAD_OPTION_KEY:
        case KEYPAD_MENU_KEY:
            this->ClearSong();
            return MESSAGE_FINISHED;
        
        default:
            return CheckState();
    }
}

//  ----------------------------------------------------------------------------
//  Odtwarzania piosenki.
void SongController::ProcessPlaying()
{
    unsigned long milis = millis();

    if (this->position < this->song_length)
    {
        char    c               = ' ';
        String  duration_str    = "";
        String  note_str        = "";
        bool    isError         = false;
        bool    read            = true;
        bool    read_next       = false;
        int     start_position  = this->position;

        while (read && this->position < this->song_length && c != ';')
        {
            c = this->song[position];

            if (isDigit(c))
            {
                if (read_next)
                    duration_str += c;
                else
                    note_str += c;
            }
            else if (c == ',')
            {
                read_next = true;
            }
            else if (c == ';')
            {
                this->position++;
                break;
            }
            else
            {
                isError = true;
                break;
            }

            this->position++;
        }

        if (note_str.length() <= 0 || duration_str.length() <= 0 || isError)
        {
            this->display_ctrl->PrintText(0, 9, "Err: " + String(start_position) + " .. " + String(this->position));
            this->ClearSong();
            delay(3000);
            return;
        }

        int note = note_str.toInt();
        int duration = duration_str.toInt();

        if (note == 0)
            delay(duration);
        else
            this->buzzer_ctrl->PlayTone(note, duration);
    }
}

#endif