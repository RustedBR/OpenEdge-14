#!/usr/bin/env python3
import yaml
import os
import re
from pathlib import Path

def add_localization_id_to_files():
    entities_dir = Path("Resources/Prototypes/_OE14/Entities")
    processed = 0
    entities_modified = 0
    
    for yaml_file in entities_dir.rglob("*.yml"):
        if "base.yml" in yaml_file.name:
            continue
            
        with open(yaml_file, 'r', encoding='utf-8') as f:
            content = f.read()
            
        try:
            data = yaml.safe_load(content)
        except:
            continue
            
        if not data or not isinstance(data, list):
            continue
            
        modified = False
        new_data = []
        
        for item in data:
            if not isinstance(item, dict):
                new_data.append(item)
                continue
                
            entity_type = item.get('type', '')
            entity_id = item.get('id', '')
            
            if entity_type == 'entity' and entity_id.startswith('OE14'):
                if 'localizationId' not in item:
                    item['localizationId'] = f'ent-{entity_id}'
                    modified = True
                    entities_modified += 1
                    
            new_data.append(item)
            
        if modified:
            with open(yaml_file, 'w', encoding='utf-8') as f:
                yaml.dump(new_data, f, allow_unicode=True, default_flow_style=False, sort_keys=False)
            processed += 1
            
    return processed, entities_modified

if __name__ == "__main__":
    files, entities = add_localization_id_to_files()
    print(f"Arquivos modificados: {files}")
    print(f"Entidades atualizadas: {entities}")
