# -*- coding: utf-8 -*-
"""Bolum 1 uretici. Icerik icerik.py'de; yerlesim ortak/b2.py'de."""
import pathlib
import sys

sys.path.insert(0, str(pathlib.Path(__file__).resolve().parents[1] / "ortak"))
import b2  # noqa: E402

if __name__ == "__main__":
    b2.calistir(__file__)
