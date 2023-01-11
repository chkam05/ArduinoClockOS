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
#include "message_controller.h"
#include "photoresistor_controller.h"
#include "sd_card_controller.h"
#include "serial_controller.h"
#include "temperature_sensor_controller.h"
#include "alarm.h"
#include "weather.h"


////////////////////////////////////////////////////////////////////////////////
//  *** CONFIGURATION ***
////////////////////////////////////////////////////////////////////////////////

#define COMMAND_NONE                    0
#define COMMAND_PROCESSED_OK            1
#define COMMAND_DISPLAY_DATETIME        2

#define DISPLAY_MODE_INTERVAL           15
#define DISPLAY_STRINGS                 3

#define DISPLAY_STATES                  3
#define DISPLAY_DATETIME_STATE          0
#define DISPLAY_TEMPERATURE_IN_STATE    1
#define DISPLAY_TEMPERATURE_OUT_STATE   2

#define GLOBAL_STATES                   5
#define GLOBAL_STATE_NORMAL             0
#define GLOBAL_STATE_MENU               1
#define GLOBAL_STATE_SETTER             2
#define GLOBAL_STATE_ALARM              3
#define GLOBAL_STATE_MESSAGE            4

const String CONFIG_FILE_NAME = "conf.ini";


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
        int   display_state                 =   DISPLAY_DATETIME_STATE;
        bool  force_display_refresh         =   false;
        int   global_state                  =   GLOBAL_STATE_NORMAL;
        bool  initialized                   =   false;

        String  input_command_value         =   "";
        char    input_key                   =   0;

        //  Display Management
        void  DisplayAlarmIsSet();
        void  DisplayClock();
        void  DisplayDate();
        void  DisplayTemperatureInside();
        void  DisplayTemperatureOutside();
        void  SetNextDisplayingState();

        //  Initialization
        void  InitializeClock();
        void  InitializeBuzzer();
        void  InitializeDisplay();
        void  InitializePhotoresistors();
        void  InitializeSdCard();
        void  InitializeTemperatureSensors();
        void  Initialize();

    public:
        Alarm             * alarm;
        KeypadController  * keypad_ctrl;
        SerialController  * serial_ctrl;

        BuzzerController              * buzzer_ctrl;
        ClockController               * clock_ctrl;
        DisplayController             * display_ctrl;
        MessageController             * msg_ctrl;
        SdCardController              * sdcard_ctrl;
        PhotoresistorController       * photoresistor_ctrl_left;
        PhotoresistorController       * photoresistor_ctrl_right;
        TemperatureSensorController   * temp_sensor_ctrl_in;
        TemperatureSensorController   * temp_sensor_ctrl_out;
        ClockTimer                    * update_timer;

        GlobalController();

        //  Alarm Management.
        void  DisableAlarm(bool save_to_file = true);
        void  SetAlarm(int hour, int minute, bool enabled = true, bool save_to_file = true);

        //  Brightness Management.
        bool  IsAutoBrightness();
        void  SetAutoBrightness(bool enabled, bool save_to_file = true);
        void  SetBrightness(int brightness, bool save_to_file = true);

        //  Buzzer Management.
        int   GetBuzzerHourNotifierInterval();
        void  SetBuzzerHourNotifierInterval(int interval = 0, bool save_to_file = true);

        //  Data Management.
        void  ProcessAlarm();
        void  ProcessAutoBrightness(bool override = false);
        void  ProcessBeepHour();
        void  ProcessSecondLedBlinking();
        void  ProcessFunctionalities();

        //  Date & Time Management.
        void  SetDate(int day, int day_week, int month, int year);
        void  SetDate(int day, int month, int year);
        void  SetTime(int hour, int min, int sec);
        void  SetTime(int hour, int min);

        //  Display Management.
        void            ForceDisplayRefresh();
        int             GetDisplayingState();
        DisplayString * GetDisplayString(int index);
        void            ProcessDisplay(bool force_refresh = false);
        void            SetDisplayingState(int displaying_state, bool reset_timer = true);

        //  Input.
        String  GetInputCommand();
        char    GetInputKey();
        bool    IsCommandValueInputed();
        void    ProcessInput();

        //  Machine States Management.
        bool  IsInitialized();
        int   GetMachineState();
        void  SetMachineState(int machine_state);
        void  FinalizeCycle();

        //  Save & Load.
        void  LoadData();
        void  SaveData();
};


////////////////////////////////////////////////////////////////////////////////
//  *** DISPLAY MANAGEMENT PRIVATE METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

//  Wyswietlanie ikonki alarmu kiedy jest ustawiony.
void GlobalController::DisplayAlarmIsSet()
{
    DisplayString * dsp_str   = this->display_strings[TEXT_ALIGN_RIGHT];
    int             position  = dsp_str->_xpos - 7;

    switch (this->alarm->GetState())
    {
        case ALARM_DISARMED:
            this->display_ctrl->DrawSprite(SPRITE_ALARM, position, 1);
            break;
        
        case ALARM_SUSPENDED:
            this->display_ctrl->DrawSprite(SPRITE_ALARM, position, 2);
            break;
    }    
}

//  ----------------------------------------------------------------------------
//  Wyswietlenie zegara na prawej stronie wyswietlacza.
void GlobalController::DisplayClock()
{
    DisplayString * dsp_str = this->display_strings[TEXT_ALIGN_RIGHT];
    bool            blink   = this->clock_ctrl->GetBlink();
    
    dsp_str->text   = this->clock_ctrl->GetTime("HM", ':', blink);
    dsp_str->offset = 1;
    this->display_ctrl->PrintDS(dsp_str, true);
}

//  ----------------------------------------------------------------------------
//  Wyswietlenie daty na lewej stronie wyswietlacza.
void GlobalController::DisplayDate()
{
    DisplayString * dsp_str = this->display_strings[TEXT_ALIGN_LEFT];

    dsp_str->text   = this->clock_ctrl->GetDate("DMy", '-');
    dsp_str->offset = 1;
    dsp_str->_xpos  = 0;

    this->display_ctrl->PrintDS(dsp_str, true);
}

//  ----------------------------------------------------------------------------
//  Wyswietlenie temperatury wewnetrznej na lewej stronie wyswietlacza.
void GlobalController::DisplayTemperatureInside()
{
    DisplayString * dsp_str = this->display_strings[TEXT_ALIGN_LEFT];
    int             temp    = this->temp_sensor_ctrl_in->GetTemperature();

    dsp_str->text   =   temp <= TEMPERATURE_SENSOR_NULL ? "-`C" : String(temp) + "`C";
    dsp_str->offset =   10;
    dsp_str->_xpos  =   8;
    dsp_str->_width +=  2;

    this->display_ctrl->DrawSprite(SPRITE_HOME, 0, 0);
    this->display_ctrl->PrintDS(dsp_str, true);
}

//  ----------------------------------------------------------------------------
//  Wyswietlenie temperatury zewnetrznej na lewej stronie wyswietlacza.
void GlobalController::DisplayTemperatureOutside()
{
    DisplayString * dsp_str = this->display_strings[TEXT_ALIGN_LEFT];
    int             temp    = this->temp_sensor_ctrl_out->GetTemperature();

    dsp_str->text   =   temp <= TEMPERATURE_SENSOR_NULL ? "-`C" : String(temp) + "`C";
    dsp_str->offset =   10;
    dsp_str->_xpos  =   8;
    dsp_str->_width +=  2;

    this->display_ctrl->DrawSprite(SPRITE_WEATHER, 0, 0);
    this->display_ctrl->PrintDS(dsp_str, true);
}

//  ----------------------------------------------------------------------------
//  Wyswietlenie nastepnych informacji na ekranie w trybie zapetlenia.
void GlobalController::SetNextDisplayingState()
{
    int new_display_state = (this->display_state + 1) % DISPLAY_STATES;

    this->display_state = new_display_state;
    this->update_timer->Reset();
}

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

    //  Inicjalizacja kontenera wiadomosci.
    this->msg_ctrl = new MessageController(this->display_ctrl);

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

    //  Zaladowanie danych z pliku.
    this->LoadData();
}

////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

//  Konstruktor klasy kontrolera globalnego
GlobalController::GlobalController()
{
    this->alarm = new Alarm();
    this->Initialize();

    this->initialized = true;
}

////////////////////////////////////////////////////////////////////////////////
//  *** ALARM PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

//  Rozbrojenie alarmu.
void GlobalController::DisableAlarm(bool save_to_file = true)
{
    this->alarm->DisableAlarm();

    if (save_to_file)
        this->SaveData();
}

//  ----------------------------------------------------------------------------
/*  Ustawienie godziny uruchomienia alarmu.
 *  @param hour: Godzina uruchomienia alarmu.
 *  @param minute: Minuta uruchomienia alarmu.
 */
void GlobalController::SetAlarm(int hour, int minute, bool enabled = true, bool save_to_file = true)
{
    this->alarm->SetAlarm(hour, minute, enabled);

    if (save_to_file)
        this->SaveData();
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
/*  Wlaczenie / wylaczenie automatycznej zmiany jasnosci ekranu.
 * @param enabled: True - wlaczenie automatycznej zmiany jasnosci ekranu; False - wylaczenie.
 */
void GlobalController::SetAutoBrightness(bool enabled, bool save_to_file = true)
{
    this->brightness_auto = enabled;

    if (save_to_file)
        this->SaveData();
}

//  ----------------------------------------------------------------------------
/* Ustawienie nowej wartosci jasnosci wyswietlacza.
 * @param brightness: Nowa wartosc jasnosci wyswietlacza miedzy.
 */
void GlobalController::SetBrightness(int brightness, bool save_to_file = true)
{
    this->brightness_auto = false;
    this->display_ctrl->SetBrightness(brightness);

    if (save_to_file)
        this->SaveData();
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
void GlobalController::SetBuzzerHourNotifierInterval(int interval = 0, bool save_to_file = true)
{
    if (interval >= 0 && interval <= 24 && (interval == 1 || interval % 3 == 0))
        this->buzzer_hour_change_interval = interval;
    else
        this->buzzer_hour_change_interval = 0;
    
    if (save_to_file)
        this->SaveData();
}

////////////////////////////////////////////////////////////////////////////////
//  *** DATA MANAGEMENT PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

//  Sprawdzenie stanu alarmu, czy ma zostac uruchomiony.
void GlobalController::ProcessAlarm()
{
    if (this->alarm->IsEnabled())
    {
        Time datetime_now = this->clock_ctrl->Now();
        
        if (this->alarm->CheckTrigger(datetime_now))
            this->SetMachineState(GLOBAL_STATE_ALARM);
    }
}

//  ----------------------------------------------------------------------------
//  Ustawienie jasnosci poprzez opcje jasnosci automatycznej.
void GlobalController::ProcessAutoBrightness(bool override = false)
{
    if (this->brightness_auto || override)
    {
        int brightness  = (this->photoresistor_ctrl_left->GetMappedBrightness(DISPLAY_MAX_BRIGHTNESS)
            + this->photoresistor_ctrl_right->GetMappedBrightness(DISPLAY_MAX_BRIGHTNESS)) / 2;
        
        this->display_ctrl->SetBrightness(brightness);
    }
}

//  ----------------------------------------------------------------------------
//  Wywolanie powiadomienia o zmianie godziny brzeczykiem.
void GlobalController::ProcessBeepHour()
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

//  ----------------------------------------------------------------------------
//  Miganie wbudowana dioda led podczas zmianiy sekundy.
void GlobalController::ProcessSecondLedBlinking()
{
    bool blink = this->clock_ctrl->GetBlink();
    digitalWrite(LED_BUILTIN, blink ? LOW : HIGH);
}

//  ----------------------------------------------------------------------------
//  Przetwarzanie funkcjonalnosci.
void GlobalController::ProcessFunctionalities()
{
    this->ProcessSecondLedBlinking();
    this->ProcessAutoBrightness();
    this->ProcessBeepHour();
    this->ProcessAlarm();
}

////////////////////////////////////////////////////////////////////////////////
//  *** DATE AND TIME MANAGEMENT PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Ustawienie nowej daty.
 *  @param day: Dzien w miesiacu od 1 do 31.
 *  @param day_week: Dzien tygodnia od 1 do 7.
 *  @param month: Miesiac od 1 do 12.
 *  @param year: Rok.
 */
void GlobalController::SetDate(int day, int day_week, int month, int year)
{
    this->clock_ctrl->SetDate(day, day_week, month, year);
}

//  ----------------------------------------------------------------------------
/*  Ustawienie nowej daty.
 *  @param day: Dzien w miesiacu od 1 do 31.
 *  @param month: Miesiac od 1 do 12.
 *  @param year: Rok.
 */
void GlobalController::SetDate(int day, int month, int year)
{
    this->clock_ctrl->SetDate(day, month, year);
}

//  ----------------------------------------------------------------------------
/*  Ustawienie nowego czasu.
 *  @param hour: Godzina od 0 do 23.
 *  @param min: Minuta od 0 do 59.
 *  @param sec: Sekunda od 0 do 59.
 */
void GlobalController::SetTime(int hour, int min, int sec)
{
    this->clock_ctrl->SetTime(hour, min, sec);
}

//  ----------------------------------------------------------------------------
/*  Ustawienie nowego czasu.
 *  @param hour: Godzina od 0 do 23.
 *  @param min: Minuta od 0 do 59.
 */
void GlobalController::SetTime(int hour, int min)
{
    this->clock_ctrl->SetTime(hour, min);
}

////////////////////////////////////////////////////////////////////////////////
//  *** DISPLAY MANAGEMENT PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

//  Force display refresh to show data for current state.
void GlobalController::ForceDisplayRefresh()
{
    this->force_display_refresh = true;
}

//  ----------------------------------------------------------------------------
/*  Pobranie indeksu aktualnie wyswietlanego elementu.
 *  @result: Indeks aktualnie wyswietlanego elementu.
 */
int GlobalController::GetDisplayingState()
{
    return this->display_state;
}

//  ----------------------------------------------------------------------------
/*  Pobranie kontenera tekstu wyswietlacza.
 *  @param index: Indeks strony wyswietlacza (0 - Lewa, 1 - Srodkowa, 2 - Prawa).
 *  @return: Struktura kontenera tekstu wyswietlacza.
 */
DisplayString * GlobalController::GetDisplayString(int index)
{
    return this->display_strings[max(0, min(index, DISPLAY_STRINGS - 1))];
}

//  ----------------------------------------------------------------------------
//  Wyswietlanie podstawowych danych na ekranie.
void GlobalController::ProcessDisplay(bool force_refresh = false)
{
    bool force_update = this->force_display_refresh || force_refresh;

    if (!force_update)
    {
        //  Pobranie danych i przetworzenie zmian automatycznych.
        Time  datetime_now  = this->clock_ctrl->Now();
        bool  auto_switch   = this->update_timer->Work(datetime_now);
        bool  day_changed   = this->clock_ctrl->HasDayChanged();

        if (day_changed)
            this->SetDisplayingState(DISPLAY_DATETIME_STATE);

        else if (auto_switch)
            this->SetNextDisplayingState();
        
        force_update = auto_switch || day_changed;
    }

    //  Wyswietlenie danych na ekranie.
    if (force_update)
    {
        switch (this->display_state)
        {
            case DISPLAY_DATETIME_STATE:
                this->DisplayDate();
                break;
            
            case DISPLAY_TEMPERATURE_IN_STATE:
                this->DisplayTemperatureInside();
                break;
            
            case DISPLAY_TEMPERATURE_OUT_STATE:
                this->DisplayTemperatureOutside();
                break;
        }

        if (this->alarm->IsEnabled())
            this->DisplayAlarmIsSet();
    }

    this->DisplayClock();
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
        this->force_display_refresh = true;
        this->display_state = displaying_state % DISPLAY_STATES;

        if (reset_timer)
            this->update_timer->Reset();
    }
}

////////////////////////////////////////////////////////////////////////////////
//  *** INPUT PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Pobranie ostatniej wywolanej komendy.
 *  @returnP Ostatnia wywolana komenda.
 */
String GlobalController::GetInputCommand()
{
    return this->input_command_value;
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
/*  Sprawdzenie czy zostala wprowadzona komenda z portu szeregowego.
 *  @return: True - komenda została wprowadzona; False - w innym wypadku.
 */
bool GlobalController::IsCommandValueInputed()
{
    return this->input_command_value != NULL && this->input_command_value.length() > 0;
}

//  ----------------------------------------------------------------------------
/*  Przetworzenie sygnalu danych wejsciowych z klawiatury.
 *  @param command_processor: Przetwarzacz polecen tekstowych.
 */
void GlobalController::ProcessInput()
{
    //  Odczytanie danych przychodzacych z urzadzen wejscia/wyjscia.
    this->input_command_value = this->serial_ctrl->ReadInputData();
    this->input_key = this->keypad_ctrl->GetPressedKey();
}

////////////////////////////////////////////////////////////////////////////////
//  *** MACHINE STATES MANAGEMENT PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Pobranie informacji o poprawnej inicjalizacji urzadzenia.
 *  @result: Informacja o poprawnej inicjalizacji urzadzenia.
 */
bool GlobalController::IsInitialized()
{
    return this->initialized;
}

//  ----------------------------------------------------------------------------
/*  Pobranie indeksu aktualnego trybu pracy uzadzenia.
 *  @result: Indeks aktualnego trybu pracy uzadzenia.
 */
int GlobalController::GetMachineState()
{
    return this->global_state;
}

//  ----------------------------------------------------------------------------
/*  Ustawienie trybu pracy uzadzenia.
 *  @param machine_state: Indeks trybu pracy uzadzenia.
 */
void GlobalController::SetMachineState(int machine_state)
{
    this->force_display_refresh = true;
    this->global_state = machine_state % GLOBAL_STATES;
    this->serial_ctrl->WriteRawData("Entering mode: " + String(machine_state % GLOBAL_STATES), this->serial_ctrl->GetLastInputDevice());
}

//  ----------------------------------------------------------------------------
//  Zakonczenie zadania zmiany trybu pracy uzadzenia.
void GlobalController::FinalizeCycle()
{
    this->force_display_refresh = false;
    this->input_command_value = "";
    this->input_key = 0;
}

////////////////////////////////////////////////////////////////////////////////
//  *** MACHINE STATES MANAGEMENT PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

//  Zaladowanie konfiguracji z pliku.
void GlobalController::LoadData()
{
    char character = ' ';
    String line = "";

    if (this->sdcard_ctrl->IsInitialized() && this->sdcard_ctrl->IsMounted())
    {
        if (!this->sdcard_ctrl->FileExists(CONFIG_FILE_NAME))
            return;
        
        File file = this->sdcard_ctrl->OpenFileToRead(CONFIG_FILE_NAME);

        this->serial_ctrl->WriteRawData("", SERIAL_COM);
        this->serial_ctrl->WriteRawData("Loading data from file...", SERIAL_COM);
        this->serial_ctrl->WriteRawData("", SERIAL_COM);

        while (file.available()) {
            while (file.available() && character != '\n')
            {
                character = file.read();

                if (character == '\t' || character == '\r')
                    continue;

                if (character != '\n')
                    line += character;
                else
                    break;
            }
            
            line.toLowerCase();
            Serial.println(line);

            //  Load alarm configuration.
            if (line.startsWith("alarm="))
            {
                line = line.substring(6);

                if (line.length() > 0)
                {
                    int colon_index = line.indexOf(':');
                    int space_index = line.indexOf(' ');

                    int alarm_hour = line.substring(0, colon_index).toInt();
                    int alarm_min = line.substring(colon_index+1, space_index).toInt();
                    bool alarm_is_enabled = (line.substring(space_index+1, space_index+3) == "on");

                    this->SetAlarm(alarm_hour, alarm_min, alarm_is_enabled, false);
                }
            }

            //  Load beep hours configuration.
            else if (line.startsWith("beep_hours="))
            {
                line = line.substring(11);

                if (line.length() > 0)
                    this->SetBuzzerHourNotifierInterval(line.toInt(), false);
            }

            //  Load brightness configuration.
            else if (line.startsWith("brightness="))
            {
                line = line.substring(11);

                if (line.length() > 1 && line.substring(0, 4) == String("auto"))
                    this->SetAutoBrightness(true, false);

                else if (line.length() > 0)
                    this->SetBrightness(max(0, min(8, line.toInt())), false);
            }

            character = ' ';
            line = "";
        }

        file.close();

        this->serial_ctrl->WriteRawData("", SERIAL_COM);
        this->serial_ctrl->WriteRawData("Configuration loaded!", SERIAL_COM);
        this->serial_ctrl->WriteRawData("", SERIAL_COM);
    }
}

//  ----------------------------------------------------------------------------
//  Zapis konfiguracji do pliku.
void GlobalController::SaveData()
{
    if (this->sdcard_ctrl->IsInitialized() && this->sdcard_ctrl->IsMounted())
    {
        File file = this->sdcard_ctrl->OpenFileToWrite(CONFIG_FILE_NAME);

        String alarm_data = String(this->alarm->hour) + ":" + String(this->alarm->minute) + " " + (this->alarm->IsEnabled() ? "on" : "off");
        String beep_data = String(this->buzzer_hour_change_interval);
        String brightness_data = this->brightness_auto ? "auto" : String(this->display_ctrl->GetBrightness());

        file.println("[configuration]");

        this->serial_ctrl->WriteRawData("Saving alarm: " + alarm_data, SERIAL_COM);
        file.println("alarm=" + alarm_data);

        this->serial_ctrl->WriteRawData("Saving beep hours: " + beep_data, SERIAL_COM);
        file.println("beep_hours=" + beep_data);

        this->serial_ctrl->WriteRawData("Saving brightness: " + brightness_data, SERIAL_COM);
        file.println("brightness=" + brightness_data);

        file.close();
    }
}

#endif