using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model;
using LiveSplit.Options;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using USB2SnesW;

namespace LiveSplit.UI.Components
{
    public class USB2SNESComponent : LogicComponent
    {
        class Split
        {
            public string name { get; set; }
            public string address { get; set; }
            public string value { get; set; }
            public string type { get; set; }
            public List<Split> more { get; set; }

            public uint addressint { get { return Convert.ToUInt32(address, 16); } }
            public uint valueint { get { return Convert.ToUInt32(value, 16); } }
        }

        class Category
        {
            public string name { get; set; }
            public List<string> splits { get; set; }
        }

        class Game
        {
            public string name { get; set; }
            public Autostart autostart { get; set; }
            public List<Category> categories { get; set; }
            public List<Split> definitions { get; set; }
        }

        class Autostart
        {
            public string active { get; set; }
            public string address { get; set; }
            public string value { get; set; }
            public string type { get; set; }

            public uint addressint { get { return Convert.ToUInt32(address, 16); } }
            public uint valueint { get { return Convert.ToUInt32(value, 16); } }
        }

        public override string ComponentName => "USB2SNES Auto Splitter";
        private Timer _update_timer;
        private ComponentSettings _settings;
        private LiveSplitState _state;
        private TimerModel _model;
        private Game _game;
        private List<string> _splits;
        private bool _inTimer;
        private bool _inasyncUpdate;
        private bool _error;
        private USB2SnesW.USB2SnesW _usb2snes;


        public USB2SNESComponent(LiveSplitState state)
        {
            _state = state;
            _settings = new ComponentSettings();
            _model = new TimerModel() { CurrentState = _state };
            _state.RegisterTimerModel(_model);

            _splits = null;
            _inTimer = false;
            _inasyncUpdate = false;
            _error = false;

            _update_timer = new Timer() { Interval = /*33*/ 1000 };
            _update_timer.Tick += (sender, args) => UpdateSplits_A();
            _update_timer.Enabled = true;

            _state.OnReset += _state_OnReset;
            _state.OnStart += _state_OnStart;
            _usb2snes = new USB2SnesW.USB2SnesW();
        }
        
        private bool checkSplits()
        {
            bool r = true;
            foreach(var c in _game.categories)
            {
                foreach (var s in c.splits)
                {
                    var d = _game.definitions.Where(x => x.name == s).FirstOrDefault();
                    if(d == null)
                    {
                        MessageBox.Show(String.Format("Split definition missing: {0}", s));
                        r = false;
                    }
                }
            }
            return r;
        }

        private bool connect()
        {
            if (!_usb2snes.Connected())
            {
                _usb2snes.Connect();
                if (_usb2snes.Connected())
                {
                    List<String> devices = _usb2snes.GetDevices();
                    if (!devices.Contains(_settings.Device))
                        return false;
                    _usb2snes.SetName("LiveSplit AutoSplitter");
                    _usb2snes.Attach(_settings.Device);
                    _usb2snes.Info(); // Info is the only neutral way to know if we are attached to the device
                }
                if (!_usb2snes.Connected())
                {
                    MessageBox.Show("Could not connect to sd2snes, check serial port settings.");
                    return false;
                }
            }
            return true;
        }

        private bool readConfig()
        {
            try
            {
                var jsonStr = File.ReadAllText(_settings.ConfigFile);
                _game = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Game>(jsonStr);
            }
            catch
            {
                MessageBox.Show("Could not open split config file, check config file settings.");
                return false;
            }
            if (!this.checkSplits())
            {
                MessageBox.Show("The split config file has missing definitions.");
                return false;
            }

            return true;
        }

        private void _state_OnStart(object sender, EventArgs e)
        {
            if(!_usb2snes.Connected())
            {
                if(!this.connect())
                {
                    _model.Reset();
                    return;
                }
            }

            if(_game == null)
            {
                if(!this.readConfig())
                {
                    _model.Reset();
                    return;
                }
            }

            _error = false;

            _splits = _game.categories.Where(c => c.name.ToLower() == _state.Run.CategoryName.ToLower()).First()?.splits;
            if(_splits == null)
            {
                MessageBox.Show("There are no splits for the current category in the split config file, check that the run category is correctly set and exists in the config file.");
            }

        }

        private void _state_OnReset(object sender, TimerPhase value)
        {
            if (_usb2snes.Connected())
            {
                if(_settings.ResetSNES)
                {
                    _usb2snes.Reset();
                }
            }
        }

        public override void Dispose()
        {
            _update_timer?.Dispose();
            if (_usb2snes.Connected())
            {
                _usb2snes.Disconnect();
            }
            _state.OnReset -= _state_OnReset;
            _state.OnStart -= _state_OnStart;            
        }

        public override Control GetSettingsControl(LayoutMode mode)
        {
            return _settings;
        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            return _settings.GetSettings(document);
        }

        public override void SetSettings(XmlNode settings)
        {
            _settings.SetSettings(settings);
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height,
            LayoutMode mode)
        {
        }

        public void DoSplit()
        {
            if (_game.name == "Super Metroid" && _usb2snes.Connected())
            {
                var data = new byte[512];
                data = _usb2snes.GetAddress((uint)(0xF509DA), (uint)512);
                int ms = (data[0] + (data[1] << 8)) * (1000 / 60);
                int sec = data[2] + (data[3] << 8);
                int min = data[4] + (data[5] << 8);
                int hr = data[6] + (data[7] << 8);
                var gt = new TimeSpan(0, hr, min, sec, ms);
                _state.SetGameTime(gt);
                _model.Split();
            }
            else
            {
                _model.Split();
            }
        }

        private bool checkSplit(Split split, uint value, uint word)
        {
            bool ret = false;
            switch (split.type)
            {
                case "bit":
                    if ((value & split.valueint) != 0) { ret = true; }
                    break;
                case "eq":
                    if (value == split.valueint) { ret = true; }
                    break;
                case "gt":
                    if (value > split.valueint) { ret = true; }
                    break;
                case "lt":
                    if (value < split.valueint) { ret = true; }
                    break;
                case "gte":
                    if (value >= split.valueint) { ret = true; }
                    break;
                case "lte":
                    if (value <= split.valueint) { ret = true; }
                    break;
                case "wbit":
                    if ((word & split.valueint) != 0) { ret = true; }
                    break;
                case "weq":
                    if (word == split.valueint) { ret = true; }
                    break;
                case "wgt":
                    if (word > split.valueint) { ret = true; }
                    break;
                case "wlt":
                    if (word < split.valueint) { ret = true; }
                    break;
                case "wgte":
                    if (word >= split.valueint) { ret = true; }
                    break;
                case "wlte":
                    if (word <= split.valueint) { ret = true; }
                    break;
            }
            return ret;
        }

        public void UpdateSplits_A()
        {
            if (_inasyncUpdate == true)
                return;

            _inasyncUpdate = true;
            Task utask1 = Task.Run((Action)UpdateSplits);
            _inasyncUpdate = false;
            return;
        }

        public void UpdateSplits()
        {
            if (_inTimer == true)
                return;

            _inTimer = true;
            if (_state.CurrentPhase == TimerPhase.NotRunning)
            {
                if(_error == false && _settings.Device != null && _game == null && (!_usb2snes.Connected()))
                {
                    if (!_usb2snes.Connected())
                    {
                        if (!this.connect())
                        {
                            _error = true;
                            _inTimer = false;
                            return;
                        }
                    }

                    if (_game == null)
                    {
                        if (!this.readConfig())
                        {
                            _error = true;
                            _inTimer = false;
                            return;
                        }
                    }
                }

                if (_game != null && _game.autostart.active == "1")
                {
                    if (_usb2snes.Connected())
                    {
                        var data = new byte[64];
                        data = _usb2snes.GetAddress((0xF50000 + _game.autostart.addressint), (uint)64);

                        uint value = (uint)data[0];
                        uint word = (uint)(data[0] + (data[1] << 8));

                        switch (_game.autostart.type)
                        {
                            case "bit":
                                if ((value & _game.autostart.valueint) != 0) { _model.Start(); }
                                break;
                            case "eq":
                                if (value == _game.autostart.valueint) { _model.Start(); }
                                break;
                            case "gt":
                                if (value > _game.autostart.valueint) { _model.Start(); }
                                break;
                            case "lt":
                                if (value < _game.autostart.valueint) { _model.Start(); }
                                break;
                            case "gte":
                                if (value >= _game.autostart.valueint) { _model.Start(); }
                                break;
                            case "lte":
                                if (value <= _game.autostart.valueint) { _model.Start(); }
                                break;
                            case "wbit":
                                if ((word & _game.autostart.valueint) != 0) { _model.Start(); }
                                break;
                            case "weq":
                                if (word == _game.autostart.valueint) { _model.Start(); }
                                break;
                            case "wgt":
                                if (word > _game.autostart.valueint) { _model.Start(); }
                                break;
                            case "wlt":
                                if (word < _game.autostart.valueint) { _model.Start(); }
                                break;
                            case "wgte":
                                if (word >= _game.autostart.valueint) { _model.Start(); }
                                break;
                            case "wlte":
                                if (word <= _game.autostart.valueint) { _model.Start(); }
                                break;
                        }
                    }
                }
            }
            else if (_state.CurrentPhase == TimerPhase.Running)
            {
                if (_splits != null)
                {
                    if (_usb2snes.Connected())
                    {
                        var splitName = _splits[_state.CurrentSplitIndex];
                        var split = _game.definitions.Where(x => x.name == splitName).First();
                        var data = new byte[64];
                        data =  _usb2snes.GetAddress((0xF50000 + split.addressint), (uint)64);

                        uint value = (uint)data[0];
                        uint word = (uint)(data[0] + (data[1] << 8));

                        bool ok = checkSplit(split, value, word);
                        if (split.more != null)
                        {
                            foreach (var moreSplit in split.more)
                            {
                                data =  _usb2snes.GetAddress((0xF50000 + moreSplit.addressint), (uint)64);

                                value = (uint)data[0];
                                word = (uint)(data[0] + (data[1] << 8));

                                ok = ok && checkSplit(moreSplit, value, word);
                            }
                        }

                        if (ok)
                        {
                            DoSplit();
                        }
                    }
                }
            }
            _inTimer = false;
        }
    }
}
