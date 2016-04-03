using System;
using System.Linq;
using UnityEngine;
using KSP.IO;

/******************************************************************************
 * Copyright (c) 2013-2014, Justin Bengtson
 * Copyright (c) 2015, George Sedov
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice,
 * this list of conditions and the following disclaimer.
 *
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 ******************************************************************************/

namespace KSPPreciseManeuver {
internal class PreciseManeuverConfig {

  internal static PreciseManeuverConfig _instance = null;

  internal static PreciseManeuverConfig getInstance () {
    if (_instance == null) _instance = new PreciseManeuverConfig ();
    return _instance;
  }

  private AssetBundle _prefabs;

  internal AssetBundle prefabs {
    get {
      if (_prefabs == null) {
        var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
        path = path.Replace(System.IO.Path.GetFileName(path), "prefabs");
        _prefabs = AssetBundle.CreateFromFile(path);
      }
      return _prefabs;
    }
  }

  private Rect _mainWindowPos = new Rect (Screen.width / 10, 20, 0, 0);
  internal Rect mainWindowPos {
    get { return _mainWindowPos; }
    set { _mainWindowPos = value; }
  }
  internal void readjustMainWindow() {
    _mainWindowPos.height = 0;
  }
  private Rect _keymapperWindowPos = new Rect (Screen.width / 5, 20, 0, 0);
  internal Rect keymapperWindowPos {
    get { return _keymapperWindowPos; }
    set { _keymapperWindowPos = value; }
  }

  internal bool showMainWindow = true;
  internal bool showKeymapperWindow = false;

  private int _increment = 0;
  internal double increment { get { return Math.Pow (10, _increment); } }
  internal double incrementDeg { get { return Math.PI * Math.Pow (10, _increment) / 180; } }
  internal int incrementRaw {
    get { return _increment; }
    set {
      if (value >= -2 && value <= 2)
        _increment = value;
    }
  }
  internal void setIncrementUp() {
    _increment = (_increment == 2) ? 2 : _increment+1;
  }
  internal void setIncrementDown() {
    _increment = (_increment == -2) ? -2 : _increment-1;
  }

  internal bool x10UTincrement { get; set; } = true;

  private bool useKSPskin = true;
  private GUISkin _skin = null;
  internal GUISkin skin {
    get {
      if (_skin == null) {
        if (useKSPskin) {
          _skin = (GUISkin)GUISkin.Instantiate(HighLogic.Skin);
          var padding = skin.button.padding;
          padding.top = 6;
          padding.bottom = 3;
        } else {
          _skin = GUI.skin;
        }
      }
      return _skin;
    }
  }

  internal enum ModuleType {
    PAGER,
    TIMER,
    INPUT,
    GIZMO,
    TOTDV,
    EJECT,
    ORBIT,
    ENCOT,
    PATCH
  };

  private static readonly string[] moduleNames = {
    "Maneuver Pager",
    "Time & Alarm",
    "Precise Input",
    "Maneuver Gizmo",
    "Total Δv",
    "Ejection angles",
    "Orbit Info",
    "Next Encounter",
    "Patches Control",
  };

  private bool[] moduleState = {
    true,
    true,
    true,
    false,
    true,
    false,
    false,
    false,
    true
  };

  internal static string getModuleName(ModuleType type) {
    return moduleNames[(int)type];
  }

  internal bool getModuleState (ModuleType type) {
    return moduleState[(int)type];
  }

  internal void setModuleState (ModuleType type, bool state) {
    moduleState[(int)type] = state;
  }

  internal enum HotkeyType {
    PROGINC,
    PROGDEC,
    PROGZER,
    NORMINC,
    NORMDEC,
    NORMZER,
    RADIINC,
    RADIDEC,
    RADIZER,
    TIMEINC,
    TIMEDEC,
    CIRCORB,
    TURNOUP,
    TURNODN,
    PAGEINC,
    PAGECON,
    HIDEWIN,
    SHOWORB,
    SHOWEJC,
    FOCTARG,
    FOCVESL,
    PLUSORB,
    MINUORB,
    PAGEX10,
    NEXTMAN,
    PREVMAN,
    MNVRDEL
  };

  private KeyCode[] hotkeys = {
    KeyCode.Keypad8,    //PROGINC
    KeyCode.Keypad5,    //PROGDEC
    KeyCode.None,       //PROGZER
    KeyCode.Keypad9,    //NORMINC
    KeyCode.Keypad7,    //NORMDEC
    KeyCode.None,       //NORMZER
    KeyCode.Keypad6,    //RADIINC
    KeyCode.Keypad4,    //RADIDEC
    KeyCode.None,       //RADIZER
    KeyCode.Keypad3,    //TIMEINC
    KeyCode.Keypad1,    //TIMEDEC
    KeyCode.None,       //CIRCORB
    KeyCode.None,       //TURNOUP
    KeyCode.None,       //TURNODN
    KeyCode.Keypad0,    //PAGEINC
    KeyCode.Keypad2,    //PAGECON
    KeyCode.P,          //HIDEWIN
    KeyCode.None,       //SHOWORB
    KeyCode.None,       //SHOWEJC
    KeyCode.None,       //FOCTARG
    KeyCode.None,       //FOCVESL
    KeyCode.None,       //PLUSORB
    KeyCode.None,       //MINUORB
    KeyCode.None,       //PAGEX10
    KeyCode.None,       //NEXTMAN
    KeyCode.None,       //PREVMAN
    KeyCode.None        //MNVRDEL
  };
  private bool[] hotkeyPresses = Enumerable.Repeat(false, Enum.GetValues(typeof(HotkeyType)).Length).ToArray ();

  internal void setHotkey (HotkeyType type, KeyCode code) {
    hotkeys[(int)type] = code;
  }

  internal KeyCode getHotkey (HotkeyType type) {
    return hotkeys[(int)type];
  }

  internal void registerHotkeyPress (HotkeyType type) {
    hotkeyPresses[(int)type] = true;
  }

  internal bool isHotkeyRegistered (HotkeyType type) {
    if (hotkeyPresses[(int)type] == true) {
      hotkeyPresses[(int)type] = false;
      return true;
    }
    return false;
  }

  /// <summary>
  /// Save our configuration to file.
  /// </summary>
  internal void saveConfig() {
    Debug.Log ("Saving PreciseManeuver settings.");
    PluginConfiguration config = KSP.IO.PluginConfiguration.CreateForType<PreciseManeuver> (null);

    foreach (HotkeyType type in Enum.GetValues (typeof (HotkeyType)))
      config[type.ToString ()] = hotkeys[(int)type].ToString ();

    foreach (ModuleType type in Enum.GetValues (typeof (ModuleType)))
      config[type.ToString ()] = moduleState[(int)type];

    config["increment"] = _increment;
    config["x10UTincrement"] = x10UTincrement;

    config["mainWindowX"] = (int)_mainWindowPos.x;
    config["mainWindowY"] = (int)_mainWindowPos.y;
    config["keyWindowX"] = (int)_keymapperWindowPos.x;
    config["keyWindowY"] = (int)_keymapperWindowPos.y;

    config.save();
  }

  /// <summary>
  /// Load any saved configuration from file.
  /// </summary>
  internal void loadConfig () {
    Debug.Log ("Loading PreciseManeuver settings.");
    PluginConfiguration config = KSP.IO.PluginConfiguration.CreateForType<PreciseManeuver> (null);
    config.load ();

    try {
      foreach (HotkeyType type in Enum.GetValues (typeof (HotkeyType)))
        hotkeys[(int)type] = (KeyCode)Enum.Parse (typeof (KeyCode), config.GetValue<String> (type.ToString (), hotkeys[(int)type].ToString ()));

      foreach (ModuleType type in Enum.GetValues (typeof (ModuleType)))
        moduleState[(int)type] = config.GetValue<bool> (type.ToString (), moduleState[(int)type]);

      _increment = config.GetValue<int> ("increment", _increment);
      x10UTincrement = config.GetValue<bool> ("x10UTincrement", x10UTincrement);

      _mainWindowPos.x = config.GetValue<int> ("mainWindowX", (int)_mainWindowPos.x);
      _mainWindowPos.y = config.GetValue<int> ("mainWindowY", (int)_mainWindowPos.y);
      _keymapperWindowPos.x = config.GetValue<int> ("keyWindowX", (int)_keymapperWindowPos.x);
      _keymapperWindowPos.y = config.GetValue<int> ("keyWindowY", (int)_keymapperWindowPos.y);
    } catch (Exception) {
      // do nothing here, the defaults are already set
    }
  }
}
}
