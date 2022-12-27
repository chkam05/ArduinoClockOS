////////////////////////////////////////////////////////////////////////////////
//  SD CARD MODULE HW-125
////////////////////////////////////////////////////////////////////////////////

#ifndef SD_CARD_CONTROLLER_H
#define SD_CARD_CONTROLLER_H

////////////////////////////////////////////////////////////////////////////////
//  *** INCLUDED LIBRARIES ***
////////////////////////////////////////////////////////////////////////////////

#include  <SPI.h>
#include  <SD.h>


////////////////////////////////////////////////////////////////////////////////
//  *** CONFIGURATION ***
////////////////////////////////////////////////////////////////////////////////

#define SDCARD_PIN_MISO   50
#define SDCARD_PIN_MOSI   51
#define SDCARD_PIN_SCK    52
#define SDCARD_PIN_CS     53


////////////////////////////////////////////////////////////////////////////////
//  *** CLASS DEFINITION ***
////////////////////////////////////////////////////////////////////////////////

class SdCardController
{
    private:
        Sd2Card   _device;
        SdVolume  _partition;

        bool  initialized = false;
        bool  mounted = false;
    
    public:
        SdCardController();

        void      Initialize();
        void      Mount();
        bool      IsInitialized();
        bool      IsMounted();

        String    GetCardType();
        uint32_t  GetPartitionBlocks();
        uint32_t  GetPartitionClusters();
        String    GetPartitionFormat();
        uint32_t  GetPartitionSize();
        float     GetPartitionSizeInGB();
        uint32_t  GetPartitionSizeInMB();
};


////////////////////////////////////////////////////////////////////////////////
//  *** PUBLIC METHOD BODIES ***
////////////////////////////////////////////////////////////////////////////////

//  Konstruktor kontrolera czytnika kart SD HW-125.
SdCardController::SdCardController()
{
    this->Initialize();
    this->Mount();
}

//  ----------------------------------------------------------------------------
//  Inicjalizacja kontrolera czytnika kart SD.
void SdCardController::Initialize()
{
    this->initialized = this->_device.init(SPI_HALF_SPEED, SDCARD_PIN_CS);

    if (this->initialized)
        this->mounted = this->_partition.init(this->_device);
}

//  ----------------------------------------------------------------------------
//  Montowanie karty SD.
void SdCardController::Mount()
{
    if (this->initialized)
        this->mounted = this->_partition.init(this->_device);
}

//  ----------------------------------------------------------------------------
/*  Sprawdzenie czy modul czytnika kart SD zostal zainicjalizowany.
 *  @return: True - modul czytnika kart SD zostal zainicjalizowany; False - w innym wypadku.
 */
bool SdCardController::IsInitialized()
{
    return this->initialized;
}

//  ----------------------------------------------------------------------------
/*  Sprawdzenie czy karta SD zostala zamontowana.
 *  @return: True - karta SD zostala zamontowana; False - w innym wypadku.
 */
bool SdCardController::IsMounted()
{
    return this->mounted;
}

//  ----------------------------------------------------------------------------
/* Pobranie typu zamontowanej karty SD.
 * @return: Typ zamontowanej karty SD.
 */
String SdCardController::GetCardType()
{
    if (this->initialized)
    {
        switch (this->_device.type())
        {
            case SD_CARD_TYPE_SD1:
                return "SD";
            case SD_CARD_TYPE_SD2:
                return "SDSC";
            case SD_CARD_TYPE_SDHC:
                return "SDHC";
            default:
                break;
        }
    }

    return "UNKNOWN";
}

//  ----------------------------------------------------------------------------
/* Pobranie informacji o ilosci blokow na partycji w systemie plikow.
 * @return: Ilosc blokow na partycji w systemie plikow.
 */
uint32_t SdCardController::GetPartitionBlocks()
{
    if (this->initialized && this->mounted)
        return this->_partition.blocksPerCluster() * this->_partition.clusterCount();
    return 0;
}

//  ----------------------------------------------------------------------------
/* Pobranie informacji o ilosci klastrow (jednostek alokacji plikow) w systemie plikow.
 * @return: Ilosc klastrow na partycji w systemie plikow.
 */
uint32_t SdCardController::GetPartitionClusters()
{
    if (this->initialized && this->mounted)
        return this->_partition.clusterCount();
    return 0;
}

//  ----------------------------------------------------------------------------
/* Pobranie informacji o type systemu plikow.
 * @return: Typ systemu plikow.
 */
String SdCardController::GetPartitionFormat()
{
    if (this->initialized && this->mounted)
        return "FAT" + String(this->_partition.fatType(), DEC);
    return "UNKNOWN";
}

//  ----------------------------------------------------------------------------
/* Pobranie informacji o rozmiarze partycji.
 * @return: Rozmiar partycji.
 */
uint32_t SdCardController::GetPartitionSize()
{
    if (this->initialized && this->mounted)
    {
        uint32_t parition_size;

        //  Pobranie ilosci blokow znajdujacych sie w klastrze.
        parition_size = this->_partition.blocksPerCluster();

        //  Pobranie ilosci klastrow.
        parition_size = parition_size * this->_partition.clusterCount();

        //  Bloki karty SD zazwyczaj posiadaja 512 bajtow (2 bloki to 1KB).
        parition_size = parition_size / 2;

        return parition_size;
    }

    return 0;
}

//  ----------------------------------------------------------------------------
/* Pobranie informacji o rozmiarze partycji w GB.
 * @return: Rozmiar partycji w GB.
 */
float SdCardController::GetPartitionSizeInGB()
{
    if (this->initialized && this->mounted)
        return this->GetPartitionSizeInMB() / 1024.0;
    return 0.0;
}

//  ----------------------------------------------------------------------------
/* Pobranie informacji o rozmiarze partycji w MB.
 * @return: Rozmiar partycji w GB.
 */
uint32_t SdCardController::GetPartitionSizeInMB()
{
    if (this->initialized && this->mounted)
        return this->GetPartitionSize() / 1024;
    return 0;
}

#endif