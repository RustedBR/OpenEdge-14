#!/usr/bin/env python3
"""Generate FTL stubs for OE14 entity localization using regex parsing."""
import os
import re
from collections import defaultdict

PROTOTYPES_DIR = "Resources/Prototypes/_OE14"
OUTPUT_DIR = "Resources/Locale/pt-BR/_OE14/entities"

CATEGORY_MAP = {
    "Entities/Objects/Weapons": "weapons",
    "Entities/Objects/Tools": "tools",
    "Entities/Objects/Food": "food",
    "Entities/Objects/Consumables": "consumables",
    "Entities/Objects/Magic": "magic",
    "Entities/Objects/Materials": "materials",
    "Entities/Objects/Misc": "misc",
    "Entities/Objects/Bureaucracy": "misc",
    "Entities/Objects/Keys": "misc",
    "Entities/Objects/Specific": "misc",
    "Entities/Mobs/": "mobs",
    "Entities/Structures/": "structures",
    "Spells/": "spells",
}

def get_category(filepath):
    for path_part, category in CATEGORY_MAP.items():
        if path_part in filepath:
            return category
    return "misc"

def sanitize_ftl(text):
    if not text:
        return ""
    text = str(text)
    text = text.replace('{', '{{').replace('}', '}}')
    return text

def extract_entities_from_file(filepath):
    """Extract entity prototypes from a YAML file using regex."""
    entities = []
    
    try:
        with open(filepath, 'r', encoding='utf-8') as fh:
            content = fh.read()
    except Exception:
        return entities
    
    # Pattern to match entity blocks
    # Matches "- type: entity" or "type: entity" at start of line
    entity_pattern = re.compile(
        r'(?:(?:^|\n)\s*-\s*type:\s*entity\s*\n)|(?:(?:^|\n)\s*type:\s*entity\s*\n)',
        re.MULTILINE
    )
    
    matches = list(entity_pattern.finditer(content))
    
    if not matches:
        return entities
    
    for i, match in enumerate(matches):
        start = match.end()
        end = matches[i + 1].start() if i + 1 < len(matches) else len(content)
        block = content[start:end]
        
        # Extract id (must be at indentation level 2)
        id_match = re.search(r'^\s{2}id:\s*(.+)$', block, re.MULTILINE)
        if not id_match:
            continue
        eid = id_match.group(1).strip().strip('"').strip("'")
        
        # Extract name (must be at indentation level 2)
        name_match = re.search(r'^\s{2}name:\s*(.+)$', block, re.MULTILINE)
        if not name_match:
            continue
        name = name_match.group(1).strip().strip('"').strip("'")
        
        # Extract description (if present)
        desc_match = re.search(r'^\s{2}description:\s*(.+)$', block, re.MULTILINE)
        desc = desc_match.group(1).strip().strip('"').strip("'") if desc_match else ""
        
        if not name or name.startswith('#'):
            continue
        
        entities.append((eid, name, desc))
    
    return entities

def main():
    entities = defaultdict(list)
    
    print("Scanning prototype files...")
    
    for root, dirs, files in os.walk(PROTOTYPES_DIR):
        for f in files:
            if not f.endswith('.yml'):
                continue
            filepath = os.path.join(root, f)
            category = get_category(filepath)
            
            found = extract_entities_from_file(filepath)
            if found:
                entities[category].extend(found)
    
    os.makedirs(OUTPUT_DIR, exist_ok=True)
    
    print(f"\nGenerating FTL stubs...")
    
    total = 0
    for category, items in entities.items():
        outpath = os.path.join(OUTPUT_DIR, f"{category}.ftl")
        with open(outpath, 'w', encoding='utf-8') as out:
            out.write(f"# OE14 Entity Names - {category.title()}\n\n")
            for eid, name, desc in items:
                name_san = sanitize_ftl(name)
                out.write(f"ent-{eid} = {name_san}\n")
                if desc:
                    desc_san = sanitize_ftl(desc)
                    out.write(f"    .desc = {desc_san}\n")
                out.write("\n")
                total += 1
    
    print(f"Generated {total} entity entries in {len(entities)} category files:")
    for cat in sorted(entities.keys()):
        print(f"  {cat}: {len(entities[cat])} entities")

if __name__ == '__main__':
    main()
