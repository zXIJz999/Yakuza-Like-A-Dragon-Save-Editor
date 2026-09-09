@echo off
echo Building Yakuza LAD Save Editor...
dotnet publish YakuzaSaveEditor.csproj -c Release
if %errorlevel% neq 0 (
    echo Build failed.
    pause
    exit /b 1
)
echo.
echo Build complete.
echo Output: bin\Release\net9.0-windows\win-x64\publish\YakuzaSaveEditor.exe
pause
