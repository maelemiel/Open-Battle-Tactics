#!/usr/bin/env python3
"""
extract_unity_assets.py — Extraction des assets Unity (UnityPy).
Usage:
    python extract_unity_assets.py [--data-dir assets/bin/Data] [--out extracted_assets]
À lancer sur la branche `reverse-engineering` (données Unity originales).
"""

import argparse
import os
import sys

import unitypy

# Compteurs: objets extraits vs ignorés, pour ne pas avaler les erreurs en silence.
stats = {"extracted": 0, "empty": 0, "failed": 0}


def write_bytes(path: str, data: bytes):
    """Écrit seulement si non vide. Compte les fichiers vides."""
    if not data:
        stats["empty"] += 1
        return
    with open(path, "wb") as f:
        f.write(data)
    stats["extracted"] += 1
    print(f"  ✓ {path}")


def extract_object(obj, out_dir: str):
    os.makedirs(out_dir, exist_ok=True)
    type_name = obj.type.name

    if type_name == "Texture2D":
        data = obj.read()
        img = data.image
        if img.getbbox() is None:
            stats["empty"] += 1
            return
        out = f"{out_dir}/Texture2D/{data.m_Name}.png"
        img.save(out)
        stats["extracted"] += 1
        print(f"  ✓ {out}")

    elif type_name == "AudioClip":
        data = obj.read()
        for i, (sample_name, sample_data) in enumerate(data.samples.items()):
            name = sample_name if sample_name else f"{data.m_Name}_{i}"
            write_bytes(f"{out_dir}/AudioClip/{name}.wav", sample_data)

    elif type_name == "TextAsset":
        data = obj.read()
        text = data.m_Script
        raw = text if isinstance(text, bytes) else str(text).encode("utf-8")
        write_bytes(f"{out_dir}/TextAsset/{data.m_Name}.txt", raw)

    elif type_name == "Shader":
        data = obj.read()
        # m_Script: bytecode désarialisé du shader (str ou bytes selon la version UnityPy).
        script = getattr(data, "m_Script", b"")
        raw = script if isinstance(script, bytes) else str(script).encode("utf-8", errors="replace")
        write_bytes(f"{out_dir}/Shader/{data.m_Name}.shader", raw)


def extract_file(filepath: str, out_dir: str):
    if not os.path.isfile(filepath):
        return
    try:
        env = unitypy.load(filepath)
    except Exception as e:
        stats["failed"] += 1
        print(f"  ✗ load {filepath}: {e}", file=sys.stderr)
        return

    for obj in env.objects:
        try:
            extract_object(obj, out_dir)
        except Exception as e:
            stats["failed"] += 1
            print(f"  ✗ {obj.type.name} in {filepath}: {e}", file=sys.stderr)


def main():
    parser = argparse.ArgumentParser(description="Extraction des assets Unity")
    parser.add_argument("--data-dir", default="assets/bin/Data")
    parser.add_argument("--out", default="extracted_assets")
    args = parser.parse_args()

    for sub in ("Texture2D", "AudioClip", "TextAsset", "Shader"):
        os.makedirs(f"{args.out}/{sub}", exist_ok=True)

    files = sorted(os.listdir(args.data_dir))
    print(f"Total fichiers à scanner : {len(files)}")

    for i, fname in enumerate(files):
        extract_file(os.path.join(args.data_dir, fname), args.out)
        if i % 50 == 0:
            print(f"Progression : {i}/{len(files)}")

    print(f"\nExtraction terminée : {stats['extracted']} extraits, "
          f"{stats['empty']} vides, {stats['failed']} en erreur")
    if stats["empty"] or stats["failed"]:
        print("⚠ Fichiers vides/erreurs détectés — voir DATA_EXTRACTION.md section Fixes.")


if __name__ == "__main__":
    main()
