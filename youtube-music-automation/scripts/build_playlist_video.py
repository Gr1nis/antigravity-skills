#!/usr/bin/env python3
"""
YouTube Music Playlist Video Builder
Автоматическая сборка часовых музыкальных видео для YouTube:
- Чтение папки с треками (MP3/WAV/FLAC)
- Вычисление длительности и генерация timestamps.txt (главы для YouTube)
- Выравнивание громкости (EBU R128 / -14 LUFS)
- Наложение статичной обложки (1920x1080) или видео-лупа
- Рендер готового MP4 через FFmpeg
"""

import argparse
import json
import os
import re
import shutil
import subprocess
import sys
from pathlib import Path


def find_ffmpeg_tool(name="ffmpeg"):
    """Ищет ffmpeg или ffprobe в PATH или стандартных путях Windows."""
    found = shutil.which(name)
    if found:
        return found

    # Типичные пути установки на Windows
    common_paths = [
        Path(os.environ.get("LOCALAPPDATA", "")) / "Microsoft" / "WinGet" / "Packages",
        Path("C:/ffmpeg/bin"),
        Path(os.environ.get("ProgramFiles", "")) / "FFmpeg" / "bin",
    ]

    for base in common_paths:
        if base.exists():
            for match in base.glob(f"**/{name}.exe"):
                if match.is_file():
                    return str(match)

    return None


def natural_sort_key(s):
    """Сортировка файлов с числами в естественном порядке (1, 2, 10 вместо 1, 10, 2)."""
    return [int(text) if text.isdigit() else text.lower() for text in re.split(r"(\d+)", str(s))]


def format_timestamp(seconds: float) -> str:
    """Преобразует секунды в формат 00:00 или 00:00:00 для YouTube глав."""
    total_sec = int(seconds)
    hours = total_sec // 3600
    minutes = (total_sec % 3600) // 60
    sec = total_sec % 60

    if hours > 0:
        return f"{hours:02d}:{minutes:02d}:{sec:02d}"
    return f"{minutes:02d}:{sec:02d}"


def get_audio_duration(ffprobe_path: str, file_path: Path) -> float:
    """Получает точную длительность аудиофайла в секундах через ffprobe."""
    cmd = [
        ffprobe_path,
        "-v", "error",
        "-show_entries", "format=duration",
        "-of", "json",
        str(file_path),
    ]
    res = subprocess.run(cmd, capture_output=True, text=True, check=True)
    data = json.loads(res.stdout)
    return float(data["format"]["duration"])


def clean_track_title(filename: str) -> str:
    """Преобразует имя файла в красивый заголовок трека."""
    name = Path(filename).stem
    # Удаляем ведущие номера вроде '01. ', '02_ ', '1 - '
    name = re.sub(r"^[\d\s._-]+", "", name)
    # Заменяем подчеркивания и дефисы на пробелы
    name = name.replace("_", " ").replace("-", " ")
    # Капитализируем слова
    return " ".join(word.capitalize() for word in name.split())


def build_playlist(args):
    ffmpeg = find_ffmpeg_tool("ffmpeg")
    ffprobe = find_ffmpeg_tool("ffprobe")

    if not ffmpeg or not ffprobe:
        print("[ОШИБКА] FFmpeg или FFprobe не найдены в системе.")
        print("Пожалуйста, выполните команду установки в PowerShell:")
        print("    winget install Gyan.FFmpeg")
        sys.exit(1)

    input_dir = Path(args.input_dir).resolve()
    if not input_dir.is_dir():
        print(f"[ОШИБКА] Папка с треками не найдена: {input_dir}")
        sys.exit(1)

    # Поиск поддерживаемых аудио файлов
    supported_exts = {".mp3", ".wav", ".flac", ".m4a", ".ogg"}
    track_files = sorted(
        [f for f in input_dir.iterdir() if f.is_file() and f.suffix.lower() in supported_exts],
        key=natural_sort_key,
    )

    if not track_files:
        print(f"[ОШИБКА] В папке '{input_dir}' не найдено аудиофайлов ({supported_exts})")
        sys.exit(1)

    print(f"[1/4] Найдено треков: {len(track_files)}")

    # Анализ треков и составление таймкодов
    tracks_info = []
    current_time = 0.0

    for idx, f in enumerate(track_files, 1):
        duration = get_audio_duration(ffprobe, f)
        title = clean_track_title(f.name) or f"Track {idx}"
        timestamp_str = format_timestamp(current_time)

        tracks_info.append({
            "path": f,
            "title": title,
            "start_time": current_time,
            "timestamp": timestamp_str,
            "duration": duration,
        })
        print(f"  {timestamp_str} - {title} ({format_timestamp(duration)})")
        current_time += duration

    total_duration_str = format_timestamp(current_time)
    print(f"\nОбщая длительность плейлиста: {total_duration_str}")

    # Запись timestamps.txt для YouTube
    output_dir = Path(args.output).parent.resolve()
    output_dir.mkdir(parents=True, exist_ok=True)
    timestamps_path = output_dir / "timestamps.txt"

    with open(timestamps_path, "w", encoding="utf-8") as tf:
        tf.write(f"🎧 Tracklist ({total_duration_str}):\n\n")
        for item in tracks_info:
            tf.write(f"{item['timestamp']} {item['title']}\n")

    print(f"[2/4] Файл таймкодов сохранен: {timestamps_path}")

    if args.dry_run:
        print("\n[DRY RUN] Проверка завершена. Видео не рендерилось.")
        return

    # Создание списка конкатенации для FFmpeg
    concat_list_path = output_dir / "concat_audio.txt"
    with open(concat_list_path, "w", encoding="utf-8") as cf:
        for item in tracks_info:
            safe_path = str(item["path"]).replace("'", "'\\''")
            cf.write(f"file '{safe_path}'\n")

    # Проверка фонового изображения или видео
    cover_path = Path(args.cover).resolve() if args.cover else None
    if not cover_path or not cover_path.exists():
        print(f"[ОШИБКА] Файл фона/обложки не найден: {args.cover}")
        sys.exit(1)

    output_path = Path(args.output).resolve()
    print(f"[3/4] Сборка аудио и наложение видеофона: {cover_path.name}")
    print(f"[4/4] Рендеринг в файл: {output_path} (это может занять несколько минут)...")

    # Команда FFmpeg
    # -loop 1 для статичной картинки
    # loudnorm нормализует громкость под -14 LUFS (стандарт YouTube)
    # yuv420p обеспечивает совместимость со всеми браузерами и плеерами
    is_video_cover = cover_path.suffix.lower() in {".mp4", ".mov", ".mkv", ".webm"}

    cmd = [ffmpeg, "-y"]

    if is_video_cover:
        cmd.extend(["-stream_loop", "-1", "-i", str(cover_path)])
    else:
        cmd.extend(["-loop", "1", "-i", str(cover_path)])

    cmd.extend([
        "-f", "concat",
        "-safe", "0",
        "-i", str(concat_list_path),
        "-vf", "scale=1920:1080:force_original_aspect_ratio=decrease,pad=1920:1080:(ow-iw)/2:(oh-ih)/2,format=yuv420p",
        "-af", "loudnorm=I=-14:TP=-1.5:LRA=11",
        "-c:v", "libx264",
        "-tune", "stillimage" if not is_video_cover else "film",
        "-preset", "veryfast",
        "-crf", "18",
        "-c:a", "aac",
        "-b:a", "320k",
        "-shortest",
        str(output_path),
    ])

    try:
        subprocess.run(cmd, check=True)
        print(f"\n✅ [ГОТОВО] Видео успешно создано: {output_path}")
        print(f"Не забудьте скопировать главы из: {timestamps_path}")
    finally:
        if concat_list_path.exists():
            concat_list_path.unlink()


def main():
    parser = argparse.ArgumentParser(description="Автоматическая сборка YouTube музыкальных видео")
    parser.add_argument("--input-dir", "-i", required=True, help="Папка с аудиофайлами треков (MP3/WAV)")
    parser.add_argument("--cover", "-c", required=True, help="Путь к картинке (1920x1080) или фоновому видео-лупу")
    parser.add_argument("--output", "-o", default="playlist_video.mp4", help="Имя выходного MP4 файла")
    parser.add_argument("--dry-run", action="store_true", help="Только сгенерировать таймкоды без рендеринга видео")

    args = parser.parse_args()
    build_playlist(args)


if __name__ == "__main__":
    main()
