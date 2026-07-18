#!/usr/bin/env python3
"""
export_game_data.py — Super Battle Tactics / Open-Battle-Tactics
Exporte toutes les données du jeu depuis dataModel.db en JSON structuré.

Usage:
    python export_game_data.py --db assets/dataModel.db --out game_data/
    python export_game_data.py --db assets/dataModel.db --out game_data/ --lang all
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


# ─── Helpers ──────────────────────────────────────────────────────────────────

def rows_to_dicts(cursor: sqlite3.Cursor) -> list[dict]:
    cols = [d[0] for d in cursor.description]
    return [dict(zip(cols, row)) for row in cursor.fetchall()]


def table_columns(db: sqlite3.Connection, table: str) -> set[str]:
    """Retourne l'ensemble des colonnes réelles d'une table."""
    cur = db.execute(f"PRAGMA table_info({table})")
    return {row[1] for row in cur.fetchall()}


def safe_select(db: sqlite3.Connection, table: str, order_by: str | None = None) -> list[dict]:
    """
    SELECT * FROM <table>, avec ORDER BY optionnel.
    Si la colonne d'ordre n'existe pas, fait le SELECT sans ORDER BY.
    Retourne [] si la table n'existe pas.
    """
    try:
        cols = table_columns(db, table)
        if order_by and order_by not in cols:
            order_by = None
        query = f"SELECT * FROM {table}"
        if order_by:
            query += f" ORDER BY {order_by} DESC"
        return rows_to_dicts(db.execute(query))
    except sqlite3.OperationalError:
        return []


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


# ─── Localisation ───────────────────────────────────────────────────────────

def get_localization_columns(db: sqlite3.Connection) -> list[str]:
    cur = db.execute("PRAGMA table_info(localization)")
    return [row[1] for row in cur.fetchall() if row[1] != "key"]


def detect_lang_col(db: sqlite3.Connection, lang: str) -> str | None:
    available = get_localization_columns(db)
    if not available:
        raise RuntimeError("La table 'localization' ne contient aucune colonne de traduction.")

    candidates = {
        "fr":  ["fr", "fr_fr", "french"],
        "en":  ["en", "en_us", "english", "en_gb", "eng"],
        "de":  ["de", "de_de", "german"],
        "es":  ["es", "es_es", "spanish"],
        "it":  ["it", "it_it", "italian"],
        "pt":  ["pt", "pt_br", "portuguese"],
        "zh":  ["zh", "zh_cn", "sch", "tch", "chinese"],
        "ja":  ["ja", "ja_jp", "jp", "japanese"],
        "ko":  ["ko", "ko_kr", "korean"],
        "ru":  ["ru", "ru_ru", "russian"],
        "ar":  ["ar", "ar_sa", "arabic"],
        "tr":  ["tr", "tr_tr", "turkish"],
        "nl":  ["nl", "nl_nl", "dutch"],
        "pl":  ["pl", "pl_pl", "polish"],
        "id":  ["id", "id_id", "indonesian"],
    }

    lower_available = {c.lower(): c for c in available}
    hints = candidates.get(lang.lower(), [lang.lower()])

    for hint in hints:
        if hint in lower_available:
            return lower_available[hint]
    for hint in hints:
        for col_lower, col in lower_available.items():
            if hint in col_lower:
                return col
    if lang in available:
        return lang
    return None


def list_available_langs(db: sqlite3.Connection):
    cols = get_localization_columns(db)
    print("\n🌍 Colonnes de localisation disponibles dans la DB :")
    print(f"   {'Colonne':<20} {'Exemple de valeur':<50}")
    print(f"   {'-'*20} {'-'*50}")
    for col in cols:
        try:
            row = db.execute(
                f'SELECT "{col}" FROM localization WHERE "{col}" IS NOT NULL AND "{col}" != \'\' LIMIT 1'
            ).fetchone()
            sample = (str(row[0])[:47] + "...") if row and len(str(row[0])) > 50 else (row[0] if row else "(vide)")
        except Exception:
            sample = "(erreur)"
        print(f"   {col:<20} {sample}")
    print(f"\n   --lang all        → exporte toutes les colonnes")
    print(f"   --lang <colonne>  → ex: --lang {cols[0]}")


def localization_dict(db: sqlite3.Connection, lang_col: str) -> dict[str, str]:
    cur = db.execute(f'SELECT key, "{lang_col}" FROM localization')
    return {row[0]: row[1] for row in cur.fetchall() if row[1]}


def resolve(locs: dict, key: str | None) -> str | None:
    if not key:
        return None
    return locs.get(key, key)


# ─── Exporteurs ───────────────────────────────────────────────────────────────

def _enrich(rows: list[dict], locs: dict, *keys: str) -> list[dict]:
    """Ajoute les champs traduits pour chaque key_name présent dans rows."""
    for row in rows:
        for k in keys:
            if k in row:
                row[k + "_text"] = resolve(locs, row[k])
    return rows


def export_units(db: sqlite3.Connection, locs: dict) -> list[dict]:
    # Colonnes garanties : id, key_name. Les autres peuvent varier.
    cols = table_columns(db, "unit")
    select_parts = ["u.*"]
    joins = []
    if "type" in cols and table_columns(db, "unit_type"):
        joins.append("LEFT JOIN unit_type ut ON u.type = ut.id")
    if "rarity" in cols and table_columns(db, "unit_rarity"):
        joins.append("LEFT JOIN unit_rarity ur ON u.rarity = ur.id")

    order = "unit_index" if "unit_index" in cols else "id"
    query = f"SELECT {', '.join(select_parts)} FROM unit u {' '.join(joins)} ORDER BY u.{order}"
    try:
        rows = rows_to_dicts(db.execute(query))
    except sqlite3.OperationalError:
        rows = safe_select(db, "unit")

    return _enrich(rows, locs, "key_name")


def export_abilities(db: sqlite3.Connection, locs: dict) -> list[dict]:
    cols = table_columns(db, "ability")
    order = "execution_order" if "execution_order" in cols else "id"
    rows = safe_select(db, "ability", order)
    for ab in rows:
        ab["ability_type_name"] = {1: "ULTRA", 2: "PRE-COMBAT", 3: "REACTIVE", 4: "PASSIVE"}.get(
            ab.get("ability_type")
        )
    return _enrich(rows, locs, "key_name", "key_description")


def export_simple_table(db: sqlite3.Connection, table: str, locs: dict,
                        *loc_keys: str, order_by: str | None = None) -> list[dict]:
    """Export générique : SELECT * + enrichissement localisation."""
    rows = safe_select(db, table, order_by)
    return _enrich(rows, locs, *loc_keys)


def export_multi_tables(db: sqlite3.Connection, tables: list[str]) -> dict:
    result = {}
    for t in tables:
        rows = safe_select(db, t)
        if rows:
            result[t] = rows
    return result


# ─── Export d'une langue ──────────────────────────────────────────────────────

def export_for_lang(db: sqlite3.Connection, lang_col: str, out_dir: Path, fmt: str):
    locs = localization_dict(db, lang_col)
    print(f"\n  📦 [{lang_col}]  {len(locs)} clés → {out_dir}/")

    def save(data, name):
        if not data:
            return
        path = out_dir / name
        if "json" in fmt:
            save_json(data, path.with_suffix(".json"))
        if "csv" in fmt and isinstance(data, list):
            save_csv(data, path.with_suffix(".csv"))

    # Unités
    units = export_units(db, locs)
    save(units, "units")
    save(export_simple_table(db, "unit_type",   locs, "key_name"),   "unit_types")
    save(export_simple_table(db, "unit_rarity", locs, "key_name"),   "unit_rarity")

    # Capacités
    abilities = export_abilities(db, locs)
    save(abilities, "abilities")

    # Événements
    save(export_simple_table(db, "event", locs, "key_name", "key_description"), "events")

    # Items
    save(export_simple_table(db, "item", locs, "key_name", "key_description"), "items")

    # Progression des unités (tables sans localisation)
    for t in [
        "unit_action", "unit_cooldown", "unit_destroy_gem_drop",
        "unit_level_progression", "unit_level_up_requirement",
        "unit_part_types", "unit_partial_level", "unit_parts",
        "unit_scrap_value", "unit_special", "unit_special_handler", "unit_weapon_anim",
    ]:
        rows = safe_select(db, t)
        if rows:
            save(rows, f"unit_progression/{t}")

    # Gacha
    for t in [
        "gacha_info_details", "gacha_info_items", "gacha_pools",
        "gacha_plinko_details", "gacha_plinko_prizes", "gacha_plinko_prize_price",
        "gacha_plinko_slot_chances", "gacha_ab_testing", "gacha_ab_testing_group_enable",
    ]:
        rows = safe_select(db, t)
        if rows:
            save(rows, f"gacha/{t}")

    # Progression / classements / IA
    for t in ["progression_division", "progression_promotion_series", "progression_tier_buckets"]:
        rows = safe_select(db, t)
        if rows:
            save(rows, f"progression/{t}")
    for t in ["leaderboards", "leaderboard_rewards"]:
        rows = safe_select(db, t)
        if rows:
            save(rows, f"leaderboards/{t}")
    for t in ["ai_army", "ai_army_parts", "ai_handler"]:
        rows = safe_select(db, t)
        if rows:
            save(rows, f"ai/{t}")

    return len(units), len(abilities)


# ─── Main ─────────────────────────────────────────────────────────────────────

def main():
    parser = argparse.ArgumentParser(description="Export Super Battle Tactics game data")
    parser.add_argument("--db",         default="assets/dataModel.db")
    parser.add_argument("--out",        default="game_data")
    parser.add_argument("--lang",       default="all",
                        help="'all' (défaut), code langue (fr,en...), nom de colonne exact, ou liste séparée par virgules")
    parser.add_argument("--format",     default="json", help="json | csv | json+csv")
    parser.add_argument("--list-langs", action="store_true")
    args = parser.parse_args()

    db_path = Path(args.db)
    if not db_path.exists():
        print(f"❌ DB introuvable: {db_path}")
        return

    db = sqlite3.connect(db_path)

    if args.list_langs:
        list_available_langs(db)
        db.close()
        return

    all_cols = get_localization_columns(db)
    out_root = Path(args.out)
    fmt      = args.format

    print(f"\n🎮 Open Battle Tactics — Export des données")
    print(f"   DB     : {db_path}")
    print(f"   Sortie : {out_root}")
    print(f"   Langue : {args.lang}")
    print(f"   Format : {fmt}\n")

    # Résoudre la liste de colonnes
    if args.lang.strip().lower() == "all":
        cols_to_export = all_cols
    else:
        requested = [l.strip() for l in args.lang.split(",") if l.strip()]
        cols_to_export = []
        for req in requested:
            col = detect_lang_col(db, req)
            if col:
                cols_to_export.append(col)
            else:
                print(f"  ⚠  '{req}' introuvable. Colonnes dispo : {all_cols}")

    if not cols_to_export:
        print("❌ Aucune colonne valide. Abandon.")
        db.close()
        return

    # Fichiers communs (une seule fois)
    print("📋 Index des tables...")
    save_json(export_multi_tables(db, [
        t["table"] for t in rows_to_dicts(
            db.execute("SELECT name AS 'table' FROM sqlite_master WHERE type='table' ORDER BY name")
        )
    # on stocke juste le compte
    ]), out_root / "tables_index.json") if False else None

    idx = []
    for row in db.execute("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name"):
        t = row[0]
        try:
            count = db.execute(f"SELECT COUNT(*) FROM {t}").fetchone()[0]
        except Exception:
            count = -1
        idx.append({"table": t, "rows": count})
    save_json(idx, out_root / "tables_index.json")

    print("⚙️  Config...")
    try:
        cfg_rows = db.execute("SELECT * FROM config").fetchall()
        cfg_cur  = db.execute("SELECT * FROM config")
        cfg = {r[0]: r[1] for r in cfg_rows} if cfg_cur.description and len(cfg_cur.description) >= 2 else {}
    except sqlite3.OperationalError:
        cfg = {}
    out_root.mkdir(parents=True, exist_ok=True)
    save_json(cfg, out_root / "config.json")

    print("🌍 Localisation complète (toutes langues)...")
    save_json(rows_to_dicts(db.execute("SELECT * FROM localization")), out_root / "localization_full.json")

    # Export par langue
    print(f"\n🔄 Export de {len(cols_to_export)} langue(s) : {cols_to_export}")
    total_units = total_abilities = 0

    for col in cols_to_export:
        lang_dir = out_root / col
        try:
            nu, na = export_for_lang(db, col, lang_dir, fmt)
            total_units     = max(total_units, nu)
            total_abilities = max(total_abilities, na)
        except Exception as e:
            print(f"  ❌ Erreur pour la langue '{col}': {e}")

    db.close()

    total_files = sum(1 for _ in out_root.rglob("*") if _.is_file())
    print(f"\n✅ Export terminé : {total_files} fichiers dans {out_root}/")
    print(f"   {total_units} unités · {total_abilities} capacités · {len(cols_to_export)} langue(s)")
    print(f"\n📁 Arborescence :")
    shown = 0
    for p in sorted(out_root.rglob("*")):
        if shown >= 80:
            print("   ... (tronqué, trop de fichiers)")
            break
        indent = "   " + "  " * (len(p.relative_to(out_root).parts) - 1)
        icon = "📂" if p.is_dir() else "📄"
        print(f"{indent}{icon} {p.name}")
        shown += 1


if __name__ == "__main__":
    main()
