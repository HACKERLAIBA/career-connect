@echo off
REM Career Connect - Create Migration ZIP Files (Batch Version)
REM This script creates zip files for each migration phase
REM Run this script in the project root directory

echo ========================================
echo Career Connect - Migration ZIP Creator
echo ========================================
echo.

REM Check if PowerShell is available
where powershell >nul 2>nul
if %errorlevel% neq 0 (
    echo ERROR: PowerShell is not available!
    echo Please use create_migration_zips.ps1 instead
    echo Or install PowerShell
    pause
    exit /b 1
)

echo Running PowerShell script...
echo.

powershell -ExecutionPolicy Bypass -File "create_migration_zips.ps1"

if %errorlevel% equ 0 (
    echo.
    echo ========================================
    echo ZIP files created successfully!
    echo ========================================
) else (
    echo.
    echo ========================================
    echo ERROR: Failed to create ZIP files!
    echo ========================================
)

echo.
pause



