#!/usr/bin/env python3
"""
rename_project.py — Renomeia CrystallPunk 14 → OpenEdge 14
Atualiza código C#, YAML, FTL, XML, docs e nomes de arquivo/diretório.

Uso:
    python3 Tools/rename_project.py [--dry-run]

Flags:
    --dry-run   Mostra o que seria alterado sem modificar arquivos
"""

import os
import sys

DRY_RUN = "--dry-run" in sys.argv

# Diretório raiz do projeto (dois níveis acima deste script: Tools/ → raiz)
ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))

BINARY_EXTENSIONS = {
    ".png", ".jpg", ".jpeg", ".webp", ".gif", ".bmp", ".ico",
    ".ogg", ".wav", ".mp3", ".opus", ".flac",
    ".ttf", ".otf", ".woff", ".woff2",
    ".rsi", ".zip", ".dll", ".exe", ".pdb", ".nupkg",
    ".db", ".sqlite", ".cache", ".bin", ".so", ".dylib",
    ".pdf", ".pyc",
}

SKIP_DIRS = {".git", "obj", "bin", ".idea", ".vs", "__pycache__"}

# ─── Substituições de CONTEÚDO (mais específico primeiro) ──────────────────
TEXT_REPLACEMENTS = [
    # Nome completo do projeto (variantes mais longas primeiro)
    ("CrystallPunk 14",  "OpenEdge 14"),
    ("CrystallPunk-14",  "OpenEdge-14"),
    ("CrystallPunk14",   "OpenEdge14"),
    # Variantes com grafia alternativa
    ("CristalEdge",      "OpenEdge"),
    ("CristalPunk",      "OpenEdge"),
    ("Crystall Punk",    "OpenEdge"),
    ("CrystallPunk",     "OpenEdge"),
    # camelCase YAML/C# (específico antes do geral)
    ("crystallPunkAllowed", "openEdgeAllowed"),
    ("crystallPunk",        "openEdge"),
    # Prefixo maiúsculo C# (cobre _CP14 em namespaces automaticamente)
    ("CP14",             "OE14"),
    # Prefixo minúsculo com separador (específico antes do bare)
    ("cp14-",            "oe14-"),
    ("cp14_",            "oe14_"),
    # Bare lowercase (catch-all)
    ("cp14",             "oe14"),
]

# ─── Substituições de NOMES DE ARQUIVO/DIRETÓRIO ──────────────────────────
NAME_REPLACEMENTS = [
    ("CrystallPunk 14",  "OpenEdge 14"),
    ("CrystallPunk-14",  "OpenEdge-14"),
    ("CrystallPunk14",   "OpenEdge14"),
    ("CristalEdge",      "OpenEdge"),
    ("CristalPunk",      "OpenEdge"),
    ("CrystallPunk",     "OpenEdge"),
    ("CP14",             "OE14"),
    ("cp14_",            "oe14_"),
    ("cp14-",            "oe14-"),
    ("cp14",             "oe14"),
]


def apply_replacements(text: str, rules: list) -> str:
    for old, new in rules:
        text = text.replace(old, new)
    return text


def is_binary(path: str) -> bool:
    ext = os.path.splitext(path)[1].lower()
    if ext in BINARY_EXTENSIONS:
        return True
    # Verifica se o conteúdo tem bytes nulos (heurística de binário)
    try:
        with open(path, "rb") as f:
            return b"\x00" in f.read(4096)
    except OSError:
        return True


def should_skip_dir(name: str) -> bool:
    return name in SKIP_DIRS


files_modified = 0
files_renamed = 0
dirs_renamed = 0
errors = 0

print(f"{'[DRY-RUN] ' if DRY_RUN else ''}Raiz do projeto: {ROOT}")
print()

# ══════════════════════════════════════════════════════════════════════════════
# Etapa 1 — Substituir conteúdo dos arquivos
# ══════════════════════════════════════════════════════════════════════════════
print("=== Etapa 1: Substituindo conteúdo dos arquivos ===")

for dirpath, dirnames, filenames in os.walk(ROOT):
    dirnames[:] = [d for d in dirnames if not should_skip_dir(d)]

    for filename in filenames:
        filepath = os.path.join(dirpath, filename)

        # Pula o próprio script
        if os.path.abspath(filepath) == os.path.abspath(__file__):
            continue

        if is_binary(filepath):
            continue

        try:
            with open(filepath, "r", encoding="utf-8", errors="ignore") as f:
                original = f.read()

            updated = apply_replacements(original, TEXT_REPLACEMENTS)

            if updated != original:
                files_modified += 1
                rel = os.path.relpath(filepath, ROOT)
                print(f"  ✏  {rel}")
                if not DRY_RUN:
                    with open(filepath, "w", encoding="utf-8") as f:
                        f.write(updated)

        except OSError as e:
            errors += 1
            print(f"  ✗  ERRO: {os.path.relpath(filepath, ROOT)}: {e}", file=sys.stderr)

# ══════════════════════════════════════════════════════════════════════════════
# Etapa 2 — Renomear arquivos
# ══════════════════════════════════════════════════════════════════════════════
print(f"\n=== Etapa 2: Renomeando arquivos ===")

files_to_rename: list[tuple[str, str]] = []
for dirpath, dirnames, filenames in os.walk(ROOT):
    dirnames[:] = [d for d in dirnames if not should_skip_dir(d)]
    for filename in filenames:
        new_name = apply_replacements(filename, NAME_REPLACEMENTS)
        if new_name != filename:
            old_path = os.path.join(dirpath, filename)
            new_path = os.path.join(dirpath, new_name)
            files_to_rename.append((old_path, new_path))

for old_path, new_path in files_to_rename:
    if not os.path.exists(old_path):
        continue
    if os.path.exists(new_path):
        print(f"  ⚠  Destino já existe, pulando: {os.path.relpath(new_path, ROOT)}")
        continue
    files_renamed += 1
    print(f"  📄 {os.path.relpath(old_path, ROOT)}")
    print(f"     → {os.path.relpath(new_path, ROOT)}")
    if not DRY_RUN:
        os.rename(old_path, new_path)

# ══════════════════════════════════════════════════════════════════════════════
# Etapa 3 — Renomear diretórios (bottom-up para evitar conflitos)
# ══════════════════════════════════════════════════════════════════════════════
print(f"\n=== Etapa 3: Renomeando diretórios ===")

dirs_to_rename: list[tuple[str, str]] = []
for dirpath, dirnames, filenames in os.walk(ROOT, topdown=False):
    dirnames[:] = [d for d in dirnames if not should_skip_dir(d)]
    # Pula a pasta raiz do projeto (deve ser renomeada manualmente após o commit)
    if os.path.abspath(dirpath) == os.path.abspath(ROOT):
        continue
    dirname = os.path.basename(dirpath)
    new_dirname = apply_replacements(dirname, NAME_REPLACEMENTS)
    if new_dirname != dirname:
        parent = os.path.dirname(dirpath)
        new_dirpath = os.path.join(parent, new_dirname)
        dirs_to_rename.append((dirpath, new_dirpath))

for old_path, new_path in dirs_to_rename:
    if not os.path.exists(old_path):
        continue
    if os.path.exists(new_path):
        print(f"  ⚠  Destino já existe, pulando: {os.path.relpath(new_path, ROOT)}")
        continue
    dirs_renamed += 1
    print(f"  📁 {os.path.relpath(old_path, ROOT)}")
    print(f"     → {os.path.relpath(new_path, ROOT)}")
    if not DRY_RUN:
        os.rename(old_path, new_path)

# ══════════════════════════════════════════════════════════════════════════════
# Resumo
# ══════════════════════════════════════════════════════════════════════════════
print()
print("=" * 60)
print(f"{'[DRY-RUN] ' if DRY_RUN else ''}Concluído!")
print(f"  Arquivos modificados : {files_modified}")
print(f"  Arquivos renomeados  : {files_renamed}")
print(f"  Diretórios renomeados: {dirs_renamed}")
if errors:
    print(f"  ERROS               : {errors}")
print()
if not DRY_RUN:
    print("Próximos passos:")
    print("  1. dotnet build -c Release   # verificar compilação")
    print("  2. Corrigir erros residuais se houver")
    print("  3. git add -A && git commit")
    print("  4. Renomear pasta raiz (fora do git):")
    print("       mv /home/rusted/Git/CrystallPunk-14 /home/rusted/Git/OpenEdge-14")
    print()
    print("ATENÇÃO — banco de dados:")
    print("  As migrations foram renomeadas (cp14_* → oe14_*).")
    print("  Para dev: apague o arquivo dev_db.db e rode o servidor para recriar.")
    print("  O servidor criará as novas colunas oe14_* a partir do zero.")
else:
    print("Execute sem --dry-run para aplicar as mudanças.")
