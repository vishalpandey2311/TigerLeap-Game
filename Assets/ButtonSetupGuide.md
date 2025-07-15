# 🔘 Button OnClick Events Setup Guide

## Language Button (Main Menu)
**Purpose:** Opens the language selection panel

### Setup:
1. Find your Language button in the Main Menu
2. In Inspector → Button component → On Click ()
3. Click "+" to add event
4. Drag LanguageSelectionPanel GameObject to object field
5. Select Function: **LanguageSelectionPanel.ShowLanguagePanel**

---

## English Language Button (Language Panel)
**Purpose:** Changes to English and closes panel

### Setup:
1. Find English button in Language Selection Panel
2. In Inspector → Button component → On Click ()
3. Add TWO events:

**Event 1:**
- Click "+" to add event
- Drag LanguageManager GameObject to object field  
- Select Function: **LanguageManager.ChangeToEnglish**

**Event 2:**
- Click "+" to add another event
- Drag LanguageSelectionPanel GameObject to object field
- Select Function: **LanguageSelectionPanel.HideLanguagePanel**

---

## Chinese Language Button (Language Panel)
**Purpose:** Changes to Chinese and closes panel

### Setup:
1. Find Chinese button in Language Selection Panel
2. In Inspector → Button component → On Click ()
3. Add TWO events:

**Event 1:**
- Click "+" to add event
- Drag LanguageManager GameObject to object field
- Select Function: **LanguageManager.ChangeToChinese**

**Event 2:**
- Click "+" to add another event
- Drag LanguageSelectionPanel GameObject to object field
- Select Function: **LanguageSelectionPanel.HideLanguagePanel**

---

## Hindi Language Button (Language Panel)
**Purpose:** Changes to Hindi and closes panel

### Setup:
1. Find Hindi button in Language Selection Panel  
2. In Inspector → Button component → On Click ()
3. Add TWO events:

**Event 1:**
- Click "+" to add event
- Drag LanguageManager GameObject to object field
- Select Function: **LanguageManager.ChangeToHindi**

**Event 2:**
- Click "+" to add another event
- Drag LanguageSelectionPanel GameObject to object field
- Select Function: **LanguageSelectionPanel.HideLanguagePanel**

---

## Back Button (Language Panel)
**Purpose:** Closes language panel without changing language

### Setup:
1. Find Back button in Language Selection Panel
2. In Inspector → Button component → On Click ()
3. Click "+" to add event
4. Drag LanguageSelectionPanel GameObject to object field
5. Select Function: **LanguageSelectionPanel.HideLanguagePanel**

---

## 🎯 Quick Reference

| Button | GameObject | Function |
|--------|------------|----------|
| Language (Main Menu) | LanguageSelectionPanel | ShowLanguagePanel |
| English | LanguageManager | ChangeToEnglish |
|         | LanguageSelectionPanel | HideLanguagePanel |
| Chinese | LanguageManager | ChangeToChinese |
|         | LanguageSelectionPanel | HideLanguagePanel |
| Hindi | LanguageManager | ChangeToHindi |
|         | LanguageSelectionPanel | HideLanguagePanel |
| Back | LanguageSelectionPanel | HideLanguagePanel |

## ✅ Testing Checklist

After setting up all buttons:

- [ ] Language button opens the language panel
- [ ] English button changes language to English and closes panel
- [ ] Chinese button changes language to Chinese and closes panel  
- [ ] Hindi button changes language to Hindi and closes panel
- [ ] Back button closes panel without changing language
- [ ] All text updates immediately when language changes
- [ ] Language preference persists after restarting the game

## 🔧 Troubleshooting

**Button doesn't work:**
- Check if GameObject reference is assigned
- Verify function is selected in dropdown
- Make sure LanguageManager and LanguageSelectionPanel scripts are attached

**Language doesn't change:**
- Check if LanguageManager GameObject exists in scene
- Verify Unity Localization package is properly setup
- Check if Localize String Event components are added to text elements
