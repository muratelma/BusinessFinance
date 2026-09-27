"""Bolum numaralarini birer kaydirir (karar 6). Kullanim: kaydir.py <icerik.py> [--yaz]
Yalniz string literal'lerde calisir. Belge 2 gondermeleri korunur. Eski 1.x / 2.x gonderme ve
'Bolum 1/2' ifadeleri raporlanir, elle cozulur."""
import io, re, sys, tokenize

BELGE2 = re.compile(r"Belge 2 (?:Bölüm \d+|§?\d+\.\d+(?:\s*(?:,|–|ve)\s*\d+\.\d+)*)")
# Cumle sonu noktasi gondermenin parcasi degildir: "→ 9.5." ve "→ 9." de kayar;
# noktadan sonra rakam gelirse (4.9.12 gibi surum) eslesmez.
ALT = re.compile(r"(?<![\d.])(\d{1,2})\.(\d)(?!\d|\.\d)")
OK_TEK = re.compile(r"(→ )(\d{1,2})(?!\d|\.\d)(?=[)\"\s,.'’]|$)")
BOLUM = re.compile(r"Bölüm (\d{1,2})")
rapor = []

def kaydir_metin(s):
    maske = {}
    def m(mt):
        k = f"\x00{len(maske)}\x00"; maske[k] = mt.group(0); return k
    s = BELGE2.sub(m, s)
    def alt(mt):
        x = int(mt.group(1))
        if x in (1, 2):
            rapor.append(("eski1-2", mt.group(0))); return mt.group(0)
        if 3 <= x <= 13:
            return f"{x-1}.{mt.group(2)}"
        return mt.group(0)
    s = ALT.sub(alt, s)
    def tek(mt):
        x = int(mt.group(2))
        if 3 <= x <= 13:
            return f"{mt.group(1)}{x-1}"
        if x in (1, 2):
            rapor.append(("tek1-2", mt.group(0)))
        return mt.group(0)
    s = OK_TEK.sub(tek, s)
    def bol(mt):
        x = int(mt.group(1))
        if 3 <= x <= 13:
            return f"Bölüm {x-1}"
        rapor.append(("bolum", mt.group(0))); return mt.group(0)
    s = BOLUM.sub(bol, s)
    for k, v in maske.items():
        s = s.replace(k, v)
    return s

def isle(yol, yaz):
    src = open(yol, encoding='utf8').read()
    toks = list(tokenize.generate_tokens(io.StringIO(src).readline))
    satirlar = src.splitlines(keepends=True)
    ofs = [0]
    for l in satirlar: ofs.append(ofs[-1] + len(l))
    degis = []
    for t in toks:
        if t.type == tokenize.STRING:
            yeni = kaydir_metin(t.string)
            if yeni != t.string:
                a = ofs[t.start[0]-1] + t.start[1]; b = ofs[t.end[0]-1] + t.end[1]
                degis.append((a, b, t.string, yeni))
    out = src
    for a, b, eski, yeni in reversed(degis):
        out = out[:a] + yeni + out[b:]
    out = re.sub(r"^NO = (\d+)$", lambda m: f"NO = {int(m.group(1))-1}", out, count=1, flags=re.M)
    if yaz:
        open(yol, 'w', encoding='utf8').write(out)
    return degis

if __name__ == '__main__':
    yaz = '--yaz' in sys.argv
    for yol in [a for a in sys.argv[1:] if a != '--yaz']:
        rapor.clear()
        d = isle(yol, yaz)
        print(f"== {yol}: {len(d)} string değişti")
        for r in rapor: print("   ELLE:", r)
