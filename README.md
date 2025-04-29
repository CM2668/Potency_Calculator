# Potency Calculator
A calculator to determine the potency of an attack in the custom made Final Fantasy TTRPG.

## How it works
### Action
- Validate numbers by removing non-numerical text, exception being hyphens when the number can be negative.
- Calculate from the equation:

$$ Damage = {roll * {potency \over 10} * (attribute * (1 + ({buff \over 100}))) \over 100} $$

- The damage is then rounded up to the next whole integer using the ceiling function.
- The calculator checks to see if a critical hit was rolled, and if so, doubles the damage.
- Lastly the damage is capped based on the character's level:
  - 9999 below level 30
  - 99999 below level 60
  - 999999 above level 60

### Defense/Spirit
- Validate numbers by removing non-numerical text, exception being hyphens when the number can be negative.
- Checks the archetype for base resistances and limits
- Calculate from the equation:

$$ Damage = {((hit * (1 - {resistance \over 100})) * (1 - chance) - attribute) - defense} $$

## Page Descriptions
Stats Page:
- Character level
- Archetype
- Two damaging attribute
- Two defense attribute

Action Page:
- Select damaging attribute
- Set crit threshold
- Set buff, possitive or negative
- Insert roll and potency
- Effect updates for total damage

Defense Page:
- Set defense level and elemental resistance
- Insert base damage taken

Spirit Page:
- Same as defense

Save/Load

Cooldown tracker
- Inset skill name
- Ten checkboxes
