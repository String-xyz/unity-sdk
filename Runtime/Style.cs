using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StringSDK
{
    [Serializable]
    public class BaseStyle 
    { 
        public string color;
        public string fontSize;

        public BaseStyle(string color, string fontSize) 
        {
            this.color = color;
            this.fontSize = fontSize;
        }
    }

    [Serializable]
    public class Autofill 
    { 
        public string backgroundColor;
        public Autofill(string backgroundColor) 
         {
            this.backgroundColor = backgroundColor;
         }
    }
    [Serializable]
    public class Placeholder 
    {
        public BaseStyle @base;
        public StateStyle focus;
        public Placeholder(BaseStyle @base, StateStyle focus)
        {
            this.@base = @base;
            this.focus = focus;
        }
    }
    [Serializable]
    public class StateStyle { 
        public string color;
        public string border;
       public StateStyle(string color, string border)
        {
            this.color = color;
            this.border = border;
        }
       public StateStyle(string color)
        {
            this.color = color;
        }
    }
   
    [Serializable]
    public class Style 
    {
        public BaseStyle @base;
        public Autofill autofill;
        public StateStyle hover;
        public StateStyle focus;
        public StateStyle valid;
        public StateStyle invalid;
        public Placeholder placeholder;

        public Style(string baseColor, string baseSize, string autofillColor, string hoverColor, string focusColor, string validColor, string invalidColor, string placeholderBaseColor, string placeholderBaseSize, string placeholderStateColor)
        {
            this.@base = new BaseStyle(baseColor, baseSize);
            this.autofill = new Autofill(autofillColor);
            this.hover = new StateStyle(hoverColor);
            this.focus = new StateStyle(focusColor);
            this.valid = new StateStyle(validColor);
            this.invalid = new StateStyle(invalidColor);
            this.placeholder = new Placeholder(new BaseStyle(placeholderBaseColor, placeholderBaseSize), new StateStyle(placeholderStateColor));
        }

        public Style() {}

        public string ToJSON()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class DefaultStyle: Style  { 
       public DefaultStyle() 
        {
            this.@base = new BaseStyle("black", "16px");
            this.autofill = new Autofill("yellow");
            this.hover = new StateStyle("blue");
            this.focus = new StateStyle("white");
            this.valid = new StateStyle("green");
            this.invalid = new StateStyle("red");
            this.placeholder = new Placeholder(new BaseStyle("ffff", "16px"), new StateStyle("blue"));
        }
    }
}