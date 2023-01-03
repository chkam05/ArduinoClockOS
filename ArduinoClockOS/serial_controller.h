////////////////////////////////////////////////////////////////////////////////
//  COM PORT SERIAL CONTROLLER
//  COM PORT BLUETOOTH CONTROLLER
////////////////////////////////////////////////////////////////////////////////

#ifndef SERIAL_CONTROLLER_H
#define SERIAL_CONTROLLER_H

////////////////////////////////////////////////////////////////////////////////
//  *** CONFIGURATION ***
////////////////////////////////////////////////////////////////////////////////

#define SERIAL_COM          0
#define SERIAL_BLUETOOTH    1

////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class SerialController
{
    private:
        const int input_types[2] = { SERIAL_COM, SERIAL_BLUETOOTH };
        int   last_device = 0;

        void  InitSerialComBT(int baudrate);
        void  InitSerialComPC(int baudrate);

    public:
        SerialController(int baudrate);

        int     GetLastInputDevice();
        String  ReadInputData();
        String  ReadRawData(int input_device);
        void    WriteRawData(String raw_data, int output_device);
};


////////////////////////////////////////////////////////////////////////////////
//  *** PRIVATE METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/* Inicjalizacja polaczenia szeregowego z urzadzeniami zewnetrznymi poprzez modul bluetooth.
 * @param baudrate: Szybkosc transmisji w bitach na sekunde.
 */
void SerialController::InitSerialComBT(int baudrate)
{
    Serial1.begin(baudrate);
    this->WriteRawData("Arduino Clock OS 2.0\nCopyright (c) Kamil Karpiński 2021", SERIAL_BLUETOOTH);
    this->WriteRawData("Bluetooth communication established in " + String(baudrate) + " bps.", SERIAL_BLUETOOTH);
    this->WriteRawData("", SERIAL_BLUETOOTH);

    this->WriteRawData("Bluetooth module: HC-06", SERIAL_COM);
    this->WriteRawData("Bluetooth passwd: 1234.", SERIAL_COM);
}

//  ----------------------------------------------------------------------------
/* Inicjalizacja polaczenia szeregowego z urzadzeniami zewnetrznymi poprzez kabel USB.
 * @param baudrate: Szybkosc transmisji w bitach na sekunde.
 */
void SerialController::InitSerialComPC(int baudrate)
{
    Serial.begin(baudrate);
    this->WriteRawData("Arduino Clock OS 2.0\nCopyright (c) Kamil Karpiński 2021", SERIAL_COM);
    this->WriteRawData("PC communication established in " + String(baudrate) + " bps.", SERIAL_COM);
    this->WriteRawData("", SERIAL_COM);
}

////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/* Konstruktor kontrolera polaczenia szeregowego z urzadzeniami zewnetrznymi.
 * @param baudrate: Szybkosc transmisji w bitach na sekunde (domyslnie 9600).
 */
SerialController::SerialController(int baudrate = 9600)
{
    this->InitSerialComPC(baudrate);
    this->InitSerialComBT(baudrate);
}

//  ----------------------------------------------------------------------------
/* Pobranie identyfikatora ostatnio uzywanego typu urzadzenia do odczytania danych.
 * @return: Identyfikator ostatnio uzywanego typu urzadzenia do odczytania danych.
 */
int SerialController::GetLastInputDevice()
{
    return this->last_device;
}

//  ----------------------------------------------------------------------------
/* Iteracyjne odczytanie danych z kolejnych urzadzen do ktorego zostaly wyslane.
 * @return: Dane odczytane z urzadzenia.
 */
String SerialController::ReadInputData()
{
    //  Inicjalizacja zmiennych roboczych/wynikowych.
    String data = "";
    
    //  Odczytanie danych.
    for (int it_index = 0; it_index < 2; it_index++)
    {
        data = ReadRawData(input_types[it_index]);
        if (data.length() > 0)
        {
            this->last_device = input_types[it_index];
            return data;
        }
    }

    return "";
}

//  ----------------------------------------------------------------------------
/* Odczytanie danych przychodzacych z urzadzenia zewnetrznego.
 * @param input_device: Typ urzadzenia z ktorego dane zostana odczytane.
 * @return: Dane odczytane z urzadzenia.
 */
String SerialController::ReadRawData(int input_device)
{
    //  Inicjalizacja zmiennych roboczych/wynikowych.
    String readed_data = "";
    String result_data = "";

    //  Pobranie danych w zaleznosci od wybranego urzadzenia.
    switch (input_device)
    {
        default:
        case SERIAL_COM:
            while(Serial.available() > 0)
            {
                readed_data = Serial.readString();
                Serial.println(readed_data);
                result_data = result_data + readed_data;
                readed_data = "";
            }
            break;
            
        case SERIAL_BLUETOOTH:
            while (Serial1.available() > 0)
            {
                readed_data = Serial1.readString();
                Serial1.println(readed_data);
                result_data = result_data + readed_data;
                readed_data = "";
            }
            break;
    }

    //  Usuniecie z wyniku nadmiaru bialych znakow i jego zwrocenie.
    result_data.trim();
    return result_data.length() > 0 ? result_data : "";
}

//  ----------------------------------------------------------------------------
/* Wyslanie danych do urzadzenia zewnetrznego za pomoca okreslonego portu komunikacyjnego.
 * @param data: Dane ktore maja zostac wyslane.
 * @param output_device: Typ urzadzenia do ktorego dane zostana wyslane.
 */
void SerialController::WriteRawData(String data, int output_device)
{
    switch (output_device)
    {
        default:
        case SERIAL_COM:
            Serial.println(data);
            break;
        
        case SERIAL_BLUETOOTH:
            Serial1.println(data);
            break;
    }
}

#endif