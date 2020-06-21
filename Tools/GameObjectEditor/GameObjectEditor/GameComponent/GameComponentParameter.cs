using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.IO;
using System.Drawing;
using System.Windows.Media;

namespace GameObjectEditor.GameComponent
{
    public class GameComponentParameter
    {
        public String Name;
        public String Type;
        public TextBox Text;

        String stringValue;
        float floatValue;
        int intValue;
        uint uintvalue;
        bool boolValue;
        Color colour;
        public TextBox textbox;


        float[] Vector3Value = new float[3];
        float[] Mat34Value = new float[12];
        int[] IntListValue = new int[0];

        public GameComponentParameter()
        {
            Name = Type = stringValue = "";
            floatValue = 0;
            intValue = 0;
            boolValue = false;
            uintvalue = 0;
            colour = Colors.White;

        }

        public GameComponentParameter(GameComponentParameter other)
        {
            Name = other.Name;
            Type = other.Type;
        }

        public String GetValue()
        {
            switch (Type)
            {
                case "Colour":
                    {
                        return colour.ToString();
                    }

                case "Uint":
                    {
                        return uintvalue.ToString();
                    }
                case "Bool":
                    {
                        return boolValue.ToString();
                    }
                case "String":
                    {
                        return stringValue;
                    }
                case "Float":
                    {
                        return floatValue.ToString();
                    }
                case "Int":
                    {
                        return intValue.ToString();
                    }
                case "Vector3":
                    {
                        return String.Format("{0},{1},{2}", Vector3Value[0], Vector3Value[1], Vector3Value[2]);
                    }
                case "Mat34":
                    {
                        return String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                                Mat34Value[0], Mat34Value[1], Mat34Value[2],
                                Mat34Value[3], Mat34Value[4], Mat34Value[5],
                                Mat34Value[6], Mat34Value[7], Mat34Value[8]);
                    }
                case "IntList":
                    {
                        String result = "";
                        if (IntListValue.Length > 0)
                        {
                            for (int i = 0; i < IntListValue.Length - 1; i++)
                            {
                                result += String.Format("{0},", IntListValue[i]);
                            }
                            result += String.Format("{0}", IntListValue[IntListValue.Length - 1]);
                        }
                        return result;
                    }

            }
            throw new Exception("GameComponentParameter::Invalid value type");
        }

        public void SetValue(String s)
        {
            switch (Type)
            {
                case "Colour":
                    {
                        string a = s.Substring(0, 2);
                        string r = s.Substring(2, 2);
                        string g = s.Substring(4, 2);
                        string b = s.Substring(6, 2);

                        int av = Convert.ToInt32(a, 16);
                        int rv = Convert.ToInt32(r, 16);
                        int gv = Convert.ToInt32(g, 16);
                        int bv = Convert.ToInt32(b, 16);

                        colour = Color.FromArgb((byte)av, (byte)rv, (byte)gv, (byte)bv);

                    }
                    break;

                case "Bool":
                    {
                        boolValue = bool.Parse(s);
                    }
                    break;
                case "String":
                    {
                        stringValue = s;
                    }
                    break;

                case "Float":
                    {
                        float.TryParse(s, out floatValue);
                    }
                    break;

                case "Int":
                    {
                        int.TryParse(s, out intValue);
                    }
                    break;
                case "Vector3":
                    {
                        string[] parts = s.Split(',');
                        for (int i = 0; i < 3; i++)
                        {
                            Vector3Value[i] = float.Parse(parts[i]);
                        }
                    }
                    break;
                case "Mat34":
                    {
                        char[] splits = new char[] { ',' };
                        string[] parts = s.Split(splits, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < 9; i++)
                        {
                            Mat34Value[i] = float.Parse(parts[i]);
                        }
                    }
                    break;
                case "IntList":
                    {
                        char[] splits = new char[] { ',' };
                        string[] parts = s.Split(splits, StringSplitOptions.RemoveEmptyEntries);
                        int n = parts.Length;
                        IntListValue = new int[n];
                        for (int i = 0; i < n; i++)
                        {
                            IntListValue[i] = int.Parse(parts[i]);
                        }
                    }
                    break;
                case "Uint":
                    {
                        uint.TryParse(s, out uintvalue);
                    }
                    break;
            }
        }

        public void Save(TextWriter tw)
        {
            String res = String.Format("{0} {1} {2}", Name, Type, GetValue());
            tw.WriteLine(res);
        }
    }
}
