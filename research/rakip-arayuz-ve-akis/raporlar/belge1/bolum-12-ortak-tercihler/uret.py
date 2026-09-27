# -*- coding: utf-8 -*-
"""Bolumu uretir: PDF, Markdown kopyasi, denetim dosyalari, onizleme.

Yerlesim ortak/kalip.py'de, Belge 1 kurallari ortak/b1.py'dedir; bu klasorun tek icerik kaynagi icerik.py'dir.
"""
import pathlib
import sys

sys.path.insert(0, str(pathlib.Path(__file__).resolve().parents[1] / "ortak"))
import b1  # noqa: E402

if __name__ == "__main__":
    b1.calistir(__file__)
