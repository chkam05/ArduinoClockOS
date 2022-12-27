////////////////////////////////////////////////////////////////////////////////
//  DALLAS LED MATRIX 8X8 MAX7219
////////////////////////////////////////////////////////////////////////////////

#ifndef DISPLAY_CONTROLLER_H
#define DISPLAY_CONTROLLER_H

////////////////////////////////////////////////////////////////////////////////
//  *** INCLUDED LIBRARIES ***
////////////////////////////////////////////////////////////////////////////////

#include <MaxMatrix.h>
#include "src/fonts.h"
#include "src/sprites.h"


////////////////////////////////////////////////////////////////////////////////
//  *** CONFIGURATION ***
////////////////////////////////////////////////////////////////////////////////

#define FONT_DIGITAL              0

#define DISPLAY_INIT_DELAY        1000
#define DISPLAY_MIN_BRIGHTNESS    0
#define DISPLAY_MAX_BRIGHTNESS    8
#define DISPLAY_PIN_CLK           10
#define DISPLAY_PIN_CS            11
#define DISPLAY_PIN_DIN           12
#define DISPLAY_SEGMETNS          1
#define DISPLAY_SEGMENT_HEIGHT    8
#define DISPLAY_SEGMENT_WIDTH     8

#define TEXT_ALIGN_LEFT           0
#define TEXT_ALIGN_CENTER         1
#define TEXT_ALIGN_RIGHT          2


////////////////////////////////////////////////////////////////////////////////
//  *** STRUCT DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

struct DisplayString
{
    //  --- VARIABLES: ---
    int _xpos       =   0;
    int _width      =   0;
    
    int font        =   FONT_DIGITAL;
    int offset      =   0;
    int step_delay  =   0;
    int text_align  =   TEXT_ALIGN_LEFT;
    String text     =   "";

    //  --- METHODS: ---
    DisplayString()
    {
        //
    }

    /*  Konstruktor struktury tekstu wyswietlacza - pełny.
     *  @param font: Indeks tablicy zawierajacej okreslona czcionke.
     *  @param offset: Przesuniecie wyswietlanego tekstu o kolumny w lewo (-) lub prawo (+).
     *  @param text_align: Wyrownanie tekstu do okreslonej pozycji na ekranie.
     *  @param text: Tekst ktory ma zostac wyswietlony.
     *  @param step_delay: Czas oczekiwania w milisekundach pomiedzy wyswietlaniem pojedynczych znakow.
     */
    DisplayString(int font, int offset, int text_align, String text, int step_delay)
    {
        this->font = font;
        this->offset = offset;
        this->text_align = max(TEXT_ALIGN_LEFT, min(text_align, TEXT_ALIGN_RIGHT));
        this->text = text;
    }

    /*  Konstruktor struktury tekstu wyswietlacza - prosty.
     *  @param font: Indeks tablicy zawierajacej okreslona czcionke.
     *  @param offset: Przesuniecie wyswietlanego tekstu o kolumny w lewo (-) lub prawo (+).
     *  @param text_align: Wyrownanie tekstu do okreslonej pozycji na ekranie.
     *  @param text: Tekst ktory ma zostac wyswietlony.
     */
    DisplayString(int font, int text_align, String text)
    {
        this->font = font;
        this->text_align = max(TEXT_ALIGN_LEFT, min(text_align, TEXT_ALIGN_RIGHT));
        this->text = text;      
    }
};


////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class DisplayController
{
    private:
        MaxMatrix *base;
        
        bool  custom_segmentation  =  false;
        bool  initialized          =  false;
        byte  buffer[10]           =  { 0, 0, B00000000, B00000000, B00000000, B00000000, B00000000, B00000000, B00000000, B00000000 };
        int   brightness           =  DISPLAY_MIN_BRIGHTNESS;
        int   segments             =  DISPLAY_SEGMETNS;

        const byte  *GetMappedFont(int font);
        void  Initialize();
        void  LoadCharacter(int font, int char_index);

        void  PrintDSCenter(DisplayString *ds, bool force_clear);
        void  PrintDSLeft(DisplayString *ds, bool force_clear);
        void  PrintDSRight(DisplayString *ds, bool force_clear);

    public:
        DisplayController(int brightness, int segments);
        
        int     GetBrightness();
        void    SetBrightness(int brightness);

        int     GetLastColumnIndex();
        int     GetWidth();
        bool    IsInitialized();

        void    Clear();
        void    ClearColumn(int column_index);
        void    ClearRange(int first_col_index, int last_col_index, int step_delay = 0);

        void    DrawPoint(int x, int y, int value);
        int     DrawSprite(const byte *sprite, int x, int sprite_index);
        int     PrintChar(int font, int x, char character);
        int     PrintText(int font, int x, String text, int step_delay);

        int     GetTextWidth(int font, String text);
        String  ClampText(int font, String text, int *width, int first_char, int left_offset, int right_offset);
        
        void    ClearDS(DisplayString *ds);
        void    PrintDS(DisplayString *ds, bool force_clear = true);
};

////////////////////////////////////////////////////////////////////////////////
//  *** PRIVATE METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

//  Inicjalizacja wyswietla i jego podstawowa konfiguracje.
void DisplayController::Initialize()
{
    this->base = new MaxMatrix(
        DISPLAY_PIN_DIN,  //  Data input
        DISPLAY_PIN_CS,   //  Chip select
        DISPLAY_PIN_CLK,  //  Clock
        this->segments
    );

    //  Inicjalizacja wyswietlacza.
    this->base->init();
    this->initialized = true;

    //  Opoznienie obslugi wyswietlacza.
    delay(DISPLAY_INIT_DELAY);

    //  Ustawienie poczatkowej jasnosci wyswietlacza.
    this->base->setIntensity(this->brightness);
}

//  ----------------------------------------------------------------------------
/* Wybor tablicy zawierajacej okreslona czcionke za pomoca przypisanemu jej indeksowi.
 * @param font: Indeks tablicy zawierajacej okreslona czcionke.
 * @return: Wybrana tablica czcionki.
 */
const byte * DisplayController::GetMappedFont(int font)
{
    switch (font)
    {
        case FONT_DIGITAL:
        default:
            return FONT_5x3;
    }
}

//  ----------------------------------------------------------------------------
/* Zaladowanie znaku do pamieci w celu wyswietlenia go na ekranie.
 * @param font: Indeks tablicay zawierajacej czcionke.
 * @param char_index: Indeks znaku w tablicy.
 */
void DisplayController::LoadCharacter(int font, int char_index)
{
    memcpy_P(this->buffer, this->GetMappedFont(font) + ((char_index - 32) * 10), 10);
}

//  ----------------------------------------------------------------------------
/* Wyswietlenie na ekranie wycentrowanego tekstu przy pomocy struktury DisplayString.
 * @param ds: Struktura DisplayString z informacjami o wyswietlanym tekscie.
 * @param force_clear: Flaga zezwalajaca na wyczyszczenie poprzedniego wyswietlanego tekstu.
 */
void DisplayController::PrintDSCenter(DisplayString *ds, bool force_clear = true)
{
    //  Inicjalizacja zmiennych roboczych/tymczasowych.
    int display_width = this->GetWidth();
    int prev_xpos = ds->_xpos;
    int prev_width = ds->_width;

    //  Obliczenie pozycji startowej tekstu i jego dlugosc.
    ds->_width = this->GetTextWidth(ds->font, ds->text);
    ds->_xpos = (display_width/2) - (ds->_width/2) + ds->offset;

    //  Wyczyszczenie poprzedniego tekstu (od lewej) jezeli flaga czyszczenia jest aktywna.
    if (force_clear && prev_xpos < ds->_xpos)
        this->ClearRange(prev_xpos, ds->_xpos);

    //  Wyswietlenie tekstu na ekranie.
    this->PrintText(ds->font, ds->_xpos, ds->text, ds->step_delay);

    //  Wyczyszczenie poprzedniego tekstu (od prawej) jezeli flaga czyszczenia jest aktywna.
    if (force_clear && ds->_xpos + ds->_width < prev_xpos + prev_width)
        this->ClearRange(ds->_xpos + ds->_width, prev_xpos + prev_width);
}

//  ----------------------------------------------------------------------------
/* Wyswietlenie na ekranie wyrownanego do lewej tekstu przy pomocy struktury DisplayString.
 * @param ds: Struktura DisplayString z informacjami o wyswietlanym tekscie.
 * @param force_clear: Flaga zezwalajaca na wyczyszczenie poprzedniego wyswietlanego tekstu.
 */
void DisplayController::PrintDSLeft(DisplayString *ds, bool force_clear = true)
{
    //  Inicjalizacja zmiennych roboczych/tymczasowych.
    int display_width = this->GetWidth();
    int prev_xpos = ds->_xpos;
    int prev_width = ds->_width;

    //  Obliczenie pozycji startowej tekstu.
    ds->_xpos = max(0, min(ds->offset, display_width-1));

    //  Wyczyszczenie poprzedniego tekstu (od lewej) jezeli flaga czyszczenia jest aktywna.
    if (force_clear && prev_xpos < ds->_xpos)
        this->ClearRange(prev_xpos, ds->_xpos);

    //  Wyswietlenie tekstu na ekranie i obliczenie jego dlugosci.
    ds->_width = this->PrintText(ds->font, ds->_xpos, ds->text, ds->step_delay);

    //  Wyczyszczenie poprzedniego tekstu (od prawej) jezeli flaga czyszczenia jest aktywna.
    if (force_clear && ds->_xpos + ds->_width < prev_xpos + prev_width)
        this->ClearRange(ds->_xpos + ds->_width, prev_xpos + prev_width);
}

//  ----------------------------------------------------------------------------
/* Wyswietlenie na ekranie wyrownanego do prawej tekstu przy pomocy struktury DisplayString.
 * @param ds: Struktura DisplayString z informacjami o wyswietlanym tekscie.
 * @param force_clear: Flaga zezwalajaca na wyczyszczenie poprzedniego wyswietlanego tekstu.
 */
void DisplayController::PrintDSRight(DisplayString *ds, bool force_clear = true)
{
    //  Inicjalizacja zmiennych roboczych/tymczasowych.
    int display_width = this->GetWidth();
    int prev_xpos = ds->_xpos;
    int prev_width = ds->_width;

    //  Obliczenie pozycji startowej tekstu i jego dlugosc.
    ds->_width = this->GetTextWidth(ds->font, ds->text);
    ds->_xpos = max(0, display_width - ds->_width - ds->offset);
    
    //  Wyczyszczenie poprzedniego tekstu (od lewej) jezeli flaga czyszczenia jest aktywna.
    if (force_clear && prev_xpos < ds->_xpos)
        this->ClearRange(prev_xpos, ds->_xpos);

    //  Wyswietlenie tekstu na ekranie i obliczenie jego dlugosci.
    this->PrintText(ds->font, ds->_xpos, ds->text, ds->step_delay);

    //  Wyczyszczenie poprzedniego tekstu (od prawej) jezeli flaga czyszczenia jest aktywna.
    if (force_clear && ds->_xpos + ds->_width < prev_xpos + prev_width)
        this->ClearRange(ds->_xpos + ds->_width, prev_xpos + prev_width);
}


////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/* Konstruktor klasy modułu wyswietlacza DALLAS LED MATRIX 8X8 MAX7219.
 * @param brightness: Poczatkowe ustawienie jasnosci wyswietlacza.
 * @param init_delay: Czas oczekiwania w milisekundach po zainicjalizowaniu wyswietlacza.
 * @param segments: Ilosc segmentow z ktorych sklada sie wyswietlacz.
 */
DisplayController::DisplayController(
  int brightness = DISPLAY_MIN_BRIGHTNESS,
  int segments = DISPLAY_SEGMETNS)
{
    this->segments = max(1, segments);
    
    this->SetBrightness(brightness);
    this->Initialize();
}

//  ----------------------------------------------------------------------------
/* Pobranie aktualnej wartosci jasnosci wyswietlacza.
 * @return: Aktualna wartosc jasnosci wyswietlacza.
 */
int DisplayController::GetBrightness()
{
    return this->brightness;
}

//  ----------------------------------------------------------------------------
/* Ustawienie nowej wartosci jasnosci wyswietlacza.
 * @param brightness: Nowa wartosc jasnosci wyswietlacza miedzy.
 */
void DisplayController::SetBrightness(int brightness = DISPLAY_MIN_BRIGHTNESS)
{
    //  Zapisanie nowej wartosci jasnosci dla wyswietlacza.
    this->brightness = max(DISPLAY_MIN_BRIGHTNESS, min(brightness, DISPLAY_MAX_BRIGHTNESS));

    //  Ustawienie nowej wartosci wyswietlacza jezeli zainicjalizowany.
    if (this->initialized)
        this->base->setIntensity(this->brightness);
}

//  ----------------------------------------------------------------------------
/* Obliczenie indeksu ostatniej kolumny dostepnej na ekranie.
 * @return: Indeks ostatniej kolumny dostepnej na ekranie.
 */
int DisplayController::GetLastColumnIndex()
{
    return this->GetWidth() - 1;
}

//  ----------------------------------------------------------------------------
/* Obliczenie ilosci dostepnych kolumn na ekranie.
 * @return: Ilosc dostepnych kolumn na ekranie.
 */
int DisplayController::GetWidth()
{
    return this->segments * DISPLAY_SEGMENT_WIDTH;
}

//  ----------------------------------------------------------------------------
/* Sprawdzenie czy ekran zostal zainicjalizowany.
 * @return: True - ekran zostal zainicjalizowany; False - w innym wypadku.
 */
bool DisplayController::IsInitialized()
{
    return this->initialized;
}

//  ----------------------------------------------------------------------------
//  Wyczysczenie aktualnej wyswietlanej zawartosci ekranu.
void DisplayController::Clear()
{
    //  Wyczyszczenie ekranu jezeli jest zainicjalizowany.
    if (this->initialized)
        this->base->clear();
}

//  ----------------------------------------------------------------------------
/* Wyczysczenie zawartosci pojedynczej kolumny na ekranie.
 * @param column_index: Indeks kolumny ktora ma zostac wyczyszczona.
 */
void DisplayController::ClearColumn(int column_index)
{
    //  Sprawdzenie czy wybrany indeks kolumny nie wykracza poza granice ekranu.
    if (column_index < 0 || column_index > this->GetLastColumnIndex())
        return;

    //  Wyczyszczenie wybranej kolumny ekranu jezeli jest zainicjalizowany.
    if (this->initialized)
        this->base->setColumn(column_index, 0);
}

//  ----------------------------------------------------------------------------
/* Wyczysczenie wybranych kolumn ekranu.
 * @param first_col_index: Indeks pierwszej kolumny ekranu ktora ma zostac wyczyszczona.
 * @param last_col_index: Indeks ostatniej kolumny ekranu ktora ma zostac wyczyszczona.
 * @param step_delay: Czas oczekiwania w milisekundach po wyczyszczeniu pojedynczej kolumny.
 */
void DisplayController::ClearRange(int first_col_index, int last_col_index, int step_delay = 0)
{
    //  Korekta indeksow wybranych kolumn.
    int col1 = max(0, min(first_col_index, this->GetLastColumnIndex()));
    int col2 = max(col1, min(last_col_index, this->GetLastColumnIndex()));

    //  Wyczyszczenie wybranych kolumn ekranu jezeli jest zainicjalizowany.
    if (this->initialized)
    {
        for (int col = col1; col < col2; col++)
        {
            //  Wyczyszczenie kolumny ekranu.
            this->base->setColumn(col, 0);

            //  Opoznienie po wyczyszczeniu kolumny ekranu.
            if (step_delay > 0)
                delay(step_delay);
        }
    }
}

//  ----------------------------------------------------------------------------
/* Ustawienie lub wyczyszczenie pojedynczego punktu na ekranie.
 * @param x: Indeks kolumny ekranu na ktorej ma zostac ustawiony badz wyczyszcony punkt.
 * @param y: Indkes punktu w osi Y ekranu ktory ma zostac ustawiony badz wyczyszcony.
 * @param value: Wartosc definiujaca zdarzenie ustawiania badz czyszczenia
 *               0 - wyczyszczenie, 1 - ustawienie.
 */
void DisplayController::DrawPoint(int x, int y, int value)
{
    //  Sprawdzenie czy wybrane wspolrzedne nie wykracza poza granice ekranu.
    if (x < 0 || x > this->GetLastColumnIndex())
        return;

    if (y < 0 || y >= DISPLAY_SEGMENT_HEIGHT)
        return;

    //  Narysowanie badz wyczyszczenie punktu na ekranie jezeli zostal zainicjalizowany.
    if (this->initialized)
        this->base->setDot(x, y, max(0, min(value, 1)));
}

//  ----------------------------------------------------------------------------
/* Wyswietlenie obrazka na ekranie w wybranej osi X.
 * @param sprite: Tablica zawierajaca obrazek ktory ma zostac narysowany.
 * @param x: Indeks kolumny ekranu od ktorej ma zostac narysowany obrazek w prawo.
 * @param sprite_index: Indeks obrazka w tablicy obrazkow ktory ma zostac narysowany.
 * @return: Dlugosc narysowanego obrazka.
 */
int DisplayController::DrawSprite(const byte *sprite, int x, int sprite_index)
{
    //  Zaladowanie obrazka do pamieci podrecznej.
    memcpy_P(this->buffer, sprite + (sprite_index * 10), 10);

    //  Narysowanie obrazka na ekranie jezeli zostal zainicjalizowany.
    if (this->initialized)
    {
        this->base->writeSprite(x, 0, this->buffer);
        return this->buffer[0];
    }

    return 0;
}

//  ----------------------------------------------------------------------------
/* Wyswietlenie znaku na ekranie.
 * @param font: Indeks tablicay zawierajacej czcionke w jakiej tekst ma zostac wyswietlony na ekranie.
 * @param x: Indeks kolumny ekranu od ktorej znak ma zostac wyswietlony w prawo.
 * @param character: Znak ktory ma zostac wyswietlony.
 * @return: Dlugosc wyswietlonego znaku.
 */
int DisplayController::PrintChar(int font, int x, char character)
{
    //  Zaladowanie znaku do pamieci podrecznej.
    this->LoadCharacter(font, character);

    //  Wyswietlenie znaku na ekranie jezeli zostal zainicjalizowany.
    if (this->initialized)
    {
        //  Manualne wyswietlenie znaku.
        if (this->custom_segmentation)
        {
            //  Wstepna konfiguracja zmiennych roboczych.
            int char_width = this->buffer[0];
            int xpos = x;
            int result_width = char_width;

            while (result_width > 0)
            {
                //  Oblicznie dlugosci znaku w okreslonym segmentcie ekranu.
                int comp = DISPLAY_SEGMENT_WIDTH - (xpos % DISPLAY_SEGMENT_WIDTH);
                int rest = result_width - comp;
                int prnt = min(result_width, comp - abs(min(rest, 0)));

                //  Wyswietlenie fragmentu znaku na segmencie ekranu.
                this->buffer[0] = prnt;
                this->base->writeSprite(xpos, 0, this->buffer);

                //  Oblicznie pozycji nastepnego fragmentu znaku.
                xpos = xpos + comp;

                //  Skopiowanie następnego fragmentu znaku do pamieci podrecznej.
                if (rest > 0)
                    for (int cpos = 2 + comp; cpos < 10; cpos++)
                        this->buffer[cpos-comp] = this->buffer[cpos];

                //  Pobranie dostepnej dlugosci kolejnego fragmentu ekranu.
                result_width = max(0, rest);
            }

            return char_width;
        }

        //  Automatyczne wyswietlenie znaku.
        else
        {
            this->base->writeSprite(x, 0, this->buffer);
            return this->buffer[0];
        }

        return 0;
    }
}

//  ----------------------------------------------------------------------------
/* Wyswietlenie tekstu na ekranie.
 * @param font: Indeks tablicay zawierajacej czcionke w jakiej tekst ma zostac wyswietlony na ekranie.
 * @param x: Indeks kolumny ekranu od ktorej text ma zostac wyswietlony w prawo.
 * @param text: Tekst ktory ma zostac wyswietlony.
 * @param step_delay: Czas oczekiwania w milisekundach po wyswietleniu pojedynczego znaku.
 */
int DisplayController::PrintText(int font, int x, String text, int step_delay = 0)
{
    //  Wstepna konfiguracja zmiennych roboczych.
    int xpos = x;
    int result_width = 0;

    //  Wyswietlenie tekstu na ekranie jezeli zostal zainicjalizowany.
    if (this->initialized)
    {
        for (int c = 0; c < text.length(); c++)
        {
            //  Wyswietlenie pojedynczego znaku na ekranie.
            int char_width = this->PrintChar(font, xpos, text[c]);

            //  Ustawienie przerwy miedzy znakami.
            this->ClearColumn(xpos + char_width);

            //  Obliczenie pozycji nastepnego znaku i aktualnej dlugosci wyswietlonego tekstu.
            xpos = xpos + (char_width + 1);
            result_width = result_width + (char_width + 1);

            //  Opoznienie po wyswietleniu pojedynczego znaku na ekranie.
            if (step_delay > 0)
                delay(step_delay);
        }
    }

    return result_width;
}

//  ----------------------------------------------------------------------------
/* Obliczenie dlugosci tekstu, ile zajmie jego wyswietlenie na ekranie.
 * @param font: Indeks tablicay zawierajacej czcionke w jakiej tekst ma zostac wyswietlony na ekranie.
 * @param text: Tekst którego długosc ma zostac obliczona.
 * @return: Dlugosc tekstu, ile zajmie jego wyswietlenie na ekranie.
 */
int DisplayController::GetTextWidth(int font, String text)
{
    //  Wstepna konfiguracja zmiennych roboczych.
    int result_width = 0;

    for (int c = 0; c < text.length(); c++)
    {
        //  Zaladowanie znaku do pamieci podrecznej.
        this->LoadCharacter(font, text[c]);

        //  Pobranie dlugosci znaku z pamieci podrecznej.
        result_width = result_width + buffer[0];

        //  Dodanie odstepu miedzy znakami.
        if (c != (text.length() - 1))
        {
            result_width = result_width + 1;
        }
    }
    
    return result_width;
}

//  ----------------------------------------------------------------------------
/* Obciecie tekstu do wybranego wolnego miejsca na ekranie.
 * @param font: Indeks tablicay zawierajacej czcionke w jakiej tekst ma zostac wyswietlony na ekranie.
 * @param text: Tekst ktorego fragment ma zostac wyswietlony.
 * @param out width: Dlugosc fragmentu tekstu, ile zajmie jego wyswietlenie na ekranie.
 * @param first_char: Indeks charakteru ktory ma sie wyswietlac jako pierwszy.
 * @param left_offset: Ograniczenie dlugosci ekranu od lewej w kolumnach.
 * @param right_offset: Ograniczenie dlugosci ekeranu od prawej w kolumnach.
 * @return: Obciety tekst mieszczacy sie na ekranie.
 */
String DisplayController::ClampText(int font, String text, int *width, int first_char, int left_offset, int right_offset)
{
    //  Wstepna konfiguracja zmiennych roboczych.
    int sub_index = 0;
    int max_width = max(0, min(this->GetWidth(), this->GetWidth() - right_offset));
    int result_width = 0;

    //  Ustawienie wstepnej wartosci zmiennych wyjsciowych.
    *width = 0;

    //  Sprawdzenie czy ograniczenia ekranu nie nachodza na siebie.
    if (max(0, left_offset) >= max_width)
        return "";

    //  Obciecie tekstu do wybranego wolnego miejsca na ekranie.
    for (int c = min(first_char, text.length()); c < text.length(); c++)
    {
        //  Zaladowanie znaku do pamieci podrecznej.
        this->LoadCharacter(font, text[c]);

        //  Pobranie dlugosci znaku z pamieci podrecznej.
        int char_width = buffer[0];

        //  Zwiekszenie tekstu wynikowego o znak lub przerwanie jezeli na ekranie nie ma juz wolnego miejsca.
        if (max(0, left_offset) + result_width + char_width < max_width)
        {
            sub_index += 1;
            width = width + (char_width + 1);
        }
        else
            break;
    }

    //  Usuniecie ostatniej kolumny traktowanej jako przerwa po znaku i zwrocenie wyniku.
    if (result_width > 0)
    {
        *width = result_width - 1;
        return text.substring(first_char, first_char + sub_index);
    }

    return "";
}

//  ----------------------------------------------------------------------------
/*  Wyszysczenie tekstu z struktury DisplayString.
 *  @param ds: Struktura DisplayString z informacjami o wyswietlanym tekscie.
 */
void DisplayController::ClearDS(DisplayString *ds)
{
    //  Wyczyszczenie poprzedniego tekstu.
    this->ClearRange(ds->_xpos, ds->_xpos + ds->_width);

    //  Wyczyszczenie danych w strukturze DisplayString.
    ds->_xpos = 0;
    ds->_width = 0;
    ds->offset = 0;
    ds->step_delay = 0;
    ds->text = "";
}

//  ----------------------------------------------------------------------------
/* Wyswietlenie na ekranie tekstu przy pomocy struktury DisplayString.
 * @param ds: Struktura DisplayString z informacjami o wyswietlanym tekscie.
 * @param force_clear: Flaga zezwalajaca na wyczyszczenie poprzedniego wyswietlanego tekstu.
 */
void DisplayController::PrintDS(DisplayString *ds, bool force_clear = true)
{
    switch (ds->text_align)
    {
        //  Wyswietl na ekranie tekst wyrownany do lewej strony.
        case 0:
            this->PrintDSLeft(ds, force_clear);
            break;

        //  Wyswietl na ekranie tekst wyrownany do srodka.
        case 1:
            this->PrintDSCenter(ds, force_clear);
            break;

        //  Wyswietl na ekranie tekst wyrownany do prawej strony.
        case 2:
            this->PrintDSRight(ds, force_clear);
            break;

        //  Wyswietl na ekranie tekst wyrownany do lewej strony.
        default:
            this->PrintDSLeft(ds, force_clear);
            break;
    }
}

#endif