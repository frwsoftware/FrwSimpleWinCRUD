//  Console Widget - A handy library to route the Console and Debug output of .NET to your GUI
//  Written by Tebjan Halm 2010, tebjan()gmx.de
//
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 3 of the License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General
//  Public License along with this library.

using System;
using System.Windows.Forms;
using System.IO;

namespace ConsoleWidget
{
    public class ConsoleStream : TextWriter
    {
        //the textbox we write into
        private RichTextBox FBox = null;

        //create writer with a texbox
        public ConsoleStream(RichTextBox box)
        {
            FBox = box;
        }

        //the textbox of this stream
        public RichTextBox TextBox
        {
            get { return FBox; }
            set { FBox = value; }
        }

        //return default encoding
        public override System.Text.Encoding Encoding { get { return System.Text.Encoding.Default; } }

        //the delegate we pass to the textbox
        delegate void SetTextCallback(string text);

        // This method is passed in to the SetTextCallBack delegate
        // to update the Text property of textBox.
        private void SetText(string text)
        {
            if (FBox.IsHandleCreated)
            {
                FBox.AppendText(text);
                FBox.ScrollToCaret();
            }
        }

        //write a string
        public override void Write(string value)
        {
            //only one thread can write in the textbox at a time
            lock (FBox)
            {
                // Check if this method is running on a different thread
                // than the thread that created the control.
                if (FBox.InvokeRequired)
                {
                    // It's on a different thread, so use Invoke.
                    SetTextCallback d = new SetTextCallback(SetText);
                    //FBox.Invoke(d, new object[] { value });

                    //write async into the textbox and return
                    FBox.BeginInvoke(d, new object[] { value });
                }
                else
                {
                    SetText(value);
                }
            }
        }

        //write a string + new line
        public override void WriteLine(string value)
        {
                Write(value + Environment.NewLine);
        }

        //overwrite all other write methods
        public override void Write(bool value) { this.Write(value.ToString()); }
        public override void Write(char value) { this.Write(value.ToString()); }
        public override void Write(char[] buffer) { this.Write(new string(buffer)); }
        public override void Write(char[] buffer, int index, int count) { this.Write(new string(buffer, index, count)); }
        public override void Write(decimal value) { this.Write(value.ToString()); }
        public override void Write(double value) { this.Write(value.ToString()); }
        public override void Write(float value) { this.Write(value.ToString()); }
        public override void Write(int value) { this.Write(value.ToString()); }
        public override void Write(long value) { this.Write(value.ToString()); }
        public override void Write(string format, object arg0) { this.WriteLine(string.Format(format, arg0)); }
        public override void Write(string format, object arg0, object arg1) { this.WriteLine(string.Format(format, arg0, arg1)); }
        public override void Write(string format, object arg0, object arg1, object arg2) { this.WriteLine(string.Format(format, arg0, arg1, arg2)); }
        public override void Write(string format, params object[] arg) { this.WriteLine(string.Format(format, arg)); }
        public override void Write(uint value) { this.WriteLine(value.ToString()); }
        public override void Write(ulong value) { this.WriteLine(value.ToString()); }
        public override void Write(object value) { this.WriteLine(value.ToString()); }
        public override void WriteLine() { this.Write(Environment.NewLine); }
        public override void WriteLine(bool value) { this.WriteLine(value.ToString()); }
        public override void WriteLine(char value) { this.WriteLine(value.ToString()); }
        public override void WriteLine(char[] buffer) { this.WriteLine(new string(buffer)); }
        public override void WriteLine(char[] buffer, int index, int count) { this.WriteLine(new string(buffer, index, count)); }
        public override void WriteLine(decimal value) { this.WriteLine(value.ToString()); }
        public override void WriteLine(double value) { this.WriteLine(value.ToString()); }
        public override void WriteLine(float value) { this.WriteLine(value.ToString()); }
        public override void WriteLine(int value) { this.WriteLine(value.ToString()); }
        public override void WriteLine(long value) { this.WriteLine(value.ToString()); }
        public override void WriteLine(string format, object arg0) { this.WriteLine(string.Format(format, arg0)); }
        public override void WriteLine(string format, object arg0, object arg1) { this.WriteLine(string.Format(format, arg0, arg1)); }
        public override void WriteLine(string format, object arg0, object arg1, object arg2) { this.WriteLine(string.Format(format, arg0, arg1, arg2)); }
        public override void WriteLine(string format, params object[] arg) { this.WriteLine(string.Format(format, arg)); }
        public override void WriteLine(uint value) { this.WriteLine(value.ToString()); }
        public override void WriteLine(ulong value) { this.WriteLine(value.ToString()); }
        public override void WriteLine(object value) { this.WriteLine(value.ToString()); }
  
    }
}
