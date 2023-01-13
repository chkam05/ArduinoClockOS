////////////////////////////////////////////////////////////////////////////////
//  MESSAGE CONTROLLER
////////////////////////////////////////////////////////////////////////////////

#ifndef MESSAGE_CONTROLLER_H
#define MESSAGE_CONTROLLER_H

////////////////////////////////////////////////////////////////////////////////
//  *** INCLUDED LIBRARIES ***
////////////////////////////////////////////////////////////////////////////////

#include "display_controller.h"
#include "keypad_controller.h"


////////////////////////////////////////////////////////////////////////////////
//  *** CONFIGURATION ***
////////////////////////////////////////////////////////////////////////////////

#define MESSAGE_NOTHING     -1
#define MESSAGE_FINISHED    0
#define MESSAGE_DISPLAYING  1

#define MESSAGE_LAST_POS    9


////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class MessageController
{
    private:
        DisplayController * display_ctrl;

        unsigned long   delay_checkpoint = 0;

        int     delay_step = 50;
        int     font = FONT_DIGITAL;
        String  message = "";
        int     character_shift = 0;
        int     position = 0;
        int     width_in_pixels = 0;

        void  ClearMessage();
    
    public:
        MessageController(DisplayController * display_ctrl);

        int   CheckState();
        int   SetupMessage(String message, int font = FONT_DIGITAL);
        int   ProcessInput(int input);
        void  UpdateDisplay();
};

////////////////////////////////////////////////////////////////////////////////
//  *** PRIVATE METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

//  Usuniecie starej wiadomosci z pamieci i przygotowanie do wyswietlenia nowej.
void MessageController::ClearMessage()
{
    this->message = "";
    this->character_shift = 0;
    this->position = this->display_ctrl->GetWidth() - 1;
    this->width_in_pixels = 0;
}

////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Konstruktor klasy modulu alarmu.
 *  @param display_ctrl: Kontroler wyswietlacza.
 */
MessageController::MessageController(DisplayController * display_ctrl)
{
    this->display_ctrl = display_ctrl;
}

//  ----------------------------------------------------------------------------
/*  Sprawdzenie stanu przetwarzania wyswietlania wiadomosci.
 *  @return: Indeks stanu przetwarzania wyswietlania wiadomosci.
 */
int MessageController::CheckState()
{
    return this->message.length() > 0 && width_in_pixels > 0 ? MESSAGE_DISPLAYING : MESSAGE_FINISHED;
}

//  ----------------------------------------------------------------------------
/*  Uruchomienie wyswietlania wiadomosci.
 *  @param message: Wiadomosc do wyswietlenia.
 *  @param font: Indeks czcionki wyswietlanej wiadomosci.
 *  @return: Indeks stanu przetwarzania wyswietlania wiadomosci.
 */
int MessageController::SetupMessage(String message, int font = FONT_DIGITAL)
{
    this->ClearMessage();

    if (message != NULL && message.length() > 0)
    {
        this->font = font;
        this->message = message;
        this->width_in_pixels = this->display_ctrl->GetTextWidth(font, message);
        this->delay_checkpoint = millis() + this->delay_step;

        this->display_ctrl->Clear();
        this->display_ctrl->DrawSprite(SPRITE_MESSAGE, 0, 0);
        
        return MESSAGE_DISPLAYING;
    }

    return MESSAGE_FINISHED;
}

//  ----------------------------------------------------------------------------
/*  Przetworzenie danych wejsciowych uzytkownika wprowadzonych z klawiatury.
 *  @param input: Dane wejsciowe z klawiatury (wcisniety klawisz).
 *  @return: Indeks stanu przetwarzania wyswietlania wiadomosci.
 */
int MessageController::ProcessInput(int input)
{
    if (input >= KEYPAD_0_KEY && input <= KEYPAD_9_KEY)
    {
        this->ClearMessage();
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
            this->ClearMessage();
            return MESSAGE_FINISHED;
        
        default:
            return CheckState();
    }
}

//  ----------------------------------------------------------------------------
//  Odswiezenie ekranu - przesuniecie wiadomosci na ekranie.
void MessageController::UpdateDisplay()
{
    unsigned long milis = millis();

    if (milis < this->delay_checkpoint || milis > (this->delay_checkpoint + this->delay_step))
    {    
        this->width_in_pixels = this->display_ctrl->PrintMessage(this->font, this->position, MESSAGE_LAST_POS, this->message, this->character_shift);
        this->delay_checkpoint = milis;

        if (this->position > MESSAGE_LAST_POS)
            this->position -=1;
    }
}

#endif