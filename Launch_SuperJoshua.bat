@echo off
echo ===================================
echo    SUPER JOSHUA - GAME LAUNCHER
echo ===================================
echo.

echo 🎮 Buscando Unity Hub...

REM Buscar Unity Hub en ubicaciones comunes
set UNITY_HUB_FOUND=0

if exist "%PROGRAMFILES%\Unity Hub\Unity Hub.exe" (
    echo ✅ Unity Hub encontrado en Program Files
    set UNITY_HUB_PATH="%PROGRAMFILES%\Unity Hub\Unity Hub.exe"
    set UNITY_HUB_FOUND=1
    goto :launch_unity
)

if exist "%PROGRAMFILES(X86)%\Unity Hub\Unity Hub.exe" (
    echo ✅ Unity Hub encontrado en Program Files ^(x86^)
    set UNITY_HUB_PATH="%PROGRAMFILES(X86)%\Unity Hub\Unity Hub.exe"
    set UNITY_HUB_FOUND=1
    goto :launch_unity
)

if exist "%LOCALAPPDATA%\Programs\Unity Hub\Unity Hub.exe" (
    echo ✅ Unity Hub encontrado en AppData Local
    set UNITY_HUB_PATH="%LOCALAPPDATA%\Programs\Unity Hub\Unity Hub.exe"
    set UNITY_HUB_FOUND=1
    goto :launch_unity
)

REM Buscar Unity directamente
echo 🔍 Buscando Unity Editor...
for /d %%i in ("%PROGRAMFILES%\Unity\Hub\Editor\*") do (
    if exist "%%i\Editor\Unity.exe" (
        echo ✅ Unity Editor encontrado: %%i
        set UNITY_EDITOR_PATH="%%i\Editor\Unity.exe"
        goto :launch_editor
    )
)

REM Si no se encuentra nada
echo ❌ Unity no encontrado automaticamente.
goto :install_instructions

:launch_unity
echo.
echo 🚀 Iniciando Unity Hub...
start "" %UNITY_HUB_PATH%
ping 127.0.0.1 -n 4 > nul
echo.
echo 📂 SIGUIENTE: En Unity Hub:
echo    1. Haz clic en "Add"
echo    2. Selecciona esta carpeta: %~dp0
echo    3. Abre el proyecto con Unity 2022.3 LTS+
echo.
echo ⚡ CONFIGURACION AUTOMATICA:
echo    Al abrir el proyecto, se configurará automáticamente.
echo    Solo acepta el dialogo y ¡estará listo para jugar!
echo.
goto :controls

:launch_editor
echo.
echo 🚀 Iniciando Unity Editor directamente...
%UNITY_EDITOR_PATH% -projectPath "%~dp0"
goto :end

:install_instructions
echo.
echo 📥 INSTRUCCIONES DE INSTALACION:
echo.
echo 1. Descarga Unity Hub desde:
echo    👉 https://unity.com/download
echo.
echo 2. Instala Unity 2022.3 LTS o superior
echo.
echo 3. Ejecuta este launcher de nuevo
echo    O abre Unity Hub manualmente y añade este proyecto:
echo    📁 %~dp0
echo.

:controls
echo 🎮 CONTROLES DEL JUEGO:
echo    • A/D o ←/→  : Mover Joshua
echo    • Espacio    : Saltar
echo    • Ctrl       : Spin Dash ^(solo como Sonic^)
echo    • Escape/P   : Pausar
echo.
echo 🌟 OBJETIVO:
echo    • Recoge la ESTRELLA para transformarte en Sonic
echo    • Recolecta monedas y anillos para puntos
echo    • Disfruta la velocidad extrema de Sonic
echo.
echo 🎯 CARACTERISTICAS:
echo    ✅ Transformacion temporal Joshua ↔ Sonic
echo    ✅ Fisica realista estilo Mario
echo    ✅ Sistema de puntuacion y vidas
echo    ✅ Spin Dash con carga progresiva
echo    ✅ UI informativa en tiempo real
echo.

:end
echo ¡Disfruta Super Joshua! 🎊
echo.
pause