# Career Connect - Create Migration ZIP Files
# This script creates zip files for each migration phase
# Run this script in the project root directory

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Career Connect - Migration ZIP Creator" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Get the script directory (project root)
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
if (-not $scriptPath) {
    $scriptPath = Get-Location
}
Set-Location $scriptPath
$projectRoot = Get-Location | Select-Object -ExpandProperty Path

Write-Host "Project root: $projectRoot" -ForegroundColor Gray
Write-Host ""

# Create migration_zips folder if it doesn't exist
$zipFolder = Join-Path $projectRoot "migration_zips"
if (-not (Test-Path $zipFolder)) {
    New-Item -ItemType Directory -Path $zipFolder -Force | Out-Null
    Write-Host "Created migration_zips folder" -ForegroundColor Green
}

# Function to create zip file
function Create-Zip {
    param(
        [string]$ZipName,
        [string[]]$Files,
        [string]$Description
    )
    
    $zipPath = Join-Path $zipFolder "$ZipName.zip"
    
    # Remove zip if it exists
    if (Test-Path $zipPath) {
        Remove-Item $zipPath -Force
        Write-Host "  Removed existing: $ZipName.zip" -ForegroundColor Gray
    }
    
    Write-Host "Creating $ZipName.zip..." -ForegroundColor Yellow
    Write-Host "  Description: $Description" -ForegroundColor Gray
    
    # Create temporary folder for files
    $tempFolder = Join-Path $zipFolder "temp_$ZipName"
    if (Test-Path $tempFolder) {
        Remove-Item $tempFolder -Recurse -Force -ErrorAction SilentlyContinue
    }
    New-Item -ItemType Directory -Path $tempFolder -Force | Out-Null
    
    $fileCount = 0
    $missingFiles = @()
    $copiedFiles = @()
    
    # Copy files to temp folder
    foreach ($file in $Files) {
        # Use absolute path from project root
        $sourcePath = Join-Path $projectRoot $file
        
        Write-Host "    Checking: $file" -ForegroundColor DarkGray
        
        if (Test-Path $sourcePath) {
            # Get item to check if it's a file or directory
            $item = Get-Item $sourcePath
            $destPath = Join-Path $tempFolder $file
            
            # Create parent directory if needed
            $destDir = Split-Path $destPath -Parent
            if ($destDir -and -not (Test-Path $destDir)) {
                New-Item -ItemType Directory -Path $destDir -Force | Out-Null
            }
            
            # Copy file or directory
            try {
                if ($item.PSIsContainer) {
                    # It's a directory - copy recursively
                    Copy-Item $sourcePath -Destination $destPath -Recurse -Force -ErrorAction Stop
                    $itemCount = (Get-ChildItem $destPath -Recurse -File).Count
                    Write-Host "      [OK] Copied directory: $file ($itemCount files)" -ForegroundColor Green
                    $fileCount += $itemCount
                } else {
                    # It's a file
                    Copy-Item $sourcePath -Destination $destPath -Force -ErrorAction Stop
                    Write-Host "      [OK] Copied file: $file" -ForegroundColor Green
                    $fileCount++
                }
                $copiedFiles += $file
            } catch {
                Write-Host "      [ERROR] Failed to copy: $file - $_" -ForegroundColor Red
                $missingFiles += $file
            }
        } else {
            Write-Host "      [NOT FOUND] Not found: $file" -ForegroundColor Yellow
            $missingFiles += $file
        }
    }
    
    # Create README for this zip
    $readmePath = Join-Path $tempFolder "README.txt"
    $readmeContent = @"
Career Connect Migration - $ZipName
================================

Description: $Description

Files included in this ZIP:
$($Files -join "`n")

Instructions:
1. Extract this ZIP to your project folder on the new PC
2. Follow the instructions in MIGRATION_GUIDE.md
3. Test that everything works before extracting the next ZIP
4. Only proceed to the next phase if this phase works correctly

Copied Files:
$($copiedFiles -join "`n")

Missing Files (if any):
$($missingFiles -join "`n")

Next Step: Extract the next ZIP file in the sequence.
"@
    Set-Content -Path $readmePath -Value $readmeContent
    
    # Verify files exist in temp folder before zipping
    $allItems = Get-ChildItem -Path $tempFolder -Recurse
    $fileItems = Get-ChildItem -Path $tempFolder -Recurse -File
    
    Write-Host "  Temp folder contains: $($allItems.Count) items, $($fileItems.Count) files" -ForegroundColor Gray
    
    # Only create zip if files were copied
    if ($fileItems.Count -gt 0) {
        try {
            # Create zip using .NET directly for better control
            Add-Type -AssemblyName System.IO.Compression.FileSystem -ErrorAction Stop
            
            # Remove existing zip if it exists
            if (Test-Path $zipPath) {
                Remove-Item $zipPath -Force
            }
            
            # Create new zip file
            $zip = [System.IO.Compression.ZipFile]::Open($zipPath, [System.IO.Compression.ZipArchiveMode]::Create)
            
            # Get all files from temp folder
            $filesToZip = Get-ChildItem -Path $tempFolder -Recurse -File
            
            foreach ($file in $filesToZip) {
                # Calculate relative path from temp folder
                $relativePath = $file.FullName.Substring($tempFolder.Length + 1).Replace('\', '/')
                
                # Add file to zip
                $null = [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $file.FullName, $relativePath)
            }
            
            $zip.Dispose()
            
            Write-Host "  [OK] Created: $ZipName.zip ($($fileItems.Count) files)" -ForegroundColor Green
            
            # Verify the ZIP was created and has content
            if (Test-Path $zipPath) {
                $zipItem = Get-Item $zipPath
                Write-Host "  ZIP size: $([math]::Round($zipItem.Length / 1KB, 2)) KB" -ForegroundColor Gray
                
                # Verify ZIP has entries
                $verifyZip = [System.IO.Compression.ZipFile]::OpenRead($zipPath)
                $entryCount = $verifyZip.Entries.Count
                $verifyZip.Dispose()
                Write-Host "  ZIP entries: $entryCount" -ForegroundColor Gray
            }
            
        } catch {
            Write-Host "  [ERROR] Failed to create ZIP: $_" -ForegroundColor Red
            Write-Host "  Error details: $($_.Exception.Message)" -ForegroundColor Red
            if (Test-Path $zipPath) {
                Remove-Item $zipPath -Force -ErrorAction SilentlyContinue
            }
        }
    } else {
        Write-Host "  [ERROR] No files found to zip for: $ZipName.zip" -ForegroundColor Red
        Write-Host "    Skipping ZIP creation" -ForegroundColor Yellow
    }
    
    if ($missingFiles.Count -gt 0) {
        Write-Host "  [WARNING] Missing files ($($missingFiles.Count)): $($missingFiles -join ', ')" -ForegroundColor Yellow
    }
    
    # Clean up temp folder
    if (Test-Path $tempFolder) {
        Remove-Item $tempFolder -Recurse -Force -ErrorAction SilentlyContinue
    }
    
    Write-Host ""
}

# Verify we're in the right directory
Write-Host "Verifying project files..." -ForegroundColor Cyan
$testFiles = @("Web.config", "CareerConnect.csproj", "database_setup.sql")
$allFound = $true
foreach ($testFile in $testFiles) {
    $testPath = Join-Path $projectRoot $testFile
    if (Test-Path $testPath) {
        Write-Host "  [OK] Found: $testFile" -ForegroundColor Green
    } else {
        Write-Host "  [NOT FOUND] Missing: $testFile" -ForegroundColor Red
        $allFound = $false
    }
}
if (-not $allFound) {
    Write-Host ""
    Write-Host "WARNING: Some required files are missing!" -ForegroundColor Yellow
    Write-Host "Make sure you're running this script from the project root directory." -ForegroundColor Yellow
    Write-Host ""
}
Write-Host ""

Write-Host "Phase 1: Foundation Files..." -ForegroundColor Cyan
$phase1Files = @(
    "Web.config",
    "Web.Debug.config",
    "Web.Release.config",
    "packages.config",
    "CareerConnect.csproj",
    "CareerConnect.csproj.user",
    "Global.asax",
    "Global.asax.cs",
    "Properties\AssemblyInfo.cs",
    "database_setup.sql"
)
Create-Zip -ZipName "01_Foundation" -Files $phase1Files -Description "Foundation files - Configuration, project files, and database setup"

Write-Host "Phase 2: Assets..." -ForegroundColor Cyan
$phase2Files = @(
    "assets"
)
Create-Zip -ZipName "02_Assets" -Files $phase2Files -Description "Static assets - CSS, JavaScript, images, and fonts"

Write-Host "Phase 3: Core Infrastructure..." -ForegroundColor Cyan
$phase3Files = @(
    "USER\usermaster.Master",
    "USER\usermaster.Master.cs",
    "USER\usermaster.Master.designer.cs",
    "Models"
)
Create-Zip -ZipName "03_Core" -Files $phase3Files -Description "Core infrastructure - Master page and models"

Write-Host "Phase 4: Root Page..." -ForegroundColor Cyan
$phase4Files = @(
    "default.aspx",
    "default.aspx.cs",
    "default.aspx.designer.cs"
)
Create-Zip -ZipName "04_RootPage" -Files $phase4Files -Description "Root default page - Homepage"

Write-Host "Phase 5: User Pages - Part 1 (Basic Pages)..." -ForegroundColor Cyan
$phase5aFiles = @(
    "USER\about.aspx",
    "USER\about.aspx.cs",
    "USER\about.aspx.designer.cs",
    "USER\Contact.aspx",
    "USER\Contact.aspx.cs",
    "USER\Contact.aspx.designer.cs"
)
Create-Zip -ZipName "05_UserPages_Part1_Basic" -Files $phase5aFiles -Description "User pages - Part 1: About and Contact pages"

Write-Host "Phase 5: User Pages - Part 2 (Authentication)..." -ForegroundColor Cyan
$phase5bFiles = @(
    "USER\Register.aspx",
    "USER\Register.aspx.cs",
    "USER\Register.aspx.designer.cs",
    "USER\Login.aspx",
    "USER\Login.aspx.cs",
    "USER\Login.aspx.designer.cs"
)
Create-Zip -ZipName "05_UserPages_Part2_Auth" -Files $phase5bFiles -Description "User pages - Part 2: Registration and Login pages"

Write-Host "Phase 5: User Pages - Part 3 (Job Pages)..." -ForegroundColor Cyan
$phase5cFiles = @(
    "USER\default.aspx",
    "USER\default.aspx.cs",
    "USER\default.aspx.designer.cs",
    "USER\job listing.aspx",
    "USER\job listing.aspx.cs",
    "USER\job listing.aspx.designer.cs",
    "USER\job details.aspx",
    "USER\job details.aspx.cs",
    "USER\job details.aspx.designer.cs"
)
Create-Zip -ZipName "05_UserPages_Part3_Jobs" -Files $phase5cFiles -Description "User pages - Part 3: Job listing and details pages"

Write-Host "Phase 5: User Pages - Part 4 (Profile)..." -ForegroundColor Cyan
$phase5dFiles = @(
    "USER\Profile.aspx",
    "USER\Profile.aspx.cs",
    "USER\Profile.aspx.designer.cs"
)
Create-Zip -ZipName "05_UserPages_Part4_Profile" -Files $phase5dFiles -Description "User pages - Part 4: User profile page"

Write-Host "Phase 6: Admin Dashboard - Part 1 (Config)..." -ForegroundColor Cyan
$phase6aFiles = @(
    "Admin\includes"
)
Create-Zip -ZipName "06_Admin_Part1_Config" -Files $phase6aFiles -Description "Admin dashboard - Part 1: Configuration files"

Write-Host "Phase 6: Admin Dashboard - Part 2 (Pages)..." -ForegroundColor Cyan
$phase6bFiles = @(
    "Admin\index.php",
    "Admin\dashboard.php",
    "Admin\users.php",
    "Admin\jobs.php",
    "Admin\companies.php",
    "Admin\categories.php",
    "Admin\applications.php",
    "Admin\resumes.php",
    "Admin\logout.php",
    "Admin\add_job.php",
    "Admin\edit_job.php"
)
Create-Zip -ZipName "06_Admin_Part2_Pages" -Files $phase6bFiles -Description "Admin dashboard - Part 2: Admin pages"

Write-Host "Phase 6: Admin Dashboard - Part 3 (Uploads)..." -ForegroundColor Cyan
$phase6cFiles = @(
    "Admin\uploads"
)
Create-Zip -ZipName "06_Admin_Part3_Uploads" -Files $phase6cFiles -Description "Admin dashboard - Part 3: Uploads folder (if contains files)"

Write-Host "Phase 7: Migration Documentation..." -ForegroundColor Cyan
$phase7Files = @(
    "MIGRATION_GUIDE.md",
    "MIGRATION_CHECKLIST.md",
    "FILE_TRANSFER_LIST.md",
    "QUICK_MIGRATION_GUIDE.md",
    "MIGRATION_README.md",
    "verify_migration.bat",
    "verify_migration.ps1",
    "create_migration_zips.ps1"
)
Create-Zip -ZipName "07_Documentation" -Files $phase7Files -Description "Migration documentation and verification scripts"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Migration ZIP Files Created!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "ZIP files created in: $zipFolder\" -ForegroundColor Yellow
Write-Host ""
Write-Host "Transfer Order:" -ForegroundColor Cyan
Write-Host "  1. 01_Foundation.zip" -ForegroundColor White
Write-Host "  2. 02_Assets.zip" -ForegroundColor White
Write-Host "  3. 03_Core.zip" -ForegroundColor White
Write-Host "  4. 04_RootPage.zip" -ForegroundColor White
Write-Host "  5. 05_UserPages_Part1_Basic.zip" -ForegroundColor White
Write-Host "  6. 05_UserPages_Part2_Auth.zip" -ForegroundColor White
Write-Host "  7. 05_UserPages_Part3_Jobs.zip" -ForegroundColor White
Write-Host "  8. 05_UserPages_Part4_Profile.zip" -ForegroundColor White
Write-Host "  9. 06_Admin_Part1_Config.zip" -ForegroundColor White
Write-Host " 10. 06_Admin_Part2_Pages.zip" -ForegroundColor White
Write-Host " 11. 06_Admin_Part3_Uploads.zip (optional)" -ForegroundColor White
Write-Host " 12. 07_Documentation.zip" -ForegroundColor White
Write-Host ""
Write-Host "Instructions:" -ForegroundColor Cyan
Write-Host "  1. Transfer ZIP files to new PC in order" -ForegroundColor White
Write-Host "  2. Extract each ZIP to project folder" -ForegroundColor White
Write-Host "  3. Test after each ZIP extraction" -ForegroundColor White
Write-Host "  4. Only proceed to next ZIP if current one works" -ForegroundColor White
Write-Host ""
Write-Host "See MIGRATION_GUIDE.md for detailed instructions." -ForegroundColor Yellow
Write-Host ""

