﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core.DataTypes;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    public partial class PatternEdit : UserControl
    {
        private OutputPattern _current;
        public Dictionary<string, OsuStatus> SaveEvents = new Dictionary<string, OsuStatus>
        {
            {"All",OsuStatus.All },
            {"Listening",OsuStatus.Listening },
            {"Playing",OsuStatus.Playing },
            {"Watching",OsuStatus.Watching },
            {"Editing",OsuStatus.Editing },
            {"Never",OsuStatus.Null }
        };



        public PatternEdit()
        {
            InitializeComponent();
            comboBox_saveEvent.DataSource = SaveEvents.Select(v => v.Key).ToList();

        }

        public OutputPattern Current
        {
            get { return _current; }
            set
            {
                if (!this.IsHandleCreated || this.IsDisposed)
                    return;
                this.BeginInvoke((MethodInvoker)(() =>
                {
                    _current = value;
                    textBox_formating.Text = value?.Pattern ?? "";
                    textBox_FileName.Text = value?.Name ?? "";
                    if (value != null)
                        comboBox_saveEvent.SelectedItem = SaveEvents.First(s => s.Value == value.SaveEvent).Key;
                }));
            }
        }

        public EventHandler<OutputPattern> DeletePattern;
        public EventHandler AddPattern;
        private Dictionary<string, string> _replacements;

        private void Save()
        {
            if (Current != null)
            {
                Current.Name = textBox_FileName.Text;
                Current.Pattern = textBox_formating.Text;
                Current.SaveEvent = SaveEvents.First(s => s.Key == (string)comboBox_saveEvent.SelectedItem).Value;
            }
        }


        private void button_Click(object sender, EventArgs e)
        {
            if (sender == button_save)
            {
                Save();
            }
            else if (sender == button_addNew)
            {
                AddPattern?.Invoke(this, EventArgs.Empty);
            }
            else if (sender == button_delete)
            {
                DeletePattern?.Invoke(this, Current);
            }
        }

        private void textBox_FileName_KeyPress(object sender, KeyPressEventArgs e)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                textBox_FileName.Text = textBox_FileName.Text.Replace(c.ToString(), "");
            }
        }

        public void SetPreview(Dictionary<string, string> replacements)
        {
            _replacements = replacements;
        }
        private void textBox_formating_TextChanged(object sender, EventArgs e)
        {
            if (_replacements == null)
                textBox_preview.Text = "Change map in osu! to see preview";
            else
            {
                var toFormat = textBox_formating.Text;
                foreach (var r in _replacements)
                {
                    toFormat = toFormat.Replace(r.Key, r.Value);
                }
                textBox_preview.Text = toFormat;
            }
        }
    }
}
