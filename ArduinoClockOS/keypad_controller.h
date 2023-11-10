////////////////////////////////////////////////////////////////////////////////
//  4x4 MATRIX MEMBRANE KEYPAD HX13B001
////////////////////////////////////////////////////////////////////////////////

#ifndef KEYPAD_CONTROLLER_H
#define KEYPAD_CONTROLLER_H

////////////////////////////////////////////////////////////////////////////////
//  *** INCLUDED LIBRARIES ***
////////////////////////////////////////////////////////////////////////////////

#include <Keypad.h>


////////////////////////////////////////////////////////////////////////////////
//  *** CONFIGURATION ***
////////////////////////////////////////////////////////////////////////////////

#define KEYPAD_COLS     4
#define KEYPAD_ROWS     4

#define KEYPAD_NO_KEY       0     //  NULL
#define KEYPAD_NEXT_KEY     35    //  #
#define KEYPAD_PREV_KEY     42    //  *
#define KEYPAD_0_KEY        48    //  0
#define KEYPAD_1_KEY        49    //  1
#define KEYPAD_2_KEY        50    //  2
#define KEYPAD_3_KEY        51    //  3
#define KEYPAD_4_KEY        52    //  4
#define KEYPAD_5_KEY        53    //  5
#define KEYPAD_6_KEY        54    //  6
#define KEYPAD_7_KEY        55    //  7
#define KEYPAD_8_KEY        56    //  8
#define KEYPAD_9_KEY        57    //  9
#define KEYPAD_SELECT_KEY   65    //  a
#define KEYPAD_BACK_KEY     66    //  b
#define KEYPAD_OPTION_KEY   67    //  c
#define KEYPAD_MENU_KEY     68    //  d

const byte  KEYPAD_PIN_COLS[KEYPAD_COLS] = {30, 32, 34, 36};
const byte  KEYPAD_PIN_ROWS[KEYPAD_COLS] = {22, 24, 26, 28};
const char  KEYPAD_MAP[KEYPAD_ROWS][KEYPAD_COLS] = {
    { '1', '2', '3', 'A' },
    { '4', '5', '6', 'B' },
    { '7', '8', '9', 'C' },
    { '*', '0', '#', 'D' }
};


////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class KeypadController
{
    private:
        Keypad *controller;

    public:
        KeypadController();

        int GetPressedKey();
};


////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

KeypadController::KeypadController()
{
    //  Inicjalizacja podrzednego sprzetowego kontrolera klawiatury.
    this->controller = new Keypad(
        makeKeymap(KEYPAD_MAP), KEYPAD_PIN_ROWS, KEYPAD_PIN_COLS, KEYPAD_ROWS, KEYPAD_COLS);
}

//  ----------------------------------------------------------------------------
int KeypadController::GetPressedKey()
{
    return this->controller->getKey();
}

#endif