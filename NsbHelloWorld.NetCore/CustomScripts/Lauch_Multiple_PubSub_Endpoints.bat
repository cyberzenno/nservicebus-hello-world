@echo off
cd..

CALL :StartPubSub t1 mario
CALL :StartPubSub t1 luigi

REM CALL :StartPubSub t2 toad
REM CALL :StartPubSub t2 princes


REM function
:StartPubSub 

cd "SimplePublisher\bin\Debug\net5.0\"
start SimplePublisher.exe %~1 %~2"

cd..
cd..
cd..
cd..

cd "SimpleSubscriber\bin\Debug\net5.0\"
start SimpleSubscriber.exe %~1 %~2"

cd..
cd..
cd..
cd..

EXIT /B 0
REM end function

pause