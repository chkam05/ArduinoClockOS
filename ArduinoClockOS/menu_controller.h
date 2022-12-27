////////////////////////////////////////////////////////////////////////////////
//  MENU CONTROLLER
////////////////////////////////////////////////////////////////////////////////

#ifndef MENU_CONTROLLER_H
#define MENU_CONTROLLER_H

////////////////////////////////////////////////////////////////////////////////
//  *** INCLUDED LIBRARIES ***
////////////////////////////////////////////////////////////////////////////////

#include "global_controller.h"


////////////////////////////////////////////////////////////////////////////////
//  *** CONFIGURATION ***
////////////////////////////////////////////////////////////////////////////////

#define MENU_NOTHING              -1
#define MENU_EXIT                 0

#define LEVEL_MENU                100
#define LEVEL_MENU_ITEMS          2
#define MENU_ITEM_SETTINGS        101
#define MENU_ITEM_EXIT            100

#define LEVEL_SETTINGS            200
#define LEVEL_SETTINGS_ITEMS      6
#define SETTINGS_ITEM_TIME        201
#define SETTINGS_ITEM_DATE        202
#define SETTINGS_ITEM_BRIGHTNESS  203
#define SETTINGS_ITEM_BEEP        204
#define SETTINGS_ITEM_ALARM       205
#define SETTINGS_ITEM_EXIT        200


////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class MenuController
{
    private:
        GlobalController * controller;

        int menu_level      =   LEVEL_MENU;
        int menu_selection  =   MENU_ITEM_SETTINGS;

        void  DisplayMenu(bool clear = false);

        int   GetMaxItem();
        int   GetMinItem();

        int   NavigateForward();
        int   NavigateBack();
        int   NavigatePrevious();
        int   NavigateNext();
    
    public:
        MenuController(GlobalController * controller);

        void  OpenMenu(int level, int selection);
        int   ProcessInput();
};


////////////////////////////////////////////////////////////////////////////////
//  *** PRIVATE METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Wyswietlenie elementu menu na ekranie badz jego wyczyszczenie.
 *  @param clear: True - wyczyszczenie ekranu; False - w innym wypadku.
 */
void MenuController::DisplayMenu(bool clear = false)
{
    DisplayController * _ds = this->controller->display_ctrl;
    int _text_offset = 10;

    _ds->Clear();

    if (!clear)
    {
        switch (this->menu_selection)
        {
            case MENU_ITEM_SETTINGS:
                _ds->DrawSprite(SPRITE_SETTINGS, 0, 1);
                _ds->PrintText(0, _text_offset, "Settings");
                break;
            
            case MENU_ITEM_EXIT:
                _ds->DrawSprite(SPRITE_EXIT, 0, 0);
                _ds->PrintText(0, _text_offset, "Exit");
                break;
            
            case SETTINGS_ITEM_TIME:
                _ds->DrawSprite(SPRITE_CLOCK, 0, 0);
                _ds->PrintText(0, _text_offset, "Time");
                break;
            
            case SETTINGS_ITEM_DATE:
                _ds->DrawSprite(SPRITE_CALENDAR, 0, 0);
                _ds->PrintText(0, _text_offset, "Date");
                break;
            
            case SETTINGS_ITEM_BRIGHTNESS:
                _ds->DrawSprite(SPRITE_BRIGHTNESS, 0, 0);
                _ds->PrintText(0, _text_offset, "Brightness");
                break;
            
            case SETTINGS_ITEM_BEEP:
                _ds->DrawSprite(SPRITE_MUSIC, 0, 0);
                _ds->PrintText(0, _text_offset, "Beep");
                break;
            
            case SETTINGS_ITEM_ALARM:
                _ds->DrawSprite(SPRITE_ALARM, 0, 0);
                _ds->PrintText(0, _text_offset, "Alarm");
                break;
            
            case SETTINGS_ITEM_EXIT:
                _ds->DrawSprite(SPRITE_EXIT, 0, 0);
                _ds->PrintText(0, _text_offset, "Back");
                break;
        }
    }
}

//  ----------------------------------------------------------------------------
/*  Pobranie ostatniego elementu aktualnej listy menu.
 *  @return: Indeks ostatniego elementu aktualnej listy menu.
 */
int MenuController::GetMaxItem()
{
    switch (this->menu_level)
    {
        case LEVEL_MENU:
            return LEVEL_MENU + LEVEL_MENU_ITEMS - 1;
        case LEVEL_SETTINGS:
            return LEVEL_SETTINGS + LEVEL_SETTINGS_ITEMS - 1;
    }
}

//  ----------------------------------------------------------------------------
/*  Pobranie pierwszego elementu aktualnej listy menu.
 *  @return: Indeks pierwszego elementu aktualnej listy menu.
 */
int MenuController::GetMinItem()
{
    return this->menu_level;
}

//  ----------------------------------------------------------------------------
/*  Powrot do nastepnego podmenu lub wybranie opcji z menu.
 *  @return: Indeks wybranego menu, opcji, badz 0 - powrot.
 */
int MenuController::NavigateForward()
{
    switch (this->menu_selection)
    {
        case MENU_ITEM_SETTINGS:
            this->menu_level = LEVEL_SETTINGS;
            this->menu_selection = SETTINGS_ITEM_TIME;
            this->DisplayMenu();
            return MENU_ITEM_SETTINGS;
        
        case MENU_ITEM_EXIT:
            this->DisplayMenu(true);
            return MENU_EXIT;
        
        case SETTINGS_ITEM_TIME:
        case SETTINGS_ITEM_DATE:
        case SETTINGS_ITEM_BRIGHTNESS:
        case SETTINGS_ITEM_BEEP:
        case SETTINGS_ITEM_ALARM:
            this->DisplayMenu(true);
            return this->menu_selection;
        
        case SETTINGS_ITEM_EXIT:
            this->menu_level = LEVEL_MENU;
            this->menu_selection = MENU_ITEM_SETTINGS;
            this->DisplayMenu();
            return MENU_NOTHING;
    }

    return MENU_NOTHING;
}

//  ----------------------------------------------------------------------------
/*  Powrot do poprzedniego menu.
 *  @return: Indeks wybranego menu, opcji, badz 0 - exit.
 */
int MenuController::NavigateBack()
{
    switch (this->menu_level)
    {
        case LEVEL_MENU:
            this->DisplayMenu(true);
            return MENU_EXIT;
        
        case LEVEL_SETTINGS:
            this->menu_level = LEVEL_MENU;
            this->menu_selection = MENU_ITEM_SETTINGS;
            this->DisplayMenu();
            return MENU_NOTHING;
    }

    return MENU_NOTHING;
}

//  ----------------------------------------------------------------------------
//  Przejscie do poprzedniego elementu na liscie menu.
int MenuController::NavigatePrevious()
{
    int maxItem = this->GetMaxItem();
    int minItem = this->GetMinItem();

    if (this->menu_selection - 1 < minItem)
        this->menu_selection = maxItem;
    else
        this->menu_selection = this->menu_selection - 1;
    
    this->DisplayMenu();
    return MENU_NOTHING;
}

//  ----------------------------------------------------------------------------
//  Przejscie do nastepnego elementu na liscie menu.
int MenuController::NavigateNext()
{
    int maxItem = this->GetMaxItem();
    int minItem = this->GetMinItem();

    if (this->menu_selection + 1 > maxItem)
        this->menu_selection = minItem;
    else
        this->menu_selection = this->menu_selection + 1;

    this->DisplayMenu();
    return MENU_NOTHING;
}

////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Konstruktor klasy kontrolera menu.
 *  @param controller: Globalny kontroler.
 */
MenuController::MenuController(GlobalController * controller)
{
    this->controller = controller;
}

//  ----------------------------------------------------------------------------
void MenuController::OpenMenu(int level = LEVEL_MENU, int selection = MENU_ITEM_SETTINGS)
{
    this->menu_level = level;
    this->menu_selection = selection;
    this->DisplayMenu();
}

//  ----------------------------------------------------------------------------
int MenuController::ProcessInput()
{
    char key = this->controller->GetInputKey();

    switch (key)
    {
        case KEYPAD_SELECT_KEY:
            return NavigateForward();
        
        case KEYPAD_BACK_KEY:
            return NavigateBack();
        
        case KEYPAD_MENU_KEY:
            this->DisplayMenu(true);
            return MENU_EXIT;
        
        case KEYPAD_NEXT_KEY:
            return this->NavigateNext();
        
        case KEYPAD_PREV_KEY:
            return this->NavigatePrevious();
        
        default:
            return MENU_NOTHING;
    }
}

#endif