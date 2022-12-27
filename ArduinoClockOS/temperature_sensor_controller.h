////////////////////////////////////////////////////////////////////////////////
//  PHOTORESISTOR GL5528
////////////////////////////////////////////////////////////////////////////////

#ifndef TEMPERATURE_SENSOR_CONTROLLER_H
#define TEMPERATURE_SENSOR_CONTROLLER_H

////////////////////////////////////////////////////////////////////////////////
//  *** INCLUDED LIBRARIES ***
////////////////////////////////////////////////////////////////////////////////

#include <Arduino.h>
#include <OneWire.h>
#include <DallasTemperature.h>


////////////////////////////////////////////////////////////////////////////////
//  *** CONFIGURATION ***
////////////////////////////////////////////////////////////////////////////////

#define TEMPERATURE_SENSOR_NULL       -127
#define TEMPERATURE_SENSOR_PIN_IN     A9
#define TEMPERATURE_SENSOR_PIN_OUT    A8


////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class TemperatureSensorController
{
    private:
        OneWire           *connection;
        DallasTemperature *sensor;

    public:
        TemperatureSensorController(int pin_input);

        int GetTemperature();
};


////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/* Konstruktor klasy modulu miernika temperatury DALLAS DS18B20.
 * @param pin_input: Pin danych.
 */
TemperatureSensorController::TemperatureSensorController(int pin_input)
{
    this->connection = new OneWire(pin_input);
    this->sensor = new DallasTemperature(connection);
    this->sensor->begin();
}

//  ----------------------------------------------------------------------------
/* Odczytanie wartosci temperatury z merinika.
 * @return: Wartosc odczytanej temperatury.
 */
int TemperatureSensorController::GetTemperature()
{
    this->sensor->requestTemperatures();
    return this->sensor->getTempCByIndex(0);
}

#endif