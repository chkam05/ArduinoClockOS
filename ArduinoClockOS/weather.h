////////////////////////////////////////////////////////////////////////////////
//  WEATHER
////////////////////////////////////////////////////////////////////////////////

#ifndef WEATHER_H
#define WEATHER_H

////////////////////////////////////////////////////////////////////////////////
//  *** INCLUDED LIBRARIES ***
////////////////////////////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class Weather
{
    private:
        byte  * wather  = new byte[25] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    public:
        Weather();
        
        int   GetWeather(Time date_time);
        void  SetWeather(String data, File * file, bool append = true);
        void  UpdateWeather(Time date_time, File * file);
};


////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

/*  Konstruktor klasy kontrolera informacji pogodowych.
 *  @param controller: Globalny kontroler.
 */
Weather::Weather()
{
    //
}

//  ----------------------------------------------------------------------------
int Weather::GetWeather(Time date_time)
{
    return 0;
}

//  ----------------------------------------------------------------------------
void Weather::SetWeather(String data, File * file, bool append = true)
{
    //
}

//  ----------------------------------------------------------------------------
void Weather::UpdateWeather(Time date_time, File * file)
{
    //
}


#endif