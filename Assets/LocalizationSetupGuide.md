# 🌍 Unity Localization Setup Guide for TigerLeap Game

## 📋 Overview
This guide will help you set up Unity's Localization Package to support multiple languages (English, Chinese, Hindi) in your TigerLeap game.

## 🛠️ Step-by-Step Setup Instructions

### **Phase 1: Unity Localization System Setup**

#### 1.1 Open Localization Tables Window
1. In Unity Editor, go to `Window > Asset Management > Localization Tables`
2. This will open the Localization Tables window

#### 1.2 Create Locales
1. In the Localization Tables window, click on "Locale Generator" tab
2. Add the following locales:
   - **English (en)** - Check "English"
   - **Chinese (zh)** - Check "Chinese (Simplified)" or "Chinese (Traditional)"
   - **Hindi (hi)** - Check "Hindi"
3. Click "Generate Locales"

#### 1.3 Create String Tables
1. In the Localization Tables window, go to "Tables" tab
2. Click "Create" > "String Table Collection"
3. Name it "MainMenuTexts"
4. Click "Create"

#### 1.4 Add String Entries
Add these entries to your String Table (MainMenuTexts):

| Key | English | Chinese | Hindi |
|-----|---------|---------|-------|
| title_game | TigerLeap Game | 虎跃游戏 | टाइगरलीप गेम |
| button_login | Login | 登录 | लॉगिन |
| button_signup | Sign Up | 注册 | साइन अप |
| button_play | Play | 开始游戏 | खेलें |
| button_settings | Settings | 设置 | सेटिंग्स |
| button_quit | Quit | 退出 | बाहर निकलें |
| button_back | Back | 返回 | वापस |
| button_language | Language | 语言 | भाषा |
| label_email | Email | 邮箱 | ईमेल |
| label_password | Password | 密码 | पासवर्ड |
| text_forgot_password | Forgot Password? | 忘记密码？ | पासवर्ड भूल गए? |
| text_loading | Loading... | 加载中... | लोड हो रहा है... |
| text_welcome | Welcome! | 欢迎！ | स्वागत है! |
| language_english | English | 英语 | अंग्रेजी |
| language_chinese | 中文 | 中文 | चीनी |
| language_hindi | हिन्दी | 印地语 | हिन्दी |
| mahjong_game | Mahjong Game | 麻将游戏 | माहजोंग गेम |
| taichi_game | Taichi Game | 太极游戏 | ताई ची गेम |
| instructions_title | Instructions | 游戏说明 | निर्देश |
| got_it | Got It! | 明白了！ | समझ गया! |

### **Phase 2: Scene Setup**

#### 2.1 Create Language Manager GameObject
1. In your Main Menu scene, create an empty GameObject named "LanguageManager"
2. Add the `LanguageManager` script to it
3. In the inspector:
   - Check "Initialize On Start"
   - Check "Show Debug Logs"
   - Right-click on the script and select "Setup Default Languages"

#### 2.2 Create Language Selection Panel
1. Create a Canvas child object named "LanguageSelectionPanel"
2. Set it up with this hierarchy:
```
LanguageSelectionPanel (Canvas/Panel)
├── BackgroundImage (Image - dark semi-transparent)
├── LanguagePanel (Panel)
│   ├── Title (Text: "Select Language" / "选择语言" / "भाषा चुनें")
│   ├── EnglishButton (Button)
│   │   └── Text (Text: "English")
│   ├── ChineseButton (Button)
│   │   └── Text (Text: "中文")
│   ├── HindiButton (Button)
│   │   └── Text (Text: "हिन्दी")
│   └── BackButton (Button)
│       └── Text (Text: "Back" / "返回" / "वापस")
```

#### 2.3 Add LanguageSelectionPanel Script
1. Add the `LanguageSelectionPanel` script to the LanguageSelectionPanel GameObject
2. Assign all the UI references in the inspector

### **Phase 3: Replace Static Text with Localized Text**

#### 3.1 Add Localize String Event Components
For each Text/TextMeshPro component that needs localization:

1. Select the Text component
2. In the Inspector, click "Add Component"
3. Search for "Localize String Event"
4. Add the component
5. In the String Reference field:
   - Set Table Collection to "MainMenuTexts"
   - Set Table Entry to the appropriate key (e.g., "button_login")

#### 3.2 Common UI Elements to Localize
Apply "Localize String Event" to these components:

**Main Menu:**
- Title Text → "title_game"
- Login Button Text → "button_login" 
- Sign Up Button Text → "button_signup"
- Language Button Text → "button_language"

**Login Panel:**
- Email Label → "label_email"
- Password Label → "label_password"
- Login Button → "button_login"
- Forgot Password Link → "text_forgot_password"

**Game Choose Panel:**
- Mahjong Button Text → "mahjong_game"
- Taichi Button Text → "taichi_game"

**General:**
- Back Buttons → "button_back"
- Loading Text → "text_loading"

### **Phase 4: Connect Button Events**

#### 4.1 Language Button Setup (Main Menu)
1. Find your main menu Language button
2. In its Button component's "On Click ()" events:
   - Click the "+" to add an event
   - Drag the LanguageSelectionPanel GameObject to the object field
   - Select "LanguageSelectionPanel.ShowLanguagePanel"

#### 4.2 Language Selection Buttons Setup
For each language button in the language selection panel:

**English Button:**
1. Add event: LanguageManager.ChangeToEnglish
2. Add event: LanguageSelectionPanel.HideLanguagePanel

**Chinese Button:**
1. Add event: LanguageManager.ChangeToChinese  
2. Add event: LanguageSelectionPanel.HideLanguagePanel

**Hindi Button:**
1. Add event: LanguageManager.ChangeToHindi
2. Add event: LanguageSelectionPanel.HideLanguagePanel

**Back Button:**
1. Add event: LanguageSelectionPanel.HideLanguagePanel

### **Phase 5: Testing**

#### 5.1 Test Language Switching
1. Play the scene
2. Click the Language button - panel should open
3. Click each language button - text should change immediately
4. Language preference should persist between sessions

#### 5.2 Common Issues & Solutions

**Issue: Text doesn't change**
- Check if Localize String Event component is added
- Verify Table Collection and Entry are set correctly
- Check if locale exists in Localization Settings

**Issue: Language panel doesn't show**
- Check if LanguageSelectionPanel script references are assigned
- Verify button OnClick events are set up correctly

**Issue: Language doesn't persist**
- LanguageManager saves to PlayerPrefs automatically
- Check if LanguageManager has "Initialize On Start" enabled

## 🎯 **Final Checklist**

- [ ] Localization Tables created with all text entries
- [ ] LanguageManager GameObject created and configured
- [ ] Language Selection Panel UI created
- [ ] All static text components have Localize String Event
- [ ] Button OnClick events connected properly
- [ ] Tested language switching works
- [ ] Language preference persists between sessions

## 📝 **Adding New Text for Localization**

When you need to add new text:

1. **Add to String Table:**
   - Open Localization Tables window
   - Go to MainMenuTexts table
   - Add new entry with key and translations

2. **Add to UI:**
   - Add "Localize String Event" component to Text
   - Set Table Collection: "MainMenuTexts"
   - Set Table Entry: your new key

3. **Test:**
   - Switch languages to verify all translations appear

## 🔧 **Advanced Features (Optional)**

- **Audio Localization:** Create Audio Table Collections for localized sound/music
- **Image Localization:** Create Asset Table Collections for localized images/icons
- **Pluralization:** Use Smart Format for plural rules in different languages
- **RTL Support:** Configure for right-to-left languages if needed

This setup provides a robust, scalable localization system that can easily be extended to support additional languages in the future.
