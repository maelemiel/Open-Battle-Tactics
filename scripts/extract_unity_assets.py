import unitypy
import os
from PIL import Image
import json

DATA_DIR = "assets/bin/Data"
OUTPUT_DIR = "extracted_assets"

os.makedirs(f"{OUTPUT_DIR}/Texture2D", exist_ok=True)
os.makedirs(f"{OUTPUT_DIR}/AudioClip", exist_ok=True)
os.makedirs(f"{OUTPUT_DIR}/TextAsset", exist_ok=True)

def extract_file(filepath):
    try:
        env = unitypy.load(filepath)
        for obj in env.objects:
            try:
                if obj.type.name == "Texture2D":
                    data = obj.read()
                    name = data.m_Name  # ← m_Name, PAS .name
                    img = data.image
                    img.save(f"{OUTPUT_DIR}/Texture2D/{name}.png")
                    print(f"[Texture2D] {name}.png")

                elif obj.type.name == "AudioClip":
                    data = obj.read()
                    name = data.m_Name
                    # AudioClip v2 : utiliser fsb_importer ou les samples
                    for i, sample in enumerate(data.samples.items()):
                        sample_name, sample_data = sample
                        out_name = sample_name if sample_name else f"{name}_{i}"
                        with open(f"{OUTPUT_DIR}/AudioClip/{out_name}.wav", "wb") as f:
                            f.write(sample_data)
                        print(f"[AudioClip] {out_name}.wav")

                elif obj.type.name == "TextAsset":
                    data = obj.read()
                    name = data.m_Name
                    text = data.m_Script
                    # m_Script peut être bytes ou str
                    mode = "wb" if isinstance(text, bytes) else "w"
                    with open(f"{OUTPUT_DIR}/TextAsset/{name}.txt", mode) as f:
                        f.write(text)
                    print(f"[TextAsset] {name}.txt")

            except Exception as e:
                pass  # Certains objets dans les bundles ne sont pas lisibles

    except Exception as e:
        pass  # Fichier non-Unity ou corrompu, skip

# Itérer sur tous les fichiers du dossier Data
files = os.listdir(DATA_DIR)
print(f"Total fichiers à scanner : {len(files)}")

for i, fname in enumerate(files):
    fpath = os.path.join(DATA_DIR, fname)
    if os.path.isfile(fpath):
        extract_file(fpath)
        if i % 50 == 0:
            print(f"Progression : {i}/{len(files)}")

print("Extraction terminée !")