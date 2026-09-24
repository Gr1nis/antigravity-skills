<#
.SYNOPSIS
    Автоматическая установка и проверка FFmpeg для сборки видео.
.DESCRIPTION
    Скрипт проверяет наличие ffmpeg в PATH. Если он не найден, устанавливает его
    через Windows Package Manager (winget) или дает прямую ссылку.
#>

Write-Host "=== Проверка окружения FFmpeg ===" -ForegroundColor Cyan

$ffmpegInstalled = $false
try {
    $null = Get-Command ffmpeg -ErrorAction Stop
    $versionOutput = (ffmpeg -version | Select-Object -First 1)
    Write-Host "[OK] FFmpeg уже установлен в системе: $versionOutput" -ForegroundColor Green
    $ffmpegInstalled = $true
} catch {
    Write-Host "[INFO] FFmpeg не найден в системном PATH." -ForegroundColor Yellow
}

if (-not $ffmpegInstalled) {
    Write-Host "`nПопытка автоматической установки через winget (Gyan.FFmpeg)..." -ForegroundColor Cyan
    try {
        $null = Get-Command winget -ErrorAction Stop
        winget install --id Gyan.FFmpeg -e --accept-package-agreements --accept-source-agreements
        Write-Host "`n[УСПЕХ] Установка завершена! Перезапустите терминал для обновления системного PATH." -ForegroundColor Green
    } catch {
        Write-Host "`n[ВНИМАНИЕ] winget недоступен. Вы можете установить FFmpeg вручную:" -ForegroundColor Red
        Write-Host "1. Скачайте архив: https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip" -ForegroundColor White
        Write-Host "2. Распакуйте папку 'bin' (где лежат ffmpeg.exe и ffprobe.exe) например в C:\ffmpeg\bin" -ForegroundColor White
        Write-Host "3. Добавьте этот путь в переменную среды PATH." -ForegroundColor White
    }
}
