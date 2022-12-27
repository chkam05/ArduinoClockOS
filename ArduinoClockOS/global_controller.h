////////////////////////////////////////////////////////////////////////////////
//  CONTROLLER
////////////////////////////////////////////////////////////////////////////////

#ifndef CONTROLLER_H
#define CONTROLLER_H

////////////////////////////////////////////////////////////////////////////////
//  *** INCLUDED LIBRARIES ***
////////////////////////////////////////////////////////////////////////////////

#include "buzzer_controller.h"
#include "clock_controller.h"
#include "clock_timer.h"
#include "display_controller.h"
#include "keypad_controller.h"
#include "photoresistor_controller.h"
#include "sd_card_controller.h"
#include "serial_controller.h"
#include "temperature_sensor_controller.h"
#include "alarm.h"


////////////////////////////////////////////////////////////////////////////////
//  *** CONFIGURATION ***
////////////////////////////////////////////////////////////////////////////////

#define COMMAND_DISPLAY_DATETIME        1

#define DISPLAY_MODE_INTERVAL           15
#define DISPLAY_STRINGS                 3

#define DISPLAY_STATES                  3
#define DISPLAY_DATETIME_STATE          0
#define DISPLAY_TEMPERATURE_IN_STATE    1
#define DISPLAY_TEMPERATURE_OUT_STATE   2

#define GLOBAL_STATES                   4
#define GLOBAL_STATE_NORMAL             0
#define GLOBAL_STATE_MENU               1
#define GLOBAL_STATE_SETTER             2
#define GLOBAL_STATE_MESSAGE            3


////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class GlobalController
{
    private:
        DisplayString * display_strings[3];

        bool  brightness_auto               =   true;
        int   buzzer_hour_change_interval   =   0;
        bool  buzzer_hour_change_complete   =   false;
        int   display_selected_index        =   0;
        int   display_state                 =   DISPLAY_DATETIME_STATE;
        int   global_state                  =   GLOBAL_STATE_NORMAL;
        bool  global_state_change           =   false;
        char  input_key                     =   0;

        //  Initialization
        void  InitializeClock();
        void  InitializeBuzzer();
        void  InitializeDisplay();
        void  InitializePhotoresistors();
        void  InitializeSdCard();
        void  InitializeTemperatureSensors();
        void  Initialize();

    public:
        KeypadController *keypad_ctrl;
        SerialController *serial_ctrl;

        BuzzerController              * buzzer_ctrl;
        ClockController               * clock_ctrl;
        DisplayController             * display_ctrl;
        SdCardController              * sdcard_ctrl;
        PhotoresistorController       * photoresistor_ctrl_left;
        PhotoresistorController       * photoresistor_ctrl_right;
        TemperatureSensorController   * temp_sensor_ctrl_in;
        TemperatureSensorController   * temp_sensor_ctrl_out;
        ClockTimer                    * update_timer;

        int   input_command           =   0;

        GlobalController();

        //  Brightness management.
        bool  IsAutoBrightness();
        void  ProcessAutoBrightness();
        void  SetAutoBrightness(bool enabled);

        //  Buzzer
        int   GetBuzzerHourNotifierInterval();
        void  SetBuzzerHourNotifierInterval(int interval = 0);
        void  BuzzerNotifyChangeHour();

        //  Display management.
        DisplayString *GetDisplayString(int index);
        DisplayString *GetSelectedDisplayString();
        void  SelectDisplayString(int index);

        //  Input.
        int   GetInputCommand();
        char  GetInputKey();
        void  ProcessInput();

        //  States and commands management.
        int   GetDisplayingState();
        void  SetDisplayingState(int displaying_state, bool reset_timer);
        void  SetNextDisplayingState();

        int   GetGlobalState();
        bool  IsGlobalChangeRequested();
        void  RequestGlobalStateChange();
        void  SetGlobalState(int machine_state);
        void  FinalizeGlobalStateChange();
};


////////////////////////////////////////////////////////////////////////////////
//  *** INITIALIZATION PRIVATE METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

//  Inicjalizacja, konfiguracja i test modulu kontrolera zegara czasu rzeczywistego.
void GlobalController::InitializeClock()
{
    this->clock_ctrl = new ClockController();
    this->serial_ctrl->WriteRawData("DS3231 CLOCK Date: " + this->clock_ctrl->GetDate("WDMY", '.'), SERIAL_COM);
    this->serial_ctrl->WriteRawData("DS3231 CLOCK Time: " + this->clock_ctrl->GetTime("HMS", ':'), SERIAL_COM);
    this->update_timer = new ClockTimer(this->clock_ctrl->Now(), DISPLAY_MODE_INTERVAL);
}

//  ----------------------------------------------------------------------------
//  Inicjalizacja, konfiguracja i test modulu kontrolera brzeczyka.
void GlobalController::InitializeBuzzer()
{
    this->buzzer_ctrl = new BuzzerController();
    this->buzzer_ctrl->PlayTone(NOTE_C8, 2);
}

//  ----------------------------------------------------------------------------
//  Inicjalizacja, konfiguracja i test modulu kontrolera wyswietlacza.
void GlobalController::InitializeDisplay()
{
    //  Konfiguracja modulu wyswietlacza.
    this->display_ctrl = new DisplayController(DISPLAY_MAX_BRIGHTNESS, 8);

    //  Inicjalizacja i konfiguracja kontenerow tekstowych wyswietlacza.
    for (int dsp_index = 0; dsp_index < DISPLAY_STRINGS; dsp_index++)
        this->display_strings[dsp_index] = new DisplayString(0, dsp_index, "");
        
    this->display_strings[TEXT_ALIGN_LEFT]->_xpos = 0;
    this->display_strings[TEXT_ALIGN_CENTER]->_xpos = this->display_ctrl->GetWidth()/2;
    this->display_strings[TEXT_ALIGN_RIGHT]->_xpos = this->display_ctrl->GetWidth()-1;

    //  Wyswietlenie logo.
    DisplayString * _display_string_center = this->display_strings[TEXT_ALIGN_CENTER];
    _display_string_center->offset = 0;
    _display_string_center->text = "AOS 3.0";
    this->display_ctrl->PrintDS(_display_string_center, false);

    //  Testowanie jasnosci wyswietlacza.
    for (int b = 0; b <= DISPLAY_MAX_BRIGHTNESS; b++)
    {
        this->display_ctrl->SetBrightness(b);
        delay(200);
    }

    //  Testowanie wyswietlacza.
    for (int x = 0; x < this->display_ctrl->GetWidth(); x++)
    {
        this->display_ctrl->DrawPoint(x, 7, 1);
        delay(10);
    }

    //  Wyczyszczenie wyswietlacza i wyswietlenie tekstu powitalnego.
    _display_string_center->text = "Welcome";
    
    this->display_ctrl->Clear();
    this->display_ctrl->PrintDS(_display_string_center, false);
    delay(2000);
    this->display_ctrl->ClearDS(_display_string_center);
    this->display_ctrl->Clear();
}

//  ----------------------------------------------------------------------------
//  Inicjalizacja, konfiguracja i test modulu kontrolera fotorezystorow.
void GlobalController::InitializePhotoresistors()
{
    this->photoresistor_ctrl_left = new PhotoresistorController(A10);
    this->photoresistor_ctrl_right = new PhotoresistorController(A11);
    this->serial_ctrl->WriteRawData("GL5528 Light Left:  " + String(this->photoresistor_ctrl_left->GetBrightness()), SERIAL_COM);
    this->serial_ctrl->WriteRawData("GL5528 Light Right: " + String(this->photoresistor_ctrl_right->GetBrightness()), SERIAL_COM);
}

//  ----------------------------------------------------------------------------
//  Inicjalizacja, konfiguracja i test modulu kontrolera czytnika kart sd.
void GlobalController::InitializeSdCard()
{
    this->sdcard_ctrl = new SdCardController();
    
    if (!this->sdcard_ctrl->IsInitialized() || !this->sdcard_ctrl->IsMounted())
    {
        this->buzzer_ctrl->PlayTone(NOTE_C7, 8);
        delay(100);
        this->buzzer_ctrl->PlayTone(NOTE_C7, 8);
        delay(100);
        this->buzzer_ctrl->PlayTone(NOTE_C7, 8);
        delay(100);
        return;
    }

    this->serial_ctrl->WriteRawData("SD-CARD HW-125 Type:     " + this->sdcard_ctrl->GetCardType(), SERIAL_COM);
    this->serial_ctrl->WriteRawData("SD-CARD HW-125 Format:   " + this->sdcard_ctrl->GetPartitionFormat(), SERIAL_COM);
    this->serial_ctrl->WriteRawData("SD-CARD HW-125 Blocks:   " + String(this->sdcard_ctrl->GetPartitionBlocks()), SERIAL_COM);
    this->serial_ctrl->WriteRawData("SD-CARD HW-125 Clusters: " + String(this->sdcard_ctrl->GetPartitionClusters()), SERIAL_COM);
    this->serial_ctrl->WriteRawData("SD-CARD HW-125 Size:     " + String(this->sdcard_ctrl->GetPartitionSizeInMB()) + "MB", SERIAL_COM);
}

//  ----------------------------------------------------------------------------
//  Inicjalizacja, konfiguracja i test kontrolera modulu sensorow temperatury.
void GlobalController::InitializeTemperatureSensors()
{
    this->temp_sensor_ctrl_in = new TemperatureSensorController(A9);
    this->temp_sensor_ctrl_out = new TemperatureSensorController(A8);
    this->serial_ctrl->WriteRawData("DALLAS DS18B20 Thermometer IN:  " + String(this->temp_sensor_ctrl_in->GetTemperature()), SERIAL_COM);
    this->serial_ctrl->WriteRawData("DALLAS DS18B20 Thermometer OUT: " + String(this->temp_sensor_ctrl_out->GetTemperature()), SERIAL_COM);
}

//  ----------------------------------------------------------------------------
//  Inicjalizacja, konfiguracja i test urzadzen peryferyjnych.
void GlobalController::Initialize()
{
    //  Inicjalizacja i konfiguracja diody kontrolnej.
    pinMode(LED_BUILTIN, OUTPUT);

    //  Inicjalizacja, konfiguracja i test modulu kontrolera brzeczyka.
    this->InitializeBuzzer();

    //  Wstepny czas oczekiwania na zainicjalizowanie sie pozostalych urzadzen.
    delay(2000);

    //  Inicjalizacja, konfiguracja i test polaczenia szeregowego i modulow kontrolnych.
    this->keypad_ctrl = new KeypadController();
    this->serial_ctrl = new SerialController();

    //  Inicjalizacja, konfiguracja i test modulu kontrolera czytnika kart sd.
    this->InitializeSdCard();

    //  Inicjalizacja, konfiguracja i test modulu kontrolera zegara czasu rzeczywistego.
    this->InitializeClock();

    //  Inicjalizacja, konfiguracja i test modulu kontrolera fotorezystorow.
    this->InitializePhotoresistors();

    //  Inicjalizacja, konfiguracja i test kontrolera modulu sensorow temperatury.
    this->InitializeTemperatureSensors();

    //  Inicjalizacja, konfiguracja i test modulu kontrolera wyswietlacza.
    this->InitializeDisplay();
}

////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

//  Konstruktor klasy kontrolera globalnego
GlobalController::GlobalController()
{
    this->Initialize();
}

////////////////////////////////////////////////////////////////////////////////
//  *** BRIGHTNESS PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Sprawdzenie czy ustawiona jest jasnosc automatyczna.
 *  @return: True - Jasnosc ustawiona automatycznie; False - w innym przypadku.
 */
bool GlobalController::IsAutoBrightness()
{
    return this->brightness_auto;
}

//  ----------------------------------------------------------------------------
//  Ustawienie jasnosci poprzez opcje jasnosci automatycznej.
void GlobalController::ProcessAutoBrightness()
{
    int _brightness = (this->photoresistor_ctrl_left->GetMappedBrightness(DISPLAY_MAX_BRIGHTNESS)
        + this->photoresistor_ctrl_right->GetMappedBrightness(DISPLAY_MAX_BRIGHTNESS)) / 2;
        
    this->display_ctrl->SetBrightness(_brightness);
}

//  ----------------------------------------------------------------------------
/*  Wlaczenie / wylaczenie automatycznej zmiany jasnosci ekranu.
 * @param enabled: True - wlaczenie automatycznej zmiany jasnosci ekranu; False - wylaczenie.
 */
void GlobalController::SetAutoBrightness(bool enabled)
{
    this->brightness_auto = enabled;
}

////////////////////////////////////////////////////////////////////////////////
//  *** BUZZER HOUR NOTIFIER PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Pobranie aktualnej konfiguracji powiadamiania o zmianie godziny berzeczykiem.
 *  @return: Interwal powiadamiania o zmianie godziny brzeczykiem
 */
int GlobalController::GetBuzzerHourNotifierInterval()
{
    return this->buzzer_hour_change_interval;
}

//  ----------------------------------------------------------------------------
//  Ustawienie interwalu powiadamiania o zmianie godziny brzeczykiem.
void GlobalController::SetBuzzerHourNotifierInterval(int interval = 0)
{
    if (interval >= 0 && interval <= 24 && (interval == 1 || interval % 3 == 0))
        this->buzzer_hour_change_interval = interval;
    else
        this->buzzer_hour_change_interval = 0;
}

//  ----------------------------------------------------------------------------
//  Wywolanie powiadomienia o zmianie godziny brzeczykiem.
void GlobalController::BuzzerNotifyChangeHour()
{
    if (this->buzzer_hour_change_interval > 0)
    {
        Time _now = this->clock_ctrl->Now();

        if (!this->buzzer_hour_change_complete && _now.min == 0 && _now.hour % this->buzzer_hour_change_interval == 0)
        {
            this->buzzer_hour_change_complete = true;
            this->buzzer_ctrl->PlayTone(NOTE_C8, 2);
        }
        else if (this->buzzer_hour_change_complete && _now.min > 0)
        {
            this->buzzer_hour_change_complete = false;
        }
    }
}

////////////////////////////////////////////////////////////////////////////////
//  *** DISPLAY PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Pobranie kontenera tekstu wyswietlacza.
 *  @param index: Indeks strony wyswietlacza (0 - Lewa, 1 - Srodkowa, 2 - Prawa).
 *  @return: Struktura kontenera tekstu wyswietlacza.
 */
DisplayString * GlobalController::GetDisplayString(int index)
{
    return this->display_strings[max(0, min(index, DISPLAY_STRINGS - 1))];
}

//  ----------------------------------------------------------------------------
/*  Pobranie ostatnio wybieranego kontenera tekstu wyswietlacza.
 *  @return: Struktura kontenera tekstu wyswietlacza.
 */
DisplayString * GlobalController::GetSelectedDisplayString()
{
    return this->display_strings[
        max(0, min(this->display_selected_index, DISPLAY_STRINGS - 1))];
}

//  ----------------------------------------------------------------------------
/*  Wybranie kontenera tekstu wyswietlacza jako ostatniego.
 *  Indeks strony wyswietlacza (0 - Lewa, 1 - Srodkowa, 2 - Prawa).
 */
void GlobalController::SelectDisplayString(int index)
{
    this->display_selected_index = max(0, min(index, DISPLAY_STRINGS - 1));
}

////////////////////////////////////////////////////////////////////////////////
//  *** INPUT PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Pobranie indeksu ostatniej wywolanej komendy.
 *  @return: Indeks ostatniej wywolanej komendy.
 */
int GlobalController::GetInputCommand()
{
    return this->input_command;
}

//  ----------------------------------------------------------------------------
/*  Pobranie indeksu ostatniego wcisnietego przycisku.
 *  @return: Indeks ostatniego wcisnietego przycisku.
 */
char GlobalController::GetInputKey()
{
    return this->input_key;
}

//  ----------------------------------------------------------------------------
//  Przetworzenie sygnalu danych wejsciowych z klawiatury.
void GlobalController::ProcessInput()
{
    this->input_key = this->keypad_ctrl->GetPressedKey();
}

////////////////////////////////////////////////////////////////////////////////
//  *** STATES & COMMANDS MANAGEMENT PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Pobranie indeksu aktualnie wyswietlanego elementu.
 *  @result: Indeks aktualnie wyswietlanego elementu.
 */
int GlobalController::GetDisplayingState()
{
    return this->display_state;
}

//  ----------------------------------------------------------------------------
/*  Ustawienie aktualnie wyswietlanego elementu.
 *  @param displaying_state: Indeks wyswietlanego elementu.
 *  @param reset_timer: Zresetowanie czasomierza autmatycznie zmieniajacego wyswietlane elementy.
 */
void GlobalController::SetDisplayingState(int displaying_state, bool reset_timer = true)
{
    if (this->global_state == GLOBAL_STATE_NORMAL)
    {
        this->RequestGlobalStateChange();
        this->display_state = displaying_state % DISPLAY_STATES;

        if (reset_timer)
            this->update_timer->Reset();
    }
}

//  ----------------------------------------------------------------------------
//  Wyswietlenie nastepnych informacji na ekranie w trybie zapetlenia.
void GlobalController::SetNextDisplayingState()
{
    if (this->global_state == GLOBAL_STATE_NORMAL)
    {
        int new_display_state = (this->display_state + 1) % DISPLAY_STATES;

        this->RequestGlobalStateChange();
        this->display_state = new_display_state;
        this->update_timer->Reset();
    }
}

//  ----------------------------------------------------------------------------
/*  Pobranie indeksu aktualnego trybu pracy uzadzenia.
 *  @result: Indeks aktualnego trybu pracy uzadzenia.
 */
int GlobalController::GetGlobalState()
{
    return this->global_state;
}

//  ----------------------------------------------------------------------------
/*  Pobranie informacji czy zmiana aktualnego trybu pracy urzadzenia zostala wywoalana.
 *  @result: True - zmiana trybu pracy urzadzenia zostala wywoalana; False - w innym przypadku.
 */
bool GlobalController::IsGlobalChangeRequested()
{
    return this->global_state_change;
}

//  ----------------------------------------------------------------------------
//  Zadanie zmiany trybu pracy uzadzenia.
void GlobalController::RequestGlobalStateChange()
{
    this->global_state_change = true;
}

//  ----------------------------------------------------------------------------
/*  Ustawienie trybu pracy uzadzenia.
 *  @param machine_state: Indeks trybu pracy uzadzenia.
 */
void GlobalController::SetGlobalState(int machine_state)
{
    this->RequestGlobalStateChange();
    this->global_state = machine_state % GLOBAL_STATES;
    this->serial_ctrl->WriteRawData("Entering mode: " + String(machine_state % GLOBAL_STATES), this->serial_ctrl->GetLastInputDevice());
}

//  ----------------------------------------------------------------------------
//  Zakonczenie zadania zmiany trybu pracy uzadzenia.
void GlobalController::FinalizeGlobalStateChange()
{
    this->global_state_change = false;
    this->input_command = 0;
    this->input_key = 0;
}

#endif