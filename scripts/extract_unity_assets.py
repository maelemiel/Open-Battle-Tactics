#!/usr/bin/env python3
"""
extract_unity_assets.py — Super Battle Tactics / Open-Battle-Tactics
Extrait les assets Unity (textures, audio, textes) depuis assets/bin/Data/

Dépendance: pip install UnityPy Pillow

Usage:
    python extract_unity_assets.py --src assets/bin/Data/ --out extracted/
    python extract_unity_assets.py --src assets/bin/Data/ --out extracted/ --types Texture2D,AudioClip
"""

import os
import argparse
from pathlib import Path

try:
    import UnityPy
    from PIL import Image
    UNITYPY_AVAILABLE = True
except ImportError:
    UNITYPY_AVAILABLE = False


SUPPORTED_TYPES = ["Texture2D", "Sprite", "AudioClip", "TextAsset", "Mesh", "Shader", "MonoBehaviour"]


def extract_texture(obj, dest_folder: Path):
    data = obj.read()
    name = data.name or f"texture_{obj.path_id}"
    dest = dest_folder / "Texture2D" / f"{name}.png"
    dest.parent.mkdir(parents=True, exist_ok=True)
    data.image.save(dest, "PNG")
    return dest


def extract_sprite(obj, dest_folder: Path):
    data = obj.read()
    name = data.name or f"sprite_{obj.path_id}"
    dest = dest_folder / "Sprite" / f"{name}.png"
    dest.parent.mkdir(parents=True, exist_ok=True)
    data.image.save(dest, "PNG")
    return dest


def extract_audio(obj, dest_folder: Path):
    data = obj.read()
    name = data.name or f"audio_{obj.path_id}"
    results = []
    for audio_name, audio_data in data.samples.items():
        dest = dest_folder / "AudioClip" / f"{name}.wav"
        dest.parent.mkdir(parents=True, exist_ok=True)
        with open(dest, "wb") as f:
            f.write(audio_data)
        results.append(dest)
    return results


def extract_text_asset(obj, dest_folder: Path):
    data = obj.read()
    name = data.name or f"text_{obj.path_id}"
    raw = bytes(data.script)
    ext = ".txt"
    try:
        content = raw.decode("utf-8")
        if content.strip().startswith("{") or content.strip().startswith("["):
            ext = ".json"
        elif content.strip().startswith("<"):
            ext = ".xml"
    except UnicodeDecodeError:
        ext = ".bin"
    dest = dest_folder / "TextAsset" / f"{name}{ext}"
    dest.parent.mkdir(parents=True, exist_ok=True)
    with open(dest, "wb") as f:
        f.write(raw)
    return dest


def extract_shader(obj, dest_folder: Path):
    data = obj.read()
    name = data.name or f"shader_{obj.path_id}"
    dest = dest_folder / "Shader" / f"{name}.shader"
    dest.parent.mkdir(parents=True, exist_ok=True)
    with open(dest, "w", encoding="utf-8", errors="replace") as f:
        try:
            f.write(str(data.script))
        except Exception:
            f.write(f"# Shader: {name}\n# (données binaires non décodables)")
    return dest


def extract_all(src: str, dest: str, types_filter: list[str], verbose: bool = True):
    if not UNITYPY_AVAILABLE:
        print("❌ UnityPy non installé. Lance: pip install UnityPy Pillow")
        return

    src_path = Path(src)
    dest_path = Path(dest)

    if not src_path.exists():
        print(f"❌ Dossier source introuvable: {src_path}")
        return

    print(f"\n🔍 Chargement des assets Unity depuis {src_path}...")
    env = UnityPy.load(str(src_path))

    stats = {t: 0 for t in SUPPORTED_TYPES}
    stats["Other"] = 0
    errors = 0

    print(f"⚙️  Extraction vers {dest_path}...\n")

    for obj in env.objects:
        type_name = obj.type.name
        if types_filter and type_name not in types_filter:
            continue

        try:
            if type_name == "Texture2D":
                p = extract_texture(obj, dest_path)
                if verbose: print(f"  🖼️  {p.name}")
                stats["Texture2D"] += 1

            elif type_name == "Sprite":
                p = extract_sprite(obj, dest_path)
                if verbose: print(f"  🎭 {p.name}")
                stats["Sprite"] += 1

            elif type_name == "AudioClip":
                ps = extract_audio(obj, dest_path)
                for p in ps:
                    if verbose: print(f"  🔊 {p.name}")
                stats["AudioClip"] += len(ps)

            elif type_name == "TextAsset":
                p = extract_text_asset(obj, dest_path)
                if verbose: print(f"  📄 {p.name}")
                stats["TextAsset"] += 1

            elif type_name == "Shader":
                p = extract_shader(obj, dest_path)
                if verbose: print(f"  ✨ {p.name}")
                stats["Shader"] += 1

            else:
                stats["Other"] += 1

        except Exception as e:
            errors += 1
            if verbose: print(f"  ⚠ Erreur [{type_name}]: {e}")

    print(f"\n✅ Extraction terminée")
    print(f"   Textures  : {stats['Texture2D']}")
    print(f"   Sprites   : {stats['Sprite']}")
    print(f"   Audio     : {stats['AudioClip']}")
    print(f"   TextAssets: {stats['TextAsset']}")
    print(f"   Shaders   : {stats['Shader']}")
    print(f"   Autres    : {stats['Other']}")
    print(f"   Erreurs   : {errors}")


def main():
    parser = argparse.ArgumentParser(description="Extract Unity assets from Super Battle Tactics")
    parser.add_argument("--src",     default="assets/bin/Data/",  help="Dossier source Unity (bin/Data/)")
    parser.add_argument("--out",     default="extracted/",         help="Dossier de sortie")
    parser.add_argument("--types",   default="",                   help="Types à extraire séparés par virgule (ex: Texture2D,AudioClip)")
    parser.add_argument("--verbose", action="store_true",          help="Affichage détaillé")
    args = parser.parse_args()

    types_filter = [t.strip() for t in args.types.split(",")] if args.types else []
    extract_all(args.src, args.out, types_filter, verbose=args.verbose)


if __name__ == "__main__":
    main()
