﻿// Copyright (C) 2017 Lex Li
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Lextm.ReStructuredText
{
    public class Content
    {
        public Content(string text)
        {
            Text = text;
            Unescape();
        }

        internal void RemoveAttribution()
        {
            if (Text.StartsWith("---"))
            {
                Text = Text.Substring("---".Length).TrimStart();
                return;
            }

            if (Text.StartsWith("--"))
            {
                Text = Text.Substring("--".Length).TrimStart();
                return;
            }

            if (Text.StartsWith("\u2014"))
            {
                Text = Text.Substring("\u2014".Length).TrimStart();
                return;
            }
        }

        public string Text { get; private set; }
        public bool IsSection
        {
            get
            {
                var pure = Text.TrimEnd();
                foreach (var item in pure)
                {
                    if (item != Text[0])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public void RemoveStart()
        {
            Text = Text.TrimStart();
        }
        
        public void RemoveEnd()
        {
            Text = Text.TrimEnd(' ', '\r', '\n');
        }

        public void RemoveLiteral()
        {
            Text = Text.TrimEnd().TrimEnd(':');
        }

        public void Append(int indentation)
        {
            var builder = new StringBuilder(Text.Length + indentation);
            for (int i = 0; i < indentation; i++)
            {
                builder.Append(' ');
            }

            Text = builder.Append(Text).ToString();
        }

        public void Append(string text)
        {
            Text = Text + text;
        }

        public void Unescape()
        {
            Regex regex = new Regex (@"\\[Ux]([0-9A-F]{2,4})", RegexOptions.IgnoreCase);
            Text = regex.Replace (Text, match => ((char)int.Parse (match.Groups[1].Value,
                NumberStyles.HexNumber)).ToString ());
        }

        public string RemoveTitle()
        {
            var regex = new Regex(":(?<title>.*):$");
            var match = regex.Match(Text);
            if (!match.Success)
            {
                return null;
            }
            
            var title = match.Groups["title"].Value;
            Text = regex.Replace(Text, string.Empty);
            return title;
        }
    }
}
