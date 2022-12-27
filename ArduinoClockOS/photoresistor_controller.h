////////////////////////////////////////////////////////////////////////////////
//  PHOTORESISTOR GL5528
////////////////////////////////////////////////////////////////////////////////

#ifndef PHOTORESISTOR_CONTROLLER_H
#define PHOTORESISTOR_CONTROLLER_H

////////////////////////////////////////////////////////////////////////////////
//  *** INCLUDED LIBRARIES ***
////////////////////////////////////////////////////////////////////////////////

#include <Arduino.h>


////////////////////////////////////////////////////////////////////////////////
//  *** CONFIGURATION ***
////////////////////////////////////////////////////////////////////////////////

#define LIGHT_SENSOR_MIN_VALUE        0
#define LIGHT_SENSOR_MAX_VALUE        1023
#define LIGHT_SENSOR_PIN_L            A10
#define LIGHT_SENSOR_PIN_R            A11


////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class PhotoresistorController
{
    private:
        int pin_input;

    public:
        PhotoresistorController(int pin_input);

        int GetBrightness();
        int GetMappedBrightness(int map_stages);
};


////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/* Konstruktor klasy modulu fotorezystora GL5528.
 * @param pin_input: Pin danych.
 */
PhotoresistorController::PhotoresistorController(int pin_input)
{
    this->pin_input = pin_input;
    pinMode(this->pin_input, INPUT);
}

//  ----------------------------------------------------------------------------
/* Odczytanie wartosci oswietlenia z fotorezystora.
 * @return: Wartosc oswietlenia.
 */
int PhotoresistorController::GetBrightness()
{
    int value = analogRead(this->pin_input);
    return max(LIGHT_SENSOR_MIN_VALUE, min(value, LIGHT_SENSOR_MAX_VALUE));
}

//  ----------------------------------------------------------------------------
/* Odczytanie zmapowanej wartosci oswietlenia z fotorezystora.
 * @param map_stages: Maksymalna wartosc ktora zostanie zmapowana.
 * @return: Zmapowana wartosc oswietlenia.
 */
int PhotoresistorController::GetMappedBrightness(int map_stages = 8)
{
    int value = this->GetBrightness();
    return map(value, LIGHT_SENSOR_MIN_VALUE, LIGHT_SENSOR_MAX_VALUE, 0, map_stages);
}

#endif