#!/usr/bin/env bash
# claude.ai bulut ortamı kurulum betiği (ortam ayarları > Setup script).
# Her yeni bulut oturumu temiz bir makinede başlar; bu betik Flutter ve .NET'i
# hazır eder. Yerel geliştirmede kullanılmaz.
set -euo pipefail

# .NET 10 SDK — Ubuntu paketi. (dotnet-install.sh ajan proxy'sinde 403 verdi.)
if ! command -v dotnet >/dev/null 2>&1; then
  apt-get update
  DEBIAN_FRONTEND=noninteractive apt-get install -y dotnet-sdk-10.0
fi

# Flutter — ui-trials testlerinin koştuğu sürüme sabit (3.47.5, stable).
FLUTTER_DIR=/opt/flutter
if [ ! -x "$FLUTTER_DIR/bin/flutter" ]; then
  git clone --depth 1 -b 3.47.5 https://github.com/flutter/flutter.git "$FLUTTER_DIR"
fi
grep -q "$FLUTTER_DIR/bin" ~/.bashrc 2>/dev/null ||
  echo "export PATH=\"$FLUTTER_DIR/bin:\$PATH\"" >> ~/.bashrc
export PATH="$FLUTTER_DIR/bin:$PATH"
flutter config --no-analytics >/dev/null 2>&1 || true
flutter --version

# Ekran görüntüsü karşılaştırması (tool/compare_screenshots.py) için.
python3 -c "import PIL" 2>/dev/null || pip install --quiet pillow
