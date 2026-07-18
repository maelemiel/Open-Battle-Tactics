#!/usr/bin/env python3
"""
export_game_data.py — Super Battle Tactics / Open-Battle-Tactics
Exporte toutes les données du jeu depuis dataModel.db en JSON structuré.

Usage:
    python export_game_data.py --db assets/dataModel.db --out game_data/
    python export_game_data.py --db assets/dataModel.db --out game_data/ --lang fr
    python export_game_data.py --db assets/dataModel.db --out game_data/ --format json+csv
    python export_game_data.py --db assets/dataModel.db --out game_data/ --list-langs
"""

import sqlite3
import json
import csv
import argparse
from pathlib import Path
from typing import Any


# ─── Helpers ─────────────────────────────────────────────────────────────────

def rows_to_dicts(cursor: sqlite3.Cursor) -> list[dict]:
    cols = [d[0] for d in cursor.description]
    return [dict(zip(cols, row)) for row in cursor.fetchall()]


def save_json(data: Any, path: Path):
    path.parent.mkdir(parents=True, exist_ok=True)
    with open(path, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)
    count = len(data) if isinstance(data, (list, dict)) else 1
    print(f"  ✓ {path}  ({count} entrées)")


def save_csv(data: list[dict], path: Path):
    if not data:
        return
    path.parent.mkdir(parents=True, exist_ok=True)
    with open(path, "w", encoding="utf-8", newline="") as f:
        writer = csv.DictWriter(f, fieldnames=data[0].keys())
        writer.writeheader()
        writer.writerows(data)
    print(f"  ✓ {path}  ({len(data)} lignes)")


def get_localization_columns(db: sqlite3.Connection) -> list[str]:
    """Retourne toutes les colonnes de la table localization (hors 'key')."""
    cur = db.execute("PRAGMA table_info(localization)")
    return [row[1] for row in cur.fetchall() if row[1] != "key"]


def detect_lang_col(db: sqlite3.Connection, lang: str) -> str:
    """
    Détecte la vraie colonne de langue dans la table localization.
    Stratégie : cherche par correspondance partielle avec le code langue.
    Si rien trouvé, prend la première colonne disponible.
    """
    available = get_localization_columns(db)

    if not available:
        raise RuntimeError("La table 'localization' ne contient aucune colonne de traduction.")

    # Correspondances candidates par ordre de priorité
    candidates = {
        "fr": ["fr", "fr_fr", "french", "francais"],
        "en": ["en", "en_us", "english", "en_gb", "eng"],
        "de": ["de", "de_de", "german", "deutsch"],
        "es": ["es", "es_es", "spanish", "espanol"],
        "it": ["it", "it_it", "italian"],
        "pt": ["pt", "pt_br", "portuguese"],
        "zh": ["zh", "zh_cn", "chinese"],
    }

    lower_available = {c.lower(): c for c in available}
    hints = candidates.get(lang.lower(), [lang.lower()])

    # Correspondance exacte
    for hint in hints:
        if hint in lower_available:
            col = lower_available[hint]
            print(f"  🌐 Langue '{lang}' → colonne '{col}'")
            return col

    # Correspondance partielle (ex: colonne '1' qui contient du EN)
    for hint in hints:
        for col_lower, col in lower_available.items():
            if hint in col_lower:
                print(f"  🌐 Langue '{lang}' → colonne '{col}' (correspondance partielle)")
                return col

    # Fallback : première colonne disponible
    fallback = available[0]
    print(f"  ⚠  Langue '{lang}' non trouvée. Colonnes disponibles: {available}")
    print(f"  ⚠  Fallback sur la première colonne: '{fallback}'")
    print(f"  💡 Utilise --list-langs pour voir toutes les colonnes disponibles.")
    return fallback


def list_available_langs(db: sqlite3.Connection):
    """Affiche les colonnes disponibles avec un aperçu du contenu."""
    cols = get_localization_columns(db)
    print("\n🌍 Colonnes de localisation disponibles dans la DB :")
    print(f"   {'Colonne':<20} {'Exemple de valeur':<50}")
    print(f"   {'-'*20} {'-'*50}")
    for col in cols:
        try:
            row = db.execute(
                f"SELECT {col} FROM localization WHERE {col} IS NOT NULL AND {col} != '' LIMIT 1"
            ).fetchone()
            sample = (row[0][:47] + "...") if row and len(str(row[0])) > 50 else (row[0] if row else "(vide)")
        except Exception:
            sample = "(erreur)"
        print(f"   {col:<20} {sample}")
    print(f"\n   Utilise --lang <colonne> pour choisir (ex: --lang {cols[0]})")


def localization_dict(db: sqlite3.Connection, lang_col: str) -> dict[str, str]:
    cur = db.execute(f'SELECT key, "{lang_col}" FROM localization')
    return {row[0]: row[1] for row in cur.fetchall() if row[1]}


def resolve(locs: dict, key: str | None) -> str | None:
    if not key:
        return None
    return locs.get(key, key)


# ─── Exporteurs ──────────────────────────────────────────────────────────────

def export_units(db: sqlite3.Connection, locs: dict) -> list[dict]:
    cur = db.execute("""
        SELECT
            u.id, u.key_name, u.type, u.rarity, u.weapon_anim,
            u.blueprint_linkage_id, u.reward_type_id, u.reward_amount,
            u.research_time, u.unlock_tier, u.can_buy_direct,
            u.found_in_gacha, u.unit_index,
            ut.id AS type_id, ur.id AS rarity_id
        FROM unit u
        LEFT JOIN unit_type ut ON u.type = ut.id
        LEFT JOIN unit_rarity ur ON u.rarity = ur.id
        ORDER BY u.unit_index
    """)
    units = rows_to_dicts(cur)
    for u in units:
        u["name"] = resolve(locs, u["key_name"])
    return units


def export_abilities(db: sqlite3.Connection, locs: dict) -> list[dict]:
    cur = db.execute("""
        SELECT
            a.id, a.key_name, a.key_description, a.ability_type,
            a.handler_id, a.action_type, a.action_boost_type,
            a.action_boost_value_a, a.action_boost_value_b,
            a.action_point, a.target_group, a.is_active, a.is_announcer,
            a.execution_order, a.unlock_tier, a.unlock_order,
            a.research_time, a.limit_one_per_battle, a.limit_one_per_round,
            a.num_kill_unit, a.icon_linkage_id, a.selection_text,
            at.id AS ability_type_id
        FROM ability a
        LEFT JOIN ability_type at ON a.ability_type = at.id
        ORDER BY a.execution_order
    """)
    abilities = rows_to_dicts(cur)
    for ab in abilities:
        ab["name"] = resolve(locs, ab["key_name"])
        ab["description"] = resolve(locs, ab["key_description"])
        ab["ability_type_name"] = {1: "ULTRA", 2: "PRE-COMBAT", 3: "REACTIVE", 4: "PASSIVE"}.get(ab["ability_type"])
    return abilities


def export_unit_types(db: sqlite3.Connection, locs: dict) -> list[dict]:
    cur = db.execute("SELECT * FROM unit_type")
    types = rows_to_dicts(cur)
    for t in types:
        t["name"] = resolve(locs, t.get("key_name"))
    return types


def export_unit_rarity(db: sqlite3.Connection, locs: dict) -> list[dict]:
    cur = db.execute("SELECT * FROM unit_rarity")
    rarities = rows_to_dicts(cur)
    for r in rarities:
        r["name"] = resolve(locs, r.get("key_name"))
    return rarities


def export_events(db: sqlite3.Connection, locs: dict) -> list[dict]:
    cur = db.execute("SELECT * FROM event ORDER BY start_timestamp DESC")
    events = rows_to_dicts(cur)
    for e in events:
        e["name"] = resolve(locs, e.get("key_name"))
        e["description"] = resolve(locs, e.get("key_description"))
    return events


def export_items(db: sqlite3.Connection, locs: dict) -> list[dict]:
    cur = db.execute("SELECT * FROM item")
    items = rows_to_dicts(cur)
    for it in items:
        it["name"] = resolve(locs, it.get("key_name"))
        it["description"] = resolve(locs, it.get("key_description"))
    return items


def export_gacha(db: sqlite3.Connection, locs: dict) -> dict:
    result = {}
    gacha_tables = [
        "gacha_info_details", "gacha_info_items", "gacha_pools",
        "gacha_plinko_details", "gacha_plinko_prizes",
        "gacha_plinko_prize_price", "gacha_plinko_slot_chances",
        "gacha_ab_testing", "gacha_ab_testing_group_enable",
    ]
    for table in gacha_tables:
        try:
            cur = db.execute(f"SELECT * FROM {table}")
            result[table] = rows_to_dicts(cur)
        except sqlite3.OperationalError:
            pass
    return result


def export_progression(db: sqlite3.Connection) -> dict:
    result = {}
    for table in ["progression_division", "progression_promotion_series", "progression_tier_buckets"]:
        try:
            cur = db.execute(f"SELECT * FROM {table}")
            result[table] = rows_to_dicts(cur)
        except sqlite3.OperationalError:
            pass
    return result


def export_ai(db: sqlite3.Connection) -> dict:
    result = {}
    for table in ["ai_army", "ai_army_parts", "ai_handler"]:
        try:
            cur = db.execute(f"SELECT * FROM {table}")
            result[table] = rows_to_dicts(cur)
        except sqlite3.OperationalError:
            pass
    return result


def export_unit_progression(db: sqlite3.Connection) -> dict:
    result = {}
    tables = [
        "unit_action", "unit_cooldown", "unit_destroy_gem_drop",
        "unit_level_progression", "unit_level_up_requirement",
        "unit_part_types", "unit_partial_level", "unit_parts",
        "unit_scrap_value", "unit_special", "unit_special_handler", "unit_weapon_anim",
    ]
    for table in tables:
        try:
            cur = db.execute(f"SELECT * FROM {table}")
            result[table] = rows_to_dicts(cur)
        except sqlite3.OperationalError:
            pass
    return result


def export_leaderboards(db: sqlite3.Connection) -> dict:
    result = {}
    for table in ["leaderboards", "leaderboard_rewards"]:
        try:
            cur = db.execute(f"SELECT * FROM {table}")
            result[table] = rows_to_dicts(cur)
        except sqlite3.OperationalError:
            pass
    return result


def export_config(db: sqlite3.Connection) -> dict:
    try:
        cur = db.execute("SELECT * FROM config")
        rows = cur.fetchall()
        if cur.description and len(cur.description) >= 2:
            return {row[0]: row[1] for row in rows}
        return {}
    except sqlite3.OperationalError:
        return {}


def export_localization_full(db: sqlite3.Connection) -> list[dict]:
    cur = db.execute("SELECT * FROM localization")
    return rows_to_dicts(cur)


def export_all_tables_index(db: sqlite3.Connection) -> list[dict]:
    cur = db.execute("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name")
    tables = [row[0] for row in cur.fetchall()]
    index = []
    for t in tables:
        try:
            count = db.execute(f"SELECT COUNT(*) FROM {t}").fetchone()[0]
        except Exception:
            count = -1
        index.append({"table": t, "rows": count})
    return index


# ─── Main ─────────────────────────────────────────────────────────────────────

def main():
    parser = argparse.ArgumentParser(description="Export Super Battle Tactics game data")
    parser.add_argument("--db",         default="assets/dataModel.db", help="Chemin vers dataModel.db")
    parser.add_argument("--out",        default="game_data",            help="Dossier de sortie")
    parser.add_argument("--lang",       default="en",                   help="Code langue ou nom de colonne (ex: en, fr, 1, en_us)")
    parser.add_argument("--format",     default="json",                 help="Format de sortie: json, csv, json+csv")
    parser.add_argument("--list-langs", action="store_true",            help="Lister les colonnes de langue disponibles et quitter")
    args = parser.parse_args()

    db_path = Path(args.db)
    if not db_path.exists():
        print(f"❌ Base de données introuvable: {db_path}")
        print("   Assure-toi d'être sur la branche reverse-engineering")
        print("   git checkout reverse-engineering")
        return

    db = sqlite3.connect(db_path)

    # Mode liste des langues disponibles
    if args.list_langs:
        list_available_langs(db)
        db.close()
        return

    print(f"\n🎮 Open Battle Tactics — Export des données")
    print(f"   DB     : {db_path}")
    print(f"   Sortie : {args.out}")
    print(f"   Langue : {args.lang}")
    print(f"   Format : {args.format}\n")

    out_dir = Path(args.out)
    fmt = args.format

    lang_col = detect_lang_col(db, args.lang)
    locs = localization_dict(db, lang_col)
    print(f"   {len(locs)} clés de localisation chargées\n")

    def save(data, name):
        path = out_dir / name
        if "json" in fmt:
            save_json(data, path.with_suffix(".json"))
        if "csv" in fmt and isinstance(data, list):
            save_csv(data, path.with_suffix(".csv"))

    print("📋 Index des tables...")
    save(export_all_tables_index(db), "tables_index")

    print("⚙️  Config...")
    cfg = export_config(db)
    out_dir.mkdir(parents=True, exist_ok=True)
    with open(out_dir / "config.json", "w", encoding="utf-8") as f:
        json.dump(cfg, f, ensure_ascii=False, indent=2)
    print(f"  ✓ config.json")

    print("⚔️  Unités...")
    units = export_units(db, locs)
    save(units, "units")
    save(export_unit_types(db, locs), "unit_types")
    save(export_unit_rarity(db, locs), "unit_rarity")
    prog = export_unit_progression(db)
    for k, v in prog.items():
        save(v, f"unit_progression/{k}")

    print("✨ Capacités...")
    abilities = export_abilities(db, locs)
    save(abilities, "abilities")

    print("🗓️  Événements...")
    save(export_events(db, locs), "events")

    print("🎁 Items...")
    save(export_items(db, locs), "items")

    print("🎰 Gacha...")
    gacha = export_gacha(db, locs)
    for k, v in gacha.items():
        save(v, f"gacha/{k}")

    print("🏆 Progression...")
    prog = export_progression(db)
    for k, v in prog.items():
        save(v, f"progression/{k}")

    print("🥇 Leaderboards...")
    lb = export_leaderboards(db)
    for k, v in lb.items():
        save(v, f"leaderboards/{k}")

    print("🤖 IA...")
    ai = export_ai(db)
    for k, v in ai.items():
        save(v, f"ai/{k}")

    print("🌍 Localisation complète...")
    save(export_localization_full(db), "localization_full")

    db.close()

    total_files = sum(1 for _ in out_dir.rglob("*") if _.is_file())
    print(f"\n✅ Export terminé : {total_files} fichiers dans {out_dir}/")
    print(f"   {len(units)} unités exportées")
    print(f"   {len(abilities)} capacités exportées")
    print("\n📁 Structure de sortie :")
    for p in sorted(out_dir.rglob("*")):
        if p.is_file():
            print(f"   {p.relative_to(out_dir)}")


if __name__ == "__main__":
    main()
