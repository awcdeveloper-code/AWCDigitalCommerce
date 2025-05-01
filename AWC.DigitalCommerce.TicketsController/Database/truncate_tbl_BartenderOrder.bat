echo off

set SERVERNAME=DESKTOP-T68QADR
set DATABASENAME=AWCDigitalCommerce

sqlcmd -S %SERVERNAME% -d %DATABASENAME% -U sa -P sa -Q "use AWCDigitalCommerce; truncate table tbl_BartenderOrder"
echo tbl_BartenderOrder fue normalizada exitosamente, por favor oprima ENTER para cerrar esta ventana
pause

