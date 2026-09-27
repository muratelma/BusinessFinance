"""Tasarım ekran görüntüsü ile Flutter ekran görüntüsünü yan yana koyar.

Kullanım:
    python3 tool/compare_screenshots.py <tasarim.png> <flutter.png> <cikti.png>

Tasarım görüntüleri `design/claude-design-handoff/screenshots/` altındadır
(2×, 828 px genişlik). Flutter görüntüleri ekran görüntüsü testleriyle üretilir:
    SCREENSHOT_DIR=/tmp/shots flutter test test/screenshots
Gereksinim: Pillow (`pip install pillow`).
"""

import sys

from PIL import Image, ImageDraw

WIDTH = 828


def main() -> None:
    design_path, flutter_path, out_path = sys.argv[1:4]
    design = Image.open(design_path).convert("RGB")
    flutter = Image.open(flutter_path).convert("RGB")
    if design.width != WIDTH:
        design = design.resize((WIDTH, int(design.height * WIDTH / design.width)))
    height = max(design.height, flutter.height)
    canvas = Image.new("RGB", (WIDTH * 2 + 24, height + 40), "white")
    canvas.paste(design, (0, 40))
    canvas.paste(flutter, (WIDTH + 24, 40))
    draw = ImageDraw.Draw(canvas)
    draw.text((10, 10), "TASARIM", fill="black")
    draw.text((WIDTH + 34, 10), "FLUTTER", fill="black")
    canvas.resize((canvas.width // 2, canvas.height // 2)).save(out_path)


if __name__ == "__main__":
    main()
